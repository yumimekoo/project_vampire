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
    private Label deathMessage;

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
        deathMessage = root.Q<Label>("deathMessage");

        playerHealth.OnPlayerDeath += ShowDeathUI;
        deathUI.rootVisualElement.style.display = DisplayStyle.None;
        loadingUI.rootVisualElement.style.display = DisplayStyle.None;


    }

    private void OnDestroy()
    {
        playerHealth.OnPlayerDeath -= ShowDeathUI;
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
        if (IngameMusic.Instance != null)
            IngameMusic.Instance.PitchDownDeath(3f);

        loadingUI.rootVisualElement.style.opacity = 0f;
        score.style.opacity = 0f;
        highScore.style.opacity = 0f;
        score.text = $"{levelManager.GetScore()}";
        highScore.text = $"{GetHighscoreText()}";
        deathMessage.text = $"{GetDeathMessageRandom()}";
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

    public string GetDeathMessageRandom()
    {
        string[] messages = new string[]
        {
            "are you even trying?",
            "kinda disappointing that you died here...",
            "tutorials help .. u know that?",
            "seriously? u died here?",
            "i think thats an skill issue",
            "try something else .. maybe candycrush?",
            "left click to shoot. just saying..",
            "i did not expect that level of incompetence",
            "first time using a keyboard?",
            "classic 'layer 8 problem' ",
            "screen is turned on?",
        };
        int index = UnityEngine.Random.Range(0, messages.Length);
        return messages[index];
    }

    public string GetHighscoreText()
    {
        int highscore = PlayerPrefs.GetInt("highscore", 0);
        if (highscore <= levelManager.GetScore())
        {
            return $"new highscore, but u could have done better tbh..";
        }
        return $"highscore: {highscore}";
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
