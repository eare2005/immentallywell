using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using ImMentallyWell.ThreeD;
using ImMentallyWell.Core;

namespace ImMentallyWell.UI
{
    /// <summary>
    /// Heads-Up Display for the 3D exploration mode.
    /// Shows the interaction prompt when the player aims at an interactable object,
    /// and a list of discovered codes as a notebook the player can toggle.
    /// </summary>
    public class HUD : MonoBehaviour
    {
        [Header("Interaction Prompt")]
        [SerializeField] private GameObject promptPanel;
        [SerializeField] private TextMeshProUGUI promptText;

        [Header("Notebook Overlay")]
        [SerializeField] private GameObject notebookPanel;
        [SerializeField] private TextMeshProUGUI notebookText;

        [Header("Settings")]
        [SerializeField] private float interactRange = 2.5f;
        [SerializeField] private LayerMask interactableLayers;
        [SerializeField] private Transform cameraTransform;

        private bool _notebookOpen;
        private readonly List<string> _codeLog = new();

        private void Start()
        {
            promptPanel.SetActive(false);
            notebookPanel.SetActive(false);

            PuzzleManager.Instance.OnCodeDiscovered += OnCodeDiscovered;
        }

        private void OnDestroy()
        {
            if (PuzzleManager.Instance != null)
                PuzzleManager.Instance.OnCodeDiscovered -= OnCodeDiscovered;
        }

        private void OnCodeDiscovered(string puzzleId, string code)
        {
            _codeLog.Add($"[{puzzleId}] → {code}");
            if (_notebookOpen)
                RefreshNotebook();
        }

        private void Update()
        {
            UpdateInteractPrompt();

            if (Input.GetKeyDown(KeyCode.Tab))
                ToggleNotebook();
        }

        // ─── Interaction prompt ───────────────────────────────────────────────

        private void UpdateInteractPrompt()
        {
            Ray ray = new Ray(cameraTransform.position, cameraTransform.forward);
            if (Physics.Raycast(ray, out RaycastHit hit, interactRange, interactableLayers))
            {
                var interactable = hit.collider.GetComponent<InteractableObject>();
                if (interactable != null && interactable.IsInteractable)
                {
                    promptText.text = interactable.InteractPrompt;
                    promptPanel.SetActive(true);
                    return;
                }
            }
            promptPanel.SetActive(false);
        }

        // ─── Notebook ─────────────────────────────────────────────────────────

        private void ToggleNotebook()
        {
            _notebookOpen = !_notebookOpen;
            notebookPanel.SetActive(_notebookOpen);
            if (_notebookOpen) RefreshNotebook();
        }

        private void RefreshNotebook()
        {
            if (_codeLog.Count == 0)
            {
                notebookText.text = "No codes found yet.\nPlay the games on the computer!";
                return;
            }
            notebookText.text = "Discovered codes:\n\n" + string.Join("\n", _codeLog);
        }
    }
}
