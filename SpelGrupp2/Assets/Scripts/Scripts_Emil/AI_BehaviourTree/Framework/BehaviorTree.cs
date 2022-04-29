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
            } else if (node.GetType() == typeof(Selector)) {
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
            } else if (node.GetType() == typeof(Selector)) {
                ((Sequence)node).SetInnerNodes(instanceNodes);
            }
        }
        node.SetAgent(agent);
    }

    private void Awake() {
        agent = GetComponent<AI_Controller>();
    }

    private void Start() {
        rootNode = Instantiate(rootNode);
        InitNode(rootNode);
    }

    public void Update() {
        rootNode.Evaluate();
    }

}
