using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CallbackSystem
{
    public class CrosshairUIListener : MonoBehaviour
    {
        [SerializeField] private Image player1CH, player2CH;
        private Image currentCH;
        void Start()
        {
            EventSystem.Current.RegisterListener<WeaponCrosshairEvent>(UpdateCrosshair);
        }

        private void UpdateCrosshair(WeaponCrosshairEvent eve)
        {
            currentCH = eve.isPlayerOne ? player1CH : player2CH;
            if (eve.usingRevolver && eve.targetInSight)
            {
                currentCH.enabled = true;
                currentCH.transform.position = eve.crosshairPos;
            } else
                currentCH.enabled = false;
        }
    }
}


