using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AIState/AI_AttackState")]
public class AI_AttackState : AI_BaseState {

    public override void enter() {
        Agent.GetComponent<EnemyAttack>().enabled = true;
    }
    public override void exit() {
        Agent.GetComponent<EnemyAttack>().enabled = false;
    }
    public override void run() {
        evalTransition();
    }

    private void evalTransition() {
        if (Agent.getAttackRange() < Vector3.Distance(Agent.getPosition(), Agent.getCurrentTarget())) {
            stateMachine.transitionTo<AI_MoveState>();
        }

    }

}
