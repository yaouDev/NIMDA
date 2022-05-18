using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace CallbackSystem
{
    public class FadingTextListener : MonoBehaviour
    {
        [SerializeField] private GameObject[] positions;
        [SerializeField] private GameObject prefab;
        private Transform pos;
        private TextMeshProUGUI tesh;

        private void Start()
        {
            EventSystem.Current.RegisterListener<FadingTextEvent>(ShowAndFade);
        }

        private void ShowAndFade(FadingTextEvent eve)
        {
            pos = eve.isPlayerOne ? positions[0].transform : positions[1].transform;
            tesh = prefab.GetComponent<TextMeshProUGUI>();
            tesh.text = eve.text;
            Instantiate(prefab, pos);
            Debug.Log($"{tesh.text + ": called true"}");
        }
    }
}

