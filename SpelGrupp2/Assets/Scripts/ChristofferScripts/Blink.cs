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
    [SerializeField] private float maxDistance = 10f;
    [SerializeField] private float blinkSpeed = 100;
    [SerializeField] private float destinationMultiplier = 0.9f;
    [SerializeField] private float cameraheight;
    [SerializeField] private TextMeshProUGUI UIText;
    [SerializeField] private Transform cam;
    [SerializeField] private ParticleSystem trail;
    [SerializeField] private GameObject player;
    [SerializeField] private LayerMask layerMask;
    private float cooldownTimer;
    private bool blinking = false;
    private CallbackSystem.PlayerAttack playerAttack;
    private Vector3 destination;

    // Start is called before the first frame update
    void Start()
    {
        playerAttack = player.GetComponent<CallbackSystem.PlayerAttack>();
        trail = GetComponentInChildren<ParticleSystem>();
        maxUses = numberOfUses;
        cooldownTimer = cooldown;
        UIText.text = numberOfUses.ToString();
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
                UIText.text = "Blink: " + numberOfUses.ToString();
            }
        }
        if (blinking)
        {
            var dist = Vector3.Distance(transform.position, destination);
            if (dist > 0.5f)
            {
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
        Debug.Log("Tryckt p� space och �r inte d�d");

        if (context.started && numberOfUses > 0)
        {

            numberOfUses -= 1;
            UIText.text = "Blink: " + numberOfUses.ToString();
            trail.Play();
            if(Physics.Raycast(transform.position + Vector3.up, playerAttack.AimingDirection, out RaycastHit hitInfo, maxDistance, layerMask))
            {
                destination = hitInfo.point * destinationMultiplier;
            }
            else
            {
                destination = (transform.position + playerAttack.AimingDirection * maxDistance) * destinationMultiplier;
            }

            blinking = true;
        }
    }
}
