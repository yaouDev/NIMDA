using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamGizmo : MonoBehaviour
{
    
    [SerializeField] private Camera cam; 
    Plane plane = new Plane(Vector3.up, Vector3.zero);
    private bool started;
    void Start()
    {
        cam = GetComponent<Camera>();
        plane = new Plane(Vector3.up, Vector3.zero);
        points = new Vector3[] {p1, p2, p3, p4};
        sPoints = new Vector2[] { Vector2.zero, Vector2.up, Vector2.one, Vector2.right };
        started = true;
    }

    //private Plane plane;
    private float enter = 0;
    private Vector2 s1, s2, s3, s4;
    private Vector2[] sPoints;
    private Vector3 p1, p2, p3, p4;
    private Vector3[] points;
    private void OnDrawGizmos()
    {
        if (!started) return;
        for (int i = 0; i < points.Length; i++)
        {
            Ray ray = cam.ViewportPointToRay(sPoints[i]);
            
            if (plane.Raycast(ray, out enter))
            {
                points[i] = ray.GetPoint(enter);
            }
        }

        for (int i = 0; i < points.Length; i++)
        {
            Gizmos.DrawLine(points[i % points.Length], points[(i + 1) % points.Length]);
        }
    }
}
