using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AIBehavior/Behavior/HealthNode")]
public class HealthNode : Node {

    //public float fleeThereshold;
    [SerializeField] private float healthThreashold;
    public override NodeState Evaluate() {

        NodeState = agent.Health.GetCurrentHealth() <= healthThreashold ? NodeState.SUCCESS : NodeState.FAILURE;
        Debug.Log("HealthNode: " + NodeState);
        return NodeState;
    }

}
