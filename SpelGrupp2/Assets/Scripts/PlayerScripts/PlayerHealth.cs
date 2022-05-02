using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Callbacks
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
        private bool alive = true;
        private bool inSafeZone = false;
        //private GameObject[] players;
        //private GameObject otherPlayer;
        private PlayerAttack attackAbility;
        private PlayerController movement;
        //private Vector3 freezeTransformAt;


        private void Start()
        {
            movement = GetComponent<PlayerController>();
            attackAbility = GetComponent<PlayerAttack>();
            //players = GameObject.FindGameObjectsWithTag("Player");
            //otherPlayer = players[0] != gameObject ? players[0] : players[1];
            //currHealth = maxHealth;
            healthReg = standardRegeneration;
            currHealth = 0.1f;
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
            if (currHealth <= 0f) {
                Die();
            }
        }

        public void Die()
        {
            //freezeTransformAt = otherPlayer.transform.position + Vector3.left;
            alive = false;
            attackAbility.Die();
            movement.Die();
            visuals.SetActive(false);
        }

        public void Respawn()
        {
            alive = true;
            currHealth = maxHealth;
            visuals.SetActive(true);
            attackAbility.Respawn();
            movement.Respawn();
            UpdateHealthUI();
        }

        private void UpdateHealthUI()
        {
           UpdateSafeZoneBuff();
            HealthRegeneration();
            UnitHealthUpdate healthEvent = new UnitHealthUpdate();
            healthEvent.isGOPlayerOne = isPlayerOne;
            healthEvent.health = currHealth;
            EventSystem.Current.FireEvent(healthEvent);
        }
        private void UpdateSafeZoneBuff() // TODO Safe zone regeneration buff 
        {
            healthReg = inSafeZone ? safezoneRegeneration : standardRegeneration;
        }

        private void HealthRegeneration()
        {
            //Time.deltaTime caused gap between 0.14->0.47 ???
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

