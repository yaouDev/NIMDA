using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "PlayerState/JumpState")]
public class PlayerJumpState : PlayerBaseState {
    
    private bool falling;

    public override void Enter() {
        
        falling = false;
        airTime = 0;
        Player.AddVelocity(Vector3.up * Player.JumpForce);
    }

    public override void Run() {

        if (falling)
            stateMachine.TransitionTo<PlayerAirState>();
        
        if (Player.IsGrounded() && Player.Velocity().y < float.Epsilon) 
            stateMachine.TransitionTo<PlayerMoveState>();
        
        AirControl();

        Vector3 gravityMovement = Player.DefaultGravity * Time.deltaTime * Vector3.down;
        
        Player.velocity += gravityMovement;
        
        Player.ApplyAirFriction();
    }

    public override void Exit() {
        
    }
}