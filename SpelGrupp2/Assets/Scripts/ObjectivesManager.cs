using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ObjectivesManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI[] objectives;
    [SerializeField] private GameObject background;
    private float backgroundHeight = 125;
    private float backgroundWidth = 400;
    private int objectivesIndex = -1;
    private bool bossNext = false;

    // Start is called before the first frame update
    void Start()
    {
        objectives[0].text = "destroy the virus";
        objectivesIndex++;
        AddObjective("find the first safe room");
    }

    public void AddObjective(string newObjective)
    {
        objectivesIndex++;
        objectives[objectivesIndex].text = newObjective;
        backgroundHeight += 40;
        background.GetComponent<RectTransform>().sizeDelta = new Vector2(backgroundWidth, backgroundHeight);
    }

    public void RemoveObjective(string oldObjective)
    {
        for (int i = 0; i < objectives.Length; i++)
        {
            if (objectives[i].text == oldObjective)
            {
                objectives[objectivesIndex].text = "";
                objectivesIndex--;
                backgroundHeight -= 40;
                background.GetComponent<RectTransform>().sizeDelta = new Vector2(backgroundWidth, backgroundHeight);
            }
        }
    }

    public void FlipBossBool()
    {
        bossNext = !bossNext;
    }

    public bool BossNext()
    {
        return bossNext;
    }
}
