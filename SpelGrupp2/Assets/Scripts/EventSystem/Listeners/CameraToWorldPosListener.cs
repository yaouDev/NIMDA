using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CallbackSystem
{
    public class CameraToWorldPosListener : MonoBehaviour
    {
        [SerializeField] private GameObject player1CharUI, player2CharUI;
        private GameObject currCharUI;
        public void Start()
        {
            EventSystem.Current.RegisterListener<CameraPosUpdateEvent>(UpdateCameraWorldPos);
        }

        private void UpdateCameraWorldPos(CameraPosUpdateEvent camera)
        {
            currCharUI = camera.isPlayerOne ? player1CharUI : player2CharUI;
            currCharUI.transform.position = camera.pos;
        }
    }
}

