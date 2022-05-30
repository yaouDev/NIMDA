using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AIBehavior/Behavior/Strafe")]
public class StrafeNode : Node, IResetableNode {

    [SerializeField] private float acceleration = 13.5f, maxSpeed = 16.5f, minStrafeDistance = 2, maxStrafeDistance = 5;
    [SerializeField] private int maxShotsToFire = 2, minShotsToFire = 5;
    [SerializeField] private float distanceFromTargetToStop;
    private Vector3 randomPos;
    Vector3 strafeTarget;
    int shotsToFire;
    public override NodeState Evaluate() {

        bool playerCond = Vector3.Distance(agent.ClosestPlayer, agent.Destination) > 1;
        bool runningCond = agent.CurrentPath != null && distanceFromTargetToStop < agent.DistanceFromTarget && playerCond && strafeTarget != Vector3.zero;
        bool shotReqCond = AIData.Instance.GetShotsFired(agent) == shotsToFire;

        if (shotReqCond) {
            strafeTarget = Vector3.zero;
            int tries = 0;
            while ((strafeTarget == Vector3.zero || DynamicGraph.Instance.IsNodeBlocked(DynamicGraph.Instance.TranslateToGrid(strafeTarget))) && tries < 10) {
                float angleOffset;
                int strafeRight = Random.Range(0, 2);
                if (strafeRight == 0) angleOffset = Random.Range(50, 121);
                else angleOffset = Random.Range(-120, -51);

                float strafeDistance = Random.Range(minStrafeDistance, maxStrafeDistance + 1f);
                randomPos = Quaternion.AngleAxis(angleOffset, Vector3.up) * (agent.ClosestPlayer - agent.Position).normalized * strafeDistance;
                strafeTarget = randomPos + agent.Position;
                agent.Destination = strafeTarget;
                agent.IsStopped = false;
                NodeState = NodeState.RUNNING;
                agent.Acceleration = acceleration;
                agent.MaxSpeed = maxSpeed;
                tries++;
            }
            // no strafeLocation was found
            if (DynamicGraph.Instance.IsNodeBlocked(DynamicGraph.Instance.TranslateToGrid(strafeTarget))) NodeState = NodeState.FAILURE;

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

    public void ResetNode() {
        randomPos = Vector3.zero;
        strafeTarget = Vector3.zero;
        shotsToFire = Random.Range(minShotsToFire, maxShotsToFire + 1);
        AIData.Instance.SetShotRequirement(agent, shotsToFire);
    }
}
