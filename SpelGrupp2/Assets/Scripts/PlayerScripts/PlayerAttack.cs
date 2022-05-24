using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

namespace CallbackSystem
{
    public class PlayerAttack : MonoBehaviour
    {
        [SerializeField] private LineRenderer lineRenderer;
        [SerializeField] private LineRenderer aimLineRenderer;
        [SerializeField] private LayerMask enemyLayerMask, laserLayerMask, wallLayermask;
        private Vector3 aimingDirection = Vector3.forward, crosshairPoint;
        private PlayerHealth health;
        private PlayerController controller;
        private Camera cam;
        private bool isAlive = true;
        [SerializeField] [Range(0f, 50f)] private float maxDistance = 30f;
        [SerializeField] private float startLaserSelfDmg = 2f;
        [SerializeField] private float laserSelfDamageIncreasePerMilliSecond = 0.5f;
        [SerializeField] private float maxSelfDamage = 10;
        [SerializeField] private float startDamage = 20f, teamDamage = 30f;
        [SerializeField] private float damageIncreasePerMilliSecond = 10;
        [SerializeField] private float maxDamage = 110;
        [SerializeField] [Range(0f, 1.18f)] private float laserAttackDelay = 1.18f;
        [SerializeField] private float beamThickness = 0.5f;
        [SerializeField] private int bullets, maxBullets;
        [SerializeField] private GameObject bullet, upgradedBullet;
        private ResourceUpdateEvent resourceEvent;
        private WeaponCrosshairEvent crosshairEvent;
        private bool laserWeapon = true;
        private bool activated = false, isPlayerOne, recentlyFired;
        private bool canShootLaser, projectionWeaponUpgraded, laserWeaponUpgraded, automaticFireUpgraded = true, canShootGun = true, targetInSight = false;
        private float reducedSelfDmg, laserWeaponCooldown, currentHitDistance, revolverCooldown;
        [SerializeField] private float damage;
        [SerializeField] private float laserSelfDmg;
        private bool chargingUP = false;
        private float startSightLineWidth = 0.05f;
        private float sightLineWidth;
        private float widthIncreacePerMilliSecond = 0.05f;
        //private float distanceToWall;
        //private RaycastHit wallHitInfo;

        /*
         * From where the players weapon and ammunition is instantiated, stored and managed.
         * Only call on ResourceEvents concering ammunition from this script using UpdateBulletCount(increase/decrease).
         */

        public bool IsAlive
        {
            get { return isAlive; }
        }
        public Vector3 AimingDirection
        {
            get { return aimingDirection; }
        }

        public void Die() => isAlive = false;

        public bool IsPlayerOne() { return isPlayerOne; }

        public bool UsingLaserWeapon() { return laserWeapon; }
        public void UpdateBulletCount(int amount)
        {
            bullets += amount;
            //resourceEvent.isPlayerOne = isPlayerOne;
            resourceEvent.ammoChange = true;
            resourceEvent.a = bullets;
            EventSystem.Current.FireEvent(resourceEvent);
        }

        private void Awake()
        {
            controller = GetComponent<PlayerController>();
            cam = GetComponentInChildren<Camera>();
            health = GetComponent<PlayerHealth>();
            resourceEvent = new ResourceUpdateEvent();
            crosshairEvent = new WeaponCrosshairEvent();
            isPlayerOne = health.IsPlayerOne();
            reducedSelfDmg = laserSelfDmg / 2;
            laserWeaponCooldown = 0f;
            revolverCooldown = 0f;
            damage = startDamage;
            sightLineWidth = startSightLineWidth;
            laserSelfDmg = startLaserSelfDmg;

        }

        [SerializeField] private Material bulletMat;
        [SerializeField] private Material laserMat;

        private void Update()
        {
            canShootLaser = (health.GetCurrenthealth() > laserSelfDmg || health.GetCurrentBatteryCount() > 0);
            // if (healthPercentage.ReturnHealth() > laserSelfDmg || healthPercentage.ReturnBatteries() > 0)
            // {
            //     canShootLaser = true;
            // }
            // else
            // {
            //     canShootLaser = false;
            // }

            // TODO joystick laser 
            if (!activated)
            {
                resourceEvent.ammoChange = true;
                resourceEvent.isPlayerOne = isPlayerOne;
                resourceEvent.a = bullets;
                EventSystem.Current.FireEvent(resourceEvent);
                activated = true;
                crosshairEvent.usingProjectileWeapon = !laserWeapon;
                crosshairEvent.isPlayerOne = isPlayerOne;
                crosshairEvent.targetInSight = targetInSight;
                EventSystem.Current.FireEvent(crosshairEvent);
            }
            if (recentlyFired && laserWeaponCooldown < 0.5f && laserWeapon)
                laserWeaponCooldown += Time.deltaTime;
            else if (recentlyFired && revolverCooldown < 0.3f && !laserWeapon)
                revolverCooldown += Time.deltaTime;
            else
                recentlyFired = false;
            /*
            if (ASCounter <= 0.1f)
                ASCounter += Time.deltaTime;
            else
                canShootGun = true;
            */

            if (isAlive)
            {
                AnimateLasers();

            }
            else
            {
                aimLineRenderer.enabled = false;
            }
        }

        /*        public void Fire(InputAction.CallbackContext context)
                {
                    if (!isAlive) return;
                    if (context.started && !recentlyFired)
                    {

                        if (laserWeapon && canShootLaser)
                        {
                            StartCoroutine(AttackDelay(laserAttackDelay));
                        }
                        else if (!laserWeapon)
                        {
                            FireProjectileWeapon();
                        }
                        recentlyFired = true;
                        laserWeaponCooldown = 0f;
                        revolverCooldown = 0f;
                    }
                }*/
        public void Fire(InputAction.CallbackContext context)
        {
            if (!isAlive) return;
            if (context.started && !recentlyFired && !laserWeapon)
            {
                FireProjectileWeapon();
                recentlyFired = true;
                revolverCooldown = 0f;
            }
            if (context.performed && laserWeapon && canShootLaser)
            {

                chargingUP = true;
                StartCoroutine(ChargeUp());

                //add channelsound

                //AudioController ac = AudioController.instance; //TODO: change audio parameter to fire with channel time!
                //ac.PlayNewInstanceWithParameter(IsPlayerOne() ? ac.player1.fire1 : ac.player2.fire1, gameObject, "laser_channel", channelTime); //laser sound
                /*                }*/
            }
            else
            {
                chargingUP = false;
            }

            if (context.canceled && laserWeapon)
            {
                StopCoroutine(ChargeUp());
                if (canShootLaser)
                {
                    ShootLaser();
                    StartCoroutine(AnimateLineRenderer(aimingDirection));
                    //add shootsound
                    //AudioController ac = AudioController.instance; //TODO: change audio parameter to fire with channel time!
                    //ac.PlayNewInstanceWithParameter(IsPlayerOne() ? ac.player1.fire1 : ac.player2.fire1, gameObject, "laser_channel", channelTime); //laser sound
                }
                recentlyFired = true;
                laserWeaponCooldown = 0f;
                damage = startDamage;
                sightLineWidth = startSightLineWidth;
                laserSelfDmg = startLaserSelfDmg;

            }
            //damage = startDamage;
        }

        IEnumerator ChargeUp()
        {
            while (damage < maxDamage && chargingUP)
            {
                yield return new WaitForSeconds(0.1f);
                damage += damageIncreasePerMilliSecond;
                if (sightLineWidth < beamThickness)
                {
                    sightLineWidth += widthIncreacePerMilliSecond;
                }
                if (laserSelfDmg < maxSelfDamage)
                {
                    laserSelfDmg += laserSelfDamageIncreasePerMilliSecond;
                }
            }
        }
        /*        IEnumerator AttackDelay(float channelTime)
                {
                    AudioController ac = AudioController.instance; //TODO: change audio parameter to fire with channel time!
                    ac.PlayNewInstanceWithParameter(IsPlayerOne() ? ac.player1.fire1 : ac.player2.fire1, gameObject, "laser_channel", channelTime); //laser sound
                    yield return new WaitForSeconds(channelTime);
                    //LaserAttack();
                }*/

        /*        private void LaserAttack()
                {
                    ShootLaser();
                    StartCoroutine(AnimateLineRenderer(aimingDirection));
                }*/
        /*
        private void ProjectileFire(InputAction.CallbackContext context)
        {
            int debugInt = 0;
            while (automaticFireUpgraded && !context.canceled && debugInt != 300)
            {
                if (canShootGun)
                { 
                    FireProjectileWeapon();
                    canShootGun = false;
                    ASCounter = 0f;
                }
                debugInt++;
                Debug.Log("debugInt reached 300");
            }
        }
        */

        public void WeaponSwap(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                laserWeapon = !laserWeapon;
                // TODO [Sound] Play weapon swap sound(s)
            }
        }

        public void WeaponSwapWithMouseWheel(InputAction.CallbackContext context)
        {
            if (context.performed && Mathf.Abs(context.ReadValue<float>()) > 100.0f)
            {
                laserWeapon = !laserWeapon;
                // TODO [Sound] Play weapon swap sound(s)
            }
        }

        //This method & Pass Through(Y) on Input Actions if up = laser & down = projectile.
        /*public void WeaponSwapWithMouseWheel(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                float scrollDelta = context.ReadValue<float>();
                Debug.Log(scrollDelta);
                if (Mathf.Abs(scrollDelta) > 100.0f)
                {
                    laserWeapon = scrollDelta > 0;
                    // TODO [Sound] Play weapon swap sound(s)
                }
            }
        }
        */

        private void AimDirection()
        {
            transform.LookAt(transform.position + aimingDirection);
        }

        private void ApplyJoystickFireDirection()
        {
            if (controller.GetRightJoystickInput().magnitude > 0.1f)
            {
                aimingDirection.x = controller.GetRightJoystickInput().x;
                aimingDirection.z = controller.GetRightJoystickInput().y;
                int sign = aimingDirection.z > 0 ? 1 : -1;
                aimingDirection.z = sign * Ease.EaseInCirc(Mathf.Abs(aimingDirection.z));
                aimingDirection.Normalize();
                aimingDirection = Quaternion.Euler(0, 45, 0) * aimingDirection;
            }
        }

        private void ShootLaser()
        {
            if (canShootLaser)
            {
                if (laserWeaponUpgraded)
                    laserSelfDmg = reducedSelfDmg;
                if (health != null)
                {
                    health.TakeDamage(laserSelfDmg);
                }

                //Check how far to not penetrable object
                /*                Physics.Raycast(transform.position + transform.forward + Vector3.up, aimingDirection, out wallHitInfo, 30.0f, wallLayermask); // TODO change to firepoint

                                distanceToWall = wallHitInfo.distance;*/

                //Check for enemies and onther penetrable objects
                foreach (RaycastHit hitInfo in Physics.SphereCastAll(transform.position + transform.forward + Vector3.up, beamThickness, aimingDirection, 30.0f, enemyLayerMask)) // TODO change to firepoint
                {
                    if (hitInfo.collider != null)
                    {
                        if (hitInfo.transform.tag == "Enemy" || hitInfo.transform.tag == "Player")
                        {
                            IDamageable damageable = hitInfo.transform.GetComponent<IDamageable>();

                            if (damageable != null) // Enemies were colliding with pickups, so moved them to enemy ( for now ) layer thus this nullcheck to avoid pickups causing issues here
                            {
                                if (hitInfo.transform.tag == "Player")
                                    damageable.TakeDamage(teamDamage);
                                else
                                    damageable.TakeDamage(damage); //TODO pickUp-object should not be on enemy-layer! // maybe they should have their own layer?
                            }
                        }
                        else if (hitInfo.transform.tag == "BreakableObject")
                        {
                            BreakableObject breakable = hitInfo.transform.GetComponent<BreakableObject>();
                            breakable.DropBoxLoot();
                        }
                    }

                }
            }
        }

        private IEnumerator AnimateLineRenderer(Vector3 direction)
        {
            Vector3[] positions = { transform.position + Vector3.up, transform.position + Vector3.up + direction * 30.0f };
            lineRenderer.SetPositions(positions);

            float t = 0.0f;
            while (t < 1.0f)
            {
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

        private void AnimateLaserSightLineRenderer(Vector3 dir)
        {
            Vector3[] positions = { transform.position + Vector3.up, transform.position + Vector3.up + dir * currentHitDistance };
            aimLineRenderer.SetPositions(positions);
            //sightLineWidth = 0.05f;
            aimLineRenderer.startWidth = sightLineWidth;
            aimLineRenderer.endWidth = sightLineWidth;
            Color color = new Color(1f, 0.2f, 0.2f);
            aimLineRenderer.startColor = color;
            aimLineRenderer.endColor = color;
        }

        private void UpdateLaserSightDistance()
        {
            Physics.Raycast(transform.position + Vector3.up, aimingDirection, out RaycastHit hit, maxDistance, laserLayerMask);
            if (hit.collider != null)
            {
                currentHitDistance = hit.distance;
                targetInSight = true;
                crosshairPoint = cam.WorldToScreenPoint(hit.point);
            }
            else
            {
                currentHitDistance = maxDistance;
                targetInSight = false;
            }
        }


        public void TargetMousePos(InputAction.CallbackContext context)
        {
            Vector3 mousePos = context.ReadValue<Vector2>();
            mousePos.z = 15.0f;
            Plane plane = new Plane(Vector3.up, transform.position + Vector3.up);
            Ray ray = cam.ScreenPointToRay(mousePos);

            if (plane.Raycast(ray, out float enter))
            {
                Vector3 hitPoint = ray.GetPoint(enter);
                aimingDirection = hitPoint + Vector3.down - transform.position;
            }
        }
        public void Respawn()
        {
            isAlive = true;
            AnimateLasers();
        }

        private void AnimateLasers()
        {
            aimLineRenderer.enabled = true;
            aimLineRenderer.material = laserWeapon ? laserMat : bulletMat;

            AimDirection();
            ApplyJoystickFireDirection();
            UpdateLaserSightDistance();
            AnimateLaserSightLineRenderer(gameObject.transform.forward);
            RenderCrosshair();
        }

        private void RenderCrosshair()
        {
            crosshairEvent.usingProjectileWeapon = laserWeapon ? false : true;
            crosshairEvent.isPlayerOne = isPlayerOne;
            crosshairEvent.crosshairPos = crosshairPoint;
            crosshairEvent.targetInSight = targetInSight;
            EventSystem.Current.FireEvent(crosshairEvent);
        }

        private void FireProjectileWeapon()
        {
            if (bullets > 0)
            {
                if (AIData.Instance.EnemyMuzzleflash != null)
                {
                    Instantiate(AIData.Instance.EnemyMuzzleflash, transform.position, Quaternion.identity);
                }
                AudioController ac = AudioController.instance;
                ac.PlayOneShotAttatched(IsPlayerOne() ? ac.player1.fire2 : ac.player2.fire2, gameObject); //Gun sound
                UpdateBulletCount(-1);
                if (projectionWeaponUpgraded)
                    Instantiate(upgradedBullet, transform.position + transform.forward + Vector3.up, transform.rotation, null);
                else
                    Instantiate(bullet, transform.position + transform.forward + Vector3.up, transform.rotation, null);
            }
        }

        private void AutomaticProjectileWeapon()
        {
            if (bullets > 0)
            {
                if (AIData.Instance.EnemyMuzzleflash != null)
                {
                    Instantiate(AIData.Instance.EnemyMuzzleflash, transform.position, Quaternion.identity);
                }
                Debug.Log("Standard projectile weapon fired!");
                AudioController ac = AudioController.instance;
                ac.PlayOneShotAttatched(IsPlayerOne() ? ac.player1.fire2 : ac.player2.fire2, gameObject); //Gun sound
                UpdateBulletCount(-1);
                if (projectionWeaponUpgraded)
                    Instantiate(upgradedBullet, transform.position + transform.forward + Vector3.up, transform.rotation, null);
                else
                    Instantiate(bullet, transform.position + transform.forward + Vector3.up, transform.rotation, null);
            }
        }

        public void UpgradeProjectileWeapon()
        {
            Debug.Log("Projectile weapon upgraded!");
            projectionWeaponUpgraded = true;
        }

        public void UpgradeLaserWeapon()
        {
            Debug.Log("Projectile weapon upgraded!");
            laserWeaponUpgraded = true;
        }

        public int ReturnBullets()
        {
            return bullets;
        }

        public int ReturnMaxBullets()
        {
            return maxBullets;
        }
    }
}
