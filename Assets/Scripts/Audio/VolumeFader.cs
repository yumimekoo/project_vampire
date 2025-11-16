using UnityEngine;
using UnityEngine.UIElements;

public class VolumeFader : MonoBehaviour
{
    [SerializeField] private float targetVolume = 0.4f;
    [SerializeField] private float delay = 1f;
    [SerializeField] private float fadeTime = 1f;

    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.volume = 0f;   // Start immer bei 0
    }

    private void Start()
    {
        StartCoroutine(FadeIn());
    }

    public void FadeOutStart()
    {
        StartCoroutine(FadeOut(2f));
    }

    private System.Collections.IEnumerator FadeIn()
    {
        // 1 Sekunde warten (oder der eingestellte delay)
        yield return new WaitForSeconds(delay);

        float elapsed = 0f;

        while (elapsed < fadeTime)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / fadeTime;
            audioSource.volume = Mathf.Lerp(0f, targetVolume, t);
            yield return null;
        }

        audioSource.volume = targetVolume;
    }

    private System.Collections.IEnumerator FadeOut(float fadeOutDuration)
    {
        float startVolume = audioSource.volume;
        float elapsed = 0f;

        while (elapsed < fadeOutDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / fadeOutDuration;
            audioSource.volume = Mathf.Lerp(startVolume, 0f, t);
            yield return null;
        }

        audioSource.volume = 0f;
    }
}