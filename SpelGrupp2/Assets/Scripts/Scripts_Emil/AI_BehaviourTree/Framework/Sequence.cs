using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sequence : Node {

    protected List<Node> nodes = new List<Node>();
    public override NodeState Evaluate() {
        bool isAnyChildRunning = false;
        foreach (Node node in nodes) {
            switch (node.Evaluate()) {
                case NodeState.RUNNING:
                isAnyChildRunning = true;
                    break;
                case NodeState.SUCCESS:
                    break;
                case NodeState.FAILURE:
                    nodeState = NodeState.FAILURE;
                    return nodeState;
                default:
                    break;
            }
        }
        nodeState = isAnyChildRunning ? NodeState.RUNNING : NodeState.SUCCESS;
        return nodeState;
    }

    public Sequence(List<Node> nodes) {
        this.nodes = nodes;
    }
}
