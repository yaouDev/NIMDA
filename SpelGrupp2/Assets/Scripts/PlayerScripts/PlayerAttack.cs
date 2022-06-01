using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using FMOD.Studio;

namespace CallbackSystem
{
    public class PlayerAttack : MonoBehaviour
    {
        [SerializeField] private LineRenderer lineRenderer;
        [SerializeField] private LineRenderer aimLineRenderer;
        [SerializeField] private LayerMask enemyLayerMask, revolverLaserSightLayerMask, laserLaserSightLayermask, wallLayermask;
        private Vector3 aimingDirection = Vector3.forward, crosshairPoint;
        private PlayerHealth health;
        private PlayerController controller;
        private Camera cam;
        private bool isAlive = true;
        [SerializeField] [Range(0f, 50f)] private float maxDistance = 30f;
        [SerializeField] private float startLaserSelfDmg = 1f;
        [SerializeField] private float laserSelfDamageIncreasePerTenthSecond = 1f;
        [SerializeField] private float laserTeamDamageIncreasePerTenthSecond = 3f;
        [SerializeField] private float maxSelfDamage = 10;
        [SerializeField] private float startDamage = 10f, startTeamDamage = 3f;
        [SerializeField] private float chargeTime = 0.1f;
        [SerializeField] private float damageIncreasePerTenthSecond = 10;
        [SerializeField] private float maxBeamThickness = 0.5f;
        [SerializeField] private float startBeamThickness = 0.05f;
        [SerializeField] private float maxDamage = 100;
        [SerializeField] private float maxTeamDamage = 30;
        [SerializeField] [Range(0f, 1.18f)] private float laserAttackDelay = 1.18f;
        [SerializeField] private int bullets, maxBullets, ammoBoxes, bulletUpgradeIncrease = 5; //reloads/ammo boxes - UPPDATERA NAMN
        [SerializeField] private GameObject bullet, upgradedBullet, explosiveBullet;
        private ResourceUpdateEvent resourceEvent;
        private WeaponCrosshairEvent crosshairEvent;
        private bool laserWeapon = true;
        private bool activated = false, isPlayerOne, recentlyFired;
        private bool canShootLaser, canShootGun = true, targetInSight = false;
        private float reducedSelfDmg, laserWeaponCooldown, currentHitDistance, revolverCooldown,
            reducedChargeTime, decreasedBeamWidth = 0.075f, decreasedThickness = 0.75f;

        private bool revolverDamageUpgraded, revolverCritUpgraded, revolverMagazineUpgraded, 
            laserDamageUpgraded, laserChargeRateUpgraded, laserBeamWidthUpgraded; 

        private System.Random rnd = new System.Random();

        [SerializeField] private float damage;
        [SerializeField] private float laserSelfDmg;
        [SerializeField] private float teamDamage;
        [SerializeField] private float beamThickness = 0.5f;

        private bool chargingUP = false;
        private float startSightLineWidth = 0.05f;
        private float sightLineWidth;
        private float widthIncreacePerTenthSecond = 0.05f;
        private float maxBeamLenght = 30.0f;
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

        private AudioController ac;
        private EventInstance laserSound;
        public void Start() => ac = AudioController.instance;

        public bool IsPlayerOne() { return isPlayerOne; }

        public bool UsingLaserWeapon() { return laserWeapon; }

        private void Awake()
        {
            controller = GetComponent<PlayerController>();
            cam = GetComponentInChildren<Camera>();
            health = GetComponent<PlayerHealth>();
            resourceEvent = new ResourceUpdateEvent();
            crosshairEvent = new WeaponCrosshairEvent();
            isPlayerOne = health.IsPlayerOne();
            reducedSelfDmg = laserSelfDamageIncreasePerTenthSecond / 2;
            reducedChargeTime = chargeTime / 2;
            laserWeaponCooldown = 0f;
            revolverCooldown = 0f;
            damage = startDamage;
            sightLineWidth = startSightLineWidth;
            laserSelfDmg = startLaserSelfDmg;
            teamDamage = startTeamDamage;
            beamThickness = startBeamThickness;
        }

        [SerializeField] private Material bulletMat;
        [SerializeField] private Material laserMat;

        private void Update()
        {
            //Debug.Log(bulletsInGun);
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
                resourceEvent.isPlayerOne = isPlayerOne;
                UpdateAmmoUI();
                crosshairEvent.usingRevolver = !laserWeapon;
                crosshairEvent.isPlayerOne = isPlayerOne;
                crosshairEvent.targetInSight = targetInSight;
                EventSystem.Current.FireEvent(crosshairEvent);
                activated = true;
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
                laserSound = ac.PlayNewInstanceWithParameter(IsPlayerOne() ? ac.player1.fire1 : ac.player2.fire1, gameObject, "isReleased", 0f);
                chargingUP = true;
                StartCoroutine(ChargeUp());
                controller.MovementSpeedReduction(true);
            }

            if (context.canceled && laserWeapon)
            {
                chargingUP = false;
                StopCoroutine(ChargeUp());
                controller.MovementSpeedReduction(false);
                if (canShootLaser)
                {
                    laserSound.setParameterByName("isReleased", 1f);

                    ShootLaser();
                    StartCoroutine(AnimateLineRenderer(aimingDirection));
                    //add shootsound

                }
                recentlyFired = true;
                laserWeaponCooldown = 0f;
                damage = startDamage;
                sightLineWidth = laserBeamWidthUpgraded ? decreasedBeamWidth : startSightLineWidth;
                laserSelfDmg = startLaserSelfDmg;
                teamDamage = startTeamDamage;
                beamThickness = laserBeamWidthUpgraded ? decreasedBeamWidth : startBeamThickness;
            }
            //damage = startDamage;
        }

        IEnumerator ChargeUp()
        {
            maxBeamThickness = laserBeamWidthUpgraded ? decreasedThickness : maxBeamThickness;
            widthIncreacePerTenthSecond = laserBeamWidthUpgraded ? decreasedBeamWidth : widthIncreacePerTenthSecond;

            while (damage < maxDamage && chargingUP)
            {
                yield return new WaitForSeconds(laserChargeRateUpgraded ?  reducedChargeTime : chargeTime);
                damage += damageIncreasePerTenthSecond;
                if (sightLineWidth < maxBeamThickness)
                {
                    sightLineWidth += widthIncreacePerTenthSecond;
                }
                if (beamThickness < maxBeamThickness)
                {
                    beamThickness += widthIncreacePerTenthSecond;
                }
                if (laserSelfDmg < maxSelfDamage)
                {
                    laserSelfDmg += laserDamageUpgraded ? reducedSelfDmg : laserSelfDamageIncreasePerTenthSecond;
                }
                if (teamDamage < maxTeamDamage)
                {
                    teamDamage += laserTeamDamageIncreasePerTenthSecond;
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
                if (health != null)
                {
                    health.TakeDamage(laserSelfDmg);
                }

                //Check how far to not penetrable object
                /*                Physics.Raycast(transform.position + transform.forward + Vector3.up, aimingDirection, out wallHitInfo, 30.0f, wallLayermask); // TODO change to firepoint

                                distanceToWall = wallHitInfo.distance;*/

                //Check for enemies and onther penetrable objects
                bool hitObstacle = false;
                bool hitShield = false;
                RaycastHit[] hits = Physics.SphereCastAll(transform.position + transform.forward + Vector3.up, beamThickness, aimingDirection, maxBeamLenght, enemyLayerMask);
                if (hits.Length > 0)
                {
                    List<RaycastHit> hitList = new List<RaycastHit>(hits);
                    hitList.Sort((x, y) => x.distance.CompareTo(y.distance));
                    //for (int i = 0; i < hitList.Count; i++)
                    //{
                    //    Debug.Log(hitList[i].transform.gameObject.name);
                    //}
                    foreach (RaycastHit hitInfo in hitList) // TODO change to firepoint
                    {
                        
                        if (hitInfo.collider != null)
                        {
                            Debug.Log(hitInfo.transform.gameObject.layer + " " + (1 << LayerMask.NameToLayer("SeeThrough")));
                            if (hitInfo.transform.gameObject.layer == 8)// (1 << LayerMask.NameToLayer("SeeThrough")))
                            {
                                hitObstacle = true;
                                return;
                            }
                            if(hitInfo.transform.gameObject.layer == 25)
                            {
                                hitShield = true;
                                IDamageable damageable = hitInfo.transform.GetComponent<IDamageable>();
                                damageable.TakeDamage(damage);
                                return;
                            }
                            else if (!hitObstacle && !hitShield &&(hitInfo.transform.tag == "Enemy" && hitInfo.collider.isTrigger == false || hitInfo.transform.tag == "Player"))
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
                            else if (!hitObstacle && !hitShield && hitInfo.transform.tag == "BreakableObject")
                            {
                                BreakableObject breakable = hitInfo.transform.GetComponent<BreakableObject>();
                                breakable.DropBoxLoot();
                            }
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
            if (!laserWeapon)
            {
                Physics.Raycast(transform.position + Vector3.up, aimingDirection, out RaycastHit hit, maxDistance, revolverLaserSightLayerMask);
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
            else
            {
                Physics.Raycast(transform.position + Vector3.up, aimingDirection, out RaycastHit hit, maxDistance, laserLaserSightLayermask);
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
            crosshairEvent.usingRevolver = laserWeapon ? false : true;
            crosshairEvent.isPlayerOne = isPlayerOne;
            crosshairEvent.crosshairPos = crosshairPoint;
            crosshairEvent.targetInSight = targetInSight;
            EventSystem.Current.FireEvent(crosshairEvent);
        }

        private GameObject currentBullet;
        private int critChance;
        private void FireProjectileWeapon()
        {
            //Debug.Log("Attempting to fire.");
            if (bullets > 0 && ammoBoxes >= 0)
            {
                if (AIData.Instance.EnemyMuzzleflash != null)
                {
                    Instantiate(AIData.Instance.EnemyMuzzleflash, transform.position, Quaternion.identity);
                }
                //Debug.Log("Firing. Shots left: " + bulletsInGun);
                AudioController ac = AudioController.instance;
                ac.PlayOneShotAttatched(IsPlayerOne() ? ac.player1.fire2 : ac.player2.fire2, gameObject); //Gun sound
                currentBullet = revolverDamageUpgraded ? upgradedBullet : bullet;
                DecreaseBulletCount();
                critChance = rnd.Next(0, 20);
                if (revolverCritUpgraded && critChance == 20)
                    currentBullet = explosiveBullet;
                Instantiate(currentBullet, transform.position + transform.forward + Vector3.up, transform.rotation, null);
            }
        }

        public int ReturnBullets()
        {
            return bullets;
        }

        public int ReturnMaxBullets()
        {
            return revolverMagazineUpgraded ? maxBullets + bulletUpgradeIncrease : maxBullets;
        }

        public void CraftAmmoBox()
        {
            ammoBoxes++;
            Debug.Log("Increased AmmoBox Count");
            UpdateAmmoUI();
        }

        public void DecreaseBulletCount()
        {
            bullets--;
            if (bullets == 0 && ammoBoxes > 0)
                Reload();
            UpdateAmmoUI();
        }

        private void UpdateAmmoUI()
        { 
            resourceEvent.ammoChange = true;
            resourceEvent.a = bullets;
            resourceEvent.magAmmo = ammoBoxes;
            EventSystem.Current.FireEvent(resourceEvent);
            Debug.Log("Updated Ammo UI");
        }
        private void Reload()
        {
            bullets = ReturnMaxBullets();
            ammoBoxes--;
        }
        public void SetBulletsOnLoad(int amount) => bullets = amount;
       
        public void UpgradeLaserWeapon() => LaserDamageUpgraded = true;
        public void LaserChargeRateUpgrade() => LaserChargeRateUpgraded = true;
        public void LaserBeamWidthUpgrade() => LaserBeamWidthUpgraded = true;
        public void UpgradeProjectileWeapon() => ProjectileWeaponUpgraded = true;
        public void RevolverCritUpgrade() => RevolverCritUpgraded = true;
        public void RevolverMagazineUpgrade() => RevolverMagazineUpgraded = true;

        public bool LaserDamageUpgraded
        {
            get { return laserDamageUpgraded; }
            set { laserDamageUpgraded = value; }
        }

        public bool LaserChargeRateUpgraded
        {
            get { return laserChargeRateUpgraded; }
            set { laserChargeRateUpgraded = value; }
        }

        public bool LaserBeamWidthUpgraded
        {
            get { return laserBeamWidthUpgraded; }
            set { laserBeamWidthUpgraded = value; }
        }

        public bool ProjectileWeaponUpgraded
        {
            get { return revolverDamageUpgraded; }
            set { revolverDamageUpgraded = value; }
        }

        public bool RevolverCritUpgraded
        {
            get { return revolverCritUpgraded; }
            set { revolverCritUpgraded = value; }
        }

        public bool RevolverMagazineUpgraded
        {
            get { return revolverMagazineUpgraded; }
            set { revolverMagazineUpgraded = value; }
        }
    }
}