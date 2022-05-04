using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicGraph : MonoBehaviour {
    private int numberOfNodes;
    private Vector3 worldSize, worldPos;
    [Header("World attributes")]
    [SerializeField] private float nodeHalfextent;
    [SerializeField] private int moduleSize = 50;
    [SerializeField] private LayerMask colliderMask;
    private Dictionary<Vector3, Dictionary<Vector3, float>> masterGraph;
    public static DynamicGraph Instance;
    CallbackSystem.EventSystem eventSystem;
    private HashSet<Vector2Int> loadedModules;

    void Start() {
        masterGraph = new Dictionary<Vector3, Dictionary<Vector3, float>>();
        Instance ??= this;
        loadedModules = new HashSet<Vector2Int>();
        loadedModules.Add(new Vector2Int(0, 0));
        loadedModules.Add(new Vector2Int(1, 0));
        loadedModules.Add(new Vector2Int(0, 1));
        loadedModules.Add(new Vector2Int(1, 1));
        loadedModules.Add(new Vector2Int(0, 2));
        loadedModules.Add(new Vector2Int(1, 2));
        eventSystem = FindObjectOfType<CallbackSystem.EventSystem>();
        eventSystem.RegisterListener<CallbackSystem.ModuleSpawnEvent>(OnModuleLoad);
        eventSystem.RegisterListener<CallbackSystem.ModuleDeSpawnEvent>(OnModuleUnload);
    }

    public void Connect(Vector3 node1, Vector3 node2, float cost) {
        if (masterGraph.ContainsKey(node1) && masterGraph.ContainsKey(node2) && !IsConnected(node1, node2)) {
            masterGraph[node1].Add(node2, cost);
            masterGraph[node2].Add(node1, cost);
        }
    }

    public void Disconnect(Vector3 firstNode, Vector3 secondNode) {
        //TODO        
    }

    public bool Contains(Vector3 node) {
        return masterGraph.ContainsKey(node);
    }

    public float GetCost(Vector3 firstNode, Vector3 secondNode) {
        if (IsConnected(firstNode, secondNode)) return masterGraph[firstNode][secondNode];
        return -1;
    }

    public void Insert(Vector3 node) {
        if (!masterGraph.ContainsKey(node)) {
            masterGraph.Add(node, new Dictionary<Vector3, float>());
        }
    }

    public bool IsConnected(Vector3 firstNode, Vector3 secondNode) {
        if (masterGraph.ContainsKey(firstNode) && masterGraph.ContainsKey(secondNode)) return masterGraph[firstNode].ContainsKey(secondNode);
        return false;
    }

    public Dictionary<Vector3, float> GetNeighbors(Vector3 node) {
        if (masterGraph.ContainsKey(node)) return masterGraph[node];
        return null;
    }

    public Vector3 GetClosestNode(Vector3 pos) {
        Vector2Int modulePos = GetModulePosFromWorldPos(pos);
        Vector3 localWorldPos = new Vector3(modulePos.x + (moduleSize / 2) - 1, 0.55f, modulePos.y + (moduleSize / 2) - 1);
        float x = 0, y = localWorldPos.y, z = x;
        float leftMostX = localWorldPos.x - (moduleSize / 2) + nodeHalfextent;
        float topMostZ = localWorldPos.z + (moduleSize / 2) - nodeHalfextent;
        float rightMostX = localWorldPos.x + (moduleSize / 2) - nodeHalfextent;
        float bottomMostZ = localWorldPos.z - (moduleSize / 2) + nodeHalfextent;

        int nodesFromLeftEdge = (int)Mathf.FloorToInt(Mathf.Abs(pos.x - leftMostX) / (nodeHalfextent * 2));
        int nodesFromTopEdge = (int)Mathf.FloorToInt(Mathf.Abs(pos.z - topMostZ) / (nodeHalfextent * 2));

        x = leftMostX + (nodesFromLeftEdge * (nodeHalfextent * 2));
        z = topMostZ - (nodesFromTopEdge * (nodeHalfextent * 2));

        // literal edge cases
        if (pos.x < leftMostX) x = leftMostX;
        else if (pos.x > rightMostX) x = rightMostX;
        if (pos.z > topMostZ) z = topMostZ;
        else if (pos.z < bottomMostZ) z = bottomMostZ;

        return new Vector3(x, y, z);
    }

    public Vector3 GetClosestNodeNotBlocked(Vector3 target, Vector3 currentPosition) {

        Vector3 currentNode = GetClosestNode(target);
        Collider[] cols = GetBlockedNode(currentNode);
        if (cols.Length != 0) {
            Vector3 directionToMoveBack = (cols[0].transform.position - currentPosition).normalized;
            float longestExtent = cols[0].bounds.extents.x;
            if (longestExtent < cols[0].bounds.extents.y) longestExtent = cols[0].bounds.extents.y;
            else if (longestExtent < cols[0].bounds.extents.z) longestExtent = cols[0].bounds.extents.z;
            if (longestExtent < nodeHalfextent * 2) longestExtent = nodeHalfextent * 2;
            currentNode += directionToMoveBack * longestExtent;
            return GetClosestNodeNotBlocked(currentNode, currentPosition);
        } else {
            currentNode = GetClosestNode(currentNode);
        }
        return currentNode;
    }

    private Vector3[] GetPossibleNeighbors(Vector3 node) {
        Vector3 firstPossibleXNeighbor = new Vector3(node.x + nodeHalfextent * 2, node.y, node.z),
        secondPossibleXNeighbor = new Vector3(node.x - nodeHalfextent * 2, node.y, node.z),
        firstPossibleZNeighbor = new Vector3(node.x, node.y, node.z + nodeHalfextent * 2),
        secondPossibleZNeighbor = new Vector3(node.x, node.y, node.z - nodeHalfextent * 2);
        return new Vector3[] { firstPossibleXNeighbor, secondPossibleXNeighbor, firstPossibleZNeighbor, secondPossibleZNeighbor };
    }

    public Dictionary<Vector3, float> GetPossibleNeighborsKV(Vector3 node) {
        Vector3 firstPossibleXNeighbor = new Vector3(node.x + nodeHalfextent * 2, node.y, node.z),
        secondPossibleXNeighbor = new Vector3(node.x - nodeHalfextent * 2, node.y, node.z),
        firstPossibleZNeighbor = new Vector3(node.x, node.y, node.z + nodeHalfextent * 2),
        secondPossibleZNeighbor = new Vector3(node.x, node.y, node.z - nodeHalfextent * 2);

        Dictionary<Vector3, float> tempNeighbors = new Dictionary<Vector3, float>();
        tempNeighbors.Add(firstPossibleXNeighbor, 1);
        tempNeighbors.Add(secondPossibleXNeighbor, 1);
        tempNeighbors.Add(firstPossibleZNeighbor, 1);
        tempNeighbors.Add(secondPossibleZNeighbor, 1);
        return tempNeighbors;
    }

    public Collider[] GetBlockedNode(Vector3 position) {
        return Physics.OverlapBox(position, new Vector3(nodeHalfextent, nodeHalfextent, nodeHalfextent), Quaternion.identity, colliderMask);
    }


    // Beware: this tanks the FPS *HARD* but is useful to see the generated pathfinding grid
    private void OnDrawGizmos() {
        int count = 0;
        foreach (Vector3 v in masterGraph.Keys) {
            bool blocked = GetBlockedNode(v).Length == 0;
            if (blocked) {
                Gizmos.color = Color.green;
                Gizmos.DrawWireCube(v, new Vector3(nodeHalfextent * 2, nodeHalfextent * 2, nodeHalfextent * 2));
            } else {
                Gizmos.color = Color.red;
                Gizmos.DrawWireCube(v, new Vector3(nodeHalfextent * 2, nodeHalfextent * 2, nodeHalfextent * 2));
            }
            count++;
        }
    }

    public Vector2Int GetModulePosFromWorldPos(Vector3 worldPos) {
        return new Vector2Int((int)worldPos.x / moduleSize, (int)worldPos.z / moduleSize);
    }

    public bool IsModuleLoaded(Vector2Int modulePos) {
        return loadedModules.Contains(modulePos);
    }

    public void CreateNeighbors(Vector3 node, Dictionary<Vector3, float> possibleNeighbors) {
        foreach (Vector3 pNeighbor in possibleNeighbors.Keys) {
            if (IsModuleLoaded(GetModulePosFromWorldPos(node)) && !masterGraph.ContainsKey(pNeighbor)) {
                Insert(pNeighbor);
                Connect(node, pNeighbor, possibleNeighbors[pNeighbor]);
            }
            foreach (Vector3 neighborNeighbor in GetPossibleNeighbors(pNeighbor)) {
                Connect(neighborNeighbor, pNeighbor, 1);
            }
        }
    }

    void OnModuleLoad(CallbackSystem.ModuleSpawnEvent spawnEvent) {
        loadedModules.Add(spawnEvent.Position);
    }

    void OnModuleUnload(CallbackSystem.ModuleDeSpawnEvent deSpawnEvent) {
        if (loadedModules.Contains(deSpawnEvent.Position)) loadedModules.Remove(deSpawnEvent.Position);
    }



}
