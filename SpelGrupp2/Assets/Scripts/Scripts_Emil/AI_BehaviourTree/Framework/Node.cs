using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class Node {
    protected NodeState nodeState;
    public NodeState NodeState { get { return nodeState; } }
    public abstract NodeState Evaluate();
}

public enum NodeState {
    RUNNING, SUCCESS, FAILURE,

}
