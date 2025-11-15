using UnityEngine;
using UnityEngine.UIElements;

public class DashUI : MonoBehaviour
{
    [SerializeField] private UIDocument dashUI;
    [SerializeField] private VisualTreeAsset dashIconTemplate;

    private VisualElement dashContainer;

    public void Awake()
    {
        dashContainer = dashUI.rootVisualElement.Q<VisualElement>("dashContainer");
    }

    public void AddDash()
    {
        var dashIcon = dashIconTemplate.CloneTree();
        dashContainer.Add(dashIcon);
    }

    public void RemoveDash()
    {
        if (dashContainer.childCount > 0)
        {
            dashContainer.RemoveAt(dashContainer.childCount - 1);
        }
    }

    public void ClearContainer()
    {
                dashContainer.Clear();
    }
}
