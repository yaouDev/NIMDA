using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "PlayerState/JumpState")]
public class PlayerJumpState : PlayerBaseState {
    
    private bool _falling;

    public override void Enter() {
        
        _falling = false;
        airTime = 0;
        Player.AddVelocity(Vector3.up * Player._jumpForce);
    }

    public override void Run() {
        if (Player.ReleasedJump() || Player.Velocity().y < float.Epsilon) 
            _falling = true;
        
        if (_falling)
            stateMachine.TransitionTo<PlayerAirState>();
        
        if (Player.IsGrounded() && Player.Velocity().y < float.Epsilon) 
            stateMachine.TransitionTo<PlayerMoveState>();
        
        AirControl();

        Vector3 gravityMovement = Player._defaultGravity * Time.deltaTime * Vector3.down;
        
        Player._velocity += gravityMovement;
        
        Player.ApplyAirFriction();
    }

    public override void Exit() {
        
    }
}