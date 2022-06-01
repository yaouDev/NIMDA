using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BlinkUI : MonoBehaviour
{
    [SerializeField] private Image blinkUI;
    private bool onCooldown;
    private float blinkTimer;
    public void UpdateBlinkUI(float timer)
    {
        blinkUI.fillAmount = 0;
        blinkTimer = timer;
        onCooldown = true;
    }

    private void Update()
    {
        if (onCooldown)
        {
            if (blinkUI.fillAmount != 1f)
            {
                blinkTimer -= Time.deltaTime;
                blinkUI.fillAmount = blinkTimer;
                blinkUI.color = Color.Lerp(Color.gray, Color.green, blinkTimer);
            }
            onCooldown = blinkUI.fillAmount == 1f ? false : true;
        }
    }

    public bool IsOnCooldown() { return onCooldown; }
}
