using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerBaseState : State
{
    private PlayerController _player;
    public float airTime = 0.0f;

    protected void AirControl() {
        Player.airControl = Mathf.Lerp(1.0f, 0.5f, airTime);
        airTime += Time.deltaTime * .5f;
        Player._velocity += Player._inputMovement * Player.airControl * 10.0f;
    }
    
    public PlayerController Player => owner;
}
