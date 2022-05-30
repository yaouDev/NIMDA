using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CallbackSystem
{
    public class CraftingTableListener : MonoBehaviour
    {
        [SerializeField] private GameObject[] tables;
        private GameObject table;
        void Start()
        {
            EventSystem.Current.RegisterListener<CraftingEvent>(EnterCraftingTable);
        }

        private void EnterCraftingTable(CraftingEvent eve)
        {
            table = eve.isPlayerOne ? tables[0] : tables[1];
            table.SetActive(eve.activate);
        }
    }
}

