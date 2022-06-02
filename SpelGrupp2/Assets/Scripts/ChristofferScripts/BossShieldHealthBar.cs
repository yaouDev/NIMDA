using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossShieldHealthBar : MonoBehaviour
{
    [SerializeField] private Image fillImage;
    [SerializeField] private Slider slider;
    [SerializeField] private EnemyShield shield;
    private float fillValue;


    void Update()
    {
        if (shield != null)
        {
            fillValue = shield.CurrentHealth / shield.FullHealth;
            slider.value = fillValue;
            if (fillValue <= 0)
            {
                slider.enabled = false;
            }
        }
    }
}
