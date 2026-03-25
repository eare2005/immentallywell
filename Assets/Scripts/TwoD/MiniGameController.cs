using ImMentallyWell.Core;
using UnityEngine;

namespace ImMentallyWell.TwoD
{
    /// <summary>
    /// Scene controller for the 2D computer screen layer.
    /// Manages which mini-game is currently running and handles returning
    /// to the 3D world.
    /// </summary>
    public class MiniGameController : MonoBehaviour
    {
        // Set by ComputerTerminal before the scene is loaded.
        public static string PendingTerminalId { get; set; } = "";

        [Header("Mini-Games")]
        [Tooltip("All mini-games available on this computer. Each must have a unique gameId.")]
        [SerializeField] private MiniGameBase[] availableGames;

        [Header("UI")]
        [Tooltip("The desktop / launcher UI that shows before a mini-game starts.")]
        [SerializeField] private GameObject desktopUI;

        private MiniGameBase _activeGame;
        private string _terminalId;

        private void Start()
        {
            _terminalId = PendingTerminalId;
            ShowDesktop();
        }

        private void Update()
        {
            // Escape key exits the computer and returns to 3D mode.
            if (Input.GetKeyDown(KeyCode.Escape))
                ExitComputer();
        }

        // ─── Desktop / launcher ───────────────────────────────────────────────

        public void ShowDesktop()
        {
            StopActiveGame();
            desktopUI?.SetActive(true);
        }

        /// <summary>Launch a mini-game by its ID (called from the desktop UI buttons).</summary>
        public void LaunchGame(string gameId)
        {
            foreach (var game in availableGames)
            {
                if (game.GameId == gameId)
                {
                    StartGame(game);
                    return;
                }
            }
            Debug.LogWarning($"[MiniGameController] No game found with id '{gameId}'.");
        }

        // ─── Game lifecycle ───────────────────────────────────────────────────

        private void StartGame(MiniGameBase game)
        {
            StopActiveGame();
            desktopUI?.SetActive(false);
            _activeGame = game;
            _activeGame.StartGame(OnGameCompleted);
        }

        private void StopActiveGame()
        {
            if (_activeGame == null) return;
            _activeGame.StopGame();
            _activeGame = null;
        }

        private void OnGameCompleted(string codeRevealed, string puzzleId)
        {
            Debug.Log($"[MiniGameController] Game completed – code '{codeRevealed}' for puzzle '{puzzleId}'.");
            PuzzleManager.Instance?.DiscoverCode(puzzleId, codeRevealed);
            ShowDesktop();
        }

        // ─── Exit ─────────────────────────────────────────────────────────────

        public void ExitComputer()
        {
            StopActiveGame();
            GameManager.Instance?.ExitComputerMode();
        }
    }
}
