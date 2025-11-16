using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using DG.Tweening;

public class MainMenuUI : MonoBehaviour
{
    public UIDocument mainUI;
    public UIDocument loadingUI;
    public VisualElement mainMenuBackground;
    private ProgressBar loadingBar;
    private Label 
        title,
        subtitle,
        highscore;
    private Button 
        playButton,
        quitButton;

    private int currentFrame;
    private float timer;
    public Sprite[] frames;
    public float frameRate = 8f;

    private void OnEnable()
    {
        MainMenuMusic music = MainMenuMusic.Instance;
        if (music != null)
            music.FadeInAndPlay(2f);
    }

    void Start()
    {
        var root = mainUI.rootVisualElement;
        mainMenuBackground = root.Q<VisualElement>("background");
        title = root.Q<Label>("labelTitle");
        subtitle = root.Q<Label>("labelSubtitle");
        highscore = root.Q<Label>("labelHighscore");
        playButton = root.Q<Button>("buttonPlay");
        quitButton = root.Q<Button>("buttonExit");

        mainMenuBackground.style.backgroundImage = new StyleBackground(frames[0]);

        highscore.text = $"{PlayerPrefs.GetInt("highscore", 0)} !";

        playButton.clicked -= OnPlayClicked;
        playButton.clicked += OnPlayClicked;

        quitButton.clicked += () =>
        {
            Application.Quit();
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
        };

        var loadingRoot = loadingUI.rootVisualElement;
        DOTween.To(
            () => (float) loadingRoot.resolvedStyle.opacity,
            x => loadingRoot.style.opacity = x,
            0f, 1f
            );


        StartPulse();
    }
    private void OnPlayClicked()
    {
        MainMenuMusic.Instance?.FadeOutAndStop(1f);
        LoadScene("SampleScene");
    }

    private void Update()
    {
        if (frames.Length == 0)
            return;

        timer += Time.deltaTime;
        if (timer >= 1f / frameRate)
        {
            timer = 0f;
            currentFrame = (currentFrame + 1) % frames.Length;
            mainMenuBackground.style.backgroundImage = new StyleBackground(frames[currentFrame]);
        }
    }


    void StartPulse()
    {
        float currentSize = 130;


        DOTween.To(
            () => currentSize,
            x => {
                currentSize = x;
                highscore.style.fontSize = x;
            },
            160,
            1f
        )
        .SetEase(Ease.InOutSine)
        .SetLoops(-1, LoopType.Yoyo); 
    }

    public void LoadScene(string name)
    {
        DOTween.To(
            () => (float) loadingUI.rootVisualElement.resolvedStyle.opacity,
            x => loadingUI.rootVisualElement.style.opacity = x,
            1f, 1f
            ).OnComplete(() =>
            {
                SceneManager.LoadSceneAsync(name);
            });
    }

}
