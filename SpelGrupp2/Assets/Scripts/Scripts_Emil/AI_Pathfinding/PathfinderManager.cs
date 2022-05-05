using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathfinderManager : MonoBehaviour {
    [SerializeField] float proximityToReusePath;
    private List<Vector3> latestCalculatedPath = new List<Vector3>();
    public static PathfinderManager Instance;
    private PriorityQueue<AI_Controller> pathQueue;

    void Awake() {
        Instance ??= this;
        pathQueue = new PriorityQueue<AI_Controller>();
    }


    private float Heuristic(Vector3 currentPos, Vector3 endPos) {
        return Mathf.Abs(currentPos.x - endPos.x) + Mathf.Abs(currentPos.z - endPos.z);
    }

    private List<Vector3> GetPath(Dictionary<Vector3, Vector3> via, Vector3 node, Vector3 end, Vector3 start) {
        List<Vector3> path = new List<Vector3>();
        if (node == end) {
            for (; node != start; node = via[node]) {
                path.Add(node);
            }
            path.Add(start);
        }
        path.Reverse();
        return path;
    }


    public void RequestPath(AI_Controller agent, Vector3 currentPosition, Vector3 endPos) {
        if (latestCalculatedPath != null && latestCalculatedPath.Count != 0) {
            bool wrongDirectionCond = Vector3.Dot(latestCalculatedPath[0].normalized, agent.Velocity.normalized) > 0;
            if (Vector3.Distance(currentPosition, latestCalculatedPath[0]) <= proximityToReusePath && endPos == latestCalculatedPath[latestCalculatedPath.Count - 1] && wrongDirectionCond) {
                List<Vector3> pathToStartOflatest = AStar(currentPosition, latestCalculatedPath[0], false);
                pathToStartOflatest.AddRange(latestCalculatedPath);
                agent.CurrentPath = pathToStartOflatest;
                agent.CurrentPathIndex = 0;
            }
        }
        if (!pathQueue.Contains(agent)) pathQueue.Insert(agent, Vector3.Distance(currentPosition, endPos));
    }

    void Update() {
        try {
            AI_Controller agentToUpdate = pathQueue.DeleteMin();
            // if agent has been deleted through module un-load
            while (agentToUpdate == null && !pathQueue.IsEmpty()) agentToUpdate = pathQueue.DeleteMin();
            agentToUpdate.CurrentPath = AStar(agentToUpdate.Position, agentToUpdate.CurrentTarget, true);
            agentToUpdate.CurrentPathIndex = 0;
        } catch (System.Exception) {
            //Debug.Log("No current requested paths");
        }


    }

    private List<Vector3> AStar(Vector3 startPos, Vector3 endPos, bool updateLatestPath) {
        PriorityQueue<Vector3> priorityQueue = new PriorityQueue<Vector3>();
        Vector3 node = Vector3.zero;
        int edgesTested = 0;
        HashSet<Vector3> explored = new HashSet<Vector3>();
        Dictionary<Vector3, Vector3> via = new Dictionary<Vector3, Vector3>();
        Dictionary<Vector3, float> cost = new Dictionary<Vector3, float>();
        Vector3 closestNode = DynamicGraph.Instance.GetClosestNode(startPos);
        DynamicGraph.Instance.Insert(closestNode);
        DynamicGraph.Instance.CreateNeighbors(closestNode, DynamicGraph.Instance.GetPossibleNeighborsKV(closestNode));

        priorityQueue.Insert(closestNode, 0);
        via[closestNode] = closestNode;
        cost[closestNode] = 0;

        while (!priorityQueue.IsEmpty()) {

            node = priorityQueue.DeleteMin();
            if (node.x == 48.5) {
                Debug.Log("Fak this shit");
            }
            explored.Add(node);
            if (node == endPos) break;
            Dictionary<Vector3, float> currEdges = DynamicGraph.Instance.GetPossibleNeighborsKV(node);
            DynamicGraph.Instance.CreateNeighbors(node, currEdges);
            foreach (Vector3 neighbor in currEdges.Keys) {
                edgesTested++;
                float tmpCost = cost[node] + DynamicGraph.Instance.GetCost(node, neighbor);
                if (DynamicGraph.Instance.Contains(neighbor) && (!cost.ContainsKey(neighbor) || tmpCost < cost[neighbor]) &&
                (DynamicGraph.Instance.GetBlockedNode(neighbor).Length == 0) || neighbor == endPos) {
                    cost[neighbor] = tmpCost;
                    float heurVal = tmpCost + Heuristic(neighbor, endPos);
                    priorityQueue.Insert(neighbor, heurVal);
                    via[neighbor] = node;
                }
            }
        }
        List<Vector3> path = GetPath(via, node, endPos, closestNode);
        if (updateLatestPath) latestCalculatedPath = path;
        return path;
    }
}


