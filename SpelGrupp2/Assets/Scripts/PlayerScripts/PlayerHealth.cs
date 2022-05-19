using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CallbackSystem {
    public class PlayerHealth : MonoBehaviour, IDamageable {
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
        private ChangeColorEvent colorEvent;
        private bool started = false;
        private UIMenus uiMenus;
        [SerializeField] private Material playerMaterial;
        private Color defaultColor;

        public bool IsPlayerOne() { return isPlayerOne; }
        private void Awake() {
            healthEvent = new HealthUpdateEvent();
            colorEvent = new ChangeColorEvent();
            UIEvent = new ActivationUIEvent();
        }
        private void Start() {
            batteryCount = 3;
            movement = GetComponent<PlayerController>();
            attackAbility = GetComponent<PlayerAttack>();
            colorEvent.isPlayerOne = isPlayerOne;
            currHealth = maxHealth;
            uiMenus = GameObject.FindObjectOfType<UIMenus>();
            defaultColor = isPlayerOne ? new Color(0.9f, 0.3f, 0.3f, 1f) : new Color(0.3f, 0.3f, 0.9f, 1f);
            playerMaterial.color = defaultColor;
            colorEvent.color = playerMaterial.color;
            EventSystem.Current.FireEvent(colorEvent);
        }
        private void Update() {
            if (!started) {
                UpdateHealthUI();
                started = true;
            }
            if (alive && currHealth != maxHealth) {
                UpdateHealthUI();
                respawnTimer = 0.0f;
            } else {
                respawnTimer += Time.deltaTime;
            }

            if (!alive && respawnTimer > respawnTime) {
                Respawn();
            }
        }

        public void TakeDamage(float damage) {
            currHealth -= damage;
            if (currHealth <= float.Epsilon && batteryCount > 0) {
                currHealth = maxHealth;
                batteryCount--;
                UpdateHealthUI(true);
            }
            if (currHealth <= 0f && batteryCount == 0) {
                Die();
            }
        }

        public bool Alive {
            get { return alive; }
        }

        public void IncreaseBattery() {
            batteryCount++;
            UpdateHealthUI();
        }

        public void Die() {
            if (alive)
                uiMenus.DeadPlayers(1);
            alive = false;
            attackAbility.Die();
            movement.Die();
            //visuals.SetActive(false);
            UIEvent.isPlayerOne = isPlayerOne;
            UIEvent.isAlive = alive;
            EventSystem.Current.FireEvent(UIEvent);

            AudioController ac = AudioController.instance;
            ac.PlayOneShotAttatched(isPlayerOne ? ac.player1.death : ac.player2.death, gameObject);
        }

        public void Respawn() {
            alive = true;
            currHealth = maxHealth;
            batteryCount = batteryRespawnCount;
            //visuals.SetActive(true);
            UIEvent.isPlayerOne = isPlayerOne;
            UIEvent.isAlive = alive;
            EventSystem.Current.FireEvent(UIEvent);
            attackAbility.Respawn();
            movement.Respawn();
            UpdateHealthUI();
            uiMenus.DeadPlayers(-1);
        }

        private void UpdateHealthUI(bool batteryDecreased = false) {
            if (batteryCount < 1) {
                HealthRegeneration();
            }
            healthEvent.isPlayerOne = isPlayerOne;
            healthEvent.health = currHealth;
            healthEvent.batteries = batteryCount;
            healthEvent.batteryDecreased = batteryDecreased;
            EventSystem.Current.FireEvent(healthEvent);
        }

        private void HealthRegeneration() {
            currHealth += (Time.deltaTime * healthReg * 1f);
            currHealth = Mathf.Min(currHealth, 100f);

        }
        public void ChooseMaterialColor(Color color)
        {
            //Event to update UI color aswell as crosshair/laserSight
            playerMaterial.color = color;
            colorEvent.color = color;
            EventSystem.Current.FireEvent(colorEvent);
        }
        public void ChooseMaterialColor()
        {
            //Event to update UI color aswell as crosshair/laserSight
            playerMaterial.color = defaultColor;
            colorEvent.color = defaultColor;
            EventSystem.Current.FireEvent(colorEvent);
        }
        public Color GetCurrentMaterialColor() { return playerMaterial.color; }

        public float GetCurrenthealth() {
            return currHealth;
        }

        public int GetCurrentBatteryCount() {
            return batteryCount;
        }

        public int GetMaxBatteryCount()
        {
            return maxBatteryCount;
        }
    }
}

