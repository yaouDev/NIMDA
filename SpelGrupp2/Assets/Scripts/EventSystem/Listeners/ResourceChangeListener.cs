using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace CallbackSystem
{
    public class ResourceChangeListener : MonoBehaviour
    {
        [Header("Each array consists of the players resources. Hover elements for tooltip")]
        [Tooltip("'Copper', 'Transistor' and 'Iron' can be found under ResourceUI and 'AmmoCount' under BatteryUI \n Element 0 = Copper \n Element 1 = Transistor \n Element 2 = Iron \n Element 3 = AmmoCount \n Element 4 = Currency")]
        [SerializeField] private TextMeshProUGUI[] player1, player2;
        private TextMeshProUGUI[] currPlayer;
        void Start()
        {
            EventSystem.Current.RegisterListener<ResourceUpdateEvent>(UpdateResources);
        }

        private void UpdateResources(ResourceUpdateEvent eve)
        {
            currPlayer = eve.isPlayerOne ? player1 : player2;
            if (eve.ammoChange)
                currPlayer[3].text = eve.a.ToString();
            else
            {
                currPlayer[0].text = eve.c.ToString();
                currPlayer[1].text = eve.t.ToString();
                currPlayer[2].text = eve.i.ToString();
                currPlayer[4].text = eve.currency.ToString();
            }

        }
    }
}

