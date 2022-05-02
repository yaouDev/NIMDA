using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AIBehavior/Utility/Inverter")]

public class Inverter : Decorator {
    public override NodeState Evaluate() {
        switch (node.Evaluate()) {
            case NodeState.RUNNING:
                NodeState = NodeState.RUNNING;
                break;
            case NodeState.SUCCESS:
                NodeState = NodeState.FAILURE;
                break;
            case NodeState.FAILURE:
                NodeState = NodeState.SUCCESS;
                break;
            default:
                break;
        }
        return NodeState;
    }

    public Inverter(Node node) {
        this.node = node;
    }

}
