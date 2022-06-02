using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class BehaviorTree : MonoBehaviour {
    private AI_Controller agent;
    [SerializeField] private Node rootNode;


    // Convoluted way of doing it. I'm sure there's a better way to instantiate all the nodes
    private void InitNode(Node node) {
        if ((node.GetType() == typeof(Selector) || node.GetType() == typeof(Sequence))) {
            List<Node> innerNodes = new List<Node>();
            if (node.GetType() == typeof(Selector)) {
                innerNodes = ((Selector)node).GetInnerNodes();
            } else if (node.GetType() == typeof(Sequence)) {
                innerNodes = ((Sequence)node).GetInnerNodes();
            }
            List<Node> instanceNodes = new List<Node>();
            foreach (Node innerNode in innerNodes) {
                Node tempNode = Instantiate(innerNode);
                InitNode(tempNode);
                instanceNodes.Add(tempNode);
            }
            if (node.GetType() == typeof(Selector)) {
                ((Selector)node).SetInnerNodes(instanceNodes);
            } else if (node.GetType() == typeof(Sequence)) {
                ((Sequence)node).SetInnerNodes(instanceNodes);
            }
        }
        node.SetAgent(agent);
    }
    private void Start() {
        rootNode = Instantiate(rootNode);
        InitNode(rootNode);
    }

    private void Awake() {
        agent = GetComponent<AI_Controller>();
    }

    /*     public void Update() {

        } */

    public void UpdateTree() {
        rootNode.Evaluate();
    }

    public void ResetTree() {
        Reset(rootNode);
    }

    private void Reset(Node node) {
        if ((node.GetType() == typeof(Selector) || node.GetType() == typeof(Sequence))) {
            List<Node> innerNodes = new List<Node>();
            if (node.GetType() == typeof(Selector)) {
                innerNodes = ((Selector)node).GetInnerNodes();
            } else if (node.GetType() == typeof(Sequence)) {
                innerNodes = ((Sequence)node).GetInnerNodes();
            }
            foreach (Node innerNode in innerNodes) {
                Reset(innerNode);
            }
        } else {

            if (node.GetType().GetInterface(nameof(IResetableNode)) != null) {
                IResetableNode resetableNode = (IResetableNode)node;
                resetableNode.ResetNode();
            }
        }
    }

}
