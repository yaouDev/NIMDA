using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class AI_BaseState : AI_State {
    private AI_Controller agent;
    public AI_Controller Agent => agent = agent ?? (AI_Controller)owner;
}
