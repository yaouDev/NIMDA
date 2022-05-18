using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AIBehavior/Behavior/EnemyShieldChase")]
public class EnemyShieldChaseNode : Node {

    [SerializeField] private float chaseSpeed = 12.8f;

    [SerializeField] private float distanceFromTargetToStop;
    private EnemyShield enemyShield;
    public override NodeState Evaluate() {
        enemyShield = agent.GetComponentInChildren<EnemyShield>();
        if (enemyShield != null) {
            if (enemyShield.CurrentHealth <= 0) {
                agent.Speed = 16f;
            }
        }

        agent.transform.rotation = new Quaternion(0, agent.transform.rotation.y, 0, agent.transform.rotation.w);
        if (agent.Destination != agent.ClosestPlayer) {
            agent.Speed = chaseSpeed;
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
