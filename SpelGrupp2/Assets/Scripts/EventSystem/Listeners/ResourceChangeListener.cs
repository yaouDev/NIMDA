using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace CallbackSystem
{
    public class ResourceChangeListener : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI[] player1, player2;
        private TextMeshProUGUI[] currPlayer;
        void Start()
        {
            EventSystem.Current.RegisterListener<ResourceUpdateEvent>(UpdateResources);
        }

        private void UpdateResources(ResourceUpdateEvent eve)
        {
            currPlayer = eve.isPlayerOne ? player1 : player2;
            currPlayer[0].text= eve.c.ToString();
            currPlayer[1].text = eve.t.ToString();
            currPlayer[2].text = eve.i.ToString();
            currPlayer[3].text = eve.a.ToString();
        }
    }
}

