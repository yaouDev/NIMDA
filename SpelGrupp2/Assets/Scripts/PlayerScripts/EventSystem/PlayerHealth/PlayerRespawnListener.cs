using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Callbacks
{
    public class PlayerRespawnListener : MonoBehaviour
    {
        private PlayerAttack[] players;
        private PlayerHealth health;
        private PlayerAttack player;
        private PlayerController movement;

        void Start()
        {
            players = FindObjectsOfType<PlayerAttack>();
            EventSystem.Current.RegisterListener<UnitRespawnEI>(RespawnPlayer);
        }

        void RespawnPlayer(UnitRespawnEI urei)
        {
            player = urei.isGOPlayerOne ? players[0] : players[1];
            health = player.GetComponent<PlayerHealth>();
            movement = player.GetComponent<PlayerController>();
            player.Respawn();
            health.Respawn();
            movement.Respawn();
        }
    }
}
