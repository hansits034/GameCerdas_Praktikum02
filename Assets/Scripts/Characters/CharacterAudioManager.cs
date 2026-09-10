using UnityEngine;
 
public class CharacterAudioManager : MonoBehaviour
{
    [SerializeField] private AudioSource _footstepSfx;
    // [SerializeField] private AudioSource _glideSfx;
    // [SerializeField] private AudioSource _punchSfx;
    // [SerializeField] private AudioSource _landingSfx;
 
    private void PlayFootstepSfx()
    {
        _footstepSfx.volume = Random.Range(0.7f, 1f);
        _footstepSfx.pitch = Random.Range(0.5f, 2.5f);
        _footstepSfx.Play();
    }
}