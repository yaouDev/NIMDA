using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FadeHint : MonoBehaviour
{
    //[SerializeField] private Vector3 fadeDistance = Vector3.up;
    [SerializeField] private CanvasGroup group;

    private void Start()
    {
        group.alpha = 0;
    }

    private void OnTriggerEnter(Collider col)
    {
        if (col.CompareTag("Player"))
        {
            Debug.Log("Entered");
            //tesh = GetComponent<TextMeshProUGUI>();
            group.alpha = 1;
            StartCoroutine(FadeText());
        }
    }

    IEnumerator FadeText()
    {
        yield return new WaitForSeconds(2);
        while ( group.alpha>0)
            {
                group.alpha -= Time.deltaTime;
                yield return null;
                //tesh.transform.position += fadeDistance;
            }
            //Destroy(gameObject);
            //tesh.SetActive(false);
    }
}
