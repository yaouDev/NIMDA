using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenExit : MonoBehaviour
{
    [SerializeField] private float openHeight = -11.7f;
    [SerializeField] private float eventDuration = 5;
    [SerializeField] private GameObject door;
    [SerializeField] private GameObject textPopup;
    private bool doorOpen;
    private Vector3 closePosition;

    // Start is called before the first frame update
    void Start()
    {
        closePosition = door.transform.position;
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.CompareTag("Player"))
        {
            textPopup.SetActive(true);
            Vector3 openPosition = closePosition + Vector3.up * openHeight;
            StartCoroutine(MoveDoor(openPosition, eventDuration));
        }
    }

    IEnumerator MoveDoor(Vector3 targetPosition, float duration)
    {
        float timeElapsed = 0;
        Vector3 startPosition = door.transform.position;
        while (timeElapsed < duration)
        {
            //Debug.Log("Moving Door");
            door.transform.position = Vector3.Lerp(startPosition, targetPosition, timeElapsed / duration);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        door.transform.position = targetPosition;
        doorOpen = true;
    }
}
