using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthNode : Node {

    private EnemyHealth health;
    private float thereshold;

    public override NodeState Evaluate() {
        nodeState = health.GetCurrentHealth() <= thereshold ? NodeState.SUCCESS : NodeState.FAILURE;
        return nodeState;
    }

    public HealthNode(EnemyHealth health, float thereshold) {
        this.health = health;
        this.thereshold = thereshold;
    }

}
