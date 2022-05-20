using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Breakhealthbar : MonoBehaviour
{
    [SerializeField] private Vector3 offset;
    private Image image;

    private void Awake()
    {
        GetComponent<Rigidbody>().AddForce(offset, ForceMode.Impulse);
        image = GetComponent<Image>();
        StartCoroutine(FadeImage());
    }

    IEnumerator FadeImage()
    {
        if (true)
        {
            for (float i = 1; i >= 0; i -= Time.deltaTime)
            {
                image.color = new Color(0, 0, 0, i);
                yield return null;
            }
        }
        Destroy(gameObject);
    }
}
