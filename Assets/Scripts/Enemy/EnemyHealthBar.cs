using UnityEngine.UI;
using UnityEngine;

public class EnemyHealthBar : MonoBehaviour
{
    [SerializeField] private Image fillImage;
    [SerializeField] private Vector3 offset = new Vector3(0, 1.5f, 0);
    private Transform target;

    public void Initialize(Transform targetTransform)
    {
        target = targetTransform;
    }

    public void UpdateHealthBar(float currentHealth, float maxHealth)
    {
        if (fillImage != null)
        {
            fillImage.fillAmount = currentHealth / maxHealth;
        }
    }

    private void LateUpdate()
    {
        if (target != null)
        {
            transform.position = target.position + offset;
            transform.rotation = Quaternion.LookRotation(Camera.main.transform.forward);
        }
    }
}

