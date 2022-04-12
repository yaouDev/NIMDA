using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(menuName = "Create CameraState/TransitionToIsometric")]
public class TransitionToIsometric : CameraBaseState
{

	// [SerializeField] [Range(1.0f, 10.0f)]
	// private float mouseSensitivityX = 10.0f;
	//
	// [SerializeField] [Range(1.0f, 10.0f)]
	// private float mouseSensitivityY = 5.0f;

	[SerializeField] 
	private Vector3 topDownOffset;
	
	[SerializeField]
	private float headHeight = 1.6f;
	
	private Vector3 depthMaskPlanePos;
	private Vector3 centroid;
	private Vector3 abovePlayer;

	[SerializeField]
	private Vector2 topDownViewRotation;

	private float percentage = 0.0f;
	
	private void Awake() {
		abovePlayer = Vector3.up * headHeight;
	}
	public override void Enter() {
		percentage = 0.0f;
	    depthMaskPlanePos = DepthMaskPlane.localPosition;
	    depthMaskPlanePos.x = -.5f;
	    DepthMaskPlane.localPosition = depthMaskPlanePos;
    }

    public override void Run() {
	    Input();

	    percentage += Time.deltaTime * 2.0f;
	    
	    // both cameras have the same rotation (fixed?)
	    CameraTransform.rotation = Quaternion.Slerp(CameraTransform.rotation,  Quaternion.Euler(topDownViewRotation.x, topDownViewRotation.y, 0.0f), percentage);

	    // the split is rotating freely between the players
	    // RotateScreenSplit();
		
	    // both cameras follow the centroid point between the players, split when necessary
	    Vector3 centroidOffsetPosition = (PlayerOther.position - PlayerThis.position) * .5f;
	    centroid = PlayerThis.position + centroidOffsetPosition;

	    CameraTransform.position = Vector3.Lerp(CameraTransform.position,  centroid + abovePlayer + CameraTransform.rotation * topDownOffset, percentage);
		
	    LerpSplitScreenLineWidth(percentage);

	    FadeObstacles();
		
	    if (percentage > 1.0f)
		    stateMachine.TransitionTo<TopDownState>();
    }
    
    private void LerpSplitScreenLineWidth(float percentage) {

	    float lineWidth = Mathf.Lerp(-.5f, -.497f, 1.0f - percentage);
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
	    // todo spherecast and fade out objects between player and camera
	    // Physics.SphereCast(_abovePlayer, 
	    // 	_cameraCollisionRadius, 
	    // 	_offsetDirection.normalized, 
	    // 	out _hit, 
	    // 	_offsetDirection.magnitude, 
	    // 	collisionMask);
    }

    public override void Exit() {
    }
    
    private void Input() {
	    // mouseMovement.x += UnityEngine.Input.GetAxisRaw(MouseX) * mouseSensitivityX;
	    // mouseMovement.y -= UnityEngine.Input.GetAxisRaw(MouseY) * mouseSensitivityY;
	    // mouseMovement.y = Mathf.Clamp(mouseMovement.y, Camera.clampLookupMax - lookOffset, Camera.clampLookupMin - lookOffset);
    }
}