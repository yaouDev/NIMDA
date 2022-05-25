using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class OpenExit : MonoBehaviour {
    [SerializeField] private float openHeight = -11.7f;
    [SerializeField] private float eventDuration = 5;
    [SerializeField] private GameObject door;
    [SerializeField] private GameObject textPopup;

    private float timeElapsed;
    private bool doorOpen;
    private bool interactableRange = false;
    private Vector3 closePosition;
    private Vector3 openPosition;
    private int playerCount = 0;

    // Start is called before the first frame update
    void Start() {
        closePosition = door.transform.position;
    }


    /*    private void OnTriggerStay(Collider col)
        {
            if (col.CompareTag("Player"))
            {
                textPopup.SetActive(true);
                interactableRange = true;
            }
        }*/
    private void OnTriggerEnter(Collider col) {
        if (col.CompareTag("Player")) {
            playerCount++;
            if (playerCount == 1) {
                textPopup.SetActive(true);
                OpenDoor();
            }
        }
    }

    public void OpenDoor() {
        if (!doorOpen) {
            //Debug.Log("Opening");
            openPosition = closePosition + Vector3.up * openHeight;
            StartCoroutine(MoveDoor(openPosition, eventDuration));

        }
    }

    IEnumerator MoveDoor(Vector3 targetPosition, float duration) {
        timeElapsed = 0;
        while (door.transform.position != targetPosition) {
            //Debug.Log("Moving Door");
            door.transform.position = Vector3.Lerp(closePosition, targetPosition, timeElapsed / duration);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        doorOpen = true;

    }
    /*    public void Interact(InputAction.CallbackContext value)
        {
            if (value.started && interactableRange)
            {
                OpenDoor();
            }
        }*/

}
