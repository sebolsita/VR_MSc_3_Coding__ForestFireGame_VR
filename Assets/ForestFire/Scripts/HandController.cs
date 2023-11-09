using System.Collections; // Import the System.Collections namespace for working with coroutines.
using System.Collections.Generic; // Import the System.Collections.Generic namespace for working with lists.
using UnityEngine; // Import the UnityEngine namespace for Unity functionality.
using UnityEngine.InputSystem; // Import the InputSystem namespace for handling input actions.

public class HandController : MonoBehaviour
{
    public InputActionProperty pinchAnimationAction; // InputActionProperty for pinch animation control.
    public InputActionProperty gripAnimationAction; // InputActionProperty for grip animation control.

    public Animator handAnimator; // Reference to the Animator component for hand animation.

    // Start is called before the first frame update
    void Start()
    {
        // Initialization logic can be added here if needed.
    }

    // Update is called once per frame
    void Update()
    {
        float triggerValue = pinchAnimationAction.action.ReadValue<float>(); // Read the pinch animation action value.
        handAnimator.SetFloat("Trigger", triggerValue); // Set the "Trigger" parameter in the animator.

        float gripValue = gripAnimationAction.action.ReadValue<float>(); // Read the grip animation action value.
        handAnimator.SetFloat("Grip", gripValue); // Set the "Grip" parameter in the animator.
    }
}
