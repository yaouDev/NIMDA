using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AIBehavior/Behavior/Back Away")]
public class BackAwayNode : Node {

    [SerializeField] private float turnSpeed = 70.0f, distanceToBackOff, acceleration = 15f, maxSpeed = 15f;
    Vector3 backOffPos;
    [SerializeField] private float distanceFromTargetToStop;
    public override NodeState Evaluate() {

        Vector3 dirOfPlayer = (agent.ClosestPlayer - agent.Position).normalized;

        float dist = Vector3.Distance(backOffPos, agent.ClosestPlayer);
        bool playerCond = Vector3.Distance(agent.ClosestPlayer, agent.Destination) > 1;
        bool runningCond = agent.CurrentPath != null && distanceFromTargetToStop < agent.DistanceFromTarget && playerCond && backOffPos != Vector3.zero;
        bool initCond = backOffPos == Vector3.zero || dist < distanceToBackOff;

        if (initCond) {
            float angleOffset = Random.Range(130, 231);
            backOffPos = agent.Position + ((Quaternion.AngleAxis(angleOffset, Vector3.up) * dirOfPlayer) * distanceToBackOff);
            agent.Acceleration = acceleration;
            agent.MaxSpeed = maxSpeed;
            agent.Destination = backOffPos;
            agent.IsStopped = false;
            if (DynamicGraph.Instance.IsNodeBlocked(DynamicGraph.Instance.GetClosestNode(backOffPos))) {
                NodeState = NodeState.FAILURE;
                backOffPos = Vector3.zero;
            } else
                NodeState = NodeState.RUNNING;
        } else if (runningCond) {
            NodeState = NodeState.RUNNING;
            agent.IsStopped = false;
        } else {
            agent.IsStopped = true;
            NodeState = NodeState.FAILURE;
            backOffPos = Vector3.zero;
        }

        return NodeState;
    }
}
