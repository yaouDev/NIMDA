using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AIBehavior/Behavior/HealthNode")]
public class HealthNode : Node {

    [SerializeField] private float healthTheresholdPercent;
    private float roundedHealth;
    private float coinFlip;
    public override NodeState Evaluate() {

        roundedHealth = Mathf.Round(agent.Health.CurrentHealthPercentage * 10) * 0.1f;
        coinFlip = Random.Range(0, 100);

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
