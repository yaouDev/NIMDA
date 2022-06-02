using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

public class Blink : MonoBehaviour
{
    [SerializeField] private int numberOfUses = 5;
    [SerializeField] private int maxUses = 5;
    [SerializeField] private float cooldown = 10, reducedCooldown = 5;
    [SerializeField] private float maxDistance = 5;
    [SerializeField] private float blinkSpeed = 100;
    [SerializeField] private float destinationMultiplier = 0.95f;
    [SerializeField] private float cameraheight;
    [SerializeField] private float explosionRange = 4f;
    [SerializeField] private float damage = 50.0f;
    [SerializeField] private float explosionForce = 50f;
    //[SerializeField] private TextMeshProUGUI UIText;
    [SerializeField] private Transform cam;
    [SerializeField] private ParticleSystem trail;
    [SerializeField] private ParticleSystem start;
    [SerializeField] private ParticleSystem finnish;
    [SerializeField] private GameObject player;
    [SerializeField] private LayerMask whatAreTargets;
    [SerializeField] private LayerMask layerMask;
    private Collider[] colliders;
    private float cooldownTimer, reverseTimer, cooldownTime, maxCooldownTimer, percentage;
    private bool blinking = false, started, isPlayerOne;
    private CallbackSystem.PlayerAttack playerAttack;
    private Vector3 destination;
    private RaycastHit hitInfo;
    private IDamageable damageable;
    private bool blinkUpgraded;
    private CallbackSystem.UpdateBlinkUIEvent blinkEvent;

    void Start()
    {
        playerAttack = player.GetComponent<CallbackSystem.PlayerAttack>();
        trail = GetComponentInChildren<ParticleSystem>();
        blinkEvent = new CallbackSystem.UpdateBlinkUIEvent();
        maxUses = numberOfUses;
        cooldownTimer = blinkUpgraded ? reducedCooldown : cooldown;
        reverseTimer = cooldownTimer;
        //UIText.text = numberOfUses.ToString();
    }

    void Update()
    {
        if (!started)
        {
            blinkEvent.isPlayerOne = playerAttack.IsPlayerOne();
            blinkEvent.blinkCountMax = maxUses;
            UpdateUIEvent(0);
            started = true;
        }

        if (numberOfUses < maxUses)
        {
            if (cooldownTimer > 0)
            {
                cooldownTimer -= Time.deltaTime;
            }
            else
            {
                numberOfUses += 1;
                cooldownTimer = blinkUpgraded ? reducedCooldown : cooldown;    
                //UIText.text = "Blink: " + numberOfUses.ToString();
            }
            UpdateUIEvent(cooldownTimer);
        }
        if (blinking)
        {
            if (Vector3.Distance(transform.position, destination) > 0.5f)
            {
                destination.y = 1f;
                transform.position = Vector3.MoveTowards(transform.position, destination, Time.deltaTime * blinkSpeed);
                PulseAttack();
            }
            else
            {
                blinking = false;
            }
        }
    }
    public void DoBlink(InputAction.CallbackContext context)
    {
        if (!playerAttack.IsAlive) return;

        if (context.started && numberOfUses > 0)
        {
            Instantiate(start, transform.position, Quaternion.identity);
            numberOfUses -= 1;
            if (numberOfUses != maxUses)
                UpdateUIEvent(cooldownTimer);
            //UIText.text = "Blink: " + numberOfUses.ToString();
            trail.Play();
            if(Physics.SphereCast(transform.position + Vector3.up, 0.45f, playerAttack.AimingDirection.normalized, out hitInfo, maxDistance, layerMask))
            {
                destination = hitInfo.point + -playerAttack.AimingDirection.normalized * destinationMultiplier;
            }
            else
            {
                destination = (transform.position + playerAttack.AimingDirection.normalized * maxDistance * destinationMultiplier);
            }
            Instantiate(finnish, destination, Quaternion.identity);
            blinking = true;
        }
    }

    void PulseAttack()
    {
        // schreenshake
        CallbackSystem.CameraShakeEvent shakeEvent = new CallbackSystem.CameraShakeEvent();
        shakeEvent.affectsPlayerOne = true;
        shakeEvent.affectsPlayerTwo = true;
        shakeEvent.magnitude = .4f;
        CallbackSystem.EventSystem.Current.FireEvent(shakeEvent);

        CheckForPlayers();

    }

    private void CheckForPlayers()
    {
        colliders = Physics.OverlapSphere(destination, explosionRange, whatAreTargets);
        foreach (Collider coll in colliders)
        {
            if (coll.CompareTag("Player") || coll.CompareTag("BreakableObject") || coll.CompareTag("Enemy"))
            {
                damageable = coll.transform.GetComponent<IDamageable>();

                if (damageable != null)
                {
                    //damage
                    damageable.TakeDamage(damage);

                    //ExplosionForce - check with Will if keep
                    Rigidbody rbTemp = coll.GetComponent<Rigidbody>();
                    if (rbTemp != null)
                    {
                        rbTemp.AddExplosionForce(explosionForce, destination, explosionRange);
                    }
                }
            }
        }
    }

    private void UpdateUIEvent(float time)
    {
        //maxcooldowntimer = cd max
        if (numberOfUses == maxUses)
            time = 0f;
        maxCooldownTimer = blinkUpgraded ? reducedCooldown : cooldown;
        //time = cooldown - time.deltatime
        //cooldowntime = 0 -> maxcooldowntimer
        //reversetimer = 
        cooldownTime = maxCooldownTimer - time;
        //percentage = 0 -> 1 progress to maxcooldowntimer 
        percentage = Mathf.InverseLerp(0f, maxCooldownTimer, cooldownTime);
        blinkEvent.fill = percentage;
        blinkEvent.blinkCount = numberOfUses;
        CallbackSystem.EventSystem.Current.FireEvent(blinkEvent);
    }

    public void DecreaseBlinkCooldown()
    {
        blinkUpgraded = true;
        UpdateUIEvent(0);
    }
    public bool BlinkUpgraded
    {
        get { return blinkUpgraded; }
        set { blinkUpgraded = value; }
    }
}
