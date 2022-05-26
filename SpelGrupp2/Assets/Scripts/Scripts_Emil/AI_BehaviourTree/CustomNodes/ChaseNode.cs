using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AIBehavior/Behavior/Chase")]
public class ChaseNode : Node {

    [SerializeField] private float acceleration = 13.5f, maxSpeed = 10f;
    [SerializeField] private float distanceFromTargetToStop;
    public override NodeState Evaluate() {

        if (agent.Destination != agent.ClosestPlayer) {
            agent.Acceleration = acceleration;
            agent.MaxSpeed = maxSpeed;
            agent.Destination = agent.ClosestPlayer;
            agent.IsStopped = false;
            NodeState = NodeState.RUNNING;
        } else if (agent.CurrentPath != null && distanceFromTargetToStop < agent.DistanceFromTarget) {
            NodeState = NodeState.RUNNING;
            agent.IsStopped = false;
        } else {
            agent.IsStopped = true;
            NodeState = NodeState.SUCCESS;
        }
        return NodeState;
    }
}
