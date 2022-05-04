using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraphManager : MonoBehaviour {

    Dictionary<Vector3, Dictionary<Vector3, float>> masterGraph;
    Vector2Int[] currentlyLoadedModules;

    Callbacks.EventSystem eventSystem;

    void Start() {
        masterGraph = new Dictionary<Vector3, Dictionary<Vector3, float>>();
        currentlyLoadedModules = new Vector2Int[4];
        // Needs to be the correct eventsystem
        eventSystem = FindObjectOfType<Callbacks.EventSystem>();

        // listen for when modules are loaded and unloaded to update master graph

        //eventSystem.RegisterListener<LoadEvent>();
        //eventSystem.RegisterListener<UnloadEvent>();
    }


}