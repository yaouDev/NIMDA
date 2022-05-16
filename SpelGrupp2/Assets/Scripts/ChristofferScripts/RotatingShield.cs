using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatingShield : MonoBehaviour
{
    [SerializeField] private Vector3 rotation;
    [SerializeField] private float rotationSpeed;
    [SerializeField] private float rotationSpeedMultiplier = 2.0f;

    private EnemyHealth enemyHealth;
    private float health;

    private void Start()
    {
        enemyHealth = GetComponentInParent<EnemyHealth>();
    }

    void Update()
    {
        health = enemyHealth.CurrentHealth;
        //Spin the shield
        transform.Rotate(rotation * rotationSpeed * Time.deltaTime);
        //If health is under 700, spin faster
        if (health < 1400)
        {
            transform.Rotate(rotation * (rotationSpeed * rotationSpeedMultiplier) * Time.deltaTime);
            
        }
        else
        {
            if (health < 800)
            {
                gameObject.layer = LayerMask.NameToLayer("Bouncing");
                transform.Rotate(rotation * (rotationSpeed * rotationSpeedMultiplier) * Time.deltaTime);
                Debug.Log("Hej och hå" + rotation);
            }

        }
    }
}
