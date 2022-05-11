using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent;
using UnityEngine;
using Unity.Jobs;
using UnityEngine.Jobs;
using Unity.Collections;
using Unity.Burst;

public class DynamicGraph : MonoBehaviour {
    private int numberOfNodes;
    [Header("World attributes")]
    [SerializeField] private float nodeHalfextent;
    [SerializeField] private int moduleSize = 50;
    [SerializeField] private LayerMask colliderMask;
    // private Dictionary<Vector3, Dictionary<Vector3, float>> masterGraph;
    private ConcurrentDictionary<Vector3, ConcurrentDictionary<Vector3, float>> masterGraph;
    private ConcurrentDictionary<Vector3, byte> blockedNodes;
    public static DynamicGraph Instance;
    private HashSet<Vector2Int> loadedModules;
    private Queue<Vector3> nodesToRemove;
    [SerializeField] private bool drawGrid = false;

    void Start() {
        masterGraph = new ConcurrentDictionary<Vector3, ConcurrentDictionary<Vector3, float>>();
        blockedNodes = new ConcurrentDictionary<Vector3, byte>();
        nodesToRemove = new Queue<Vector3>();
        Instance ??= this;
        loadedModules = new HashSet<Vector2Int>();

        CallbackSystem.EventSystem.Current.RegisterListener<CallbackSystem.ModuleSpawnEvent>(OnModuleLoad);
        CallbackSystem.EventSystem.Current.RegisterListener<CallbackSystem.ModuleDeSpawnEvent>(OnModuleUnload);
    }

    void Update() {
        //RemoveUnloadedNodes();
    }

    public void Connect(Vector3 firstNode, Vector3 secondNode, float cost) {
        if (masterGraph.ContainsKey(firstNode) && masterGraph.ContainsKey(secondNode) && !IsConnected(firstNode, secondNode)) {
            masterGraph[firstNode].TryAdd(secondNode, cost);
            masterGraph[secondNode].TryAdd(firstNode, cost);
        }
    }

    public void Disconnect(Vector3 firstNode, Vector3 secondNode) {
        if (masterGraph.ContainsKey(firstNode) && masterGraph.ContainsKey(secondNode) && IsConnected(firstNode, secondNode)) {
            float outVal;
            masterGraph[firstNode].TryRemove(secondNode, out outVal);
            masterGraph[secondNode].TryRemove(firstNode, out outVal);
        }
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
            masterGraph.TryAdd(node, new ConcurrentDictionary<Vector3, float>());
        }
    }

    public bool IsConnected(Vector3 firstNode, Vector3 secondNode) {
        if (masterGraph.ContainsKey(firstNode) && masterGraph.ContainsKey(secondNode)) return masterGraph[firstNode].ContainsKey(secondNode);
        return false;
    }

    public ConcurrentDictionary<Vector3, float> GetNeighbors(Vector3 node) {
        if (masterGraph.ContainsKey(node)) return masterGraph[node];
        return null;
    }

    public void AddBlockedNode(Vector3 node) {
        blockedNodes.TryAdd(node, 0);
    }

    public bool IsNodeBlocked(Vector3 node) {
        return blockedNodes.ContainsKey(node);
    }

    public Vector3 GetClosestNode(Vector3 pos) {
        Vector2Int modulePos = GetModulePosFromWorldPos(pos);
        Vector3 localWorldPos = new Vector3(0, 1.6f, 0);
        if (modulePos.x == 0) localWorldPos.x = modulePos.x;
        else localWorldPos.x = (modulePos.x * moduleSize);
        if (modulePos.y == 0) localWorldPos.z = modulePos.y;
        else localWorldPos.z = (modulePos.y * moduleSize);
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

    public Vector3 GetClosestNodeNotBlocked(Vector3 target) {
        Vector3[] pNeighbors = GetPossibleNeighbors(target);
        Vector3 closestNode = Vector3.zero;
        foreach (Vector3 pNeighbor in pNeighbors) {
            if (GetBlockedNode(pNeighbor).Length == 0 && (closestNode == Vector3.zero || Vector3.Distance(closestNode, target) > Vector3.Distance(pNeighbor, closestNode)))
                closestNode = pNeighbor;
            else {
                foreach (Vector3 nPNeighbor in GetPossibleNeighbors(pNeighbor)) {
                    if (GetBlockedNode(pNeighbor).Length == 0 && (closestNode == Vector3.zero || Vector3.Distance(closestNode, target) > Vector3.Distance(nPNeighbor, closestNode)))
                        closestNode = nPNeighbor;
                }
            }
        }
        return closestNode;
    }

    private Vector3[] GetPossibleNeighbors(Vector3 node) {
        Vector3 firstPossibleXNeighbor = new Vector3(node.x + nodeHalfextent * 2, node.y, node.z),
        secondPossibleXNeighbor = new Vector3(node.x - nodeHalfextent * 2, node.y, node.z),
        firstPossibleZNeighbor = new Vector3(node.x, node.y, node.z + nodeHalfextent * 2),
        secondPossibleZNeighbor = new Vector3(node.x, node.y, node.z - nodeHalfextent * 2);
        return new Vector3[] { firstPossibleXNeighbor, secondPossibleXNeighbor, firstPossibleZNeighbor, secondPossibleZNeighbor };
    }

    public Dictionary<Unity.Mathematics.float3, float> GetPossibleNeighborsKV(Vector3 node) {
        Vector3 firstPossibleXNeighbor = new Vector3(node.x + nodeHalfextent * 2, node.y, node.z),
        secondPossibleXNeighbor = new Vector3(node.x - nodeHalfextent * 2, node.y, node.z),
        firstPossibleZNeighbor = new Vector3(node.x, node.y, node.z + nodeHalfextent * 2),
        secondPossibleZNeighbor = new Vector3(node.x, node.y, node.z - nodeHalfextent * 2);

        Dictionary<Unity.Mathematics.float3, float> tempNeighbors = new Dictionary<Unity.Mathematics.float3, float>();
        tempNeighbors.Add(firstPossibleXNeighbor, 1);
        tempNeighbors.Add(secondPossibleXNeighbor, 1);
        tempNeighbors.Add(firstPossibleZNeighbor, 1);
        tempNeighbors.Add(secondPossibleZNeighbor, 1);
        return tempNeighbors;
    }

    private Collider[] GetBlockedNode(Vector3 position) {
        return Physics.OverlapBox(position, new Vector3(nodeHalfextent, nodeHalfextent, nodeHalfextent), Quaternion.identity, colliderMask);
    }

    // Beware: this tanks the FPS *HARD* but is useful to see the generated pathfinding grid
    private void OnDrawGizmos() {
        if (drawGrid) {
            int count = 0;
            foreach (Vector3 v in masterGraph.Keys) {
                bool blocked = !IsNodeBlocked(v); //.Length == 0;
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
    }

    public Vector2Int GetModulePosFromWorldPos(Vector3 worldPos) {
        int xOffset = (int)worldPos.x % moduleSize, yOffset = (int)worldPos.z % moduleSize;
        int xAdd = 0, yAdd = 0;
        if (xOffset < -((float)moduleSize / 2)) xAdd = -1;
        if (xOffset > ((float)moduleSize / 2)) xAdd = 1;
        if (yOffset < -((float)moduleSize / 2)) yAdd = -1;
        if (yOffset > ((float)moduleSize / 2)) yAdd = +1;
        return new Vector2Int((int)worldPos.x / moduleSize + xAdd, (int)worldPos.z / moduleSize + yAdd);

    }

    public bool IsModuleLoaded(Vector2Int modulePos) {
        return loadedModules.Contains(modulePos);
    }

    public HashSet<Vector2Int> GetLoadedModules() {
        return loadedModules;
    }

    public void CreateNeighbors(Vector3 node, Dictionary<Unity.Mathematics.float3, float> possibleNeighbors) {
        foreach (Vector3 pNeighbor in possibleNeighbors.Keys) {
            if (IsModuleLoaded(GetModulePosFromWorldPos(node)) && !masterGraph.ContainsKey(pNeighbor)) {
                
                
                
                
                if (pNeighbor.x > 75) {
                    Debug.Log("Wat");
                }



                Insert(pNeighbor);
                Connect(node, pNeighbor, possibleNeighbors[pNeighbor]);
            }
            foreach (Vector3 neighborNeighbor in GetPossibleNeighbors(pNeighbor)) {
                Connect(neighborNeighbor, pNeighbor, 1);
            }
        }
    }

    private void OnModuleLoad(CallbackSystem.ModuleSpawnEvent spawnEvent) {
        loadedModules.Add(spawnEvent.Position);
        AddBlockedNodes(spawnEvent.Position);

    }

    void MarkNodesForDeletion(Vector2Int module) {
        Vector3 worldPos = new Vector3(module.x + (moduleSize / 2), 1.6f, module.y + (moduleSize / 2));
        float leftMostX = worldPos.x - (moduleSize / 2) + nodeHalfextent;
        float topMostZ = worldPos.z + (moduleSize / 2) - nodeHalfextent;
        float rightMostX = worldPos.x + (moduleSize / 2) - nodeHalfextent;

        Vector3 node = new Vector3(leftMostX, worldPos.y, topMostZ);

        for (int i = 0; i <= moduleSize * moduleSize; i++) {
            if (masterGraph.ContainsKey(node))
                nodesToRemove.Enqueue(node);
            if (node.x == rightMostX) {
                node.x = leftMostX;
                node.z -= (nodeHalfextent * 2);
            } else {
                node.x += (nodeHalfextent * 2);
            }
        }
    }

    private void OnModuleUnload(CallbackSystem.ModuleDeSpawnEvent deSpawnEvent) {
        if (loadedModules.Contains(deSpawnEvent.Position)) loadedModules.Remove(deSpawnEvent.Position);
        MarkNodesForDeletion(deSpawnEvent.Position);
        RemoveUnloadedNodes();

    }

    private void RemoveUnloadedNodes() {
        for (int i = 0; i < 50 || nodesToRemove.Count == 0; i++) {
            if (nodesToRemove.Count > 0) {
                Vector3 nodeToRemove = nodesToRemove.Dequeue();
                if (!loadedModules.Contains(GetModulePosFromWorldPos(nodeToRemove))) {
                    List<Vector3> connectedNodes = new List<Vector3>(GetNeighbors(nodeToRemove).Keys);
                    foreach (Vector3 neighbor in connectedNodes) {
                        Disconnect(nodeToRemove, neighbor);
                    }
                    ConcurrentDictionary<Vector3, float> outVal;
                    masterGraph.TryRemove(nodeToRemove, out outVal);
                }
            } else break;
        }
    }

    private void AddBlockedNodes(Vector2Int module) {
        Vector3 localWorldPos = new Vector3(0, 1.6f, 0);
        if (module.x == 0) localWorldPos.x = module.x;
        else localWorldPos.x = (module.x * moduleSize);
        if (module.y == 0) localWorldPos.z = module.y;
        else localWorldPos.z = (module.y * moduleSize);

        float leftMostX = localWorldPos.x - ((float)moduleSize / 2) + nodeHalfextent;
        float topMostZ = localWorldPos.z + ((float)moduleSize / 2) - nodeHalfextent;
        float rightMostX = localWorldPos.x + ((float)moduleSize / 2) - nodeHalfextent;

        Vector3 node = new Vector3(leftMostX, localWorldPos.y, topMostZ);

        for (int i = 0; i <= moduleSize * moduleSize; i++) {
            if (GetBlockedNode(node).Length != 0) {
                blockedNodes.TryAdd(node, 0);
                AIData.Instance.AddCoverSpot(GetClosestNodeNotBlocked(node));
            }
            if (node.x == rightMostX) {
                node.x = leftMostX;
                node.z -= (nodeHalfextent * 2);
            } else {
                node.x += (nodeHalfextent * 2);
            }
        }

    }
}
