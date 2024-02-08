using UnityEngine;
using System.Collections.Generic;

namespace UnitySampleAssets.Effects {
    public class WaterHoseParticles : MonoBehaviour {
        private List<ParticleCollisionEvent> collisionEvents = new List<ParticleCollisionEvent>(16);

        public static float lastSoundTime;
        public float force = 1;

        private ParticleSystem ps;

        private void OnParticleCollision(GameObject other) {
            if (ps == null)
                ps = GetComponent<ParticleSystem>();
            
            int safeLength = ParticlePhysicsExtensions.GetSafeCollisionEventSize(ps);
            if (collisionEvents.Count < safeLength) {
                collisionEvents = new List<ParticleCollisionEvent>(safeLength);
            }

            int numCollisionEvents = ps.GetCollisionEvents(other, collisionEvents);
            int i = 0;

            while (i < numCollisionEvents)
            {

                if (Time.time > lastSoundTime + 0.2f)
                {
                    lastSoundTime = Time.time;
                }


                Collider col = (Collider) collisionEvents[i].colliderComponent;

                if (col.attachedRigidbody != null)
                {
                    Vector3 vel = collisionEvents[i].velocity;
                    col.attachedRigidbody.AddForce(vel*force, ForceMode.Impulse);
                }

                other.BroadcastMessage("Extinguish", SendMessageOptions.DontRequireReceiver);

                i++;
            }
        }
    }
}