using System.Collections;
using UnityEngine;

public class MainMenuMusic : MonoBehaviour
{
    private static MainMenuMusic instance;
    public static MainMenuMusic Instance => instance;
    [SerializeField] private AudioSource audioSource;
    private bool isFading = false;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject); // NEUE Instanz zerstören
            return;
        }

        instance = this;
        DontDestroyOnLoad(this.gameObject);

        audioSource.loop = true;
        FadeInAndPlay(2f);
   
    }

    public void FadeOutAndStop(float duration)
    {
               StartCoroutine(FadeOutCoroutine(duration));
    }

    public void FadeInAndPlay(float duration)
    {
        if (audioSource.isPlaying || isFading)
            return;

        audioSource.volume = 0;
        audioSource.enabled = true;

        isFading = true;
        audioSource.Play();
        StartCoroutine(FadeInCoroutine(duration));
    }

    private IEnumerator FadeInCoroutine(float duration)
    {
        float targetVolume = 0.4f;
        float time = 0;

        while (time < duration)
        {
            time += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(0, targetVolume, time / duration);
            yield return null;
        }

        audioSource.volume = targetVolume;
        isFading = false;
    }

    private IEnumerator FadeOutCoroutine(float duration)
    {
        float startVolume = audioSource.volume;
        float time = 0;

        while (time < duration)
        {
            time += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(startVolume, 0, time / duration);
            yield return null;
        }
        audioSource.Stop();
        audioSource.volume = startVolume;
    }
}
