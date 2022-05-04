using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CallbackSystem
{
    public class PlayerOnCameraPos : MonoBehaviour
    {
        [SerializeField] private bool isPlayerOne;
        private Vector3 screenPos, offset = new Vector3(0f, 2.5f, 0f);
        private Camera cam;
        private CameraPosUpdateEvent cameraEvent;

        private void Start()
        {
            cam = gameObject.GetComponentInChildren<Camera>();
            cameraEvent = new CameraPosUpdateEvent();
            cameraEvent.isPlayerOne = isPlayerOne;
        }

        void Update()
        { 
            FindPosOnScreen();
            cameraEvent.pos = screenPos;
            EventSystem.Current.FireEvent(cameraEvent);
        }

        private void FindPosOnScreen()
        {
            screenPos = cam.WorldToScreenPoint(transform.position + offset);
        }
    }
}

