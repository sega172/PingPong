using UnityEngine;

[RequireComponent(typeof(Paddle))]
public class BotPaddle : MonoBehaviour
{
    [Header("Основная сложность (0 – легко, 1 – сложно)")]
    [Range(0f, 1f)]
    [SerializeField] private float _difficulty = 0.5f;

    [Header("Параметры, привязанные к сложности")]
    [SerializeField] private AnimationCurve _reactionDelayCurve = AnimationCurve.EaseInOut(0f, 0.5f, 1f, 0.05f);
    [SerializeField] private AnimationCurve _accuracyCurve = AnimationCurve.EaseInOut(0f, 0.2f, 1f, 0.95f);
    [SerializeField] private AnimationCurve _speedFactorCurve = AnimationCurve.EaseInOut(0f, 0.3f, 1f, 1f);

    [Header("Дополнительная настройка")]
    [SerializeField] private float _maxPredictionOffset = 0.8f;   // максимальная ошибка предсказания (в юнитах)
    [SerializeField] private float _gain = 2.5f;                 // чувствительность при приближении к цели

    private Paddle _paddle;
    private Ball _ball;
    private float _targetY;                 // целевая позиция (после задержки)
    private float _lastTargetY;             // последняя вычисленная позиция
    private float _timeSinceLastUpdate;     // таймер задержки
    private float _currentDelay;            // текущая задержка (зависит от сложности)
    private float _currentAccuracy;         // текущая точность (0..1)
    private float _currentSpeedFactor;      // множитель скорости (0..1)

    private void Start()
    {
        _paddle = GetComponent<Paddle>();
        _ball = Ball.Instance;              // предполагается, что Ball имеет статический Instance

        if (_ball == null)
        {
            Debug.LogError("BotPaddle: Ball.Instance не найден!");
            enabled = false;
        }

        ScoreManager.OnScoreChanged += RecalculateDifficulty;
    }

    private void RecalculateDifficulty(int newScore)
    {
        _difficulty = Mathf.Lerp(0f, 1f, newScore / 10f);
    }

    private void FixedUpdate()
    {
        // Если мяч неактивен – стоим на месте
        if (_ball == null || !_ball.Active)
        {
            _paddle.SetDirection(0f);
            return;
        }

        // Обновляем параметры в зависимости от сложности
        _currentDelay = _reactionDelayCurve.Evaluate(_difficulty);
        _currentAccuracy = _accuracyCurve.Evaluate(_difficulty);
        _currentSpeedFactor = _speedFactorCurve.Evaluate(_difficulty);

        // Предсказываем позицию мяча на уровне нашей ракетки (по Y)
        float predictedY = PredictBallY();

        // Добавляем ошибку в зависимости от точности
        float error = (1f - _currentAccuracy) * _maxPredictionOffset * Random.Range(-1f, 1f);
        predictedY += error;

        // Задержка реакции (обновляем целевую позицию только после таймера)
        _timeSinceLastUpdate += Time.fixedDeltaTime;
        if (_timeSinceLastUpdate >= _currentDelay)
        {
            _targetY = predictedY;
            _timeSinceLastUpdate = 0f;
        }

        // Вычисляем направление движения
        float currentY = transform.position.y;
        float diff = _targetY - currentY;

        // Если разница маленькая – стоим, иначе двигаемся с учётом скоростного множителя
        float direction;
        if (Mathf.Abs(diff) < 0.01f)
        {
            direction = 0f;
        }
        else
        {
            // Насыщенный сигнал: чем дальше, тем быстрее, но не больше 1
            direction = Mathf.Clamp(diff * _gain, -1f, 1f);
            // Умножаем на скоростной множитель (чем ниже сложность, тем медленнее разгон)
            direction *= _currentSpeedFactor;
        }

        _paddle.SetDirection(direction);
    }

    /// <summary>
    /// Предсказывает Y-координату мяча в момент, когда он достигнет X-координаты ракетки.
    /// </summary>
    private float PredictBallY()
    {
        Vector3 ballPos = _ball.transform.position;
        Vector3 ballVel = _ball.GetComponent<Rigidbody>().linearVelocity;

        // Если мяч летит в обратную сторону (не к нам), возвращаем текущую позицию мяча (или середину)
        float sign = Mathf.Sign(transform.position.x);
        if (ballVel.x * sign <= 0f)
        {
            // Мяч удаляется – просто держимся в центре или предсказываем, что он вернётся (можно упростить)
            return 0f;
        }

        // Время до достижения X ракетки (учитываем только горизонтальную скорость)
        float timeToReach = (transform.position.x - ballPos.x) / ballVel.x;
        if (timeToReach < 0f) timeToReach = 0f;

        // Предсказываем Y через это время (без учёта столкновений со стенами – это уже не критично)
        float predictedY = ballPos.y + ballVel.y * timeToReach;

        // Ограничиваем в пределах игрового поля (чтобы не улетать за границы)
        // Значения yMin/yMax можно получить из Paddle (через рефлексию) или задать константами.
        // Я возьму их через рефлексию, но если не хотите – можно сделать публичными полями.
        float yMin = GetPaddleBound("yMin");
        float yMax = GetPaddleBound("yMax");
        predictedY = Mathf.Clamp(predictedY, yMin, yMax);

        return predictedY;
    }

    // Вспомогательный метод для получения приватных полей границ (чтобы не переписывать Paddle)
    private float GetPaddleBound(string fieldName)
    {
        var field = typeof(Paddle).GetField(fieldName, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        if (field != null)
            return (float)field.GetValue(_paddle);
        return -4f; // запасные значения
    }
}