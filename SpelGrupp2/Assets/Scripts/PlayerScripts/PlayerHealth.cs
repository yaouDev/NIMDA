using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CallbackSystem
{
    public class PlayerHealth : MonoBehaviour, IDamageable
    {
        [SerializeField] private float respawnTime = 10.0f;
        [SerializeField] private bool isPlayerOne;
        [SerializeField] private int batteryCount, batteryRespawnCount, maxBatteryCount;
        [SerializeField] private float healthReg;
        [SerializeField] private GameObject visuals;
        private float maxHealth = 100f;
        private float currHealth;
        private float respawnTimer;
        private bool alive = true;
        private bool started = false, decreaseDamageUpgrade;
        private UIMenus uiMenus;
        private PlayerAttack attackAbility;
        private PlayerController movement;
        private Crafting crafting;
        private Color defaultColor, currentColor;
        private Blink blink;
        private AudioController ac;
        private ChangeColorEvent colorEvent;
        private HealthUpdateEvent healthEvent;
        private ActivationUIEvent UIEvent;


        public bool IsPlayerOne() { return isPlayerOne; }
        private void Awake()
        {
            healthEvent = new HealthUpdateEvent();
            colorEvent = new ChangeColorEvent();
            UIEvent = new ActivationUIEvent();
        }
        private void Start()
        {
            batteryCount = 3;
            crafting = GetComponent<Crafting>();
            movement = GetComponent<PlayerController>();
            attackAbility = GetComponent<PlayerAttack>();
            blink = GetComponent<Blink>();
            colorEvent.isPlayerOne = isPlayerOne;
            currHealth = maxHealth;
            uiMenus = GameObject.FindObjectOfType<UIMenus>();
            defaultColor = isPlayerOne ? new Color(0.3f, 0.9f, 0.3f, 1f) : new Color(0.3f, 0.3f, 0.9f, 1f);
            ac = AudioController.instance;
            currentColor = defaultColor;
        }
        private void Update()
        {
            if (!started)
            {
                ChooseMaterialColor();
                UpdateHealthUI();
                started = true;
            }
            if (alive && currHealth != maxHealth)
            {
                UpdateHealthUI();
                respawnTimer = 0.0f;
            }
            else
            {
                respawnTimer += Time.deltaTime;
            }

            if (!alive && respawnTimer > respawnTime)
            {
                Respawn();
            }
        }

        public void TakeDamage(float damage)
        {
            currHealth -= decreaseDamageUpgrade ? damage * 0.5f : damage;
            if (alive)
            {
                CallbackSystem.CameraShakeEvent shakeEvent = new CameraShakeEvent();
                shakeEvent.affectsPlayerOne = isPlayerOne;
                shakeEvent.affectsPlayerTwo = !isPlayerOne;
                float magnitude = Mathf.Max(.28f, damage * .01f);
                shakeEvent.magnitude = magnitude;
                EventSystem.Current.FireEvent(shakeEvent);
                ac.PlayOneShotAttatched(IsPlayerOne() ? ac.player1.hurt : ac.player2.hurt, gameObject);
            }

            if (currHealth <= float.Epsilon && batteryCount > 0)
            {
                currHealth = maxHealth;
                batteryCount--;
                ac.PlayOneShotAttatched(IsPlayerOne() ? ac.player1.batteryDelpetion : ac.player2.batteryDelpetion, gameObject);
                UpdateHealthUI(true);
            }
            if (currHealth <= 0f && batteryCount == 0)
            {
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
            if (alive)
            {
                uiMenus.DeadPlayers(1);
                crafting.BisectResources();
                UpdateHealthUI();
            }
            alive = false;
            attackAbility.Die();
            movement.Die();
            UIEvent.isPlayerOne = isPlayerOne;
            UIEvent.isAlive = alive;
            EventSystem.Current.FireEvent(UIEvent);
            ac.PlayOneShotAttatched(isPlayerOne ? ac.player1.death : ac.player2.death, gameObject);
        }

        public void Respawn()
        {
            alive = true;
            currHealth = maxHealth;
            batteryCount = batteryRespawnCount;
            UIEvent.isPlayerOne = isPlayerOne;
            UIEvent.isAlive = alive;
            EventSystem.Current.FireEvent(UIEvent);
            attackAbility.Respawn();
            movement.Respawn();
            UpdateHealthUI();
            uiMenus.DeadPlayers(-1);
        }

        public void SetDefaultStats()
        {
            DecreaseDamageUpgraded = false;
            movement.MovementSpeedUpgraded = false;
            blink.BlinkUpgraded = false;
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
            currHealth = Mathf.Min(currHealth, maxHealth);

        }
        
        public void ChooseMaterialColor(Color color)
        {
            colorEvent.color = color;
            EventSystem.Current.FireEvent(colorEvent);
        }

        public void ChooseMaterialColor()
        {
            colorEvent.color = defaultColor;
            EventSystem.Current.FireEvent(colorEvent);
        }

        public Color GetCurrentMaterialColor()
        {
            return currentColor; }

        public float GetCurrenthealth()
        {
            return currHealth;
        }

        public float GetMaxHealth() { return maxHealth; }

        public void SetCurrentHealth(float value)
        {
            currHealth = value;
            UpdateHealthUI();
        }

        public int GetCurrentBatteryCount()
        {
            return batteryCount;
        }

        public int GetMaxBatteryCount()
        {
            return maxBatteryCount;
        }

        public void DecreaseDamageUpgrade() => DecreaseDamageUpgraded = true;

        public void SetBatteriesOnLoad(int amount)
        {
            batteryCount = amount;
        }

        public void SetHealthOnLoad(float amount)
        {
            currHealth = amount;
        }

        public bool DecreaseDamageUpgraded
        {
            get { return decreaseDamageUpgrade; }
            set { decreaseDamageUpgrade = value; }
        }

        public bool Alive
        {
            get { return alive; }
        }
    }
}

