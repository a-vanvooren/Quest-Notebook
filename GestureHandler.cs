// Assets/QuestNotebook/Scripts/GestureHandler.cs
using UnityEngine;

/// <summary>
/// Attach to the NotebookWindow root. Assign leftHand and rightHand transforms (e.g., controller pointers).
/// Pinch/grab input is abstracted to simple bools you can set from XR input or temporary keys.
/// When both hands are "grabbing", the note follows the midpoint and scales with hand distance.
/// </summary>
public class GestureHandler : MonoBehaviour
{
    public Transform leftHand;
    public Transform rightHand;

    [Tooltip("Simulate grab in Editor with keys (G for left, H for right). Replace with XR Input in production.")]
    public bool leftGrabbingSim;
    public bool rightGrabbingSim;

    private bool leftGrabbing;
    private bool rightGrabbing;

    private float initialDistance;
    private Vector3 initialScale;
    private Vector3 initialMidpoint;
    private Vector3 initialPosition;

    private void Update()
    {
        // Sim inputs in Editor:
        if (Application.isEditor)
        {
            if (Input.GetKeyDown(KeyCode.G)) leftGrabbingSim = !leftGrabbingSim;
            if (Input.GetKeyDown(KeyCode.H)) rightGrabbingSim = !rightGrabbingSim;
        }

        leftGrabbing = leftGrabbingSim;  // Replace with XR button/pinch state
        rightGrabbing = rightGrabbingSim;

        if (leftGrabbing && rightGrabbing && leftHand != null && rightHand != null)
        {
            var midpoint = (leftHand.position + rightHand.position) * 0.5f;
            var dist = Vector3.Distance(leftHand.position, rightHand.position);

            if (initialDistance == 0f)
            {
                initialDistance = dist;
                initialScale = transform.localScale;
                initialMidpoint = midpoint;
                initialPosition = transform.position;
            }

            // Move
            var deltaMid = midpoint - initialMidpoint;
            transform.position = initialPosition + deltaMid;

            // Scale
            if (initialDistance > 0.0001f)
            {
                float s = dist / initialDistance;
                transform.localScale = initialScale * s;
            }

            // Face the user gently
            if (Camera.main != null)
            {
                var fwd = (transform.position - Camera.main.transform.position).normalized;
                fwd.y = 0f;
                if (fwd.sqrMagnitude > 0.0001f)
                    transform.rotation = Quaternion.LookRotation(fwd, Vector3.up);
            }
        }
        else
        {
            initialDistance = 0f;
        }
    }
}
