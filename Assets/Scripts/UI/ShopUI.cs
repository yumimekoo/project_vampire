using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;

public class ShopUI : MonoBehaviour
{
    [SerializeField] public UIDocument shopUI;
    [SerializeField] public WaveManager waveManager;
    [SerializeField] private PauseControllerUI pauseUI;
    [SerializeField] private PlayerTabMenuController tabUI;
    [SerializeField] private OverlayUI overlayUI;
    private Button nextWave;

    public void Awake()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;
        nextWave = root.Q<Button>("nextWave");

        nextWave.clicked += () =>
        {
            waveManager.OnNextWaveButton();
            HideUI();
        };

        HideUI();
    }

    public void HideUI()
    {
        overlayUI.ShowUI();
        GameState.inShop = false;
        shopUI.rootVisualElement.style.display = DisplayStyle.None;
    }

    public void ShowUI()
    {
        overlayUI.HideUI();
        tabUI.HideUI();
        pauseUI.HideUI();
        GameState.inShop = true;
        shopUI.rootVisualElement.style.display = DisplayStyle.Flex;
    }
}
