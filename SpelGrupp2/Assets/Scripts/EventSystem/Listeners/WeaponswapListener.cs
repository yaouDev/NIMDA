using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CallbackSystem
{
    public class WeaponswapListener : MonoBehaviour
    {
        [SerializeField] private Image[] HudOne, HudTwo;
        private Image[] currentHUD;
        private Color basic = new Color(1, 1, 1, 1), fade = new Color(0.5f, 0.5f, 0.5f, 0.5f);
        void Start()
        {
            EventSystem.Current.RegisterListener<UpdateCurrentWeaponEvent>(SwapWeaponIcon);
        }

        private void SwapWeaponIcon(UpdateCurrentWeaponEvent eve)
        {
            currentHUD = eve.isPlayerOne ? HudOne : HudTwo;
            if (eve.usingLaserWeapon)
            {
                currentHUD[0].color = basic;
                currentHUD[1].color = fade;
            }
            else
            {
                currentHUD[1].color = basic;
                currentHUD[0].color = fade;
            }
        }
    }
}

