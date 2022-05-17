using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AIBehavior/Behavior/PlayerRange")]
public class PlayerRangeNode : Node {
    [SerializeField] private float maxRange = 20f, minRange = 3f;

    public override NodeState Evaluate() {
        float dist = Vector3.Distance(agent.ClosestPlayer, agent.Position);
        NodeState = dist <= maxRange && dist > minRange && agent.TargetInSight ? NodeState.SUCCESS : NodeState.FAILURE;
        return NodeState;
    }
}
