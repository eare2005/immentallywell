using ImMentallyWell.Core;
using UnityEngine;
using UnityEngine.Events;

namespace ImMentallyWell.ThreeD
{
    /// <summary>
    /// A door (or any gate/barrier) that can be locked or unlocked.
    /// Locked state: blocks the player and shows a message.
    /// Unlocked state: opens when interacted with.
    ///
    /// Attach to a door GameObject. The door can be unlocked either:
    ///   - Automatically when a specific puzzle is solved (subscribe to PuzzleManager.OnPuzzleSolved)
    ///   - Manually via <see cref="Unlock"/> from a UnityEvent
    /// </summary>
    public class Door : InteractableObject
    {
        [Header("Door")]
        [Tooltip("If non-empty, this door automatically unlocks when the named puzzle is solved.")]
        [SerializeField] private string requiredPuzzleId = "";

        [Header("Animation")]
        [SerializeField] private Animator doorAnimator;
        [SerializeField] private string openTrigger = "Open";

        [Header("Escape door")]
        [Tooltip("If true, interacting while unlocked triggers the win state.")]
        [SerializeField] private bool isEscapeDoor;

        [Header("Events")]
        [SerializeField] private UnityEvent onUnlocked;
        [SerializeField] private UnityEvent onOpened;

        private bool _isUnlocked;
        private bool _isOpen;

        private void Start()
        {
            _isUnlocked = string.IsNullOrEmpty(requiredPuzzleId);
            UpdatePrompt();

            if (!string.IsNullOrEmpty(requiredPuzzleId))
                PuzzleManager.Instance.OnPuzzleSolved += OnPuzzleSolved;
        }

        private void OnDestroy()
        {
            if (PuzzleManager.Instance != null)
                PuzzleManager.Instance.OnPuzzleSolved -= OnPuzzleSolved;
        }

        protected override void OnInteract()
        {
            if (!_isUnlocked)
            {
                Debug.Log($"[Door] This door is locked. Solve puzzle '{requiredPuzzleId}' to open it.");
                return;
            }

            if (_isOpen) return;

            OpenDoor();
        }

        private void OpenDoor()
        {
            _isOpen = true;
            isInteractable = false;
            doorAnimator?.SetTrigger(openTrigger);
            Debug.Log("[Door] The door swings open!");
            onOpened.Invoke();

            if (isEscapeDoor)
                GameManager.Instance?.TriggerEscape();
        }

        public void Unlock()
        {
            if (_isUnlocked) return;
            _isUnlocked = true;
            UpdatePrompt();
            Debug.Log($"[Door] Door unlocked!");
            onUnlocked.Invoke();
        }

        private void OnPuzzleSolved(string puzzleId)
        {
            if (puzzleId == requiredPuzzleId)
                Unlock();
        }

        private void UpdatePrompt()
        {
            interactPrompt = _isUnlocked ? "Press [E] to open door" : "This door is locked";
        }
    }
}
