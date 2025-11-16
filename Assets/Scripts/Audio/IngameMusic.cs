using System;
using System.Collections;
using UnityEngine;

public class IngameMusic : MonoBehaviour
{
    private static IngameMusic instance;
    public static IngameMusic Instance => instance;
    [SerializeField] private AudioSource audioSource;
    private bool isFading = false;

    private Coroutine pitchRoutine;

    private float normalPitch = 1f;
    private float lowPitch = 0.8f;

    private float normalVolume = 0.2f;
    private float lowVolume = 0.07f;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject); 
            return;
        }

        instance = this;
        ///DontDestroyOnLoad(this.gameObject);

        audioSource.loop = true;

    }

    private void OnEnable()
    {
        FadeInAndPlay(1f);
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
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = elapsed / duration;
            audioSource.volume = Mathf.Lerp(0f, 0.2f, t);
            yield return null;
        }

        audioSource.volume = 0.2f;
    }

    private IEnumerator FadeOutCoroutine(float duration)
    {
        float startVolume = audioSource.volume;
        float time = 0;

        while (time < duration)
        {
            time += Time.unscaledDeltaTime;
            audioSource.volume = Mathf.Lerp(startVolume, 0, time / duration);
            yield return null;
        }
        audioSource.Stop();
        isFading = false;
    }

    public void PitchDown(float duration)
    {
        if (this == null || audioSource == null)
            return;
        StartPitchRoutine(lowPitch, lowVolume, duration);
    }

    public void PitchDownDeath(float duration)
    {
        if (this == null || audioSource == null)
            return;
        StartPitchRoutine(0.2f, lowVolume, duration);
    }

    public void PitchUp(float duration)
    {
        if (this == null || audioSource == null)
            return;
        StartPitchRoutine(normalPitch, normalVolume, duration);
    }

    private void StartPitchRoutine(float targetPitch, float targetVolume, float duration)
    {
        if (this == null)
            return;
        if (!audioSource)
            return;
        if (pitchRoutine != null)
            StopCoroutine(pitchRoutine);

        pitchRoutine = StartCoroutine(PitchCoroutine(targetPitch, targetVolume, duration));
    }

    private IEnumerator PitchCoroutine(float targetPitch, float targetVolume, float duration)
    {
        float startPitch = audioSource.pitch;
        float startVolume = audioSource.volume;

        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = elapsed / duration;

            audioSource.pitch = Mathf.Lerp(startPitch, targetPitch, t);
            audioSource.volume = Mathf.Lerp(startVolume, targetVolume, t);

            yield return null;
        }

        audioSource.pitch = targetPitch;
        audioSource.volume = targetVolume;

        pitchRoutine = null;
    }
}