using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "PlayerState/AirState")]
public class PlayerAirState : PlayerBaseState {
	
	private int _doubleJump = 1;
	
	public override void Enter() {
		_doubleJump = 1;
	}

	public override void Run() {
		if (Player.IsGrounded())
			stateMachine.TransitionTo<PlayerMoveState>();

		if (Player.PressedJump() && _doubleJump > 0) {
			_doubleJump--;
			airTime = 0;
			Player._velocity.y = 0.0f;
			Player._velocity += Vector3.up * Player._jumpForce;
		}

		AirControl();

		Vector3 gravityMovement = Player._defaultGravity * Player.jumpFallVelocityMultiplier * Time.deltaTime * Vector3.down;
		
		Player._velocity += gravityMovement;
		
		Player.ApplyAirFriction();
	}

	public override void Exit() {
		
	}
}
