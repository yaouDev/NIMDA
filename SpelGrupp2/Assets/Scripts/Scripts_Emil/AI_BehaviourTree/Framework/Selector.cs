using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AIBehavior/Framework/Selector")]

public class Selector : Node {

    [SerializeField] protected List<Node> nodes = new List<Node>();
    public override NodeState Evaluate() {
        foreach (Node node in nodes) {
            switch (node.Evaluate()) {
                case NodeState.RUNNING:
                    nodeState = NodeState.RUNNING;
                    return nodeState;
                case NodeState.SUCCESS:
                    nodeState = NodeState.SUCCESS;
                    return nodeState;
                case NodeState.FAILURE:
                    break;
                default:
                    break;
            }
        }
        nodeState = NodeState.FAILURE;
        return nodeState;
    }

    public Selector(List<Node> nodes) {
        this.nodes = nodes;
    }

    public List<Node> GetInnerNodes() {
        return nodes;
    }
    public void SetInnerNodes(List<Node> innerNodes) { nodes = innerNodes; }
}
