using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatingShield : MonoBehaviour
{
    [SerializeField] private Vector3 rotation;
    [SerializeField] private float rotationSpeed;
    /*    [SerializeField] private GameObject shield1;
        [SerializeField] private GameObject shield2;
        [SerializeField] private GameObject shield3;
        [SerializeField] private GameObject shield4;*/

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
        if (health < 700)
        {
            transform.Rotate(rotation * (rotationSpeed * 2) * Time.deltaTime);
            
        }
        else
        {
            if (health < 400)
            {
                gameObject.layer = LayerMask.NameToLayer("Bouncing");
                transform.Rotate(rotation * (rotationSpeed * 2) * Time.deltaTime);
                Debug.Log("Hej och hå");
            }

        }
    }
}
