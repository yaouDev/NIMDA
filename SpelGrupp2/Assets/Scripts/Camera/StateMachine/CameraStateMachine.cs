using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CameraStateMachine
{
	private CameraState currentState;
	private CameraState queuedState;
	private CameraController _owner;
	private Dictionary<Type, CameraState> _states = new Dictionary<Type, CameraState>();
    
	public CameraStateMachine(CameraController owner, List<CameraState> states) {
		_owner = owner;

		foreach (CameraState state in states) {
			CameraState instance = UnityEngine.Object.Instantiate(state);
			instance.PlayerThis = owner.GetPlayerThis();
			instance.PlayerOther = owner.GetPlayerOther();
			instance.DepthMaskHolder = owner.GetDepthMaskHolder();
			instance.DepthMaskPlane = owner.GetDepthMaskPlane();
			instance.CameraTransform = owner.GetCameraTransform();
			instance.PlayerController = owner.GetPlayerController();
			instance.thisTransform = owner.transform;
			instance.owner = _owner;
			instance.stateMachine = this;
			_states.Add(instance.GetType(), instance);

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
	}

	public void TransitionTo<T>() where T : CameraState {
		queuedState = _states[typeof(T)];
	}
}
