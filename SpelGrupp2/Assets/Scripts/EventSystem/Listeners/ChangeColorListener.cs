using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CallbackSystem
{
    public class ChangeColorListener : MonoBehaviour
    {
        [SerializeField] private Image[] borders, crosshairs;
        private Image playerBorder, playerCrosshair;
        private void Start()
        {
            EventSystem.Current.RegisterListener<ChangeColorEvent>(ChangePlayerColor);
        }

        private void ChangePlayerColor(ChangeColorEvent eve)
        {
            playerBorder = eve.isPlayerOne ? borders[0] : borders[1];
            playerCrosshair = eve.isPlayerOne ? crosshairs[0] : crosshairs[1];
            playerBorder.color = eve.color;
            playerCrosshair.color = eve.color;
        }
    }
}

