
using UnityEngine;

public abstract class State : ScriptableObject
{
    public PlayerController owner;
    public StateMachine stateMachine;
    public abstract void Enter();
    public abstract void Run();
    public abstract void Exit();
}
