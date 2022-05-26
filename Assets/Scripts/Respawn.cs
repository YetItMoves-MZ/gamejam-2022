using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Player
{
    public class Respawn : MonoBehaviour
    {
        [SerializeField] private Transform player;
        [SerializeField] private PlayerInteraction playerInteraction;

        public void RespawnPlayer()
        {
            // TODO things that do need to reset after death will be reset here.
            // LIST OF WHAT TO RESET: boulder, cieling beneath boulder, box beanth boulder, both doors, placed box, ork, key
            // note: add list of spawn points for everything

            playerInteraction.ResetStats();

            this.player.transform.position = this.transform.position;
            Physics.SyncTransforms();
        }
    }
}