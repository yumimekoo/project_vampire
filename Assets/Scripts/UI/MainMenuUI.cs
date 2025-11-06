using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

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
    void Start()
    {
        var root = mainUI.rootVisualElement;
        mainMenuBackground = root.Q<VisualElement>("background");
        title = root.Q<Label>("labelTitle");
        subtitle = root.Q<Label>("labelSubtitle");
        highscore = root.Q<Label>("labelHighscore");
        playButton = root.Q<Button>("buttonPlay");
        quitButton = root.Q<Button>("buttonExit");

        playButton.clicked += () =>
        {
            LoadSceneWithProgress("SampleScene");
        };

        quitButton.clicked += () =>
        {
            Application.Quit();
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
        };

        var loadingRoot = loadingUI.rootVisualElement;
        loadingBar = loadingRoot.Q<ProgressBar>("loading");
        loadingUI.rootVisualElement.style.display = DisplayStyle.None;
    }

    public void LoadSceneWithProgress(string name)
    {
        StartCoroutine(LoadSceneAsync(name));
    }

    public IEnumerator LoadSceneAsync(string name)
    {
        loadingUI.rootVisualElement.style.display = DisplayStyle.Flex;
        loadingBar.value = 0f;
       
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(name);
        asyncLoad.allowSceneActivation = false;

        while (!asyncLoad.isDone)
        {
            float progress = Mathf.Clamp01(asyncLoad.progress / 0.9f);
            loadingBar.value = progress * 100f;
            if (asyncLoad.progress >= 0.9f)
            {
                asyncLoad.allowSceneActivation = true;
            }
            yield return null;
        }
    }


}
