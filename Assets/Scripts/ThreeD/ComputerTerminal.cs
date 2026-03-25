using ImMentallyWell.Core;
using ImMentallyWell.TwoD;
using UnityEngine;

namespace ImMentallyWell.ThreeD
{
    /// <summary>
    /// A computer terminal in the 3D world.
    /// When the player interacts with it, the game switches to 2D computer mode.
    /// </summary>
    public class ComputerTerminal : InteractableObject
    {
        [Header("Computer Terminal")]
        [Tooltip("Unique ID passed to the 2D mini-game screen so it knows which terminal launched it.")]
        [SerializeField] private string terminalId = "main_terminal";

        [Header("Visual Feedback")]
        [SerializeField] private Renderer screenRenderer;
        [SerializeField] private Material screenOnMaterial;
        [SerializeField] private Material screenOffMaterial;

        private bool _isPoweredOn = true;

        private void Start()
        {
            UpdateScreenVisual();
        }

        protected override void OnInteract()
        {
            if (!_isPoweredOn)
            {
                Debug.Log($"[ComputerTerminal] Terminal '{terminalId}' is powered off.");
                return;
            }

            Debug.Log($"[ComputerTerminal] Sitting at terminal '{terminalId}'.");

            // Tell the 2D mini-game layer which terminal the player is at.
            MiniGameController.PendingTerminalId = terminalId;
            GameManager.Instance.EnterComputerMode();
        }

        // ─── Power ────────────────────────────────────────────────────────────

        public void SetPowered(bool on)
        {
            _isPoweredOn = on;
            interactPrompt = on ? "Press [E] to use computer" : "The computer is off";
            UpdateScreenVisual();
        }

        private void UpdateScreenVisual()
        {
            if (screenRenderer == null) return;
            screenRenderer.material = _isPoweredOn ? screenOnMaterial : screenOffMaterial;
        }
    }
}
