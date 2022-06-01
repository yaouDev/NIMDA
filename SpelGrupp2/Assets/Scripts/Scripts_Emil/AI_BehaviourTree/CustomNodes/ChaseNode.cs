using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AIBehavior/Behavior/Chase")]
public class ChaseNode : Node {

    [SerializeField] private float acceleration = 13.5f, maxSpeed = 10f;
    [SerializeField] private float distanceFromTargetToStop;
    private Animator animator;
    public override NodeState Evaluate() {
        animator = agent.GetComponent<Animator>();
        if (agent.Destination != agent.ClosestPlayer || (agent.Destination == agent.ClosestPlayer && agent.CurrentPath == null)) {
            agent.Acceleration = acceleration;
            agent.MaxSpeed = maxSpeed;
            agent.Destination = agent.ClosestPlayer;
            animator.SetBool("IsChasing", true);
            agent.IsStopped = false;

            NodeState = NodeState.RUNNING;
        } else if (agent.CurrentPath != null && distanceFromTargetToStop < agent.DistanceFromTarget) {
            NodeState = NodeState.RUNNING;
            animator.SetBool("IsChasing", true);
            agent.IsStopped = false;
        } else {
            animator.SetBool("IsChasing", false);
            agent.IsStopped = true;
            NodeState = NodeState.SUCCESS;
        }
        return NodeState;
    }
}
