using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gamekit3D
{
    [RequireComponent(typeof(Collider))]
    public class DeathVolume : MonoBehaviour
    {
        public new AudioSource audio;
        public SendToServer sendToServer;

        void OnTriggerEnter(Collider other)
        {
            var pc = other.GetComponent<PlayerController>();
            
            if (pc != null)
            {
                sendToServer.LogPositionOnHitLocal();
                
                var playerDamageable = pc.GetComponent<Damageable>();
                if (playerDamageable != null)
                {

                    // Construct a proper DamageMessage
                    Damageable.DamageMessage damageMessage = new Damageable.DamageMessage
                    {
                        amount = playerDamageable.currentHitPoints, 
                        damageSource = new Vector3(sendToServer.mainPlayer.transform.position.x,
                                                  sendToServer.mainPlayer.transform.position.y + 1,
                                                  sendToServer.mainPlayer.transform.position.z),
                        direction = Vector3.zero,
                        damager = this,
                        throwing = false
                    };


                    pc.Die(damageMessage);
                }
                else
                {
                    Debug.LogError("Player's Damageable component is missing.");
                }

            }
            else
            {
                Debug.LogError("PC is null");
            }

            
            if (audio != null)
            {
                audio.transform.position = other.transform.position;
                if (!audio.isPlaying)
                    audio.Play();
            }
        }

        void Reset()
        {
            if (LayerMask.LayerToName(gameObject.layer) == "Default")
                gameObject.layer = LayerMask.NameToLayer("Environment");
            var c = GetComponent<Collider>();
            if (c != null)
                c.isTrigger = true;
        }

    }
}
