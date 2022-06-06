using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "PlayerState/AirState")]
public class PlayerAirState : PlayerBaseState {
	
	private int doubleJump = 1;
	
	public override void Enter() {
		doubleJump = 1;
	}

	public override void Run() {
		if (Player.IsGrounded())
			stateMachine.TransitionTo<PlayerMoveState>();
		

		AirControl();

		Vector3 gravityMovement = Player.DefaultGravity * Player.JumpFallVelocityMultiplier * Time.deltaTime * Vector3.down;
		
		Player.velocity += gravityMovement;
		
		Player.ApplyAirFriction();
	}

	public override void Exit() {
		
	}
}
