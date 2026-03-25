using ImMentallyWell.Core;
using ImMentallyWell.UI;
using UnityEngine;
using UnityEngine.Events;

namespace ImMentallyWell.ThreeD
{
    /// <summary>
    /// A puzzle in the 3D world (e.g. a combination lock, a door keypad, a safe)
    /// that is solved by entering a code discovered in a 2D mini-game.
    /// </summary>
    public class EscapeRoomPuzzle : InteractableObject
    {
        [Header("Puzzle")]
        [Tooltip("Must match the puzzleId used when DiscoverCode() was called in the 2D game.")]
        [SerializeField] private string puzzleId = "safe_code";

        [Tooltip("Maximum digits the player can enter before the attempt is evaluated.")]
        [SerializeField] private int codeLength = 4;

        [Header("Events")]
        [SerializeField] private UnityEvent onSolved;
        [SerializeField] private UnityEvent onFailed;

        private string _currentInput = "";
        private bool _isSolved;

        protected override void OnInteract()
        {
            if (_isSolved)
            {
                Debug.Log($"[EscapeRoomPuzzle] '{puzzleId}' is already solved.");
                return;
            }

            if (!PuzzleManager.Instance.HasCode(puzzleId))
            {
                Debug.Log($"[EscapeRoomPuzzle] You haven't found the code for '{puzzleId}' yet. " +
                          "Check the computer games!");
                return;
            }

            // Open the on-screen keypad UI.
            KeypadUI.Instance?.Open(puzzleId, codeLength, OnCodeEntered);
        }

        private void OnCodeEntered(string enteredCode)
        {
            if (PuzzleManager.Instance.TrySolvePuzzle(puzzleId, enteredCode))
            {
                _isSolved = true;
                isInteractable = false;
                Debug.Log($"[EscapeRoomPuzzle] '{puzzleId}' solved! The lock opens.");
                onSolved.Invoke();
            }
            else
            {
                Debug.Log($"[EscapeRoomPuzzle] Wrong code for '{puzzleId}'.");
                onFailed.Invoke();
            }
        }
    }
}
