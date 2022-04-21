using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaseNode : Node {

    private AI_Controller agent;
    float distanceFromTargetToStop;
    public override NodeState Evaluate() {
        if (agent.getCurrentPath() == null || agent.getCurrentPathIndex() == agent.getCurrentPath().Count - 1 && distanceFromTargetToStop > agent.getDistanceFromTarget()) {
            agent.StartCoroutine(agent.updatePath());
            agent.IsStopped = false;
            nodeState = NodeState.RUNNING;
        } else if (agent.getCurrentPath() != null && agent.getCurrentPathIndex() != agent.getCurrentPath().Count - 1) {
            nodeState = NodeState.RUNNING;
            agent.IsStopped = false;
        } else {
            agent.IsStopped = true;
            nodeState = NodeState.SUCCESS;
        }
        return nodeState;
    }

    public ChaseNode(AI_Controller agent, float distanceToStop) {
        this.agent = agent;
        distanceFromTargetToStop = distanceToStop;
    }
}
