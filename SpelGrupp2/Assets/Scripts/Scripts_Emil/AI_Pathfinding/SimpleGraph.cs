using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleGraph : MonoBehaviour {
    private int numberOfNodes;
    private Vector3 worldSize, worldPos;
    [Header("World attributes")]
    [SerializeField] private float nodeHalfextent;
    [SerializeField] private LayerMask colliderMask;
    [SerializeField] private int moduleSize = 50;
    private Dictionary<Vector3, Dictionary<Vector3, float>> masterGraph;
    public static SimpleGraph Instance;
    CallbackSystem.EventSystem eventSystem;
    private Vector2Int[] loadedModules;

    void Awake() {
        masterGraph = new Dictionary<Vector3, Dictionary<Vector3, float>>();
        worldPos = transform.position; //gameObject.GetComponent<BoxCollider>().center; //transform.TransformPoint(gameObject.GetComponent<BoxCollider>().center);
        worldPos.y = gameObject.GetComponent<BoxCollider>().center.y;
        worldSize = gameObject.transform.localScale;
        worldPos.y += worldSize.y / 2;
        numberOfNodes = (int)((worldSize.x / (nodeHalfextent * 2)) * (worldSize.y / (nodeHalfextent * 2)) * (worldSize.z / (nodeHalfextent * 2)));
        InitGraph();
        Instance ??= this;
        loadedModules = new Vector2Int[6];
        eventSystem = FindObjectOfType<CallbackSystem.EventSystem>();
        /*       eventSystem.RegisterListener<LoadEvent>(methodHere);
              eventSystem.RegisterListener<UnloadEvent>(methodHere); */
    }

    public void Connect(Vector3 node1, Vector3 node2, float cost) {
        if (masterGraph.ContainsKey(node1) && masterGraph.ContainsKey(node2) && !IsConnected(node1, node2)) {
            masterGraph[node1].Add(node2, cost);
            masterGraph[node2].Add(node1, cost);
        }
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

    public Dictionary<Vector3, float> getNeighbors(Vector3 node) {
        if (masterGraph.ContainsKey(node)) return masterGraph[node];
        return null;
    }

    public Vector3 GetClosestNode(Vector3 pos) {
        Vector3 localWorldPos = new Vector3(worldPos.x, worldPos.y + (worldSize.y / 2), worldPos.z);
        float x = 0, y = localWorldPos.y, z = x;
        float leftMostX = worldPos.x - (worldSize.x / 2) + nodeHalfextent;
        float topMostZ = worldPos.z + (worldSize.z / 2) - nodeHalfextent;
        float rightMostX = worldPos.x + (worldSize.x / 2) - nodeHalfextent;
        float bottomMostZ = worldPos.z - (worldSize.z / 2) + nodeHalfextent;

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

    // this is *very* hardcoded and will most likely not work if the the "world" changes
    private void InitGraph() {
        float startXPos = worldPos.x - worldSize.x / 2;
        float startZPos = worldPos.z + worldSize.z / 2;
        Vector3 startPos = new Vector3(startXPos, worldPos.y, startZPos);
        startPos.x += nodeHalfextent;
        startPos.z -= nodeHalfextent;
        startPos.y += nodeHalfextent;
        Insert(startPos);
        Vector3 currentPos = startPos;
        Vector3 prevPos = startPos;
        while (masterGraph.Count < numberOfNodes) {
            // whole row is done
            if (currentPos.x >= worldPos.x + worldSize.x / 2 - nodeHalfextent) {
                currentPos.x = startPos.x;
                currentPos.z -= nodeHalfextent * 2;
            }
            // whole "vertical layer" is done 
            else if (currentPos.x >= worldPos.x + worldSize.x / 2 - nodeHalfextent && currentPos.z <= worldPos.z - worldSize.z / 2 + nodeHalfextent) {
                currentPos.x = startPos.x;
                currentPos.z = startPos.z;
                currentPos.y += nodeHalfextent * 2;
            } else {
                currentPos.x += nodeHalfextent * 2;
            }
            Insert(currentPos);
        }
        foreach (Vector3 node in masterGraph.Keys) {
            Vector3[] possibleNeighbors = GetPossibleNeighbors(node);
            foreach (Vector3 pNeighbor in possibleNeighbors) {
                Connect(node, pNeighbor, 1);
            }
        }
    }

    private Vector3[] GetPossibleNeighbors(Vector3 node) {
        Vector3 firstPossibleXNeighbor = new Vector3(node.x + nodeHalfextent * 2, node.y, node.z),
        secondPossibleXNeighbor = new Vector3(node.x - nodeHalfextent * 2, node.y, node.z),
        firstPossibleYNeighbor = new Vector3(node.x, node.y + nodeHalfextent * 2, node.z),
        secondPossibleYNeighbor = new Vector3(node.x, node.y - nodeHalfextent * 2, node.z),
        firstPossibleZNeighbor = new Vector3(node.x, node.y, node.z + nodeHalfextent * 2),
        secondPossibleZNeighbor = new Vector3(node.x, node.y, node.z + nodeHalfextent * 2);
        return new Vector3[] { firstPossibleXNeighbor, secondPossibleXNeighbor, firstPossibleYNeighbor, secondPossibleYNeighbor, firstPossibleZNeighbor, secondPossibleZNeighbor };
    }

    Dictionary<Vector3, float> GetPossibleNeighborsKV(Vector3 node) {
        Vector3 firstPossibleXNeighbor = new Vector3(node.x + nodeHalfextent * 2, node.y, node.z),
        secondPossibleXNeighbor = new Vector3(node.x - nodeHalfextent * 2, node.y, node.z),
        firstPossibleYNeighbor = new Vector3(node.x, node.y + nodeHalfextent * 2, node.z),
        secondPossibleYNeighbor = new Vector3(node.x, node.y - nodeHalfextent * 2, node.z),
        firstPossibleZNeighbor = new Vector3(node.x, node.y, node.z + nodeHalfextent * 2),
        secondPossibleZNeighbor = new Vector3(node.x, node.y, node.z + nodeHalfextent * 2);

        Dictionary<Vector3, float> tempNeighbors = new Dictionary<Vector3, float>();
        tempNeighbors.Add(firstPossibleXNeighbor, 1);
        tempNeighbors.Add(secondPossibleXNeighbor, 1);
        tempNeighbors.Add(firstPossibleYNeighbor, 1);
        tempNeighbors.Add(secondPossibleYNeighbor, 1);
        tempNeighbors.Add(firstPossibleZNeighbor, 1);
        tempNeighbors.Add(secondPossibleZNeighbor, 1);
        return tempNeighbors;
    }

    public Collider[] GetBlockedNode(Vector3 position) {
        return Physics.OverlapBox(position, new Vector3(nodeHalfextent, nodeHalfextent, nodeHalfextent), Quaternion.identity, colliderMask);
    }


    // Beware: this tanks the FPS *HARD* but is useful to see the generated pathfinding grid
    /* private void OnDrawGizmos() {
        int count = 0;
        foreach (Vector3 v in nodes.Keys) {
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
    } */

    Vector2Int GetModulePosFromWorldPos(Vector3 worldPos) {
        return new Vector2Int((int)worldPos.x / moduleSize, (int)worldPos.z / moduleSize);
    }

    private bool IsModuleLoaded(Vector2Int modulePos) {
        foreach (Vector2Int pos in loadedModules) {
            if (modulePos == pos) return true;
        }
        return false;
    }

    public void CreateNeighbors(Vector3 node) {
        foreach (Vector3 pNeighbor in GetPossibleNeighborsKV(node).Keys) {
            if (IsModuleLoaded(GetModulePosFromWorldPos(node))) {
                masterGraph.Add(pNeighbor, new Dictionary<Vector3, float>());
                Connect(node, pNeighbor, 1);
            }
        }
    }


}
