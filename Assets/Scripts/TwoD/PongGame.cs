using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace ImMentallyWell.TwoD
{
    /// <summary>
    /// A simple Pong-style 2D mini-game.
    ///
    /// The player controls a paddle at the bottom of the screen and must bounce
    /// a ball past the CPU paddle at the top. Reaching the win score reveals
    /// the hidden code for the linked 3D puzzle.
    /// </summary>
    public class PongGame : MiniGameBase
    {
        [Header("Pong Game – Settings")]
        [Tooltip("The code revealed when the player wins this game.")]
        [SerializeField] private string hiddenCode = "2358";

        [Tooltip("Score the player must reach to win.")]
        [SerializeField] private int winScore = 3;

        [Tooltip("Ball speed (units per second).")]
        [SerializeField] private float ballSpeed = 5f;

        [Tooltip("Player paddle speed.")]
        [SerializeField] private float paddleSpeed = 6f;

        [Header("Scene References")]
        [SerializeField] private RectTransform ball;
        [SerializeField] private RectTransform playerPaddle;
        [SerializeField] private RectTransform cpuPaddle;
        [SerializeField] private RectTransform playfield;

        [Header("UI")]
        [SerializeField] private TextMeshProUGUI scoreText;
        [SerializeField] private TextMeshProUGUI messageText;
        [SerializeField] private GameObject revealPanel;
        [SerializeField] private TextMeshProUGUI revealCodeText;

        private Vector2 _ballVelocity;
        private int _playerScore;
        private int _cpuScore;
        private bool _gameActive;
        private Coroutine _gameLoop;

        // ─── MiniGameBase overrides ───────────────────────────────────────────

        protected override void OnGameStart()
        {
            _playerScore = 0;
            _cpuScore = 0;
            revealPanel.SetActive(false);
            messageText.gameObject.SetActive(false);
            UpdateScoreUI();
            _gameLoop = StartCoroutine(GameLoop());
        }

        protected override void OnGameStop()
        {
            _gameActive = false;
            if (_gameLoop != null)
            {
                StopCoroutine(_gameLoop);
                _gameLoop = null;
            }
        }

        // ─── Game loop ────────────────────────────────────────────────────────

        private IEnumerator GameLoop()
        {
            yield return StartCoroutine(ServeBall());

            while (true)
            {
                yield return null; // movement is handled in UpdateBall / UpdatePaddles
            }
        }

        private IEnumerator ServeBall()
        {
            _gameActive = false;
            messageText.gameObject.SetActive(true);
            messageText.text = "Get ready…";
            yield return new WaitForSeconds(1.5f);
            messageText.gameObject.SetActive(false);

            // Reset positions.
            ball.anchoredPosition = Vector2.zero;
            float angle = Random.Range(30f, 60f) * (Random.value > 0.5f ? 1 : -1);
            float rad = angle * Mathf.Deg2Rad;
            _ballVelocity = new Vector2(Mathf.Sin(rad), Mathf.Cos(rad)) * ballSpeed;
            _gameActive = true;
        }

        private void Update()
        {
            if (!_gameActive) return;
            UpdateBall();
            UpdatePlayerPaddle();
            UpdateCpuPaddle();
        }

        private void UpdateBall()
        {
            Rect field = playfield.rect;
            Vector2 pos = ball.anchoredPosition + _ballVelocity * Time.deltaTime;

            // Wall bounce (left / right).
            float halfW = field.width * 0.5f - ball.rect.width * 0.5f;
            if (Mathf.Abs(pos.x) >= halfW)
            {
                _ballVelocity.x *= -1;
                pos.x = Mathf.Sign(pos.x) * halfW;
            }

            float halfH = field.height * 0.5f;

            // Check paddle collisions.
            CheckPaddleCollision(ref pos, ball, playerPaddle, -halfH + playerPaddle.rect.height, ref _ballVelocity, 1f);
            CheckPaddleCollision(ref pos, ball, cpuPaddle, halfH - cpuPaddle.rect.height, ref _ballVelocity, -1f);

            // Scoring.
            if (pos.y > halfH)
            {
                _playerScore++;
                UpdateScoreUI();
                StartCoroutine(CheckWinOrServe());
                return;
            }
            if (pos.y < -halfH)
            {
                _cpuScore++;
                UpdateScoreUI();
                StartCoroutine(CheckWinOrServe());
                return;
            }

            ball.anchoredPosition = pos;
        }

        private static void CheckPaddleCollision(ref Vector2 pos, RectTransform ball,
            RectTransform paddle, float paddleY, ref Vector2 velocity, float directionSign)
        {
            float halfPaddleW = paddle.rect.width * 0.5f;
            float halfBallH = ball.rect.height * 0.5f;

            bool nearY = directionSign > 0
                ? pos.y - halfBallH <= paddleY + paddle.rect.height && pos.y >= paddleY
                : pos.y + halfBallH >= paddleY - paddle.rect.height && pos.y <= paddleY;

            bool withinX = Mathf.Abs(pos.x - paddle.anchoredPosition.x) < halfPaddleW;

            if (nearY && withinX)
            {
                velocity.y *= -1;
                pos.y = paddleY + (directionSign > 0 ? paddle.rect.height + halfBallH : -paddle.rect.height - halfBallH);
            }
        }

        private void UpdatePlayerPaddle()
        {
            float input = Input.GetAxis("Horizontal");
            Vector2 pos = playerPaddle.anchoredPosition;
            float halfFieldW = playfield.rect.width * 0.5f - playerPaddle.rect.width * 0.5f;
            pos.x = Mathf.Clamp(pos.x + input * paddleSpeed * Time.deltaTime * 100f, -halfFieldW, halfFieldW);
            playerPaddle.anchoredPosition = pos;
        }

        private void UpdateCpuPaddle()
        {
            // Simple CPU: follow the ball horizontally.
            Vector2 pos = cpuPaddle.anchoredPosition;
            float halfFieldW = playfield.rect.width * 0.5f - cpuPaddle.rect.width * 0.5f;
            float target = Mathf.Clamp(ball.anchoredPosition.x, -halfFieldW, halfFieldW);
            pos.x = Mathf.MoveTowards(pos.x, target, paddleSpeed * 0.6f * Time.deltaTime * 100f);
            cpuPaddle.anchoredPosition = pos;
        }

        // ─── Score & win ──────────────────────────────────────────────────────

        private void UpdateScoreUI()
        {
            scoreText.text = $"You: {_playerScore}  |  CPU: {_cpuScore}";
        }

        private IEnumerator CheckWinOrServe()
        {
            _gameActive = false;
            if (_playerScore >= winScore)
            {
                revealPanel.SetActive(true);
                revealCodeText.text = $"CODE: {hiddenCode}";
                yield return new WaitForSeconds(3f);
                CompleteGame(hiddenCode);
            }
            else if (_cpuScore >= winScore)
            {
                messageText.gameObject.SetActive(true);
                messageText.text = "CPU wins! Try again.";
                _playerScore = 0;
                _cpuScore = 0;
                UpdateScoreUI();
                yield return new WaitForSeconds(1.5f);
                messageText.gameObject.SetActive(false);
                yield return StartCoroutine(ServeBall());
            }
            else
            {
                yield return StartCoroutine(ServeBall());
            }
        }
    }
}
