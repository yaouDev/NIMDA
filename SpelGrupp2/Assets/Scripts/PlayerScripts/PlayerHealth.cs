using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CallbackSystem
{
    public class PlayerHealth : MonoBehaviour, IDamageable
    {
        [SerializeField] private GameObject visuals;
        [SerializeField] private float respawnTime = 10.0f;
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
        private Crafting crafting;
        private ActivationUIEvent UIEvent;
        private ChangeColorEvent colorEvent;
        private bool started = false, decreaseDamageUpgrade;
        private UIMenus uiMenus;
        [SerializeField] private Material playerMaterial;
        private Color defaultColor;

        private AudioController ac;

        public bool IsPlayerOne() { return isPlayerOne; }
        private void Awake()
        {
            healthEvent = new HealthUpdateEvent();
            colorEvent = new ChangeColorEvent();
            UIEvent = new ActivationUIEvent();
        }
        private void Start()
        {
            //Cursor.visible = false;
            batteryCount = 3;
            crafting = GetComponent<Crafting>();
            movement = GetComponent<PlayerController>();
            attackAbility = GetComponent<PlayerAttack>();
            colorEvent.isPlayerOne = isPlayerOne;
            currHealth = maxHealth;
            uiMenus = GameObject.FindObjectOfType<UIMenus>();
            defaultColor = isPlayerOne ? new Color(0.9f, 0.3f, 0.3f, 1f) : new Color(0.3f, 0.3f, 0.9f, 1f);
            playerMaterial.color = defaultColor;
            colorEvent.color = playerMaterial.color;
            //EventSystem.Current.FireEvent(colorEvent);
            ac = AudioController.instance;
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
                //if(enemyDmg)
                ac.PlayOneShotAttatched(IsPlayerOne() ? ac.player1.hurt : ac.player2.hurt, gameObject);
                //else
                //enemy damage sound here
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

        public bool Alive
        {
            get { return alive; }
        }

        public void IncreaseBattery()
        {
            batteryCount++;
            UpdateHealthUI();
        }

        public void SetBatteriesOnLoad(int amount){
            batteryCount = amount;
        }

        public void SetHealthOnLoad(float amount){
            currHealth = amount;
        }

        public void Die()
        {
            if (alive)
            {
                uiMenus.DeadPlayers(1);
                crafting.BisectResources();
            }
            alive = false;
            attackAbility.Die();
            movement.Die();
            //visuals.SetActive(false);
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
            //visuals.SetActive(true);
            UIEvent.isPlayerOne = isPlayerOne;
            UIEvent.isAlive = alive;
            EventSystem.Current.FireEvent(UIEvent);
            attackAbility.Respawn();
            movement.Respawn();
            UpdateHealthUI();
            uiMenus.DeadPlayers(-1);
        }

        public void SetNewHealth(float value)
        {
            maxHealth = value;
            currHealth = maxHealth;
            UpdateHealthUI();
        }

        //TODO Hedén
        public void SetDefaultStats()
        {
            maxHealth = 100f;
            currHealth = (currHealth > maxHealth) ? maxHealth : currHealth;
            movement.SetDefaultMovementSpeed();
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
            playerMaterial.color = color;
            colorEvent.color = color;
            EventSystem.Current.FireEvent(colorEvent);
        }
        public void ChooseMaterialColor()
        {
            playerMaterial.color = defaultColor;
            colorEvent.color = defaultColor;
            EventSystem.Current.FireEvent(colorEvent);
        }
        public Color GetCurrentMaterialColor() { return playerMaterial.color; }

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

        public bool DecreaseDamageUpgraded
        {
            get { return decreaseDamageUpgrade; }
            set { decreaseDamageUpgrade = value; }
        }
    }
}

