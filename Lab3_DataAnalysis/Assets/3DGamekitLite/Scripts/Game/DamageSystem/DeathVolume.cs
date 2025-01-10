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
                    //como el damage message de acido no llega bien a server, lo mandaremos directo
                    sendToServer.LogHit(
                        new Vector3(sendToServer.mainPlayer.transform.position.x,
                                                  sendToServer.mainPlayer.transform.position.y + 1,
                                                  sendToServer.mainPlayer.transform.position.z),
                        Vector3.zero,
                        this,
                        5,
                        false,
                        true
                        );

                    pc.Die(damageMessage);
                }

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
