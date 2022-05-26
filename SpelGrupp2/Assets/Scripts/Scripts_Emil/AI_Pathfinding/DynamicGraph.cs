using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent;
using UnityEngine;

public class DynamicGraph : MonoBehaviour {
    [Header("World attributes")]
    [SerializeField] private float nodeHalfextent, groundLevel = 1.6f;
    [SerializeField] private int moduleSize = 50;
    [SerializeField] private LayerMask colliderMask;
    [SerializeField] private bool drawGrid = false;
    [SerializeField] private bool usePlayTestModules = true;
    private ConcurrentDictionary<Vector3, ConcurrentDictionary<Vector3, float>> masterGraph;
    private HashSet<Vector3> blockedNodes;
    public static DynamicGraph Instance;
    private HashSet<Vector2Int> loadedModules;
    private Queue<Vector3> nodesToRemove;


    void Start() {
        masterGraph = new ConcurrentDictionary<Vector3, ConcurrentDictionary<Vector3, float>>();
        blockedNodes = new HashSet<Vector3>();
        nodesToRemove = new Queue<Vector3>();
        Instance ??= this;
        loadedModules = new HashSet<Vector2Int>();

        CallbackSystem.EventSystem.Current.RegisterListener<CallbackSystem.ModuleSpawnEvent>(OnModuleLoad);
        CallbackSystem.EventSystem.Current.RegisterListener<CallbackSystem.ModuleDeSpawnEvent>(OnModuleUnload);

        if (usePlayTestModules) {
            Vector2Int module1 = new Vector2Int(0, 0);
            Vector2Int module2 = new Vector2Int(-1, 3);
            Vector2Int module3 = new Vector2Int(0, 3);
            Vector2Int module4 = new Vector2Int(0, 1);
            Vector2Int module5 = new Vector2Int(0, 2);
            Vector2Int module6 = new Vector2Int(-1, 2);
            Vector2Int module7 = new Vector2Int(0, 4);
            Vector2Int module8 = new Vector2Int(1, 2);
            Vector2Int module9 = new Vector2Int(-1, 2);
            Vector2Int module10 = new Vector2Int(1, 4);
            Vector2Int module11 = new Vector2Int(-1, 4);
            Vector2Int module12 = new Vector2Int(1, 3);
            Vector2Int module13 = new Vector2Int(1, 7);
            Vector2Int module14 = new Vector2Int(1, 8);
            Vector2Int module15 = new Vector2Int(-1, 9);
            Vector2Int module16 = new Vector2Int(-1, 6);
            Vector2Int module17 = new Vector2Int(1, 6);
            Vector2Int module18 = new Vector2Int(0, 7);
            Vector2Int module19 = new Vector2Int(1, 7);
            Vector2Int module20 = new Vector2Int(0, 6);
            Vector2Int module21 = new Vector2Int(0, 9);
            Vector2Int module22 = new Vector2Int(-1, 1);
            Vector2Int module23 = new Vector2Int(1, 9);
            Vector2Int module24 = new Vector2Int(-1, 8);
            Vector2Int module25 = new Vector2Int(0, 8);
            Vector2Int module26 = new Vector2Int(-1, 7);
            Vector2Int bossmod = new Vector2Int(0, 11);

            loadedModules.Add(module1);
            loadedModules.Add(module2);
            loadedModules.Add(module3);
            loadedModules.Add(module4);
            loadedModules.Add(module5);
            loadedModules.Add(module6);
            loadedModules.Add(module7);
            loadedModules.Add(module8);
            loadedModules.Add(module9);
            loadedModules.Add(module10);
            loadedModules.Add(module11);
            loadedModules.Add(module12);
            loadedModules.Add(module13);
            loadedModules.Add(module14);
            loadedModules.Add(module15);
            loadedModules.Add(module16);
            loadedModules.Add(module17);
            loadedModules.Add(module18);
            loadedModules.Add(module19);
            loadedModules.Add(module20);
            loadedModules.Add(module21);
            loadedModules.Add(module22);
            loadedModules.Add(module23);
            loadedModules.Add(module24);
            loadedModules.Add(module25);
            loadedModules.Add(module26);
            loadedModules.Add(bossmod);

            AddBlockedNodes(module1);
            AddBlockedNodes(module2);
            AddBlockedNodes(module3);
            AddBlockedNodes(module4);
            AddBlockedNodes(module5);
            AddBlockedNodes(module6);
            AddBlockedNodes(module7);
            AddBlockedNodes(module8);
            AddBlockedNodes(module9);
            AddBlockedNodes(module10);
            AddBlockedNodes(module11);
            AddBlockedNodes(module12);
            AddBlockedNodes(module13);
            AddBlockedNodes(module14);
            AddBlockedNodes(module15);
            AddBlockedNodes(module16);
            AddBlockedNodes(module17);
            AddBlockedNodes(module18);
            AddBlockedNodes(module19);
            AddBlockedNodes(module20);
            AddBlockedNodes(module21);
            AddBlockedNodes(module22);
            AddBlockedNodes(module23);
            AddBlockedNodes(module24);
            AddBlockedNodes(module25);
            AddBlockedNodes(module26);
            AddBlockedNodes(bossmod);
        }
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

    public bool IsNodeBlocked(Vector3 node) {
        return blockedNodes.Contains(node);
    }

    public bool AllNeighborsBlocked(Vector3 node) {
        foreach (Vector3 neighbor in GetPossibleNeighbors(node)) {
            if (!IsNodeBlocked(neighbor)) return false;
        }
        return true;
    }

    /// <summary>
    /// Gets the highest & lowest world x & z values for a given module coordinate
    /// </summary>
    /// <param name="module"> the module to get extreme positions from </param>
    /// <returns> an array of 4 float values: 
    /// index 0: leftmost x, index 1: rightmost x, index 2: topmost z, index 3: bottommost z 
    /// </returns>
    private float[] GetModuleEdgePositions(Vector2Int module) {
        Vector3 localWorldPos = new Vector3(0, groundLevel, 0);
        if (module.x == 0) localWorldPos.x = module.x;
        else localWorldPos.x = (module.x * moduleSize);
        if (module.y == 0) localWorldPos.z = module.y;
        else localWorldPos.z = (module.y * moduleSize);

        float leftMostX = localWorldPos.x - (moduleSize / 2) + nodeHalfextent;
        float topMostZ = localWorldPos.z + (moduleSize / 2) - nodeHalfextent;
        float rightMostX = localWorldPos.x + (moduleSize / 2) - nodeHalfextent;
        float bottomMostZ = localWorldPos.z - (moduleSize / 2) + nodeHalfextent;
        float[] extremePos = { leftMostX, rightMostX, topMostZ, bottomMostZ };
        return extremePos;
    }

    public Vector3 GetClosestNode(Vector3 pos) {

        float[] edgePos = GetModuleEdgePositions(GetModulePosFromWorldPos(pos));
        float x = 0, y = groundLevel, z = x;

        int nodesFromLeftEdge = (int)Mathf.FloorToInt(Mathf.Abs(pos.x - edgePos[0]) / (nodeHalfextent * 2));
        int nodesFromTopEdge = (int)Mathf.FloorToInt(Mathf.Abs(pos.z - edgePos[2]) / (nodeHalfextent * 2));

        x = edgePos[0] + (nodesFromLeftEdge * (nodeHalfextent * 2));
        z = edgePos[2] - (nodesFromTopEdge * (nodeHalfextent * 2));

        // literal edge cases
        if (pos.x < edgePos[0]) x = edgePos[0];
        else if (pos.x > edgePos[1]) x = edgePos[1];
        if (pos.z > edgePos[2]) z = edgePos[2];
        else if (pos.z < edgePos[3]) z = edgePos[3];

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

    public Vector3[] GetPossibleNeighbors(Vector3 node) {
        Vector3 firstPossibleXNeighbor = new Vector3(node.x + nodeHalfextent * 2, node.y, node.z),
        secondPossibleXNeighbor = new Vector3(node.x - nodeHalfextent * 2, node.y, node.z),
        firstPossibleZNeighbor = new Vector3(node.x, node.y, node.z + nodeHalfextent * 2),
        secondPossibleZNeighbor = new Vector3(node.x, node.y, node.z - nodeHalfextent * 2),
        firstTopDiagonalNeighbor = new Vector3(firstPossibleXNeighbor.x, node.y, firstPossibleZNeighbor.z),
        secondTopDiagonalNeighbor = new Vector3(secondPossibleXNeighbor.x, node.y, firstPossibleZNeighbor.z),
        firstBottomDiagnoalNeighbor = new Vector3(firstPossibleXNeighbor.x, node.y, secondPossibleZNeighbor.z),
        secondBottomDiagonalNeighbor = new Vector3(secondPossibleXNeighbor.x, node.y, secondPossibleZNeighbor.z);
        return new Vector3[] { firstPossibleXNeighbor, secondPossibleXNeighbor, firstPossibleZNeighbor, secondPossibleZNeighbor,
        firstTopDiagonalNeighbor, secondTopDiagonalNeighbor, firstBottomDiagnoalNeighbor, secondBottomDiagonalNeighbor };
    }

    private Collider[] GetBlockedNode(Vector3 position) {
        return Physics.OverlapBox(position, new Vector3(nodeHalfextent, nodeHalfextent, nodeHalfextent), Quaternion.identity, colliderMask);
    }

    // Beware: this tanks the FPS *HARD* but is useful to see the generated pathfinding grid
    private void OnDrawGizmos() {
        if (drawGrid) {
            Vector3 box = new Vector3(nodeHalfextent * 2, nodeHalfextent * 2, nodeHalfextent * 2);
            foreach (Vector3 node in masterGraph.Keys) {
                if (!IsNodeBlocked(node)) {
                    Gizmos.color = Color.green;
                    Gizmos.DrawWireCube(node, box);
                } else {
                    Gizmos.color = Color.red;
                    Gizmos.DrawWireCube(node, box);
                }
            }
        }
    }

    public Vector2Int GetModulePosFromWorldPos(Vector3 worldPos) {
        int xOffset = (int)worldPos.x % moduleSize, yOffset = (int)worldPos.z % moduleSize;
        int xAdd = 0, yAdd = 0;
        if (xOffset <= -((float)moduleSize / 2) + nodeHalfextent) xAdd = -1;
        if (xOffset >= ((float)moduleSize / 2) - nodeHalfextent) xAdd = 1;
        if (yOffset <= -((float)moduleSize / 2) + nodeHalfextent) yAdd = -1;
        if (yOffset >= ((float)moduleSize / 2) - nodeHalfextent) yAdd = +1;
        return new Vector2Int((int)worldPos.x / moduleSize + xAdd, (int)worldPos.z / moduleSize + yAdd);
    }

    public bool IsModuleLoaded(Vector2Int modulePos) {
        return loadedModules.Contains(modulePos);
    }

    public HashSet<Vector2Int> GetLoadedModules() {
        return loadedModules;
    }

    public void CreateNeighbors(Vector3 node, Vector3[] possibleNeighbors) {
        for (int i = 0; i < possibleNeighbors.Length; i++) {
            if (IsModuleLoaded(GetModulePosFromWorldPos(node)) && !masterGraph.ContainsKey(possibleNeighbors[i])) {
                Insert(possibleNeighbors[i]);
                Connect(node, possibleNeighbors[i], 1);
            }
            foreach (Vector3 neighborNeighbor in GetPossibleNeighbors(possibleNeighbors[i])) {
                Connect(neighborNeighbor, possibleNeighbors[i], 1);
            }
        }
    }

    private void OnModuleLoad(CallbackSystem.ModuleSpawnEvent spawnEvent) {
        loadedModules.Add(spawnEvent.Position);
        AddBlockedNodes(spawnEvent.Position);

    }

    void MarkNodesForDeletion(Vector2Int module) {

        float[] edgePos = GetModuleEdgePositions(module);

        Vector3 node = new Vector3(edgePos[0], groundLevel, edgePos[2]);

        for (int i = 0; i <= moduleSize * moduleSize; i++) {
            if (masterGraph.ContainsKey(node))
                nodesToRemove.Enqueue(node);
            if (node.x == edgePos[1]) {
                node.x = edgePos[0];
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
        int counter = 0;
        while (counter < 50 && nodesToRemove.Count != 0) {
            Vector3 nodeToRemove = nodesToRemove.Dequeue();
            if (!loadedModules.Contains(GetModulePosFromWorldPos(nodeToRemove))) continue;
            List<Vector3> connectedNodes = new List<Vector3>(GetNeighbors(nodeToRemove).Keys);
            foreach (Vector3 neighbor in connectedNodes) {
                Disconnect(nodeToRemove, neighbor);
            }
            ConcurrentDictionary<Vector3, float> outVal;
            masterGraph.TryRemove(nodeToRemove, out outVal);
            counter++;
        }
    }

    private void AddBlockedNodes(Vector2Int module) {

        float[] edgePos = GetModuleEdgePositions(module);

        Vector3 node = new Vector3(edgePos[0], groundLevel, edgePos[2]);

        for (int i = 0; i <= moduleSize * moduleSize; i++) {
            if (GetBlockedNode(node).Length != 0) {
                blockedNodes.Add(node);
                AIData.Instance.AddCoverSpot(GetClosestNodeNotBlocked(node));
            }
            if (node.x == edgePos[1]) {
                node.x = edgePos[0];
                node.z -= (nodeHalfextent * 2);
            } else {
                node.x += (nodeHalfextent * 2);
            }
        }
    }
}
