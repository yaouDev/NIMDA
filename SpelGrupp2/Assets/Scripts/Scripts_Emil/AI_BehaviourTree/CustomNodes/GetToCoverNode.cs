using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "AIBehavior/Behavior/GetToCover")]
public class GetToCoverNode : Node {

    [SerializeField] private float acceleration = 15f, maxSpeed = 18f;
    public override NodeState Evaluate() {
        if (Vector3.Distance(agent.Destination, agent.Position) > 0.2f) {
            agent.IsStopped = false;
            NodeState = NodeState.RUNNING;
            agent.Acceleration = acceleration;
            agent.MaxSpeed = maxSpeed;
        } else {
            agent.IsStopped = true;
            NodeState = NodeState.SUCCESS;
        }
        return NodeState;
    }
}
