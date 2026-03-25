using System.Collections.Generic;
using ImMentallyWell.Core;
using ImMentallyWell.TwoD;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ImMentallyWell.UI
{
    /// <summary>
    /// Controls the in-game computer desktop UI.
    /// Shows icons for available mini-games and a notebook listing discovered codes.
    /// </summary>
    public class TerminalUI : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private MiniGameController miniGameController;

        [Header("Desktop Icons")]
        [Tooltip("Parent transform that holds the auto-generated game icon buttons.")]
        [SerializeField] private Transform iconContainer;
        [SerializeField] private Button gameIconPrefab;

        [Header("Code Notebook")]
        [SerializeField] private GameObject notebookPanel;
        [SerializeField] private TextMeshProUGUI notebookText;
        [SerializeField] private Button notebookButton;
        [SerializeField] private Button closeNotebookButton;

        [Header("Exit")]
        [SerializeField] private Button exitButton;

        private readonly List<string> _discoveredCodesLog = new();

        private void Start()
        {
            exitButton.onClick.AddListener(miniGameController.ExitComputer);
            notebookButton.onClick.AddListener(ToggleNotebook);
            closeNotebookButton.onClick.AddListener(() => notebookPanel.SetActive(false));
            notebookPanel.SetActive(false);

            PuzzleManager.Instance.OnCodeDiscovered += OnCodeDiscovered;
            BuildGameIcons();
        }

        private void OnDestroy()
        {
            if (PuzzleManager.Instance != null)
                PuzzleManager.Instance.OnCodeDiscovered -= OnCodeDiscovered;
        }

        // ─── Desktop icons ────────────────────────────────────────────────────

        private void BuildGameIcons()
        {
            var games = miniGameController.GetComponentsInChildren<MiniGameBase>(includeInactive: true);
            foreach (var game in games)
            {
                Button icon = Instantiate(gameIconPrefab, iconContainer);
                var label = icon.GetComponentInChildren<TextMeshProUGUI>();
                if (label != null) label.text = game.DisplayName;

                string capturedId = game.GameId; // capture for closure
                icon.onClick.AddListener(() => miniGameController.LaunchGame(capturedId));
            }
        }

        // ─── Notebook ─────────────────────────────────────────────────────────

        private void ToggleNotebook()
        {
            notebookPanel.SetActive(!notebookPanel.activeSelf);
            if (notebookPanel.activeSelf)
                RefreshNotebook();
        }

        private void OnCodeDiscovered(string puzzleId, string code)
        {
            _discoveredCodesLog.Add($"[{puzzleId}] → {code}");
            if (notebookPanel.activeSelf)
                RefreshNotebook();
        }

        private void RefreshNotebook()
        {
            if (_discoveredCodesLog.Count == 0)
            {
                notebookText.text = "No codes found yet.\nPlay the games on this computer!";
                return;
            }
            notebookText.text = "Discovered codes:\n\n" + string.Join("\n", _discoveredCodesLog);
        }
    }
}
