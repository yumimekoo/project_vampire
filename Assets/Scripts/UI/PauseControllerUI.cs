using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class PauseControllerUI : MonoBehaviour
{
    [SerializeField] private UIDocument pauseUI;
    private Button
        continueButton,
        newGameButton,
        exitButton;
    private Coroutine timeScaleCoroutine;

    void Start()
    {
        var root = pauseUI.rootVisualElement;
        continueButton = root.Q<Button>("buttonContinue");
        newGameButton = root.Q<Button>("buttonNewGame");
        exitButton = root.Q<Button>("buttonExit");

        continueButton.clicked += ContinueButton;
        newGameButton.clicked += NewGameButton;
        exitButton.clicked += ExitButton;

        pauseUI.rootVisualElement.style.display = DisplayStyle.None;
    }

    private void ExitButton()
    {
        GameState.inPauseMenu = false;
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    private void NewGameButton()
    { 
        pauseUI.rootVisualElement.style.display = DisplayStyle.None;
        GameState.inPauseMenu = false;
        Time.timeScale = 1f;
        SceneManager.LoadScene("SampleScene");
    }

    private void ContinueButton()
    {
        Debug.Log("In Continue, click registerd");
        pauseUI.rootVisualElement.style.display = DisplayStyle.None;
        GameState.inPauseMenu = false;
        if (timeScaleCoroutine != null)
        {
            StopCoroutine(timeScaleCoroutine);
        }
        timeScaleCoroutine = StartCoroutine(AdjustTimeScale(Time.timeScale, 1f));
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape) && !GameState.inShop)
        {
            if (!GameState.inPauseMenu)
            {

                pauseUI.rootVisualElement.style.display = DisplayStyle.Flex;
                GameState.inPauseMenu = true;
                if (timeScaleCoroutine != null)
                {
                    StopCoroutine(timeScaleCoroutine);
                }
                timeScaleCoroutine = StartCoroutine(AdjustTimeScale(Time.timeScale, 0f));
            }
            else if (GameState.inPauseMenu) {
                ContinueButton();
            }
        }
    }

    private IEnumerator AdjustTimeScale(float start, float end)
    {
        float t = 0f;
        float duration = 0.2f;
        while (t < 1f)
        {
            t += Time.unscaledDeltaTime / duration;
            Time.timeScale = Mathf.Lerp(start, end, t);
            Time.fixedDeltaTime = 0.02f * Time.timeScale;
            yield return null;
        }
        Time.timeScale = end;
        Time.fixedDeltaTime = 0.02f * Time.timeScale;
    }
}
