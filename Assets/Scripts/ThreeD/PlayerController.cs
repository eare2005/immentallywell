using UnityEngine;

namespace ImMentallyWell.ThreeD
{
    /// <summary>
    /// First-person player controller for the 3D escape-room exploration.
    /// Handles movement, looking, and interaction with objects in the room.
    /// </summary>
    [RequireComponent(typeof(CharacterController))]
    public class PlayerController : MonoBehaviour
    {
        [Header("Movement")]
        [SerializeField] private float moveSpeed = 3f;
        [SerializeField] private float gravity = -9.81f;

        [Header("Look")]
        [SerializeField] private Transform cameraTransform;
        [SerializeField] private float mouseSensitivity = 100f;
        [SerializeField] private float verticalLookClamp = 80f;

        [Header("Interaction")]
        [Tooltip("How far the player can reach to interact with objects.")]
        [SerializeField] private float interactRange = 2.5f;
        [SerializeField] private LayerMask interactableLayers;

        private CharacterController _controller;
        private Vector3 _velocity;
        private float _xRotation;
        private bool _isInputEnabled = true;

        private void Awake()
        {
            _controller = GetComponent<CharacterController>();
            LockCursor(true);
        }

        private void OnEnable()
        {
            Core.GameManager.Instance.OnModeChanged += HandleModeChanged;
            Core.GameManager.Instance.OnPlayerInputEnabled += SetInputEnabled;
        }

        private void OnDisable()
        {
            if (Core.GameManager.Instance != null)
            {
                Core.GameManager.Instance.OnModeChanged -= HandleModeChanged;
                Core.GameManager.Instance.OnPlayerInputEnabled -= SetInputEnabled;
            }
        }

        private void Update()
        {
            if (!_isInputEnabled) return;

            HandleMovement();
            HandleLook();
            HandleInteractionInput();
        }

        // ─── Movement ─────────────────────────────────────────────────────────

        private void HandleMovement()
        {
            float x = Input.GetAxis("Horizontal");
            float z = Input.GetAxis("Vertical");

            Vector3 move = transform.right * x + transform.forward * z;
            _controller.Move(move * moveSpeed * Time.deltaTime);

            if (_controller.isGrounded && _velocity.y < 0)
                _velocity.y = -2f;

            _velocity.y += gravity * Time.deltaTime;
            _controller.Move(_velocity * Time.deltaTime);
        }

        // ─── Camera look ──────────────────────────────────────────────────────

        private void HandleLook()
        {
            float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
            float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

            _xRotation -= mouseY;
            _xRotation = Mathf.Clamp(_xRotation, -verticalLookClamp, verticalLookClamp);

            cameraTransform.localRotation = Quaternion.Euler(_xRotation, 0f, 0f);
            transform.Rotate(Vector3.up * mouseX);
        }

        // ─── Interaction ──────────────────────────────────────────────────────

        private void HandleInteractionInput()
        {
            if (!Input.GetKeyDown(KeyCode.E)) return;

            Ray ray = new Ray(cameraTransform.position, cameraTransform.forward);
            if (Physics.Raycast(ray, out RaycastHit hit, interactRange, interactableLayers))
            {
                var interactable = hit.collider.GetComponent<InteractableObject>();
                interactable?.Interact();
            }
        }

        // ─── Mode switching ───────────────────────────────────────────────────

        private void HandleModeChanged(Core.GameManager.GameMode mode)
        {
            _isInputEnabled = mode == Core.GameManager.GameMode.ThreeD;
            LockCursor(_isInputEnabled);
        }

        private void SetInputEnabled(bool enabled)
        {
            _isInputEnabled = enabled;
            LockCursor(enabled);
        }

        private static void LockCursor(bool locked)
        {
            Cursor.lockState = locked ? CursorLockMode.Locked : CursorLockMode.None;
            Cursor.visible = !locked;
        }
    }
}
