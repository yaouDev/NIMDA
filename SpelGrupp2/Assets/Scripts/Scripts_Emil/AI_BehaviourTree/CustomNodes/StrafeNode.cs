using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AIBehavior/Behavior/Strafe")]
public class StrafeNode : Node {

    [SerializeField] private float strafeSpeed = 16.5f;
    [SerializeField] private int maxShotsToFire = 2, minShotsToFire = 5, minStrafeDistance = 2, maxStrafeDistance = 5;
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
                float angleOffset = Random.Range(-120, 121);
                int strafeDistance = Random.Range(minStrafeDistance, maxStrafeDistance + 1);
                randomPos = Quaternion.AngleAxis(angleOffset, Vector3.up) * (agent.ClosestPlayer - agent.Position).normalized * strafeDistance;
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
