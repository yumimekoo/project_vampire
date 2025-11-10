using System;
using UnityEngine;
using UnityEngine.UIElements;

public class HealthBarUnderlayUI : MonoBehaviour
{
    public UIDocument healthbarunderlayUI;
    private ProgressBar healthBar;
    [SerializeField] private PlayerHealth playerHealth;

    private void Awake()
    {
        var root = healthbarunderlayUI.rootVisualElement;
        healthBar = root.Q<ProgressBar>("healthBar");

        playerHealth.OnHealthChanged += UpdateHealthBar;
    }

    private void UpdateHealthBar(float current, float max)
    {
        if (healthBar == null)
            return;
        healthBar.value = (current / max) * 100f;
    }
}

