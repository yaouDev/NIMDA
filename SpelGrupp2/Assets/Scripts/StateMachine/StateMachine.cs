using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class StateMachine {
    
    private State currentState;
    private State queuedState;
    private PlayerController owner;
    private Dictionary<Type, State> states = new Dictionary<Type, State>();
    
    public StateMachine(PlayerController owner, List<State> states) {
        this.owner = owner;

        foreach (State state in states) {
            State instance = UnityEngine.Object.Instantiate(state);
            instance.owner = this.owner;
            instance.stateMachine = this;
            this.states.Add(instance.GetType(), instance);

            currentState ??= instance;
        }

        queuedState = currentState;
        currentState?.Enter();
    }

    public void Run() {
        if (currentState != queuedState) {
            currentState.Exit();
            currentState = queuedState;
            currentState.Enter();
        }
        
        currentState.Run();
        
        owner.UpdateVelocity();
        owner.ResolveOverlap();
        
        owner.transform.position +=  Time.deltaTime * owner.velocity * owner.movementSpeedReduced;
        owner.inputMovement = Vector3.zero;
    }

    public void TransitionTo<T>() where T : State {
        queuedState = states[typeof(T)];
    }
}