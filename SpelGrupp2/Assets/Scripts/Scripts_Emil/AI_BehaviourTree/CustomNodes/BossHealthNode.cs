using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AIBehavior/Behavior/BossHealthNode")]
public class BossHealthNode : Node {

    //public float fleeThereshold;
    [SerializeField] private float healthTheresholdPercent;
    public override NodeState Evaluate() {
        float percent = healthTheresholdPercent / 100;
        NodeState = agent.Health.CurrentHealthPercentage <= percent ? NodeState.SUCCESS : NodeState.FAILURE;
        return NodeState;
    }

}
