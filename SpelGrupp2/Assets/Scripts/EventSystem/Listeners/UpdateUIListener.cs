using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CallbackSystem
{
    public class UpdateUIListener : MonoBehaviour
    {
        [SerializeField] private GameObject[] UIs;
        private GameObject UI;
        void Start()
        {
            EventSystem.Current.RegisterListener<UpdateUIEvent>(UpdateUI);
        }

        private void UpdateUI(UpdateUIEvent eve)
        {
            UI = eve.isPlayerOne ? UIs[0] : UIs[1];
            if (eve.isAlive)
                UI.gameObject.SetActive(true);
            else
                UI.gameObject.SetActive(false);
        }
    }
}

