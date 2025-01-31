using UnityEngine;

public class PlayerHandle : MonoBehaviour
{
    public Animator animator;        // Reference to the Animator component
    public float moveSpeed = 5f;     // Movement speed of the player
    private Vector3 targetPosition;  // Position to move towards
    private bool isMoving = false;   // Is the character currently moving?

    void Start()
    {
        targetPosition = transform.position;  // Start at the current position
    }

    void Update()
    {
        // Check if the character is close enough to stop moving
        if (isMoving && Vector3.Distance(transform.position, targetPosition) < 0.1f)
        {
            isMoving = false;
            animator.SetBool("isWalking", false);  // Set to idle when not moving
        }

        // Move character towards the target position
        if (isMoving)
        {
            MoveToPosition(targetPosition);
        }
    }

    // Set a new target position
    public void MoveToPosition(Vector3 newPosition)
    {
        targetPosition = newPosition;
        isMoving = true;
        animator.SetBool("isWalking", true);  // Trigger walking animation
        FaceMovementDirection();
    }

    // Move towards the target position and face the direction of movement
    private void MoveToPosition()
    {
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
    }

    // Rotate the character to face the direction of movement
    private void FaceMovementDirection()
    {
        Vector3 direction = targetPosition - transform.position;
        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
        }
    }
}
