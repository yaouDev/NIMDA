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
        private Crafting crafting;
        private ActivationUIEvent UIEvent;
        private ChangeColorEvent colorEvent;
        private bool started = false;
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
            currHealth -= damage;
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

        public bool Alive
        {
            get { return alive; }
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
                //DropLoot();
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
        }

        public void SetDefaultStats()
        {
            maxHealth = 100f;
            currHealth = (currHealth > maxHealth) ? maxHealth : currHealth;
            movement.SetDefaultVelocity();
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

        private Vector3 dropOffset;
        private GameObject drop;
        [SerializeField] private GameObject[] dropTable = new GameObject[3];
        /*
        public void DropLoot()
        {
            dropOffset = new Vector3(Random.Range(-1f, 1f), 1f, Random.Range(-1f, 1f));
            //dropOffset = Random.onUnitSphere * 2f;
            dropOffset.y = Mathf.Abs(dropOffset.y);
            for(int index = 0; index < 3; index++)
            {
                for (int i = 0; i < crafting.GetHalfResourceAmount(index); i++)
                {
                    drop = dropTable[index];
                    GameObject loot = Instantiate(drop, transform.position + dropOffset, Quaternion.identity);
                    Destroy(loot, 15f);
                }
                crafting.ReduceResourceByHalf(index);
            }
            crafting.UpdateResources();
        }
        */
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
    }
}

