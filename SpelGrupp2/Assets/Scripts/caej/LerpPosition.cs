using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LerpPosition : MonoBehaviour
{
    private Vector3 startPosition;
    public Vector3 offset;
    [SerializeField] private AnimationCurve curve;
    [SerializeField] private float speed = 1f;
    [SerializeField] private float lerpThreshold = 0.01f;
    private float current;
    private float target = 1f;

    // Start is called before the first frame update
    void Start()
    {
        startPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        current = Mathf.MoveTowards(current, target, speed * Time.deltaTime);
        transform.position = Vector3.Lerp(startPosition, startPosition + offset, curve.Evaluate(current));

        if (Vector3.Distance(transform.position, startPosition + offset) < lerpThreshold) target = 0;
        if (Vector3.Distance(transform.position, startPosition) < lerpThreshold) target = 1;
    }
}
