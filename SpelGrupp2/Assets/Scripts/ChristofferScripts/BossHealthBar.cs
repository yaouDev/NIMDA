using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossHealthBar : MonoBehaviour
{
    [SerializeField] private Image fillImage;
    private Slider slider;
    private EnemyHealth health;
    private float fillValue;
    // Start is called before the first frame update
    private void Awake()
    {
        slider = FindObjectOfType<Slider>();
        health = gameObject.GetComponent<EnemyHealth>();
    }

    // Update is called once per frame
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
