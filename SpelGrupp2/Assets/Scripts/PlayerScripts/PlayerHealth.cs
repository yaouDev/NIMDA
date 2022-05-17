using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CallbackSystem
{
    public class PlayerHealth : MonoBehaviour, IDamageable
    {
        [SerializeField] private GameObject visuals;
        [SerializeField] private float respawnTime = 5.0f;
        [SerializeField] private bool isPlayerOne;
        [SerializeField] private int batteryCount, batteryRespawnCount, maxBatteryCount;
        [SerializeField] private float healthReg;
        private float maxHealth = 100f;
        private float currHealth;
        private float respawnTimer;
        private bool alive = true;
        private PlayerAttack attackAbility;
        private PlayerController movement;
        private HealthUpdateEvent healthEvent;
        private ActivationUIEvent UIEvent;
        private bool started = false;

        public bool IsPlayerOne() { return isPlayerOne; }
        private void Awake()
        {
            healthEvent = new HealthUpdateEvent();
            UIEvent = new ActivationUIEvent();
        }
        private void Start()
        {
            batteryCount = 3;
            movement = GetComponent<PlayerController>();
            attackAbility = GetComponent<PlayerAttack>();
            currHealth = maxHealth;
        }
        private void Update()
        {
            if (!started)
            {
                UpdateHealthUI();
                started = true;
            }
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
            if(currHealth <= float.Epsilon && batteryCount > 0)
            {
                currHealth = maxHealth;
                batteryCount--;
                UpdateHealthUI(true);
            }
            if (currHealth <= 0f && batteryCount == 0) {
                Die();
            }
        }

        public void IncreaseBattery()
        {
            batteryCount++;
            UpdateHealthUI();
        }

        public void Die()
        {
            alive = false;
            attackAbility.Die();
            movement.Die();
            visuals.SetActive(false);
            UIEvent.isPlayerOne = isPlayerOne;
            UIEvent.isAlive = alive;
            EventSystem.Current.FireEvent(UIEvent);

            AudioController ac = AudioController.instance;
            ac.PlayOneShotAttatched(isPlayerOne ? ac.player1.death : ac.player2.death, gameObject);
        }

        public void Respawn()
        {
            alive = true;
            currHealth = maxHealth;
            batteryCount = batteryRespawnCount;
            visuals.SetActive(true);
            UIEvent.isPlayerOne = isPlayerOne;
            UIEvent.isAlive = alive;
            EventSystem.Current.FireEvent(UIEvent);
            attackAbility.Respawn();
            movement.Respawn();
            UpdateHealthUI();
        }

        private void UpdateHealthUI(bool batteryDecreased = false)
        {
            if (batteryCount < 1)
            {
                HealthRegeneration();
            }            
            healthEvent.isPlayerOne = isPlayerOne;
            healthEvent.health = currHealth;
            healthEvent.batteries = batteryCount;
            healthEvent.batteryDecreased = batteryDecreased;
            EventSystem.Current.FireEvent(healthEvent);
        }

        private void HealthRegeneration()
        {
            currHealth += (Time.deltaTime * healthReg * 1f);
            currHealth = Mathf.Min(currHealth, 100f);

        }

        public float ReturnHealth()
        {
            return currHealth;
        }

        public int ReturnBatteries()
        {
            return batteryCount;
        }
    }
}

