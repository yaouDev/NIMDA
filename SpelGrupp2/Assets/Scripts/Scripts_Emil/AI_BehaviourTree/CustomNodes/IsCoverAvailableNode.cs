using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent;
using UnityEngine;

[CreateAssetMenu(menuName = "AIBehavior/Behavior/IsCoverAvailable")]

public class IsCoverAvailableNode : Node {
    private Transform getBestSpot;
    //private Transform[] availableCoverSpots;
    private List<Cover> availableCovers;
    [SerializeField] private float minAngle = 90;
    [SerializeField] private LayerMask layerMask;
    private Vector3 prevCastPos = Vector3.zero;
    //private Transform target;


    /*    public IsCoverAvailableNode(Cover[] availableCovers, Transform target)
        {
            this.availableCovers = availableCovers;
            this.target = target;
        }
    */
    /*     public override NodeState Evaluate() {
            availableCovers = AIData.Instance.GetActiveCovers();
            Transform bestSpot = FindBestCoverSpot();
            AIData.Instance.SetBestCoverSpot(bestSpot);
            return bestSpot != null ? NodeState.SUCCESS : NodeState.FAILURE;
        }

        private Transform FindBestCoverSpot() {
            getBestSpot = AIData.Instance.GetBestCoverSpot();

            if (getBestSpot != null) {
                if (CheckIfSpotIsValid(getBestSpot)) {
                    return getBestSpot;
                }
            }

            float minAngle = 90;
            Transform bestSpot = null;
            availableCovers = AIData.Instance.GetActiveCovers();

            for (int i = 0; i < availableCovers.Count; i++) {
                Transform bestSpotInCover = FindBestSpotInCover(availableCovers[i], ref minAngle);
                if (bestSpotInCover != null) {
                    bestSpot = bestSpotInCover;
                }
            }
            return bestSpot;
        }

        private Transform FindBestSpotInCover(Cover cover, ref float minAngle) {
            Transform[] availableSpots = cover.GetCoverSpots();
            Transform bestSpot = availableSpots[0];

            for (int i = 0; i < availableSpots.Length; i++) {
                Vector3 direction = agent.ClosestPlayer - availableSpots[i].position;
                if (CheckIfSpotIsValid(availableSpots[i])) {
                    float angle = Vector3.Angle(availableSpots[i].forward, direction);
                    if (angle < minAngle && 
                    (bestSpot != null && Vector3.Distance(agent.Position, availableSpots[i].position) < Vector3.Distance(agent.Position, bestSpot.position)
                     || !CheckIfSpotIsValid(bestSpot))) {
                        minAngle = angle;
                        bestSpot = availableSpots[i];
                    }
                }
            }
            return bestSpot;
        } */



    private bool CheckIfSpotIsValid(Vector3 spot) {
        RaycastHit hit;
        Vector3 direction = agent.ClosestPlayer - spot;
        if (Physics.Raycast(spot, direction, out hit, Mathf.Infinity, layerMask)) {
            if (!hit.collider.CompareTag("Player") && Vector3.Distance(spot, agent.ClosestPlayer) > 4f) {
                return true;
            }

        }
        return false;
    }

    private Vector3 GetValidCoverFromModule(Vector2Int module) {
        ConcurrentDictionary<Vector3, byte> possibleCovers = AIData.Instance.GetNearbyCoverSpots(module);
        Vector3 bestCoverSpot = new Vector3(Mathf.Infinity, 0, 0);
        if (possibleCovers != null) {
            foreach (Vector3 cover in possibleCovers.Keys) {
                bool distCond = Vector3.Distance(agent.Position, cover) < Vector3.Distance(agent.Position, bestCoverSpot);
                if (CheckIfSpotIsValid(cover) && (bestCoverSpot.x == Mathf.Infinity || distCond)) {
                    bestCoverSpot = cover;
                    //break; // breaking as to avoid looping through all of the covers but might be desirable to do so to find the closest one
                }
            }
        }

        return bestCoverSpot;
    }

    public override NodeState Evaluate() {

        // if (prevCastPos == Vector3.zero || Vector3.Distance(prevCastPos, agent.Destination) > 4f || agent.Destination == agent.ClosestPlayer) {

        Vector2Int currentModule = DynamicGraph.Instance.GetModulePosFromWorldPos(agent.Position);
        Vector3 bestCoverSpot = GetValidCoverFromModule(currentModule);

        if (bestCoverSpot.x == Mathf.Infinity) {
            foreach (Vector2Int module in DynamicGraph.Instance.GetLoadedModules()) {
                if (module != currentModule) {
                    bestCoverSpot = GetValidCoverFromModule(module);
                }
                if (bestCoverSpot.x != Mathf.Infinity) break;
            }
        }

        //AIData.Instance.SetBestCoverSpot(bestCoverSpot);
        if (bestCoverSpot.x != Mathf.Infinity) agent.Destination = bestCoverSpot;

        NodeState = bestCoverSpot.x == Mathf.Infinity ? NodeState.FAILURE : NodeState.SUCCESS;
        prevCastPos = agent.Position;
        /*   } else {
              NodeState = NodeState.SUCCESS;
          } */

        return NodeState;
    }
}
