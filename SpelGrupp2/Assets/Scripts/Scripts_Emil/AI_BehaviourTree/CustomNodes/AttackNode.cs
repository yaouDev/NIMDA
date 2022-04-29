using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AIBehavior/Behavior/Attack")]
public class AttackNode : Node {
    public override NodeState Evaluate() {
        if (Vector3.Distance(agent.GetPosition(), agent.GetCurrentTarget()) <= agent.GetAttackRange()) {
            if (agent.GetEnemyAttack().IsShooting()) {
                agent.GetEnemyAttack().SetShooting(false);
                agent.GetEnemyAttack().StartCoroutine(agent.GetEnemyAttack().AttackDelay());
            }
            agent.IsStopped = true;
            nodeState = NodeState.SUCCESS;
        } else nodeState = NodeState.FAILURE;
        return nodeState;
    }
}
