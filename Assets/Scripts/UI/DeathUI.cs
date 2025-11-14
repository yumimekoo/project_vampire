using DG.Tweening;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class DeathUI : MonoBehaviour
{
    
    [SerializeField] private UIDocument deathUI;
    [SerializeField] private UIDocument loadingUI;
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private LevelManager levelManager;
    private float timer;
    public Sprite[] frames;
    public Sprite[] framesAshes;
    public Sprite stillframeFadeIn;
    public float frameRate = 10f;
    public float ashFrameRate = 6f;

    private VisualElement deathImage;
    private VisualElement stillFrame;
    private VisualElement ashes;
    private Button newGame;
    private Button quitGame;
    private Label score;
    private Label highScore;

    public void Awake()
    {
        var root = deathUI.rootVisualElement;
        deathImage = root.Q<VisualElement>("deathScreen");
        stillFrame = root.Q<VisualElement>("stillFrame");
        ashes = root.Q<VisualElement>("ashes");
        newGame = root.Q<Button>("buttonNewGame");
        quitGame = root.Q<Button>("buttonQuitGame");
        score = root.Q<Label>("scoreLabel");
        highScore = root.Q<Label>("highscoreLabel");
        newGame.clicked += () => StartCoroutine(LoadScene("SampleScene"));
        quitGame.clicked += () => StartCoroutine(LoadScene("MainMenu"));

        playerHealth.OnPlayerDeath += ShowDeathUI;
        deathUI.rootVisualElement.style.display = DisplayStyle.None;
        loadingUI.rootVisualElement.style.display = DisplayStyle.None;
    }

    private void OnQuitGameClicked()
    {
        throw new NotImplementedException();
    }

    private void OnNewGameClicked()
    {
        GameState.Reset();
        SceneManager.LoadScene("SampleScene");
    }

    public IEnumerator LoadScene(string name)
    {
        DOTween.To(
            () => (float) loadingUI.rootVisualElement.resolvedStyle.opacity,
            x => loadingUI.rootVisualElement.style.opacity = x,
            1f, 1f
            ).OnComplete(() =>
            {
                GameState.Reset();
                SceneManager.LoadSceneAsync(name);
            });
        yield return null;
    }

    public void ShowDeathUI()
    {
        loadingUI.rootVisualElement.style.opacity = 0f;
        score.style.opacity = 0f;
        highScore.style.opacity = 0f;
        score.text = $"Score: {levelManager.GetScore()}";
        highScore.text = $"High Score: {PlayerPrefs.GetInt("highscore", 0)}";
        quitGame.style.opacity = 0f;
        newGame.style.opacity = 0f;
        stillFrame.style.display = DisplayStyle.None;
        loadingUI.rootVisualElement.style.display = DisplayStyle.Flex;
        deathUI.rootVisualElement.style.display = DisplayStyle.Flex;
        StartCoroutine(PlayDeathAnimation());
    }

    public IEnumerator PlayDeathAnimation()
    {
        yield return PlayDeathAnimationFrames();

        yield return FadeInStillFrame();

        yield return PlayAshesAnimation();

        yield return FadeInUIElements();
    }

    private IEnumerator PlayDeathAnimationFrames()
    {
        for (int i = 0; i < frames.Length; i++)
        {
            deathImage.style.backgroundImage = new StyleBackground(frames[i]);
            yield return new WaitForSeconds(1f / frameRate);
        }
    }

    private IEnumerator FadeInStillFrame()
    {
        
        stillFrame.style.display = DisplayStyle.Flex;
        stillFrame.style.backgroundImage = new StyleBackground(stillframeFadeIn);
        stillFrame.style.opacity = 0f;

        DOTween.To(
            () => stillFrame.style.opacity.value,
            x => stillFrame.style.opacity = x,
            1f,
            2f
            ).WaitForCompletion();
        yield return new WaitForSeconds(1f);
    }
    private IEnumerator PlayAshesAnimation()
    {
        stillFrame.style.backgroundImage = new StyleBackground(); 

        for (int i = 0; i < framesAshes.Length; i++)
        {
            ashes.style.backgroundImage = new StyleBackground(framesAshes[i]);
            yield return new WaitForSeconds(1f / ashFrameRate);
        }
    }

    private IEnumerator FadeInUIElements()
    {
        DOTween.To(
            () => quitGame.style.opacity.value,
            x => quitGame.style.opacity = x,
            1f,
            2f
            );
        DOTween.To(
            () => newGame.style.opacity.value,
            x => newGame.style.opacity = x,
            1f,
            2f
            );
        DOTween.To(
            () => score.style.opacity.value,
            x => score.style.opacity = x,
            1f,
            2f
            );
        DOTween.To(
            () => highScore.style.opacity.value,
            x => highScore.style.opacity = x,
            1f,
            2f
            );
        yield return new WaitForSeconds(2f);
    }
}
