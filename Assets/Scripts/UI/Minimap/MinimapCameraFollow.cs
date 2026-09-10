using UnityEngine;

public class MinimapCameraFollow : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform target;

    private float height;

    private void Start()
    {
        height = transform.position.y;
    }

    private void LateUpdate()
    {
        if (target == null)
            return;

        transform.position = new Vector3(
            target.position.x,
            height,
            target.position.z
        );
    }
}
