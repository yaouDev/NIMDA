using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

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
    [SerializeField] private ObjectivesManager objectivesManager;
    private bool isRunning;

    // Start is called before the first frame update
    void Start()
    {
        closePosition = door.transform.position;
        siren.enabled = false; 
        spawnController = GameObject.Find("EnemySpawnController").GetComponent<EnemySpawnController>();
        objectivesManager = GameObject.Find("ObjectivesManager").GetComponent<ObjectivesManager>();
    }

/*    private void OnTriggerStay(Collider col)
    {
        if (col.CompareTag("Player"))
        {
            textPopup.SetActive(true);
            interactableRange = true;

        }
    }*/
    private void OnTriggerEnter(Collider col)
    {
        if (col.CompareTag("Player") && !isRunning)
        {
            textPopup.SetActive(true);
            isRunning = true;
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
            siren.enabled = true;
            openPosition = closePosition + Vector3.up * openHeight;
            StartCoroutine(MoveDoor(openPosition, eventDuration));
            spawnController.GeneratorRunning(true);
            objectivesManager.RemoveObjective("start the generator");
            //objectivesManager.AddObjective("let the generator finish");
            objectivesManager.AddObjective("survive the horde");
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
        //To-do: uppdatera inte objectives via coroutine
        //objectivesManager.RemoveObjective("let the generator finish");
        //objectivesManager.RemoveObjective("open the safe room");
        objectivesManager.RemoveObjective("survive the horde");
        objectivesManager.AddObjective("enter safe room");
    }
/*    public void Interact(InputAction.CallbackContext value)
    {
       
        if (value.performed && interactableRange)
        {
            Debug.Log(value);
            Debug.Log("startas");
            StartGenerator();
            textPopup.GetComponent<TextMeshPro>().text = "";
            objectivesManager.RemoveObjective("start the generator");
            objectivesManager.AddObjective("let the generator finish");
            objectivesManager.AddObjective("survive the horde");
        }
    }*/
}
