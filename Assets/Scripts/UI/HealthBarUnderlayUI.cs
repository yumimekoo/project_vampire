using System;
using UnityEngine;
using UnityEngine.UIElements;

public class HealthBarUnderlayUI : MonoBehaviour
{
    public UIDocument healthbarunderlayUI;
    private ProgressBar healthBar, xpBar;
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private LevelManager levelManager;

    private void Awake()
    {
        var root = healthbarunderlayUI.rootVisualElement;
        healthBar = root.Q<ProgressBar>("healthBar");
        xpBar = root.Q<ProgressBar>("xpBar");

        playerHealth.OnHealthChanged += UpdateHealthBar;
        levelManager.OnXPChanged += UpdateXPBar;
    }

    private void UpdateHealthBar(float current, float max)
    {
        if (healthBar == null)
            return;
        healthBar.value = (current / max) * 100f;
    }

    private void UpdateXPBar(float current, float max)
    {
        if (xpBar == null)
            return;
        xpBar.value = (current / max) * 100f;
    }

    private void OnDestroy()
    {
        playerHealth.OnHealthChanged -= UpdateHealthBar;
        levelManager.OnXPChanged -= UpdateXPBar;
    }

    public void ShowUI()
    {
        healthbarunderlayUI.rootVisualElement.style.display = DisplayStyle.Flex;
    }
    public void HideUI()
    {
        healthbarunderlayUI.rootVisualElement.style.display = DisplayStyle.None;
    }
}

