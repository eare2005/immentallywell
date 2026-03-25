using UnityEngine;
using UnityEngine.Events;

namespace ImMentallyWell.ThreeD
{
    /// <summary>
    /// Base class for anything the player can interact with in the 3D world.
    /// Attach to furniture, doors, notes, computers, etc.
    /// </summary>
    public abstract class InteractableObject : MonoBehaviour
    {
        [Header("Interaction")]
        [Tooltip("Short label shown in the HUD when the player aims at this object.")]
        [SerializeField] protected string interactPrompt = "Press [E] to interact";

        [Tooltip("Whether the player can currently interact with this object.")]
        [SerializeField] protected bool isInteractable = true;

        [Header("Events")]
        [SerializeField] private UnityEvent onInteracted;

        public string InteractPrompt => interactPrompt;
        public bool IsInteractable => isInteractable;

        /// <summary>Called by <see cref="PlayerController"/> when the player presses E.</summary>
        public void Interact()
        {
            if (!isInteractable) return;
            OnInteract();
            onInteracted.Invoke();
        }

        /// <summary>Override in subclasses to implement interaction behaviour.</summary>
        protected abstract void OnInteract();

        /// <summary>Enable or disable interaction at runtime (e.g. after a door is unlocked).</summary>
        public void SetInteractable(bool value) => isInteractable = value;
    }
}
