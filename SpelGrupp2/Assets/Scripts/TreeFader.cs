using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeFader : MonoBehaviour
{
    [SerializeField] private MeshRenderer[] meshRenderer;
    private Material[] materials;
    [SerializeField][Range(0.0f, 1.0f)] private float transparency;
    private void Awake()
    {
        //meshRenderer = GetComponentsInChildren<MeshRenderer>();
        materials = new Material[meshRenderer.Length];
        for (int i = 0; i < meshRenderer.Length; i++)
        {
            materials[i] = meshRenderer[i].material;
        }
    }

    public float MinTransparency = .1f;
    private void Update()
    {
        float sin = Mathf.Max(MinTransparency, Mathf.Abs( Mathf.Sin(Time.time)));
        transparency = sin;
        for (int i = 0; i < materials.Length; i++)
        {
            materials[i].SetFloat("_transparency", Mathf.Lerp(.5f, 1.5f, transparency));
        }
    }
}
