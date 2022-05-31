using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CallbackSystem
{
    public class CraftingTableListener : MonoBehaviour
    {
        [SerializeField] private GameObject[] tables;
        private GameObject table;
        private TabGroup buttonTab;
        void Start()
        {
            EventSystem.Current.RegisterListener<CraftingEvent>(EnterCraftingTable);
        }

        private void EnterCraftingTable(CraftingEvent eve)
        {
            table = eve.isPlayerOne ? tables[0] : tables[1];
            table.SetActive(eve.activate);
            if (eve.successfulCraft && eve.activate)
            {
                buttonTab = table.GetComponentInChildren<TabGroup>();
                buttonTab.SetPlayerOne(eve.isPlayerOne);
                if (buttonTab.selectedButton != buttonTab.GetDefaultColorButton())
                    buttonTab.selectedButton.interactable = false;
                buttonTab.buttonsDictionary[buttonTab.selectedButton.name] = true;
            }        
        }
    }
}

