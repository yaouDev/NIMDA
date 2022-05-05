using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CallbackSystem
{
    // public fields are supposed to start with capital letters
    public abstract class Event
    {
        public GameObject GameObject;
        public bool isPlayerOne;
    }

    public class DebugEvent : Event
    {
        public string DebugText;
    }

    public class DieEvent : DebugEvent
    {
        public AudioClip DeathSound;
        public float TimeToDestroy;
        public List<ParticleSystem> ParticleSystems;
        public Renderer Renderer;
    }

    public class HealthUpdateEvent : Event
    {
        public float health;
        public int batteries;

    }

    public class RespawnEvent : Event
    {

    }

    public class CameraPosUpdateEvent : Event
    {
        public Vector3 pos;
    }

    public class ResourceUpdateEvent : Event
    {
        public int c, t, i;
    }

    public class UpdateUIEvent : Event
    {
        public bool isAlive;

    }

    /// <summary>
    /// Walls is an integer between 0-15 representing
    /// all possible combinations of wall configurations.
    /// Bitshifting is involved.
    /// 8 | N - wall to N
    /// 4 | S - wall to S
    /// 2 | E - wall to E
    /// 1 | W - wall to W
    /// </summary>
    public class ModuleSpawnEvent : Event
    {
        public Vector2Int Position;
        public int Walls;
    }
    
    public class ModuleDeSpawnEvent : Event
    {
        public Vector2Int Position;
        public int Walls;
    }
}