using UnityEngine;

namespace ImMentallyWell.TwoD
{
    /// <summary>
    /// Abstract base class for all 2D mini-games that run on the in-game computer.
    /// Each mini-game has a unique ID and, when completed, reveals a code for
    /// a specific 3D puzzle.
    /// </summary>
    public abstract class MiniGameBase : MonoBehaviour
    {
        [Header("Mini-Game Identity")]
        [Tooltip("Unique ID used to launch this game from the desktop UI.")]
        [SerializeField] private string gameId;

        [Tooltip("The 3D puzzle this game's code unlocks. Must match a PuzzleManager puzzleId.")]
        [SerializeField] private string linkedPuzzleId;

        [Tooltip("Human-readable name shown on the in-game desktop icon.")]
        [SerializeField] private string displayName;

        public string GameId => gameId;
        public string LinkedPuzzleId => linkedPuzzleId;
        public string DisplayName => displayName;

        protected System.Action<string, string> CompletionCallback;
        protected bool IsRunning { get; private set; }

        /// <summary>
        /// Called by <see cref="MiniGameController"/> to start this game.
        /// </summary>
        /// <param name="onCompleted">
        /// Invoke when the player wins: pass (codeRevealed, linkedPuzzleId).
        /// </param>
        public void StartGame(System.Action<string, string> onCompleted)
        {
            CompletionCallback = onCompleted;
            IsRunning = true;
            gameObject.SetActive(true);
            OnGameStart();
        }

        /// <summary>Called by <see cref="MiniGameController"/> to force-stop the game.</summary>
        public void StopGame()
        {
            if (!IsRunning) return;
            IsRunning = false;
            gameObject.SetActive(false);
            OnGameStop();
        }

        /// <summary>Implement game start logic here.</summary>
        protected abstract void OnGameStart();

        /// <summary>Implement cleanup logic here (called on force-stop or after completion).</summary>
        protected abstract void OnGameStop();

        /// <summary>
        /// Call this from the subclass when the player has won the mini-game.
        /// </summary>
        protected void CompleteGame(string revealedCode)
        {
            if (!IsRunning) return;
            IsRunning = false;
            gameObject.SetActive(false);
            CompletionCallback?.Invoke(revealedCode, linkedPuzzleId);
        }
    }
}
