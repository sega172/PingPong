using UnityEngine;

[RequireComponent(typeof(Paddle))]
public class BotPaddle : MonoBehaviour
{
    [Range(0f, 1f)]
    public float difficulty;

    [SerializeField] private float _hitsToDegradeBase = 7f;
    [SerializeField] private float _hitsToDegradePerScore = 0.66f;
    [SerializeField] private float _randomizationPercent = 0.3f;
    [SerializeField] private float _maxErrorOffset = 2.8f;
    [SerializeField] private float _minFatalMissOffset = 2.2f;
    [SerializeField] private float _maxReactionDelay = 0.22f;

    private Paddle _paddle;
    private Ball _ball;
    private float _currentTargetY;
    private float _currentErrorOffset;
    private float _targetErrorOffset;
    private float _lastBallDirX;
    private float _currentReactionDelay;
    private float _reactionTimer;
    private bool _wasBallApproaching;
    private bool _willMissThisApproach;
    private int _rallyHits;
    private int _currentScore;
    private int _hitsToDegradeForCurrentRally;

    public int HitsToDegrade => _hitsToDegradeForCurrentRally > 0
        ? _hitsToDegradeForCurrentRally
        : Mathf.Max(1, Mathf.RoundToInt(_hitsToDegradeBase + _hitsToDegradePerScore * _currentScore));

    public float DegradationFactor => Mathf.Clamp01((float)_rallyHits / HitsToDegrade);

    private void Start()
    {
        _paddle = GetComponent<Paddle>();
        _ball = GameManager.Instance._ball;
        _currentTargetY = transform.position.y;

        _paddle.OnReflect += HandlePaddleReflect;
        GameManager.OnSetControls += HandleSetControls;

        _currentScore = ScoreManager.Score;
        ResetRally();
        ScoreManager.OnScoreChanged += HandleScoreChanged;
    }

    private void FixedUpdate()
    {
        if (_ball == null || !_ball.Active)
        {
            _currentTargetY = Mathf.Lerp(_currentTargetY, 0f, 5f * Time.fixedDeltaTime);
            _paddle.SetTargetY(_currentTargetY);
            return;
        }

        Vector3 ballVel = _ball.Velocity;
        float paddleX = transform.position.x;
        bool isBallApproaching = (paddleX > 0 && ballVel.x > 0) || (paddleX < 0 && ballVel.x < 0);

        float degradation = DegradationFactor;
        difficulty = 1f - degradation;

        float yMin = GameManager.Instance.DownWallPoint.position.y;
        float yMax = GameManager.Instance.UpWallPoint.position.y;

        // Обработка момента, когда мяч только начал лететь в сторону бота
        if (isBallApproaching && !_wasBallApproaching)
        {
            _reactionTimer = 0f;
            _currentReactionDelay = degradation * _maxReactionDelay;

            // Вероятность промаха растёт по мере деградации (падения difficulty)
            float missChance = degradation;
            _willMissThisApproach = Random.value < missChance;

            float predictedY = PredictBallYAtPaddleX(yMin, yMax);
            if (_willMissThisApproach)
            {
                float missSign = (predictedY >= 0f) ? -1f : 1f;
                _targetErrorOffset = missSign * Random.Range(_minFatalMissOffset, _maxErrorOffset);
            }
            else
            {
                float maxEdgeOffset = degradation * 0.7f;
                _targetErrorOffset = Random.Range(-maxEdgeOffset, maxEdgeOffset);
            }
        }

        _wasBallApproaching = isBallApproaching;
        _lastBallDirX = ballVel.x;

        if (isBallApproaching)
        {
            _reactionTimer += Time.fixedDeltaTime;

            if (_reactionTimer >= _currentReactionDelay)
            {
                float predictedY = PredictBallYAtPaddleX(yMin, yMax);
                _currentErrorOffset = Mathf.Lerp(_currentErrorOffset, _targetErrorOffset, 6f * Time.fixedDeltaTime);
                _currentTargetY = predictedY + _currentErrorOffset;
            }
        }
        else
        {
            _currentErrorOffset = 0f;
            _currentTargetY = Mathf.Lerp(_currentTargetY, 0f, 3f * Time.fixedDeltaTime);
        }

        _paddle.SetTargetY(_currentTargetY);
    }

    private void OnDestroy()
    {
        if (_paddle != null)
            _paddle.OnReflect -= HandlePaddleReflect;

        GameManager.OnSetControls -= HandleSetControls;
        ScoreManager.OnScoreChanged -= HandleScoreChanged;
    }

    public void RecalculateDifficulty(int score)
    {
        _currentScore = score;
        ResetRally();
    }

    private void ResetRally()
    {
        _rallyHits = 0;
        _currentErrorOffset = 0f;
        _targetErrorOffset = 0f;
        _reactionTimer = 0f;
        _currentReactionDelay = 0f;
        _wasBallApproaching = false;
        _willMissThisApproach = false;

        float baseHits = _hitsToDegradeBase + _hitsToDegradePerScore * _currentScore;
        float minHits = baseHits * (1f - _randomizationPercent);
        float maxHits = baseHits * (1f + _randomizationPercent);
        _hitsToDegradeForCurrentRally = Mathf.Max(1, Mathf.RoundToInt(Random.Range(minHits, maxHits)));
    }

    private void HandlePaddleReflect()
    {
        _rallyHits++;
    }

    private void HandleSetControls(bool enable)
    {
        if (enable)
            ResetRally();
        else
        {
            _rallyHits = 0;
            _currentErrorOffset = 0f;
            _targetErrorOffset = 0f;
            _wasBallApproaching = false;
        }
    }

    private void HandleScoreChanged(int score)
    {
        _currentScore = score;
        ResetRally();
    }

    private float PredictBallYAtPaddleX(float yMin, float yMax)
    {
        Vector3 ballPos = _ball.transform.position;
        Vector3 ballVel = _ball.Velocity;

        float paddleX = transform.position.x;
        float sign = Mathf.Sign(paddleX);

        if (Mathf.Sign(ballVel.x) != sign)
            return (yMin + yMax) * 0.5f;

        if (Mathf.Abs(ballVel.x) < 0.001f)
            return ballPos.y;

        float timeToReach = (paddleX - ballPos.x) / ballVel.x;
        if (timeToReach <= 0f)
            return ballPos.y;

        float height = yMax - yMin;
        if (height <= 0f)
            return ballPos.y;

        float rawY = ballPos.y + ballVel.y * timeToReach;
        float normalizedY = rawY - yMin;

        return yMin + Mathf.PingPong(normalizedY, height);
    }
}