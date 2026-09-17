using UnityEngine;

[RequireComponent(typeof(Paddle))]
public class BotPaddle : MonoBehaviour
{
    [Range(0f, 1f)]
    public float difficulty = 0.5f;

    [SerializeField] private float _gainBase = 1.5f;
    [SerializeField] private float _gainPerDifficulty = 5f;
    [SerializeField] private float _maxPredictionOffset = 2f;

    private Paddle _paddle;
    private Ball _ball;

    private void Start()
    {
        _paddle = GetComponent<Paddle>();
        _ball = GameManager.Instance._ball;

        RecalculateDifficulty(ScoreManager.Score);
        ScoreManager.OnScoreChanged += RecalculateDifficulty;
    }

    private void FixedUpdate()
    {
        if (_ball == null || !_ball.Active)
        {
            _paddle.SetDirection(0f);
            return;
        }

        float yMin = GameManager.Instance.DownWallPoint.position.y;
        float yMax = GameManager.Instance.UpWallPoint.position.y;

        float predictedY = PredictBallYAtPaddleX(yMin, yMax);

        float errorOffset = (1f - difficulty) * Random.Range(-_maxPredictionOffset, _maxPredictionOffset);
        float targetY = predictedY + errorOffset;

        float currentY = transform.position.y;

        float gain = _gainBase + difficulty * _gainPerDifficulty;
        float direction = Mathf.Clamp((targetY - currentY) * gain, -1f, 1f);

        float deadZone = Mathf.Lerp(0.3f, 0.05f, difficulty);
        if (Mathf.Abs(targetY - currentY) < deadZone)
            direction = 0f;

        _paddle.SetDirection(direction);
    }

    private void OnDestroy()
    {
        ScoreManager.OnScoreChanged -= RecalculateDifficulty;
    }

    private void RecalculateDifficulty(int score)
    {
        difficulty = Mathf.Clamp01(score / 10f);
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