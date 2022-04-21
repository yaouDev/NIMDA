using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class AI_StateMachine {
    private AI_State currentState;
    private AI_State queuedState;

    private Stack<AI_State> automaton;
    private Dictionary<Type, AI_State> stateDict = new Dictionary<Type, AI_State>();

    public AI_StateMachine(object actor, AI_State[] states) {
        foreach (AI_State state in states) {
            AI_State instance = UnityEngine.Object.Instantiate(state);
            instance.owner = actor;
            instance.stateMachine = this;
            stateDict.Add(instance.GetType(), instance);
            if (currentState == null) {
                currentState = instance;
                queuedState = currentState;
            }
        }
        currentState?.enter();
    }

    public void transitionTo<T>() where T : AI_State {
        queuedState = stateDict[typeof(T)];
    }

    // TODO, implement this 
    public void transitionBack() {
        if (automaton.Count != 0)
            queuedState = automaton.Pop();
    }

    public void run() {
        updateState();
        currentState.run();
    }

    private void updateState() {
        if (queuedState != currentState) {
            currentState?.exit();
            //automaton.Push(currentState);
            currentState = queuedState;
            currentState.enter();
        }
    }
}