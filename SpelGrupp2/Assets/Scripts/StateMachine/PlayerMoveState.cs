using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "PlayerState/MoveState")]
public class PlayerMoveState : PlayerBaseState {

    public override void Enter() {
    
    }

    public override void Run() {
        
        if (Player.inputMovement.magnitude > float.Epsilon)
            Player.Accelerate(Player.inputMovement);
        else
            Player.Decelerate();

        Vector3 gravityMovement = Player.DefaultGravity * Time.deltaTime * Vector3.down;
        
        Player.velocity += gravityMovement;
    }

    public override void Exit() {
    
    }
}