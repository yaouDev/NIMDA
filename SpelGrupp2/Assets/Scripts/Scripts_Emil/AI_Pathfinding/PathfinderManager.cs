using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Jobs;
using UnityEngine.Jobs;
using Unity.Collections;
using Unity.Mathematics;

public class PathfinderManager : MonoBehaviour {
    [SerializeField] float proximityToReusePath;
    private List<Vector3> latestCalculatedPath = new List<Vector3>();
    public static PathfinderManager Instance;
    private PriorityQueue<AI_Controller> pathQueue;
    private Dictionary<int, List<Vector3>> latestEnemyPath = new Dictionary<int, List<Vector3>>();
    JobHandle job;
    List<AI_Controller> agentsToUpdate = new List<AI_Controller>();
    NativeList<Vector3> startPositionsTmp;
    NativeList<Vector3> endPositionsTMp;

    void Awake() {
        Instance ??= this;
        pathQueue = new PriorityQueue<AI_Controller>();
        job = new JobHandle();
        startPositionsTmp = new NativeList<Vector3>(Allocator.Persistent);
        endPositionsTMp = new NativeList<Vector3>(Allocator.Persistent);
    }


    private float Heuristic(Vector3 currentPos, Vector3 endPos) {
        return Mathf.Abs(currentPos.x - endPos.x) + Mathf.Abs(currentPos.z - endPos.z);
    }

    private List<Vector3> GetPath(Dictionary<Unity.Mathematics.float3, Unity.Mathematics.float3> via, Vector3 node, Vector3 end, Vector3 start) {
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

            bool wrongDirectionCond = Vector3.Dot((latestCalculatedPath[0] - agent.Position).normalized, (endPos - agent.Position).normalized) > 0;

            lock (latestCalculatedPath) {
                if (Vector3.Distance(currentPosition, latestCalculatedPath[0]) <= proximityToReusePath && endPos == latestCalculatedPath[latestCalculatedPath.Count - 1] && wrongDirectionCond) {
                    List<Vector3> pathToStartOflatest = AStar(currentPosition, latestCalculatedPath[0], false);
                    pathToStartOflatest.AddRange(latestCalculatedPath);
                    agent.CurrentPath = pathToStartOflatest;
                    agent.CurrentPathIndex = 0;
                }
            }


        }
        if (!pathQueue.Contains(agent)) pathQueue.Insert(agent, Vector3.Distance(currentPosition, endPos));
    }


    void Update() {

        if (!pathQueue.IsEmpty() && job.IsCompleted) {
            job.Complete();

            for (int i = 0; i < agentsToUpdate.Count; i++) {
                try {
                    agentsToUpdate[i].CurrentPath = latestEnemyPath[i];
                    agentsToUpdate[i].CurrentPathIndex = 0;
                } catch (System.Exception) {
                    Debug.Log("Something broke the pathfinding");
                    continue;
                }

            }

            agentsToUpdate.Clear();
            startPositionsTmp.Clear();
            endPositionsTMp.Clear();

            while (!pathQueue.IsEmpty()) {
                AI_Controller agent = pathQueue.DeleteMin();
                if (agent == null) continue;
                agentsToUpdate.Add(agent);
                startPositionsTmp.Add(agent.Position);
                endPositionsTMp.Add(agent.CurrentTarget);
            }

            AStarJob aStarJob = new AStarJob {
                startPositions = startPositionsTmp,
                endPositions = endPositionsTMp,
            };

            int perThread;
            if (agentsToUpdate.Count < 10) perThread = agentsToUpdate.Count;
            else perThread = agentsToUpdate.Count / 10;

            job = aStarJob.Schedule(agentsToUpdate.Count, perThread, default(JobHandle));
        }
    }

    private void OnDestroy() {
        try {
            startPositionsTmp.Dispose();
            endPositionsTMp.Dispose();
        } catch (System.Exception) {
            // so it doesn't spam on exit playmode
        }
    }

    public void UpdateAgentLatestPath(int agentId, List<Vector3> path) {
        if (latestEnemyPath.ContainsKey(agentId)) latestEnemyPath[agentId] = path;
        else latestEnemyPath.Add(agentId, path);
    }

    private List<Vector3> AStar(Vector3 startPos, Vector3 endPos, bool updateLatestPath) {
        PriorityQueue<Vector3> priorityQueue = new PriorityQueue<Vector3>();
        Vector3 node = Vector3.zero;
        Dictionary<float3, float3> via = new Dictionary<float3, float3>();
        Dictionary<float3, float> cost = new Dictionary<float3, float>();
        Vector3 closestNode = DynamicGraph.Instance.GetClosestNode(startPos);
        DynamicGraph.Instance.Insert(closestNode);
        DynamicGraph.Instance.CreateNeighbors(closestNode, DynamicGraph.Instance.GetPossibleNeighbors(closestNode));

        priorityQueue.Insert(closestNode, 0);
        via[closestNode] = closestNode;
        cost[closestNode] = 0;

        while (!priorityQueue.IsEmpty()) {

            node = priorityQueue.DeleteMin();

            if (node == endPos) break;
            Vector3[] currEdges = DynamicGraph.Instance.GetPossibleNeighbors(node);
            DynamicGraph.Instance.CreateNeighbors(node, currEdges);
            foreach (Vector3 neighbor in currEdges) {
                float tmpCost = cost[node] + DynamicGraph.Instance.GetCost(node, neighbor);

                bool nodeIsFree = !DynamicGraph.Instance.IsNodeBlocked(neighbor); //.Length == 0;


                if (DynamicGraph.Instance.Contains(neighbor) && (!cost.ContainsKey(neighbor) || tmpCost < cost[neighbor]) &&
                nodeIsFree || neighbor == endPos) {
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

    // not possible to use [BurstCompile] beacuse of managed code. Would probably need to completely restructure the project to enable :(
    public struct AStarJob : IJobParallelFor {
        [ReadOnly] public NativeList<Vector3> startPositions;
        [ReadOnly] public NativeList<Vector3> endPositions;

        public void Execute(int index) {
            PriorityQueue<float3> priorityQueue = new PriorityQueue<float3>();
            Vector3 node = Vector3.zero;
            Dictionary<float3, float3> via = new Dictionary<float3, float3>();
            Dictionary<float3, float> cost = new Dictionary<float3, float>();
            Vector3 closestNode = DynamicGraph.Instance.GetClosestNode(startPositions[index]);
            DynamicGraph.Instance.Insert(closestNode);
            DynamicGraph.Instance.CreateNeighbors(closestNode, DynamicGraph.Instance.GetPossibleNeighbors(closestNode));

            priorityQueue.Insert(closestNode, 0);
            via[closestNode] = closestNode;
            cost[closestNode] = 0;

            while (!priorityQueue.IsEmpty()) {

                node = priorityQueue.DeleteMin();

                if (node == endPositions[index]) break;
                Vector3[] currEdges = DynamicGraph.Instance.GetPossibleNeighbors(node);
                DynamicGraph.Instance.CreateNeighbors(node, currEdges);
                foreach (Vector3 neighbor in currEdges) {
                    float tmpCost = cost[node] + DynamicGraph.Instance.GetCost(node, neighbor);

                    bool nodeIsFree = !DynamicGraph.Instance.IsNodeBlocked(neighbor);

                    if (DynamicGraph.Instance.Contains(neighbor) && (!cost.ContainsKey(neighbor) || tmpCost < cost[neighbor]) &&
                    nodeIsFree || neighbor == endPositions[index]) {
                        cost[neighbor] = tmpCost;
                        float heurVal = tmpCost + PathfinderManager.Instance.Heuristic(neighbor, endPositions[index]);
                        priorityQueue.Insert(neighbor, heurVal);
                        via[neighbor] = node;
                    }
                }
            }
            List<Vector3> lPath = PathfinderManager.Instance.GetPath(via, node, endPositions[index], closestNode);
            PathfinderManager.Instance.UpdateAgentLatestPath(index, lPath);
            PathfinderManager.Instance.latestCalculatedPath = lPath;
        }
    }
}


