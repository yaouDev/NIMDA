using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AIBehavior/Behavior/HealthNode")]
public class HealthNode : Node {

    //public float fleeThereshold;
    [SerializeField] private float healthTheresholdPercent;
    public override NodeState Evaluate() {
        //NodeState = agent.Health.CurrentHealthPercentage <= healthTheresholdPercent ? NodeState.SUCCESS : NodeState.FAILURE;

        float roundedHealth = Mathf.Round(agent.Health.CurrentHealthPercentage * 10) * 0.1f;
        int coinFlip = Random.Range(0, 100);

        if (roundedHealth <= 0.5f) {
            if (roundedHealth > 0.4f) {
                if (coinFlip < 50) NodeState = NodeState.SUCCESS;
            } else if (roundedHealth <= 0.4f && roundedHealth > 0.3f) {
                if (coinFlip < 70) NodeState = NodeState.SUCCESS;
            } else if (roundedHealth <= 0.3f && roundedHealth > 0.2f) {
                if (coinFlip < 90) NodeState = NodeState.SUCCESS;
            } else {
                NodeState = NodeState.SUCCESS;
            }
        } else NodeState = NodeState.FAILURE;
        return NodeState;
    }

}
