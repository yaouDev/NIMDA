using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AIBehavior/Behavior/Chase")]
public class ChaseNode : Node {

    [SerializeField] private float turnSpeed = 70.0f;
    Vector3 closestTarget;
    Vector3 relativePos;

    Quaternion rotation;

    [SerializeField] private float distanceFromTargetToStop;
    public override NodeState Evaluate() {

        //Find Closest Player
        closestTarget = agent.ClosestPlayer + Vector3.up;
        relativePos = closestTarget - agent.transform.position;

        // Rotate the Enemy towards the player
        rotation = Quaternion.LookRotation(relativePos, Vector3.up);
        agent.transform.rotation = Quaternion.RotateTowards(agent.transform.rotation,
                                                            rotation, Time.deltaTime * turnSpeed);
        agent.transform.rotation = new Quaternion(0, agent.transform.rotation.y, 0, agent.transform.rotation.w);
        if (agent.Destination != agent.ClosestPlayer) {
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
