using UnityEngine;

[RequireComponent(typeof(Paddle))]
public class BotPaddle : MonoBehaviour
{
    [Header("Настройки бота")]
    [Range(0f, 1f)]
    public float difficulty = 0.5f;          // 0 – слабый, 1 – идеальный

    [Header("Параметры управления")]
    [SerializeField] private float gainBase = 1.5f;
    [SerializeField] private float gainPerDifficulty = 5f;
    [SerializeField] private float maxPredictionOffset = 2f;

    private Paddle paddle;
    private Ball ball;

    private void Start()
    {
        paddle = GetComponent<Paddle>();
        if (Ball.Instance != null)
            ball = Ball.Instance;
        else
            Debug.LogWarning("BotPaddle: Ball.Instance не найден!");

        RecalculateDifficulty(ScoreManager.Score);
        ScoreManager.OnScoreChanged += RecalculateDifficulty;
    }

    private void RecalculateDifficulty(int score)
    {
        difficulty = Mathf.Clamp01(score / 10f);
    }

    private void FixedUpdate()
    {
        if (ball == null || !ball.Active)
        {
            paddle.SetDirection(0f);
            return;
        }

        // Получаем границы от GameManager
        float yMin, yMax;

        yMin = GameManager.Instance.DownWallPoint.position.y;
        yMax = GameManager.Instance.UpWallPoint.position.y;


        // Прогнозируем Y мяча на уровне ракетки
        float predictedY = PredictBallYAtPaddleX(yMin, yMax);

        // Добавляем ошибку в зависимости от сложности
        float errorOffset = (1f - difficulty) * Random.Range(-maxPredictionOffset, maxPredictionOffset);
        float targetY = predictedY + errorOffset;

        // Текущая позиция ракетки
        float currentY = transform.position.y;

        // Вычисляем направление с учётом сложности
        float gain = gainBase + difficulty * gainPerDifficulty;
        float direction = Mathf.Clamp((targetY - currentY) * gain, -1f, 1f);

        // Мёртвая зона
        float deadZone = Mathf.Lerp(0.3f, 0.05f, difficulty);
        if (Mathf.Abs(targetY - currentY) < deadZone)
            direction = 0f;

        paddle.SetDirection(direction);
    }

    /// <summary>
    /// Предсказывает координату Y мяча в момент, когда он достигнет X ракетки.
    /// Учитывает отскоки от верхней и нижней границ.
    /// </summary>
    private float PredictBallYAtPaddleX(float yMin, float yMax)
    {
        Vector3 ballPos = ball.transform.position;
        Vector3 ballVel = ball.GetComponent<Rigidbody>().linearVelocity;

        float paddleX = transform.position.x;
        float sign = Mathf.Sign(paddleX);

        // Если мяч летит от ракетки – возвращаем центр поля
        if (Mathf.Sign(ballVel.x) != sign)
            return (yMin + yMax) * 0.5f;

        // Защита от деления на ноль или очень малую скорость
        if (Mathf.Abs(ballVel.x) < 0.001f)
            return ballPos.y; // или центр – как больше нравится

        float timeToReach = (paddleX - ballPos.x) / ballVel.x;
        if (timeToReach <= 0f)
            return ballPos.y;

        // Симуляция движения по Y с отскоками
        float y = ballPos.y;
        float vy = ballVel.y;
        float dt = 0.02f;
        float timeSim = 0f;
        int maxIterations = 10000; // защита от зацикливания
        int iterations = 0;

        while (timeSim < timeToReach && iterations < maxIterations)
        {
            float step = Mathf.Min(dt, timeToReach - timeSim);
            y += vy * step;
            timeSim += step;
            iterations++;

            if (y < yMin)
            {
                y = yMin + (yMin - y);
                vy = -vy;
            }
            else if (y > yMax)
            {
                y = yMax - (y - yMax);
                vy = -vy;
            }
        }

        return y;
    }
}