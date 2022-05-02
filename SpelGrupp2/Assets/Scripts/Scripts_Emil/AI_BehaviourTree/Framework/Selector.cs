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
                    NodeState = NodeState.RUNNING;
                    return NodeState;
                case NodeState.SUCCESS:
                    NodeState = NodeState.SUCCESS;
                    return NodeState;
                case NodeState.FAILURE:
                    break;
                default:
                    break;
            }
        }
        NodeState = NodeState.FAILURE;
        return NodeState;
    }

    public Selector(List<Node> nodes) {
        this.nodes = nodes;
    }

    public List<Node> GetInnerNodes() {
        return nodes;
    }
    public void SetInnerNodes(List<Node> innerNodes) { nodes = innerNodes; }
}
