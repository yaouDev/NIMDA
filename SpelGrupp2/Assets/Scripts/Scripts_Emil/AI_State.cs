using UnityEngine;

public abstract class AI_State : ScriptableObject {

    public AI_StateMachine stateMachine;
    public object owner;
    public virtual void enter() { }
    public virtual void exit() { }
    public virtual void run() { }
}