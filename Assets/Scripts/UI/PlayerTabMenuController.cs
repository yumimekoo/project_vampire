using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerTabMenuController : MonoBehaviour
{
    [SerializeField] private UIDocument tabMenu; 
    private bool isTabHeld = false;
    private Coroutine timeScaleCoroutine;

    void Start()
    {
        tabMenu.rootVisualElement.style.display = DisplayStyle.None;
    }
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Tab))
        {
            if(!isTabHeld)
                OpenTabMenu();
        }

        if(Input.GetKeyUp(KeyCode.Tab))
        {
            if(isTabHeld)
                CloseTabMenu();
        }
    }

    private void OpenTabMenu()
    {
        if(GameState.inShop || GameState.inPauseMenu)
        {
            return;
        }   

        GameState.inTabPauseMenu = true;
        isTabHeld = true;

        tabMenu.rootVisualElement.style.display = DisplayStyle.Flex;
        if (timeScaleCoroutine != null)
        {
            StopCoroutine(timeScaleCoroutine);
        }
        timeScaleCoroutine = StartCoroutine(AdjustTimeScale(Time.timeScale, 0f));
    }

    private void CloseTabMenu()
    {
        GameState.inTabPauseMenu = false;
        isTabHeld = false;
        tabMenu.rootVisualElement.style.display = DisplayStyle.None;

        if(timeScaleCoroutine != null)
        {
            StopCoroutine(timeScaleCoroutine);
        }
        timeScaleCoroutine = StartCoroutine(AdjustTimeScale(Time.timeScale, 1f));
    }

    private IEnumerator AdjustTimeScale(float start, float end)
    {
        float t = 0f;
        float duration = 0.6f;
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
