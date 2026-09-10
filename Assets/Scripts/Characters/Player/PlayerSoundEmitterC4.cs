using UnityEngine;

public class PlayerSoundEmitterC4 : MonoBehaviour
{
    [Header("Sound Settings")]
    [SerializeField] private float walkSoundRadius = 5f;
    [SerializeField] private LayerMask npcLayer;

    private void Update()
    {
        // Cek jika tombol WASD / pergerakan sedang ditekan
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        if (h != 0 || v != 0)
        {
            EmitFootstepSound();
        }
    }

    private void EmitFootstepSound()
    {
        // Cari semua collider NPC di sekitar radius suara
        Collider[] colliders = Physics.OverlapSphere(transform.position, walkSoundRadius, npcLayer);

        foreach (Collider col in colliders)
        {
            if (col.TryGetComponent<NPCBrainC4>(out var brain))
            {
                brain.HearSound(transform.position);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, walkSoundRadius);
    }
}