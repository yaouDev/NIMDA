using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent;
using UnityEngine;

[CreateAssetMenu(menuName = "AIBehavior/Behavior/IsCoverAvailable")]

public class IsCoverAvailableNode : Node {
    [SerializeField] private LayerMask layerMask;
    private bool distanceCondition;
    private Vector3 bestCoverSpot;
    private RaycastHit hit;
    private Vector3 direction;
    private Vector2Int currentModule;
    private ConcurrentDictionary<Vector3, byte> possibleCovers;
    private bool CheckIfSpotIsValid(Vector3 spot) {
        
        direction = agent.ClosestPlayer - spot;
        if (Physics.Raycast(spot, direction, out hit, Mathf.Infinity, layerMask)) {
            if (!hit.collider.CompareTag("Player") && Vector3.Distance(spot, agent.ClosestPlayer) > 4f && DynamicGraph.Instance.Contains(spot)) {
                return true;
            }
        }
        return false;
    }

    private Vector3 GetValidCoverFromModule(Vector2Int module) {
        possibleCovers = AIData.Instance.GetNearbyCoverSpots(module);
        bestCoverSpot = new Vector3(Mathf.Infinity, 0, 0);
        if (possibleCovers != null) {
            foreach (Vector3 cover in possibleCovers.Keys) {
                distanceCondition = Vector3.Distance(agent.Position, cover) < Vector3.Distance(agent.Position, bestCoverSpot);
                if (CheckIfSpotIsValid(cover) && (bestCoverSpot.x == Mathf.Infinity || distanceCondition)) {
                    bestCoverSpot = cover;
                    break; // breaking as to avoid looping through all of the covers but might be desirable to do so to find the closest one
                }
            }
        }

        return bestCoverSpot;
    }

    public override NodeState Evaluate() {

        currentModule = DynamicGraph.Instance.GetModulePosFromWorldPos(agent.Position);
        bestCoverSpot = GetValidCoverFromModule(currentModule);

        if (bestCoverSpot.x == Mathf.Infinity) {
            foreach (Vector2Int module in DynamicGraph.Instance.GetLoadedModules()) {
                if (module != currentModule) {
                    bestCoverSpot = GetValidCoverFromModule(module);
                }
                if (bestCoverSpot.x != Mathf.Infinity) break;
            }
        }

        if (bestCoverSpot.x != Mathf.Infinity) agent.Destination = bestCoverSpot;

        NodeState = bestCoverSpot.x == Mathf.Infinity ? NodeState.FAILURE : NodeState.SUCCESS;

        return NodeState;
    }
}
