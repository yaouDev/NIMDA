using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleGraph : MonoBehaviour {
    private int numberOfNodes;
    private Vector3 worldSize, worldPos;
    [SerializeField] private float nodeHalfextent;
    [SerializeField] LayerMask colliderMask;
    private const int POSSIBLE_EDGES = 6;
    private Dictionary<Vector3, Dictionary<Vector3, float>> nodes;
    public static SimpleGraph instance;

    void Awake() {
        nodes = new Dictionary<Vector3, Dictionary<Vector3, float>>();
        worldPos = gameObject.GetComponent<BoxCollider>().center;
        worldSize = gameObject.transform.localScale;
        worldPos.y += worldSize.y / 2;
        numberOfNodes = (int)((worldSize.x / (nodeHalfextent * 2)) * (worldSize.y / (nodeHalfextent * 2)) * (worldSize.z / (nodeHalfextent * 2)));
        initGraph();
        instance ??= this;
    }

    public void connect(Vector3 node1, Vector3 node2, float cost) {
        if (nodes.ContainsKey(node1) && nodes.ContainsKey(node2) && !isConnected(node1, node2)) {
            nodes[node1].Add(node2, cost);
            nodes[node2].Add(node1, cost);
        }
    }

    public float getCost(Vector3 node1, Vector3 node2) {
        if (isConnected(node1, node2)) return nodes[node1][node2];
        return -1;
    }

    public void insert(Vector3 node) {
        if (!nodes.ContainsKey(node)) {
            nodes.Add(node, new Dictionary<Vector3, float>());
        }
    }

    public bool isConnected(Vector3 node1, Vector3 node2) {
        if (nodes.ContainsKey(node1) && nodes.ContainsKey(node2)) return nodes[node1].ContainsKey(node2);
        return false;
    }

    public Dictionary<Vector3, float> getNeighbors(Vector3 node) {
        if (nodes.ContainsKey(node)) return nodes[node];
        return null;
    }

    // gives the exact closest box but has to loop through all the nodes in the graph each time. Not effective
    public Vector3 getClosestBox2(Vector3 pos) {
        float shortestDistAway = Mathf.Infinity;
        Vector3 currentSmallestNode = Vector3.zero;
        foreach (Vector3 node in nodes.Keys) {
            Vector3 tmpVec = new Vector3(Mathf.Abs(node.x - pos.x), Mathf.Abs(node.y - pos.y), Mathf.Abs(node.z - pos.z));
            if (tmpVec.magnitude < shortestDistAway) {
                currentSmallestNode = node;
                shortestDistAway = tmpVec.magnitude;
            }
        }
        return currentSmallestNode;
    }

    // gives the closest box in constant O(1) time. Much better than above
    public Vector3 getClosestBox(Vector3 pos) {
        Vector3 localWorldPos = new Vector3(worldPos.x, worldPos.y + (worldSize.y / 2), worldPos.z);
        float x = 0, y = x, z = x;
        float[] axis = new float[3] { x, y, z };
        for (int i = 0; i < 3; i++) {
            float currentWorldAxis = 0, currentAxis = 0, currentWorldAxisSize = 0;
            switch (i) {
                case 0:
                    currentWorldAxis = localWorldPos.x;
                    currentWorldAxisSize = worldSize.x;
                    currentAxis = pos.x;
                    break;
                case 1:
                    currentWorldAxis = localWorldPos.y;
                    currentWorldAxisSize = worldSize.y;
                    currentAxis = pos.y;
                    break;
                case 2:
                    currentWorldAxis = localWorldPos.z;
                    currentWorldAxisSize = worldSize.z;
                    currentAxis = pos.z;
                    break;
            }
            int numberOfNodesFromCenter = Mathf.Abs((int)((currentWorldAxisSize / 2) / (nodeHalfextent * 2)));
            float distanceFromCenter = Mathf.Abs(Mathf.Abs(currentWorldAxis) - Mathf.Abs(currentAxis));
            float numberOfNodesToPos = distanceFromCenter / (nodeHalfextent * 2);
            if (numberOfNodesToPos - (int)numberOfNodesToPos <= 0.5f) numberOfNodesToPos += 0.5f;
            int nodesToMove = nodesToMove = numberOfNodesFromCenter - (int)Mathf.Round(numberOfNodesToPos);
            if (nodesToMove < 0) nodesToMove = 0;
            float moveDist = nodesToMove * (nodeHalfextent * 2);
            if (currentAxis < 0) axis[i] = currentWorldAxis - (currentWorldAxisSize / 2) + moveDist + nodeHalfextent;
            else axis[i] = currentWorldAxis + (currentWorldAxisSize / 2) - moveDist - nodeHalfextent;
        }
        return new Vector3(axis[0], axis[1], axis[2]);
    }

    public Vector3 getClosestBoxNotBlocked(Vector3 target, Vector3 currentPosition) {

        Vector3 currentBox = getClosestBox(target);
        Collider[] cols = getBlockedNode(currentBox);
        if (cols.Length != 0) {
            Vector3 directionToMoveBack = (cols[0].transform.position - currentPosition).normalized;
            float longestExtent = cols[0].bounds.extents.x;
            if (longestExtent < cols[0].bounds.extents.y) longestExtent = cols[0].bounds.extents.y;
            else if (longestExtent < cols[0].bounds.extents.z) longestExtent = cols[0].bounds.extents.z;
            if (longestExtent < nodeHalfextent * 2) longestExtent = nodeHalfextent * 2;
            currentBox += directionToMoveBack * longestExtent;
            return getClosestBoxNotBlocked(currentBox, currentPosition);
        } else {
            currentBox = getClosestBox(currentBox);
        }
        return currentBox;
    }


    // Start is called before the first frame update
    void Start() {
        /*  nodes = new Dictionary<Vector3, Dictionary<Vector3, float>>();
         worldPos = gameObject.GetComponent<BoxCollider>().center;
         worldSize = gameObject.transform.localScale;
         worldPos.y += worldSize.y / 2;
         numberOfNodes = (int)((worldSize.x / (nodeHalfextent * 2)) * (worldSize.y / (nodeHalfextent * 2)) * (worldSize.z / (nodeHalfextent * 2)));
         initGraph(); */

    }

    // this is *very* hardcoded and will most likely not work if the the "world" changes
    private void initGraph() {
        float startXPos = worldPos.x - worldSize.x / 2;
        float startZPos = worldPos.z + worldSize.z / 2;
        Vector3 startPos = new Vector3(startXPos, worldPos.y, startZPos);
        startPos.x += nodeHalfextent;
        startPos.z -= nodeHalfextent;
        startPos.y += nodeHalfextent;
        insert(startPos);
        Vector3 currentPos = startPos;
        Vector3 prevPos = startPos;
        while (nodes.Count < numberOfNodes) {
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
            insert(currentPos);
        }
        foreach (Vector3 node in nodes.Keys) {
            Vector3[] possibleNeighbors = getPossibleNeighbors(node);
            foreach (Vector3 pNeighbor in possibleNeighbors) {
                connect(node, pNeighbor, 1);
            }
        }
    }

    Vector3[] getPossibleNeighbors(Vector3 node) {
        Vector3 firstPossibleXNeighbor = new Vector3(node.x + nodeHalfextent * 2, node.y, node.z),
        secondPossibleXNeighbor = new Vector3(node.x - nodeHalfextent * 2, node.y, node.z),
        firstPossibleYNeighbor = new Vector3(node.x, node.y + nodeHalfextent * 2, node.z),
        secondPossibleYNeighbor = new Vector3(node.x, node.y - nodeHalfextent * 2, node.z),
        firstPossibleZNeighbor = new Vector3(node.x, node.y, node.z + nodeHalfextent * 2),
        secondPossibleZNeighbor = new Vector3(node.x, node.y, node.z + nodeHalfextent * 2);
        return new Vector3[] { firstPossibleXNeighbor, secondPossibleXNeighbor, firstPossibleYNeighbor, secondPossibleYNeighbor, firstPossibleZNeighbor, secondPossibleZNeighbor };
    }

    public Collider[] getBlockedNode(Vector3 position) {
        return Physics.OverlapBox(position, new Vector3(nodeHalfextent, nodeHalfextent, nodeHalfextent), Quaternion.identity, colliderMask);
    }


    // Beware: this tanks the FPS *HARD* but is useful to see the generated pathfinding grid
    /*  private void OnDrawGizmos() {
         int count = 0;
         foreach (Vector3 v in nodes.Keys) {
             bool blocked = getBlockedNode(v).Length == 0;
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

}
