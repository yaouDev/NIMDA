
using UnityEngine;

public abstract class CameraState : ScriptableObject
{
	[HideInInspector]
	public CameraController owner;
	public CameraStateMachine stateMachine;
	public Transform CameraTransform { get; set; }
	public Transform PlayerThis { get; set; }
	public Transform PlayerOther { get; set; }
	public Transform DepthMaskHolder { get; set; }
	public Transform DepthMaskPlane { get; set; }
	public Transform thisTransform { get; set; }
	
	public PlayerController PlayerController { get; set; }

	public abstract void Enter();
	public abstract void Run();
	public abstract void Exit();
	
	protected Vector2 mouseMovement;
	
	protected const float lookOffset = 90;

	public CameraController Camera => owner;
}
