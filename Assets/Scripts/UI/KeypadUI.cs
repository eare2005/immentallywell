using ImMentallyWell.Core;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ImMentallyWell.UI
{
    /// <summary>
    /// On-screen numeric keypad used by <see cref="ThreeD.EscapeRoomPuzzle"/>
    /// to let the player enter a code they discovered on the computer.
    /// </summary>
    public class KeypadUI : MonoBehaviour
    {
        public static KeypadUI Instance { get; private set; }

        [Header("UI References")]
        [SerializeField] private GameObject panel;
        [SerializeField] private TextMeshProUGUI displayText;
        [SerializeField] private TextMeshProUGUI feedbackText;
        [SerializeField] private Button[] digitButtons; // 0-9
        [SerializeField] private Button backspaceButton;
        [SerializeField] private Button submitButton;
        [SerializeField] private Button cancelButton;

        private string _currentInput = "";
        private int _codeLength;
        private string _activePuzzleId;
        private System.Action<string> _callback;

        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;

            panel.SetActive(false);

            for (int i = 0; i < digitButtons.Length; i++)
            {
                int digit = i; // capture
                digitButtons[i].onClick.AddListener(() => PressDigit(digit.ToString()));
            }
            backspaceButton.onClick.AddListener(Backspace);
            submitButton.onClick.AddListener(Submit);
            cancelButton.onClick.AddListener(Close);
        }

        // ─── Open / Close ─────────────────────────────────────────────────────

        /// <summary>
        /// Show the keypad for a specific puzzle.
        /// </summary>
        /// <param name="puzzleId">Which puzzle is being solved.</param>
        /// <param name="codeLength">Expected number of digits.</param>
        /// <param name="callback">Called with the entered string when submitted.</param>
        public void Open(string puzzleId, int codeLength, System.Action<string> callback)
        {
            _activePuzzleId = puzzleId;
            _codeLength = codeLength;
            _callback = callback;
            _currentInput = "";
            feedbackText.text = "";
            UpdateDisplay();
            panel.SetActive(true);

            // Disable player movement while the keypad is open.
            GameManager.Instance?.SetPlayerInputEnabled(false);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        public void Close()
        {
            panel.SetActive(false);
            GameManager.Instance?.SetPlayerInputEnabled(true);
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        // ─── Input ────────────────────────────────────────────────────────────

        private void PressDigit(string digit)
        {
            if (_currentInput.Length >= _codeLength) return;
            _currentInput += digit;
            UpdateDisplay();
        }

        private void Backspace()
        {
            if (_currentInput.Length == 0) return;
            _currentInput = _currentInput[..^1];
            UpdateDisplay();
        }

        private void Submit()
        {
            if (_currentInput.Length == 0) return;
            panel.SetActive(false);
            GameManager.Instance?.SetPlayerInputEnabled(true);
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            _callback?.Invoke(_currentInput);
        }

        private void UpdateDisplay()
        {
            // Show entered digits; pad remaining with dashes.
            string display = _currentInput.PadRight(_codeLength, '-');
            displayText.text = display;
        }
    }
}
