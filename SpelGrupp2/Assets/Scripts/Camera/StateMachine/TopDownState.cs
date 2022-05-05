using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(menuName = "Create CameraState/CameraTopDownState")]
public class TopDownState : CameraState
{
	[SerializeField] 
	private LayerMask collisionMask;

	[SerializeField]
	private Vector2 topDownViewRotation;
	
	[SerializeField] 
	private Vector3 topDownOffset;
	
	[SerializeField]
	private float headHeight = 1.6f;
	
	[SerializeField] [Range(1.0f, 20.0f)]
	private float splitMagnitude = 4.0f;

	[SerializeField] [Range(1.0f, 20.0f)] 
	private float thirdPersonSplitDistance = 15.0f;
	
	private Vector3 centroid;
	private Vector3 abovePlayer;
	private Vector3 stencilPlanePosition;
	private Vector3 depthMaskPlanePos;
	private readonly Quaternion _ninetyDegrees = Quaternion.Euler(0.0f, 90.0f, 0.0f);

	private void Awake() {
		abovePlayer = Vector3.up * headHeight;
	}

	public override void Enter() {
		depthMaskPlanePos = DepthMaskPlane.localPosition;
		depthMaskPlanePos.x = -.5f;
		DepthMaskPlane.localPosition = depthMaskPlanePos;
	}

	private Vector3 lerpOffset;
	private float smoothDollyTime;
	private float smoothDampMinVal = .1f;
	private float smoothDampMaxVal = 1.0f;
	private Vector3 smoothDampCurrentVelocity;
	
	public override void Run() {
		Input();
		
		// both cameras have the same rotation (fixed?)
		CameraTransform.rotation = Quaternion.Euler(topDownViewRotation.x, topDownViewRotation.y, 0.0f);

		// the split is rotating freely between the players
		RotateScreenSplit();
		
		// both cameras follow the centroid point between the players, split when necessary
		Vector3 centroidOffsetPosition = (PlayerOther.position - PlayerThis.position) * .5f;
		centroid = PlayerThis.position + Vector3.ClampMagnitude( centroidOffsetPosition, splitMagnitude );
		
		
		
		
		
		
		// TODO hack

		Vector3 offsetDirection = -CameraTransform.forward * 8;//((abovePlayer + thisTransform.position) - Camera.transform.position);
		
		Physics.SphereCast((Vector3.Distance(PlayerOther.position, PlayerThis.position) < splitMagnitude * 2 ? centroidOffsetPosition : Vector3.zero) + thisTransform.position + abovePlayer, 
			.5f,
			offsetDirection.normalized,
			out  RaycastHit  hit, 
			offsetDirection.magnitude, 
			collisionMask);
		
		Debug.DrawRay(centroidOffsetPosition + thisTransform.position + abovePlayer, offsetDirection, Color.magenta);
		
		Vector3 offset;
		if (hit.collider)
		{
			offset = topDownOffset.normalized * hit.distance;
			Debug.Log(hit.collider.name);
		}
		else
		{
			offset = topDownOffset;
		}
		
		smoothDollyTime = hit.collider ? smoothDampMinVal : smoothDampMaxVal;
		lerpOffset = Vector3.SmoothDamp(lerpOffset, offset, ref smoothDampCurrentVelocity, smoothDollyTime);
		
		CameraTransform.position = centroid + abovePlayer + CameraTransform.rotation * lerpOffset;

		// TODO hack
		
		// CameraTransform.position = centroid + abovePlayer + CameraTransform.rotation * topDownOffset; // TODO HACK!!

		
		
		
		
		
		
		LerpSplitScreenLineWidth(centroidOffsetPosition.magnitude);

		FadeObstacles();
		
		//if (Vector3.Distance(PlayerThis.position, PlayerOther.position) > thirdPersonSplitDistance)
		//	stateMachine.TransitionTo<TransitionToSplitState>();
	}

	private void LerpSplitScreenLineWidth(float offsetMagnitude) {

		float t = Remap(offsetMagnitude, splitMagnitude, splitMagnitude + 1.0f, 0.0f, 1.0f); 
		float lineWidth = Mathf.Lerp(-.5f, -.497f, t);
		depthMaskPlanePos.x = lineWidth;
		DepthMaskPlane.localPosition = depthMaskPlanePos;
	}

	public static float Remap (float from, float fromMin, float fromMax, float toMin,  float toMax) {
		
		float fromAbs = from - fromMin;
		float fromMaxAbs = fromMax - fromMin;      
      
		float normal = fromAbs / fromMaxAbs;

		float toMaxAbs = toMax - toMin;
		float toAbs = toMaxAbs * normal;

		float to = toAbs + toMin;
		return to;
	}

	private void FadeObstacles() {
		// TODO fade objects in front of camera
		// Physics.SphereCast(_abovePlayer, 
		// 	_cameraCollisionRadius, 
		// 	_offsetDirection.normalized, 
		// 	out _hit, 
		// 	_offsetDirection.magnitude, 
		// 	collisionMask);
	}

	public override void Exit() {
	}
	
	protected void Input() {
		// _mouseMovement.x += playerController.GetCameraMovement().x * mouseSensitivityX * 100;// 0;// TODO UnityEngine.Input.GetAxisRaw(MouseX) * mouseSensitivityX;
		// Debug.Log(_mouseMovement.x);
		// _mouseMovement.y -= playerController.GetCameraMovement().y * mouseSensitivityX * 100;// TODO UnityEngine.Input.GetAxisRaw(MouseY) * mouseSensitivityY;
		// _mouseMovement.y = Mathf.Clamp(_mouseMovement.y, clampLookupMax - LookOffset, clampLookupMin - LookOffset);
	}
	
	private void RotateScreenSplit() {
		Vector3 angle = (PlayerOther.position - PlayerThis.position).normalized;
		Vector3 quarterAngle = _ninetyDegrees * angle;
		Vector3 screenAngle = Vector3.ProjectOnPlane(quarterAngle, -CameraTransform.transform.forward);
		DepthMaskHolder.rotation = Quaternion.LookRotation(CameraTransform.forward, screenAngle);
	}
}
