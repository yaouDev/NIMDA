using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AIBehavior/Behavior/PlayerRange")]
public class PlayerRangeNode : Node {
    [SerializeField] private float maxRange = 20f, minRange = 3f;
    private float dist;
    public override NodeState Evaluate() {
        dist = Vector3.Distance(agent.ClosestPlayer, agent.Position);
        NodeState = dist <= maxRange && dist > minRange && agent.TargetInSight ? NodeState.SUCCESS : NodeState.FAILURE;
        return NodeState;
    }
}
