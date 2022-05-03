using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CallbackSystem {
    // public fields are supposed to start with capital letters
    public abstract class Event {
        public GameObject GameObject;
    }

    public class DebugEvent : Event {
        public string DebugText;
    }

    public class DieEvent : DebugEvent {
        public AudioClip DeathSound;
        public float TimeToDestroy;
        public List<ParticleSystem> ParticleSystems;
        public Renderer Renderer;

    }

}


