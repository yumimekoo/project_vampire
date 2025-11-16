using System;
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
      
    }

    private void OnEnable()
    {
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
        // 1 Sekunde warten (oder der eingestellte delay)
        yield return new WaitForSeconds(0.2f);

        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            audioSource.volume = Mathf.Lerp(0f, 0.4f, t);
            yield return null;
        }

        audioSource.volume = 0.4f;
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
        isFading = false;
    }
}
