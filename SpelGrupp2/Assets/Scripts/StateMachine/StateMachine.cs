using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class StateMachine {
    
    private State _currentState;
    private State _queuedState;
    private PlayerController _owner;
    private Dictionary<Type, State> _states = new Dictionary<Type, State>();
    
    public StateMachine(PlayerController owner, List<State> states) {
        _owner = owner;

        foreach (State state in states) {
            State instance = UnityEngine.Object.Instantiate(state);
            instance.owner = _owner;
            instance.stateMachine = this;
            _states.Add(instance.GetType(), instance);

            _currentState ??= instance;
        }

        _queuedState = _currentState;
        _currentState?.Enter();
    }

    public void Run() {
        if (_currentState != _queuedState) {
            _currentState.Exit();
            _currentState = _queuedState;
            _currentState.Enter();
        }
        
        _currentState.Run();
        
        _owner.UpdateVelocity();
        _owner.ResolveOverlap();
        
        _owner.transform.position +=  Time.deltaTime * _owner._velocity;
        _owner._inputMovement = Vector3.zero;
        _owner._pressedJump = false;
        _owner._releasedJump = false;
    }

    public void TransitionTo<T>() where T : State {
        _queuedState = _states[typeof(T)];
    }
}