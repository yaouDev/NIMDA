using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossShieldHealthBar : MonoBehaviour
{
    [SerializeField] private Image fillImage;
    private float fillValue;
    private Slider slider;
    private EnemyShield shield;

    void Start()
    {
        slider = FindObjectOfType<Slider>();
        shield = gameObject.GetComponent<EnemyShield>();
    }


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
