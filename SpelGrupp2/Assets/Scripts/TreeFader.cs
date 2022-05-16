using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeFader : MonoBehaviour
{
    private Material[] materials;
    private bool fadingOut;
    private float targetTransparency = 1.0f;
    [SerializeField] private MeshRenderer[] meshRenderer;
    [SerializeField] private float minTransparency = .1f;
    [SerializeField][Range(0.0f, 1.0f)] private float transparency = 1.0f;

    private void Awake()
    {
        materials = new Material[meshRenderer.Length];
        for (int i = 0; i < meshRenderer.Length; i++)
        {
            materials[i] = meshRenderer[i].material;
        }
    }
    
    private void Update()
    {
        if (fadingOut)
        {
            transparency -= Time.deltaTime;
            
            if (transparency <= minTransparency)
            {
                fadingOut = false;
                transparency = minTransparency;
            }
            SetFade(transparency);
        }
        else if (transparency < 1.0f)
        {
            transparency += Time.deltaTime;
            
            if (transparency >= 1.0f)
            {
                transparency = 1.0f;
            }
            SetFade(transparency);
        }
    }

    private void Debug()
    {
        float sin = Mathf.Max(minTransparency, Mathf.Abs( Mathf.Sin(Time.time)));
        transparency = sin;
        for (int i = 0; i < materials.Length; i++)
        {
            materials[i].SetFloat("_transparency", Mathf.Lerp(.5f, 1.5f, transparency));
        }
    }
    
    // public void FadeIn()
    // {
    //     fadingOut = false;
    //     StopAllCoroutines();
    //     StartCoroutine(FadeInCoroutine());
    // }
    
    public void FadeOut()
    {
        if (!fadingOut)
        {
            fadingOut = true;
            // StopAllCoroutines();
            // StartCoroutine(FadeOutCoroutine());
        }
    }

    // private IEnumerator FadeInCoroutine()
    // {
    //     while (transparency <= 1.0f)
    //     {
    //         transparency += Time.deltaTime;
    //         SetFade(transparency);
    //         yield return null;
    //     }
    //
    //     transparency = 1.0f;
    //     SetFade(transparency);
    // }
    //
    // private IEnumerator FadeOutCoroutine()
    // {
    //     while (transparency >= minTransparency)
    //     {
    //         transparency -= Time.deltaTime;
    //         SetFade(transparency);
    //         yield return null;
    //     }
    //
    //     transparency = minTransparency;
    //     SetFade(transparency);
    // }

    private void SetFade(float t)
    {
        for (int i = 0; i < materials.Length; i++)
        {
            materials[i].SetFloat("_transparency", Mathf.Lerp(.5f, 1.5f, t));
        }
    }
}
