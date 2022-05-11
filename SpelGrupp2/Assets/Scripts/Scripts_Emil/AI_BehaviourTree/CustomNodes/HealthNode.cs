using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AIBehavior/Behavior/HealthNode")]
public class HealthNode : Node {

    //public float fleeThereshold;

    public override NodeState Evaluate() {
        NodeState = agent.Health.GetCurrentHealth() <= agent.Health.GetFleeHealthTreshold() ? NodeState.SUCCESS : NodeState.FAILURE;
        return NodeState;
    }

}
