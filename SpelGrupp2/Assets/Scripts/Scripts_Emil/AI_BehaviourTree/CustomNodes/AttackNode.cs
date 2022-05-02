using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AIBehavior/Behavior/Attack")]
public class AttackNode : Node {
    public override NodeState Evaluate() {
/*         if (Vector3.Distance(agent.Position, agent.CurrentTarget) <= agent.AttackRange) {
            if (agent.Attack.IsShooting()) {
                agent.Attack.SetShooting(false);
                agent.Attack.StartCoroutine(agent.Attack.AttackDelay());
            }
            agent.IsStopped = true;
            NodeState = NodeState.SUCCESS;
        } else NodeState = NodeState.FAILURE; */
        return NodeState.FAILURE;
    }
}
