using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ObjectivesManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI[] objectives; // Array of objective "spots" to be used -Willow
    [SerializeField] private GameObject background; // Panel behind objectives text -Willow
    private float backgroundHeight = 85; // Starting height of background panel -Willow
    private float backgroundWidth = 400; // Background panel width. Needed for RectTransform.sizeDelta() method -Willow
    private int objectivesIndex = -1; // Index used for TMP array -Willow
    private bool bossNext = false; // bool used by safe rooms calling on manager -Willow

    // Start is called before the first frame update
    void Start()
    {
        AddObjective("destroy the virus");
        AddObjective("find the first safe room");
    }

    public void AddObjective(string newObjective)
    {
        objectivesIndex++; // Updates objectivesIndex -Willow
        objectives[objectivesIndex].text = newObjective; // Sets TextMeshPro-element in index's position to match string parameter -Willow
        backgroundHeight += 40; // Updates background panel height to correspond to new objective in list -Willow
        background.GetComponent<RectTransform>().sizeDelta = new Vector2(backgroundWidth, backgroundHeight); // Sets background panel to have said height -Willow
    }

    public void RemoveObjective(string oldObjective)
    {
        for (int i = 0; i < objectives.Length; i++) // Iterates through TextMeshPro-elements -Willow
        {
            if (objectives[i].text == oldObjective) // Compares text component (string) of current TMP element to the parameter (string) -Willow
            {
                objectives[objectivesIndex].text = ""; // Sets text component of the relevant TMP element to be blank -Willow
                objectivesIndex--; // Updates objectivesIndex -Willow
                backgroundHeight -= 40; // Updates background panel height to correspond to removed objective -Willow
                background.GetComponent<RectTransform>().sizeDelta = new Vector2(backgroundWidth, backgroundHeight); // Sets background panel to have said height -Willow
            }
        }
    }

    public bool BossNext()
    {
        return bossNext; // Used by Safe Room scripts to determine which objective to add to the list upon safe room exit.
    }

    public void FlipBossBool()
    {
        bossNext = !bossNext; // Used by Safe Room scripts. Called by first Safe Room in level so the next one will know not to add the wrong objective to the list.
    }

}
