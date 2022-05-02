using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Callbacks
{
    public class PlayerDeathListener : MonoBehaviour
    {
        private PlayerAttack[] players;
        PlayerHealth health;
        PlayerAttack player;
        PlayerController movement;
        void OnEnable()
        {
            players = FindObjectsOfType<PlayerAttack>();
            EventSystem.Current.RegisterListener<UnitDeathEI>(PlayerDeath);
        }

        private void PlayerDeath(UnitDeathEI dei)
        {
            player = dei.isGOPlayerOne ? players[0] : players[1];
            health = player.GetComponent<PlayerHealth>();
            movement = player.GetComponent<PlayerController>();
            player.Die();
            health.Die();
            movement.Die();
            Debug.Log(player + " died");

        }
    }
}

