using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AIBehavior/Behavior/Strafe")]
public class StrafeNode : Node {

    [SerializeField] private float turnSpeed = 70.0f, strafeSpeed = 16.5f;
    [SerializeField] private int maxShotsToFire = 2, minShotsToFire = 5;
    [SerializeField] private float distanceFromTargetToStop;
    private Vector3 randomPos;
    private int xPos;
    private int zPos;
    Vector3 strafeTarget;
    int shotsToFire;
    public override NodeState Evaluate() {

        bool playerCond = Vector3.Distance(agent.ClosestPlayer, agent.Destination) > 1;
        bool runningCond = agent.CurrentPath != null && distanceFromTargetToStop < agent.DistanceFromTarget && playerCond && strafeTarget != Vector3.zero;
        bool shotReqCond = AIData.Instance.GetShotsFired(agent) == shotsToFire;

        if (shotReqCond) {
            strafeTarget = Vector3.zero;
            while (strafeTarget == Vector3.zero || DynamicGraph.Instance.IsNodeBlocked(DynamicGraph.Instance.GetClosestNode(strafeTarget))) {
                xPos = Random.Range(-5, 5);
                zPos = Random.Range(-5, 5);
                randomPos = new Vector3(xPos, 0, zPos);
                strafeTarget = randomPos + agent.Position;
                agent.Destination = strafeTarget;
                agent.IsStopped = false;
                NodeState = NodeState.RUNNING;
                agent.Speed = strafeSpeed;
            }

        } else if (runningCond) {
            NodeState = NodeState.RUNNING;
            agent.IsStopped = false;
        } else {
            agent.IsStopped = true;
            NodeState = NodeState.FAILURE;
            strafeTarget = Vector3.zero;
        }

        if (shotsToFire == 0 || (AIData.Instance.GetShotsFired(agent) >= shotsToFire && shotsToFire != 0)) {
            shotsToFire = Random.Range(minShotsToFire, maxShotsToFire + 1);
            AIData.Instance.SetShotRequirement(agent, shotsToFire);
        }

        return NodeState;
    }
}
