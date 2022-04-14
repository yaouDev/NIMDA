using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AIState/AI_MoveState")]
public class AI_MoveState : AI_BaseState {

    public float speed;
    public float timeBetweenPathUpdates;
    int currentPathIndex;


    private PathfinderManager pathfinder = PathfinderManager.instance;
    public override void enter() {
    }
    public override void exit() {
    }
    public override void run() {
        List<Vector3> currentPath = Agent.getCurrentPath();
        Vector3 currentPosition = Agent.getPosition();
        currentPathIndex = Agent.getCurrentPathIndex();

        if (currentPath != null && currentPath.Count != 0 && Vector3.Distance(currentPosition, Agent.getCurrentTarget()) > Agent.getAttackRange()) {
            if (Vector3.Distance(currentPosition, currentPath[currentPathIndex]) > 0.5f || (currentPathIndex == currentPath.Count - 1 && Vector3.Distance(currentPosition, currentPath[currentPathIndex]) > 2f)) {
                int indexesToLerp = 4;
                if (currentPath.Count - 1 - currentPathIndex < 4) indexesToLerp = currentPath.Count - 1 - currentPathIndex;
                Vector3 lerpForceToAdd = (Vector3.Lerp(currentPath[currentPathIndex], currentPath[currentPathIndex + indexesToLerp], 0.5F) - currentPosition).normalized * speed;
                Vector3 forceTadd = lerpForceToAdd;
                if (currentPathIndex != currentPath.Count - 1 && Agent.getVelocity().magnitude < 1) forceTadd = (currentPath[currentPathIndex] - currentPosition).normalized * speed * 5;
                Agent.addForce(forceTadd);
                if (currentPathIndex != currentPath.Count - 1 && Vector3.Distance(currentPath[currentPathIndex + 1], currentPosition) < Vector3.Distance(currentPath[currentPathIndex], currentPosition)) currentPathIndex++;
            } else if (currentPathIndex < currentPath.Count - 2) {
                Agent.advanceCurrentPathIndex();
            }
        }
        evalTransition();
    }

    private void evalTransition() {
        if (Agent.getAttackRange() >= Vector3.Distance(Agent.getPosition(), Agent.getCurrentTarget()) && Agent.targetInSight()) {
            stateMachine.transitionTo<AI_AttackState>();
        }
    }

}
