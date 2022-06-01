using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "PlayerState/MoveState")]
public class PlayerMoveState : PlayerBaseState {

    public override void Enter() {
    
    }

    public override void Run() {
        
        if (Player._inputMovement.magnitude > float.Epsilon)
            Player.Accelerate(Player._inputMovement);
        else
            Player.Decelerate();
        
        // if (Player._jumped) {
        //     stateMachine.TransitionTo<PlayerJumpState>();
        //     Player._jumped = false;
        // }
        // if (!Player.Grounded())
        //    stateMachine.TransitionTo<PlayerAirState>();
        
        Vector3 gravityMovement = Player._defaultGravity * Time.deltaTime * Vector3.down;
        
        Player._velocity += gravityMovement;
    }

    public override void Exit() {
    
    }
}