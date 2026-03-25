using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace ImMentallyWell.TwoD
{
    /// <summary>
    /// A simple 2D number-sequence mini-game.
    ///
    /// The player is shown a series of numbers that flash on screen one at a time.
    /// They must remember and type the sequence to receive the hidden code for the
    /// corresponding 3D puzzle.
    ///
    /// This is one of potentially many mini-games available on the in-game computer.
    /// </summary>
    public class CodeGeneratorGame : MiniGameBase
    {
        [Header("Code Generator Game – Settings")]
        [Tooltip("The 4-digit (or longer) code this game will reveal when the player wins.")]
        [SerializeField] private string hiddenCode = "7491";

        [Tooltip("Number of digits in the sequence the player must memorise.")]
        [SerializeField] private int sequenceLength = 4;

        [Tooltip("How long each number flashes on screen (seconds).")]
        [SerializeField] private float flashDuration = 0.8f;

        [Tooltip("Gap between flashes (seconds).")]
        [SerializeField] private float flashGap = 0.3f;

        [Header("UI References")]
        [SerializeField] private TextMeshProUGUI instructionText;
        [SerializeField] private TextMeshProUGUI flashDisplay;
        [SerializeField] private TMP_InputField playerInput;
        [SerializeField] private Button submitButton;
        [SerializeField] private TextMeshProUGUI feedbackText;
        [SerializeField] private GameObject revealPanel;
        [SerializeField] private TextMeshProUGUI revealCodeText;

        private int[] _sequence;
        private string _sequenceString;
        private Coroutine _flashRoutine;

        // ─── MiniGameBase overrides ───────────────────────────────────────────

        protected override void OnGameStart()
        {
            GenerateSequence();
            ResetUI();
            submitButton.onClick.AddListener(OnSubmit);
            _flashRoutine = StartCoroutine(PlayFlashSequence());
        }

        protected override void OnGameStop()
        {
            submitButton.onClick.RemoveListener(OnSubmit);
            if (_flashRoutine != null)
            {
                StopCoroutine(_flashRoutine);
                _flashRoutine = null;
            }
        }

        // ─── Sequence generation ──────────────────────────────────────────────

        private void GenerateSequence()
        {
            _sequence = new int[sequenceLength];
            var sb = new System.Text.StringBuilder();
            for (int i = 0; i < sequenceLength; i++)
            {
                _sequence[i] = Random.Range(0, 10);
                sb.Append(_sequence[i]);
            }
            _sequenceString = sb.ToString();
        }

        // ─── Flash sequence coroutine ─────────────────────────────────────────

        private IEnumerator PlayFlashSequence()
        {
            instructionText.text = "Watch the sequence!";
            playerInput.interactable = false;
            submitButton.interactable = false;
            flashDisplay.gameObject.SetActive(true);

            yield return new WaitForSeconds(0.5f);

            for (int i = 0; i < sequenceLength; i++)
            {
                flashDisplay.text = _sequence[i].ToString();
                yield return new WaitForSeconds(flashDuration);
                flashDisplay.text = "";
                yield return new WaitForSeconds(flashGap);
            }

            flashDisplay.gameObject.SetActive(false);
            instructionText.text = "Now type the sequence you saw:";
            playerInput.interactable = true;
            submitButton.interactable = true;
            playerInput.Select();
        }

        // ─── Submission ───────────────────────────────────────────────────────

        private void OnSubmit()
        {
            string entered = playerInput.text.Trim();
            feedbackText.gameObject.SetActive(true);

            if (entered == _sequenceString)
            {
                feedbackText.text = "Correct! The hidden code has been unlocked.";
                feedbackText.color = Color.green;
                ShowRevealPanel();
            }
            else
            {
                feedbackText.text = $"Wrong! The sequence was: {_sequenceString}. Try again.";
                feedbackText.color = Color.red;
                playerInput.text = "";

                // Re-play the sequence after a short delay.
                if (_flashRoutine != null) StopCoroutine(_flashRoutine);
                _flashRoutine = StartCoroutine(DelayedReplay());
            }
        }

        private IEnumerator DelayedReplay()
        {
            yield return new WaitForSeconds(1.5f);
            feedbackText.gameObject.SetActive(false);
            _flashRoutine = StartCoroutine(PlayFlashSequence());
        }

        // ─── Reveal panel ─────────────────────────────────────────────────────

        private void ShowRevealPanel()
        {
            revealPanel.SetActive(true);
            revealCodeText.text = $"CODE: {hiddenCode}";
            submitButton.interactable = false;
            playerInput.interactable = false;

            // Complete after a brief pause so the player can read the code.
            StartCoroutine(CompleteAfterDelay(3f));
        }

        private IEnumerator CompleteAfterDelay(float delay)
        {
            yield return new WaitForSeconds(delay);
            CompleteGame(hiddenCode);
        }

        // ─── UI helpers ───────────────────────────────────────────────────────

        private void ResetUI()
        {
            feedbackText.gameObject.SetActive(false);
            revealPanel.SetActive(false);
            playerInput.text = "";
            playerInput.interactable = false;
            submitButton.interactable = false;
            flashDisplay.text = "";
        }
    }
}
