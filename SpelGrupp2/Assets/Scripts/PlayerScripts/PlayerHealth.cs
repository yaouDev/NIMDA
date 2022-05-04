using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CallbackSystem
{
    public class PlayerHealth : MonoBehaviour
    {
        [SerializeField] private GameObject visuals;
        [SerializeField] private float respawnTime = 5.0f;

        [SerializeField][Range(0f, 1f)] private float standardRegeneration = 0.25f, safezoneRegeneration = 0.5f;
        [SerializeField] private bool isPlayerOne;
        private float maxHealth = 1f;
        private float healthReg;
        private float currHealth;
        private float respawnTimer;
        [SerializeField] private int batteryCount, batteryRespawnCount, maxBatteryCount;
        private bool alive = true;
        private bool inSafeZone = false;
        private PlayerAttack attackAbility;
        private PlayerController movement;
        private HealthUpdateEvent healthEvent;


        private void Awake()
        {
            healthEvent = new HealthUpdateEvent();
        }
        private void Start()
        {
            batteryCount = 2;
            movement = GetComponent<PlayerController>();
            attackAbility = GetComponent<PlayerAttack>();
            healthReg = standardRegeneration;
            currHealth = 0.75f;
        }

        private void Update()
        {
            if (alive && currHealth != maxHealth)
            {
                UpdateHealthUI();
                respawnTimer = 0.0f;
            }
            else { 
                respawnTimer += Time.deltaTime;
            }

            if (!alive && respawnTimer > respawnTime)
            {
                Respawn();
            }
        }

        public void TakeDamage(float damage)
        {
            currHealth -= damage;
            if(currHealth <= 0f && batteryCount > 0)
            {
                currHealth = maxHealth;
                batteryCount--;
            }
            if (currHealth <= 0f && batteryCount == 0) {
                Die();
            }
        }

        public void Die()
        {
            alive = false;
            attackAbility.Die();
            movement.Die();
            visuals.SetActive(false);
        }

        public void Respawn()
        {
            alive = true;
            currHealth = maxHealth;
            batteryCount = batteryRespawnCount;
            visuals.SetActive(true);
            attackAbility.Respawn();
            movement.Respawn();
            UpdateHealthUI();
        }

        private void UpdateHealthUI()
        {
           UpdateSafeZoneBuff();
            HealthRegeneration();
            healthEvent.isPlayerOne = isPlayerOne;
            healthEvent.health = currHealth;
            healthEvent.batteries = batteryCount;
            EventSystem.Current.FireEvent(healthEvent);
        }
        private void UpdateSafeZoneBuff() // TODO Safe zone regeneration buff 
        {
            healthReg = inSafeZone ? safezoneRegeneration : standardRegeneration;
        }

        private void HealthRegeneration()
        {
            currHealth += (Time.deltaTime * healthReg);
            currHealth = Mathf.Min(currHealth, 1f);

        }

        public void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.tag.Equals("Safezone")) { inSafeZone = true; }
        }

        public void OnCollisionExit(Collision collision)
        {
            if (collision.gameObject.tag.Equals("Safezone")) { inSafeZone = false; }
        }
    }
}

