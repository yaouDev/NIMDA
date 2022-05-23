using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CallbackSystem
{
    public class CrosshairUIListener : MonoBehaviour
    {
        //private PlayerAttack[] players;
        //private PlayerAttack player;
        [SerializeField] private Image player1CH, player2CH;
        private Image currentCH;
        void Start()
        {
            //players = FindObjectsOfType<PlayerAttack>();
            //player = players[0].IsPlayerOne() ? players[0] : players[1];
            EventSystem.Current.RegisterListener<WeaponCrosshairEvent>(UpdateCrosshair);
        }

        private void UpdateCrosshair(WeaponCrosshairEvent eve)
        {
            currentCH = eve.isPlayerOne ? player1CH : player2CH;
            if (eve.usingRevolver && eve.targetInSight)
            {
                currentCH.enabled = true;
                currentCH.transform.position = eve.crosshairPos;
                //Debug.Log($"{eve.crosshairPos}");
            } else
                currentCH.enabled = false;
        }
    }
}


