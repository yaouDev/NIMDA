using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CallbackSystem
{
    public class CrosshairProjectileWeapon : MonoBehaviour
    {
        private Vector3 screenPos, raycastPoint;
        [SerializeField] private Camera cam;

        private void Update()
        {
            FindPosOnScreen();
           // eve.crosshairPos = screenPos;
           // EventSystem.Current.FireEvent(eve);

        }
        private void FindPosOnScreen()
        {
            screenPos = cam.WorldToScreenPoint(raycastPoint);
        }
    }
}
