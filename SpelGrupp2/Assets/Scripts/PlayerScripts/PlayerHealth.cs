using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Callbacks
{
    public class PlayerHealth : MonoBehaviour
    {
        [SerializeField] private GameObject visuals;
        [SerializeField] private float respawnTime = 5.0f;

        private float healthReg, superReg = 2f, standardReg = 1f; // TODO Safe zone regeneration buff 
        private float maxHealth = 1f;
        private float currHealth;
        private float respawnTimer;
        private bool inSafeZone = false;
        private bool alive = true;
        private UnitHealthUpdate healthEvent;

        private void Start()
        {
            currHealth = maxHealth;
            healthEvent = new UnitHealthUpdate();
            EventSystem.Current.FireEvent(healthEvent);
        }

        private void Update()
        {
            if (alive)
            {
                UpdateHealthUI();
                respawnTimer = 0.0f;
            }
            else { respawnTimer += Time.deltaTime; }

            if (!alive && respawnTimer > respawnTime) { Respawn(); }
        }

        public void TakeDamage(float damage)
        {
            currHealth -= damage;
            Debug.Log("Took damage");

            DecreaseBatteryEI decreaseUI = new DecreaseBatteryEI();
            EventSystem.Current.FireEvent(decreaseUI);
            decreaseUI.healthPercentage = currHealth;

            if (currHealth == 0f) { Die(); }
        }

        public void Die()
        {
            alive = false;
            visuals.SetActive(false);
        }

        public void Respawn()
        {
            alive = true;
            visuals.SetActive(true);
            UnitRespawnEI respawnEI = new UnitRespawnEI();
            EventSystem.Current.FireEvent(respawnEI);
            currHealth = maxHealth;
        }

        private void UpdateHealthUI()
        {
            UpdateSafeZoneBuff();
            HealthRegeneration();
            healthEvent.health = currHealth;
        }
        private void UpdateSafeZoneBuff() // TODO Safe zone regeneration buff 
        {
            healthReg = inSafeZone ? superReg : standardReg;
        }

        private void HealthRegeneration()
        {
            currHealth += Time.deltaTime * healthReg;
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

