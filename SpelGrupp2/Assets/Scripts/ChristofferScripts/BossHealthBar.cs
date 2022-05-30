using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossHealthBar : MonoBehaviour
{
    [SerializeField] private Image fillImage;
    private float fillValue;
    private Slider slider;
    private EnemyHealth health;
    private void Awake()
    {
        slider = FindObjectOfType<Slider>();
        health = gameObject.GetComponent<EnemyHealth>();
    }

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
