using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;

public class ShopUI : MonoBehaviour
{
    [SerializeField] public UIDocument shopUI;
    [SerializeField] public WaveManager waveManager;
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

    public void HideUI() => shopUI.rootVisualElement.style.display = DisplayStyle.None;
    public void ShowUI() => shopUI.rootVisualElement.style.display = DisplayStyle.Flex;
}
