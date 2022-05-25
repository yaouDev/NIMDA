using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

public class Blink : MonoBehaviour
{
    [SerializeField] private int numberOfUses = 5;
    [SerializeField] private int maxUses = 5;
    [SerializeField] private float cooldown = 2;
    [SerializeField] private float maxDistance = 5;
    [SerializeField] private float blinkSpeed = 100;
    [SerializeField] private float destinationMultiplier = 0.95f;
    [SerializeField] private float cameraheight;
    //[SerializeField] private TextMeshProUGUI UIText;
    [SerializeField] private Transform cam;
    [SerializeField] private ParticleSystem trail;
    [SerializeField] private ParticleSystem start;
    [SerializeField] private ParticleSystem finnish;
    [SerializeField] private GameObject player;
    [SerializeField] private LayerMask layerMask;
    private float cooldownTimer;
    private bool blinking = false;
    private CallbackSystem.PlayerAttack playerAttack;
    private Vector3 destination;
    private RaycastHit hitInfo;

    // Start is called before the first frame update
    void Start()
    {
        playerAttack = player.GetComponent<CallbackSystem.PlayerAttack>();
        trail = GetComponentInChildren<ParticleSystem>();
        maxUses = numberOfUses;
        cooldownTimer = cooldown;
        //UIText.text = numberOfUses.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        if (numberOfUses < maxUses)
        {
            if (cooldownTimer > 0)
            {
                cooldownTimer -= Time.deltaTime;
            }
            else
            {
                numberOfUses += 1;
                cooldownTimer = cooldown;
                //UIText.text = "Blink: " + numberOfUses.ToString();
            }
        }
        if (blinking)
        {
            var dist = Vector3.Distance(transform.position, destination);
            if (dist > 0.5f)
            {
                destination.y = 1f;
                transform.position = Vector3.MoveTowards(transform.position, destination, Time.deltaTime * blinkSpeed);
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
            //UIText.text = "Blink: " + numberOfUses.ToString();
            trail.Play();
            if(Physics.SphereCast(transform.position + Vector3.up, 0.45f, playerAttack.AimingDirection.normalized, out hitInfo, maxDistance, layerMask))
            {
                destination = hitInfo.point * destinationMultiplier;
            }
            else
            {
                destination = (transform.position + playerAttack.AimingDirection.normalized * maxDistance * destinationMultiplier);
            }
            Instantiate(finnish, destination, Quaternion.identity);
            blinking = true;
        }
    }

}
