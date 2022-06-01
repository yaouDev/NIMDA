using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace CallbackSystem
{
    public class BlinkUIListener : MonoBehaviour
    {
        [SerializeField] private Image blinkPlayerOne, blinkPlayerTwo;
        [SerializeField] private TextMeshProUGUI counterPlayerOne, counterPlayerTwo;
        private TextMeshProUGUI counterBox;
        private Image blinkUI;

        void Start()
        {
            EventSystem.Current.RegisterListener<UpdateBlinkUIEvent>(UpdateBlinkUI);
        }

        private void UpdateBlinkUI(UpdateBlinkUIEvent eve)
        {
            blinkUI = eve.isPlayerOne ? blinkPlayerOne : blinkPlayerTwo;
            counterBox = eve.isPlayerOne ? counterPlayerOne : counterPlayerTwo;

            counterBox.text = eve.blinkCount.ToString();
            blinkUI.fillAmount = eve.fill;
            blinkUI.color = Color.Lerp(Color.gray, Color.green, eve.fill);      
        }
    }
}

