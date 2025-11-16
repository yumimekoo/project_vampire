using UnityEngine;

public class SoundEffectAwait : MonoBehaviour
{
    [SerializeField] private float delay = 1f;
    [SerializeField] private float targetVolume = 0.5f;

    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.volume = 0f; // Sofort auf 0 beim Szenenstart
    }

    private void Start()
    {
        Invoke(nameof(SetVolume), delay);
    }

    private void SetVolume()
    {
        audioSource.volume = targetVolume;
    }
}
