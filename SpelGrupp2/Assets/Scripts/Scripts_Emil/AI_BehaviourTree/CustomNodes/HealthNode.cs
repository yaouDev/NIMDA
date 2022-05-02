using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AIBehavior/Behavior/LowHealthRetreat")]
public class LowHealthRetreatNode : Node {

    public float fleeThereshold;

    public override NodeState Evaluate() {
        nodeState = agent.GetEnemyHealth().GetCurrentHealth() <= fleeThereshold ? NodeState.SUCCESS : NodeState.FAILURE;
        return nodeState;
    }

}
