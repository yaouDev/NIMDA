using UnityEngine;

public class MoveToPosition : MonoBehaviour
{
    [SerializeField] private Vector3 offset;
    [SerializeField] private Quaternion rotationOffset;
    [SerializeField] private Transform followTransform;

    private void Update()
    {
        transform.position = followTransform.position + offset;
        transform.rotation = followTransform.rotation * rotationOffset;
    }
}