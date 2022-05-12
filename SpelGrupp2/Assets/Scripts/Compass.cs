using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Compass : MonoBehaviour {
    public RectTransform arrow;
    public Transform player;
    private Transform quest;
    public Transform quest1;
    public Transform quest2;

    void Start()
    {
        quest = quest1;
    }

    [SerializeField] private Quaternion camRot;
    void Update() {
        Vector3 dir = camRot * (new Vector2(quest.transform.position.x, quest.transform.position.z) - new Vector2(player.transform.position.x, player.transform.position.z));
        float angle = Vector2.SignedAngle(Vector2.right, dir);
        arrow.rotation = Quaternion.Euler(0, 0, angle);
    }

    public void UpdateQuest()
    {
        quest = quest2;
    }
}
