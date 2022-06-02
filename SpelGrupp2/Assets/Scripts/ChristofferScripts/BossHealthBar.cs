using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossHealthBar : MonoBehaviour
{
    [SerializeField] private Image fillImage;
    [SerializeField] private Slider slider;
    [SerializeField] private EnemyHealth health;
    private float fillValue;

    void Update()
    {
        if (health != null)
        {
            fillValue = health.CurrentHealth / health.GetFullHealth();
            slider.value = fillValue;
            if (fillValue <= 0)
            {
                slider.enabled = false;
            }
        }
    }
}
