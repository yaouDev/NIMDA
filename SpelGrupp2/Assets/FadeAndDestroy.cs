using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FadeAndDestroy : MonoBehaviour
{
    [SerializeField] private Vector3 fadeDistance = Vector3.up;
    private TextMeshProUGUI tesh;

    private void Awake()
    {
        tesh = GetComponent<TextMeshProUGUI>();
        StartCoroutine(SpawnFadingText());
    }

    IEnumerator SpawnFadingText()
    {
        if (true)
        {
            for (float i = 1; i >= 0; i -= Time.deltaTime)
            {
                tesh.color = new Color(0, 1, 0, i);
                tesh.transform.position += fadeDistance;
                yield return null;
            }
            Destroy(gameObject);

        }
    }
}
