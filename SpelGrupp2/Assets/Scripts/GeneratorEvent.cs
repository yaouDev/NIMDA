using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneratorEvent : MonoBehaviour
{

    [SerializeField] private float openHeight;
    [SerializeField] private float eventDuration = 60;
    [SerializeField] private GameObject door;
    [SerializeField] private GameObject textPopup;
    private bool doorOpen;
    private Vector3 closePosition;
    [SerializeField] private EnemySpawnController spawnController;

    // Start is called before the first frame update
    void Start()
    {
        closePosition = door.transform.position;
        spawnController = GameObject.Find("EnemySpawnController").GetComponent<EnemySpawnController>();
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.CompareTag("Player"))
        {
            textPopup.SetActive(true);
            //Debug.Log("Starting Generator");
            StartGenerator();
        }
    }

    void OnTriggerExit()
    {
        textPopup.SetActive(false);
    }

    void StartGenerator()
    {
        if (!doorOpen)
        {
            //Debug.Log("Opening");
            Vector3 openPosition = closePosition + Vector3.up * openHeight;
            StartCoroutine(MoveDoor(openPosition, eventDuration));
            spawnController.GeneratorRunning(true);
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
