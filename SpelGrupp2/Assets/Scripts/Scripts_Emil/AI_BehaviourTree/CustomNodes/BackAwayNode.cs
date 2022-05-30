using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AIBehavior/Behavior/Back Away")]
public class BackAwayNode : Node, IResetableNode {

    [SerializeField] private float turnSpeed = 70.0f, distanceToBackOff, acceleration = 15f, maxSpeed = 15f;
    [SerializeField] private float distanceFromTargetToStop;
    private float dist;
    private float angleOffset;
    private bool playerCond;
    private bool runningCond;
    private bool initCond;
    private Vector3 backOffPos;
    private Vector3 dirOfPlayer;
    public override NodeState Evaluate() {

        dirOfPlayer = (agent.ClosestPlayer - agent.Position).normalized;

        dist = Vector3.Distance(backOffPos, agent.ClosestPlayer);
        playerCond = Vector3.Distance(agent.ClosestPlayer, agent.Destination) > 1;
        runningCond = agent.CurrentPath != null && distanceFromTargetToStop < agent.DistanceFromTarget && playerCond && backOffPos != Vector3.zero;
        initCond = backOffPos == Vector3.zero || dist < distanceToBackOff;

        if (initCond) {
            angleOffset = Random.Range(130, 231);
            backOffPos = agent.Position + ((Quaternion.AngleAxis(angleOffset, Vector3.up) * dirOfPlayer) * distanceToBackOff);
            agent.Acceleration = acceleration;
            agent.MaxSpeed = maxSpeed;
            agent.Destination = backOffPos;
            agent.IsStopped = false;
            if (DynamicGraph.Instance.IsNodeBlocked(DynamicGraph.Instance.TranslateToGrid(backOffPos))) {
                NodeState = NodeState.FAILURE;
                backOffPos = Vector3.zero;
            } else
                NodeState = NodeState.RUNNING;
        } else if (runningCond) {
            NodeState = NodeState.RUNNING;
            agent.IsStopped = false;
        } else {
            agent.IsStopped = true;
            NodeState = NodeState.FAILURE;
            backOffPos = Vector3.zero;
        }
        return NodeState;
    }

    public void ResetNode() {
        backOffPos = Vector3.zero;
        dist = 0f;
        angleOffset = 0f;
        playerCond = false;
        runningCond = false;
        initCond = false;
        dirOfPlayer = Vector3.zero;
    }
}
