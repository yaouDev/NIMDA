using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using CallbackSystem;

[CreateAssetMenu(menuName = "Create CameraState/CameraSplitScreenState")]
public class SplitScreenState : CameraState {
	
	[SerializeField] 
	private LayerMask collisionMask;

	[SerializeField] [Range(1.0f, 10.0f)]
	private float mouseSensitivityX = 10.0f;
	
	[SerializeField] [Range(1.0f, 10.0f)]
	private float mouseSensitivityY = 1.0f;

	[SerializeField] [Range(1.0f, 150.0f)] 
	private float rightStickSpeedX = 100.0f;
	
	[SerializeField] [Range(1.0f, 150.0f)] 
	private float rightStickSpeedY = 85.0f;
	
	private float headHeight = 1.6f;

	[SerializeField]
	private bool firstPerson;

	private Vector2 topDownViewRotation = new Vector2(55, 45);
	
	private Vector3 topDownOffset = new Vector3(0.0f, 1.0f, -20.0f);
    
	[SerializeField] [Range(0.0f, 1.0f)] 
	private float _smoothCameraPosTime = 0.105f;
	
	private float splitScreenOffsetFactor = 8.0f;
    
	[SerializeField]
	private float smoothDampMinVal = 0.05f;
	
	[SerializeField]
	private float smoothDampMaxVal = 0.25f;
	
	[SerializeField] [Range(1.0f, 20.0f)] 
	private float isometricDistance = 8.0f;
    
	private RaycastHit _hit;
	private Vector3 _cameraPos;
	private Vector3 _abovePlayer;
	private Vector3 _smoothDampCurrentVelocityLateral;
	private Vector3 _offsetTarget;
	private Vector3 _lerpOffset;
	private Vector3 splitScreenOffset;
	private Vector3 _smoothDampCurrentVelocity;
	private Vector3 depthMaskPlanePos;
	private float _cameraCollisionRadius = .5f;
	private float _smoothDollyTime;
	private float splitScreenWidth = -.497f;
	private RaycastHit[] hits = new RaycastHit[10];
	private bool isRightMostPlayer;
	private float splitMagnitude = 13.0f;
	private float lateralSplitMagnitude = 7.5f;
	private float zoomedInDistance = -12.0f;
	private float zoomedOutDistance = -20.0f;
	private float timeSinceStateStarted;
	private float zoomDistance;
	
	private void Awake() 
	{
		EventSystem.Current.RegisterListener<CameraShakeEvent>(ShakeCamera);
		EventSystem.Current.RegisterListener<BossRoomEvent>(BossRoomEvent);
	}
	
	private void BossRoomEvent(BossRoomEvent bossRoomEvent)
	{
		bossRoom = bossRoomEvent.insideBossRoom;
	}
	
	public override void Enter()
	{
		trauma = 0;
		isPlayerOne = owner.IsPlayerOne();

		timeSinceStateStarted = 0;
		_cameraPos = thisTransform.position;
		depthMaskPlanePos = DepthMaskPlane.localPosition;
		depthMaskPlanePos.x = splitScreenWidth;
		DepthMaskPlane.localPosition = depthMaskPlanePos;
		
		
		// different splitMagnitude x/y axis on screen
		float distanceInCameraViewSpace = (Quaternion.Euler(0, -45, 0) * (PlayerOther.position - PlayerThis.position)).z;
		float inv = Mathf.InverseLerp(0.0f, 20.0f, Mathf.Abs(distanceInCameraViewSpace));
		float dynamicSplitMagnitude = Mathf.Lerp(splitMagnitude, lateralSplitMagnitude, inv);
		float distanceFraction = Vector3.Distance(PlayerThis.position, PlayerOther.position) * .5f / dynamicSplitMagnitude;
		zoomDistance = Mathf.Lerp(zoomedInDistance, zoomedOutDistance, distanceFraction);
		
		// split rotation is kept vertical on the screen
		isRightMostPlayer = (Quaternion.Euler(0, -45, 0) * (PlayerThis.position - PlayerOther.position)).x > 0;
		Vector3 screenAngle = Vector3.ProjectOnPlane(isRightMostPlayer ? Vector3.up : Vector3.down, CameraTransform.transform.forward);
		DepthMaskHolder.rotation = Quaternion.LookRotation(CameraTransform.forward, screenAngle);
	}

	public override void Run()
	{
		timeSinceStateStarted += Time.deltaTime;
		
		CameraTransform.rotation = Quaternion.Euler(topDownViewRotation.x, topDownViewRotation.y, 0.0f);
		
		_cameraPos = Vector3.SmoothDamp(_cameraPos, thisTransform.position, 
			ref _smoothDampCurrentVelocityLateral, _smoothCameraPosTime);

		_abovePlayer = _cameraPos + Vector3.up * headHeight;

		splitScreenOffset = Vector3.ProjectOnPlane( 
			isRightMostPlayer
				? -CameraTransform.transform.right * splitScreenOffsetFactor
				: CameraTransform.transform.right * splitScreenOffsetFactor, 
			Vector3.up); 
		
		// Camera zoom
		topDownOffset.z = Mathf.Lerp(zoomDistance, zoomedOutDistance, timeSinceStateStarted);
		
		// Camera Position
		CameraTransform.position = splitScreenOffset + _abovePlayer + CameraTransform.rotation * topDownOffset;

		CameraShake(1);

		FadeObstacles();

		// different splitMagnitude x/y axis on screen
		float distanceInCameraViewSpace = (Quaternion.Euler(0, -45, 0) * (PlayerOther.position - PlayerThis.position)).z;
		float inv = Mathf.InverseLerp(0.0f, 20.0f, Mathf.Abs(distanceInCameraViewSpace));
		float dynamicSplitMagnitude = Mathf.Lerp(splitMagnitude, lateralSplitMagnitude, inv);
		float distanceFraction = Vector3.Distance(PlayerThis.position, PlayerOther.position) * .5f / dynamicSplitMagnitude;
		
		if (distanceFraction < 0.95f)//Vector3.Distance(PlayerThis.position, PlayerOther.position) < isometricDistance)
			stateMachine.TransitionTo<TransitionToIsometric>();

		if (distanceFraction < 0.95f &&
		    (!isRightMostPlayer && (Quaternion.Euler(0, -45, 0) * (PlayerThis.position - PlayerOther.position)).x > 0 ||
		     isRightMostPlayer && (Quaternion.Euler(0, -45, 0) * (PlayerThis.position - PlayerOther.position)).x < 0 )) 
		{
			stateMachine.TransitionTo<TransitionToIsometricOtherSide>();
		}
		
		if (bossRoom)
			stateMachine.TransitionTo<BossRoomState>();
	}

	private void FadeObstacles() {
		Vector3 offsetDirection = -CameraTransform.forward;
		hits = new RaycastHit[10];
		
		Physics.SphereCastNonAlloc(
			PlayerThis.position,// + splitScreenOffset,
			8.0f,
			offsetDirection.normalized,
			hits,
			25.0f, 
			collisionMask);

		for (int i = 0; i < hits.Length; i++)
		{
			if (hits[i].collider)
			{
				float dot = Vector3.Dot(
					(PlayerThis.position - hits[i].transform.position).normalized,
					new Vector3(1.0f, 0.0f, 1.0f));
				
				TreeFader tf = hits[i].transform.GetComponent<TreeFader>();
				if (tf != null && dot > 0)
				{
					tf.FadeOut();
				}
			}
		}
	}
	
	private float trauma;
	private float easedTrauma;
	private float cameraShakeFalloffSpeed = 2.0f;
	private Vector3 cameraShakeOffset;
	private float shakeSpeed = 10.0f;
	private float vibrationSpeed = 20.0f;
	private float rotationFactor = 5.0f;
	private bool isPlayerOne;
	private CallbackSystem.CameraShakeEvent shakeEvent = new CameraShakeEvent();

	private void ShakeCamera(CameraShakeEvent cameraShake)
	{
		if (cameraShake.affectsPlayerOne && isPlayerOne || cameraShake.affectsPlayerTwo && !isPlayerOne)
			ShakeCamera(cameraShake.magnitude);
	}

	public void ShakeCamera(float magnitude) => trauma += magnitude;

	private void CameraShake(float distanceFraction)
	{
		// perlin within Range(-1, 1)
		float perlinNoiseX = (Mathf.PerlinNoise(0, Time.time * shakeSpeed) - .5f) * 2;
		float perlinNoiseY = (Mathf.PerlinNoise(.5f, Time.time * shakeSpeed) - .5f) * 2;
		// add perlin within Range(-.25, .25)
		perlinNoiseX += (Mathf.PerlinNoise(.25f, Time.time * vibrationSpeed) - .5f) * .5f;
		perlinNoiseY += (Mathf.PerlinNoise(.75f, Time.time * vibrationSpeed) - .5f) * .5f;

		// decrease trauma over time
		trauma = Mathf.Clamp(trauma -= Time.deltaTime * cameraShakeFalloffSpeed, 0.0f, 1.0f);
		
		easedTrauma = Ease.EaseInQuad(trauma);
		
		// Gamepad.current.SetMotorSpeeds(trauma, easedTrauma);

		cameraShakeOffset = 
			CameraTransform.rotation * 
			new Vector3(perlinNoiseX * easedTrauma, perlinNoiseY * easedTrauma, 0.0f);
		
		CameraTransform.rotation *= 
			Quaternion.Euler(
				perlinNoiseX * easedTrauma * rotationFactor, 
				perlinNoiseY * easedTrauma * rotationFactor, 
				Mathf.Lerp(perlinNoiseX, perlinNoiseY, .5f) * easedTrauma * rotationFactor);
	}

	public override void Exit() {
		
	}
}
