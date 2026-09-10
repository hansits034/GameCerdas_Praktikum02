using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;

    [Header("Rotation Settings")]
    [SerializeField] private float rotationSpeed = 10f;

    [Header("Animation")]
    [SerializeField] private Animator _animator;
    private Vector3 movement;

    private void Update()
    {
        MovePlayer();

        UpdateAnimation();
    }

    private void MovePlayer()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        movement = new Vector3(
            horizontal,
            0f,
            vertical
        ).normalized;

        // Gerakkan Player
        transform.position +=
            movement *
            moveSpeed *
            Time.deltaTime;

        // Putar Player mengikuti arah gerakan
        if (movement != Vector3.zero)
        {
            Quaternion targetRotation =
                Quaternion.LookRotation(movement);

            transform.rotation =
                Quaternion.Slerp(
                    transform.rotation,
                    targetRotation,
                    rotationSpeed * Time.deltaTime
                );
        }
    }

    private void UpdateAnimation()
    {
        _animator.SetFloat("Movement", movement.magnitude * moveSpeed);
    }
}