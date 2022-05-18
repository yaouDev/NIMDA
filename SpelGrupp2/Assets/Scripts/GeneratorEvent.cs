using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GeneratorEvent : MonoBehaviour
{
    [SerializeField] private float openHeight;
    [SerializeField] private float eventDuration = 20;
    [SerializeField] private GameObject door;
    [SerializeField] private GameObject textPopup;
    [SerializeField] private EnemySpawnController spawnController;
    [SerializeField] private Light siren;
    private float timeElapsed;
    private bool doorOpen;
    private bool interactableRange = false; 
    private Vector3 closePosition;
    private Vector3 openPosition;

    // Start is called before the first frame update
    void Start()
    {
        closePosition = door.transform.position;
        siren.enabled = false; 
        spawnController = GameObject.Find("EnemySpawnController").GetComponent<EnemySpawnController>();
    }

    private void OnTriggerStay(Collider col)
    {
        if (col.CompareTag("Player"))
        {
            textPopup.SetActive(true);
            interactableRange = true;

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
            siren.enabled = true;
            openPosition = closePosition + Vector3.up * openHeight;
            StartCoroutine(MoveDoor(openPosition, eventDuration));
            spawnController.GeneratorRunning(true);
        }
    }

    IEnumerator MoveDoor(Vector3 targetPosition, float duration)
    {
        timeElapsed = 0;
        while (door.transform.position != targetPosition)
        {
            //Debug.Log("Moving Door");
            door.transform.position = Vector3.Lerp(closePosition, targetPosition, timeElapsed / duration);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        doorOpen = true;
        siren.enabled = false; 

    }
    public void Interact(InputAction.CallbackContext value)
    {
        if (value.started && interactableRange)
        {
            StartGenerator();
        }
    }
}
