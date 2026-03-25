using UnityEngine;
using UnityEngine.SceneManagement;

namespace ImMentallyWell.Core
{
    /// <summary>
    /// Central game state manager.
    /// Handles switching between 3D exploration mode and 2D mini-game mode,
    /// and tracks the overall escape room progress.
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        public enum GameMode
        {
            ThreeD,   // Player is exploring the room in 3D
            TwoD      // Player is interacting with the in-game computer (2D mini-games)
        }

        public static GameManager Instance { get; private set; }

        [Header("Scene Names")]
        [Tooltip("The name of the main 3D exploration scene.")]
        [SerializeField] private string threeDSceneName = "MainRoom";
        [Tooltip("The name of the 2D computer / mini-game scene.")]
        [SerializeField] private string twoDSceneName = "ComputerScreen";

        [Header("State")]
        public GameMode CurrentMode { get; private set; } = GameMode.ThreeD;

        /// <summary>Raised whenever the game mode switches.</summary>
        public event System.Action<GameMode> OnModeChanged;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        /// <summary>
        /// Switch to the 2D computer mini-game view.
        /// Called when the player sits down at the in-game computer.
        /// </summary>
        public void EnterComputerMode()
        {
            if (CurrentMode == GameMode.TwoD) return;

            CurrentMode = GameMode.TwoD;
            OnModeChanged?.Invoke(CurrentMode);
            SceneManager.LoadSceneAsync(twoDSceneName, LoadSceneMode.Additive);
        }

        /// <summary>
        /// Return to the 3D exploration mode.
        /// Called when the player exits the computer.
        /// </summary>
        public void ExitComputerMode()
        {
            if (CurrentMode == GameMode.ThreeD) return;

            CurrentMode = GameMode.ThreeD;
            OnModeChanged?.Invoke(CurrentMode);
            SceneManager.UnloadSceneAsync(twoDSceneName);
        }

        /// <summary>
        /// Enable or disable player movement input without affecting time scale.
        /// Used by UI overlays (e.g. the keypad) that need the cursor free.
        /// </summary>
        public void SetPlayerInputEnabled(bool enabled)
        {
            OnPlayerInputEnabled?.Invoke(enabled);
        }

        /// <summary>
        /// Raised when player input should be enabled or disabled.
        /// <see cref="ThreeD.PlayerController"/> subscribes to this.
        /// </summary>
        public event System.Action<bool> OnPlayerInputEnabled;

        /// <summary>Restart the game from the beginning.</summary>
        public void RestartGame()
        {
            PuzzleManager.Instance?.ResetAllPuzzles();
            SceneManager.LoadScene(threeDSceneName);
            CurrentMode = GameMode.ThreeD;
        }

        /// <summary>Trigger the win state – the player has escaped!</summary>
        public void TriggerEscape()
        {
            Debug.Log("You escaped! Congratulations!");
            // TODO: Load a win / credits scene
        }
    }
}
