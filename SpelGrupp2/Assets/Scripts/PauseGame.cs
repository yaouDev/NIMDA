using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseGame : MonoBehaviour
{

    [SerializeField] private GameObject pauseScreen;

    // Start is called before the first frame update
    void Start()
    {
        pauseScreen.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("HELP"))
        {
            if (pauseScreen.activeInHierarchy)
            {
                Unpause();
            }
            if (!pauseScreen.activeInHierarchy)
            {
                Pause();
            }
        }
    }

    private void Pause()
    {
        Time.timeScale = 0;
        pauseScreen.SetActive(true);
    }

    private void Unpause()
    {
        Time.timeScale = 1;
        pauseScreen.SetActive(false);
    }
}
