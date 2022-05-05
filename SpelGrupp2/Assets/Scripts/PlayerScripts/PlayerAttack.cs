using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

namespace CallbackSystem {
    public class PlayerAttack : MonoBehaviour {
        [SerializeField] private LineRenderer lineRenderer;
        [SerializeField] private LineRenderer aimLineRenderer;
        [SerializeField] private LayerMask enemyLayerMask;
        private Vector3 aimingDirection = Vector3.forward;
        private PlayerHealth health;
        private PlayerController controller;
        private Camera cam;
        private bool isAlive = true;
        [SerializeField][Range(0f, 1f)] private float laserSelfDmg = 0.25f;
        [SerializeField] private int bullets = 10;
        [SerializeField] private GameObject bullet;
        private ResourceUpdateEvent ammoEvent;
        private bool laserWeapon = true;
        private bool activated = false, isPlayerOne;


        public void Respawn() => isAlive = true;
        public void Die() => isAlive = false;


        private void Awake() {
            controller = GetComponent<PlayerController>();
            cam = GetComponentInChildren<Camera>();
            health = GetComponent<PlayerHealth>();
            ammoEvent = new ResourceUpdateEvent();
            isPlayerOne = health.IsPlayerOne();

        }
        private void Update() {
            // TODO joystick laser 
            if (!activated)
            {
                ammoEvent.isPlayerOne = isPlayerOne;
                ammoEvent.a = bullets;
                EventSystem.Current.FireEvent(ammoEvent);
            }

            if (isAlive) {
                aimLineRenderer.enabled = laserWeapon;
                AimDirection();
                ApplyJoystickFireDirection();
                AnimateLaserSightLineRenderer(gameObject.transform.forward);
            } else {
                aimLineRenderer.enabled = false;
            }
        }

        public void Fire(InputAction.CallbackContext context) {
            if (context.started && isAlive) {
                if (laserWeapon) {
                    AudioController.instance?.TriggerTest(); //TODO [Carl August Erik] Make a prefab with what's needed for AudioController
                    ShootLaser();
                    StartCoroutine(AnimateLineRenderer(aimingDirection));
                } else {
                    FireProjectileWeapon();
                }
            }
        }

        public void WeaponSwap(InputAction.CallbackContext context) {
            if (context.performed) {
                laserWeapon = !laserWeapon;
                // TODO [Sound] Play weapon swap sound(s)
            }
        }

        public void WeaponSwapWithMouseWheel(InputAction.CallbackContext context) {
            if (context.performed) {
                float scrollDelta = context.ReadValue<float>();

                if (Mathf.Abs(scrollDelta) > 100.0f) {
                    laserWeapon = scrollDelta > 0;
                    // TODO [Sound] Play weapon swap sound(s)
                }
            }
        }

        private void AimDirection() {
            transform.LookAt(transform.position + aimingDirection);
        }

        private void ApplyJoystickFireDirection() {

            if (controller.GetRightJoystickInput().magnitude > 0.1f) {
                aimingDirection.x = controller.GetRightJoystickInput().x;
                aimingDirection.z = controller.GetRightJoystickInput().y;
                aimingDirection.Normalize();
            }
        }


        private void ShootLaser() {
            health.TakeDamage(laserSelfDmg);
            Physics.Raycast(transform.position + transform.forward + Vector3.up, aimingDirection, out RaycastHit hitInfo, 30.0f, enemyLayerMask);
            if (hitInfo.collider != null) {
                EnemyHealth enemy = hitInfo.transform.GetComponent<EnemyHealth>();
                if (enemy != null) // Enemies were colliding with pickups, so moved them to enemy ( for now ) layer thus this nullcheck to avoid pickups causing issues here
                    enemy.TakeDamage(); //TODO pickUp-object should not be on enemy-layer! // maybe they should have their own layer?
            }
        }


        private IEnumerator AnimateLineRenderer(Vector3 direction) {
            Vector3[] positions = { transform.position + Vector3.up, transform.position + Vector3.up + direction * 30.0f };
            lineRenderer.SetPositions(positions);
            float t = 0.0f;
            while (t < 1.0f) {
                float e = Mathf.Lerp(Ease.EaseOutQuint(t), Ease.EaseOutBounce(t), t);
                float lineWidth = Mathf.Lerp(.5f, .0f, e);
                lineRenderer.startWidth = lineWidth;
                lineRenderer.endWidth = lineWidth;
                Color color = Color.Lerp(Color.white, Color.red, Ease.EaseInQuart(t));
                lineRenderer.startColor = color;
                lineRenderer.endColor = color;
                t += Time.deltaTime * 3.0f;
                yield return null;
            }

            lineRenderer.startWidth = 0.0f;
            lineRenderer.endWidth = 0.0f;
        }

        private void AnimateLaserSightLineRenderer(Vector3 dir) {
            Vector3[] positions = { transform.position + Vector3.up, transform.position + Vector3.up + dir * 30.0f };
            aimLineRenderer.SetPositions(positions);
            float lineWidth = 0.05f;
            aimLineRenderer.startWidth = lineWidth;
            aimLineRenderer.endWidth = lineWidth;
            Color color = new Color(1f, 0.2f, 0.2f);
            aimLineRenderer.startColor = color;
            aimLineRenderer.endColor = color;
        }

        public void TargetMousePos(InputAction.CallbackContext context) {
            Vector3 mousePos = context.ReadValue<Vector2>();
            mousePos.z = 15.0f;
            Plane plane = new Plane(Vector3.up, transform.position + Vector3.up);
            Ray ray = cam.ScreenPointToRay(mousePos);

            if (plane.Raycast(ray, out float enter)) {
                Vector3 hitPoint = ray.GetPoint(enter);
                aimingDirection = hitPoint + Vector3.down - transform.position;
            }
        }

        public void FireProjectileWeapon() {
            if (bullets > 0) {
                AudioController.instance?.TriggerTest(); //TODO [Carl August Erik] Make a prefab with what's needed for AudioController
                bullets--;
                ammoEvent.isPlayerOne = isPlayerOne;
                ammoEvent.a = bullets;
                EventSystem.Current.FireEvent(ammoEvent);
                Instantiate(bullet, transform.position + transform.forward + Vector3.up, transform.rotation, null);

            }
        }
    }
}
