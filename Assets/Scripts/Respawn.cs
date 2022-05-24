using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Player
{
    public class RespawnScript : MonoBehaviour
    {
        [SerializeField] private Transform player;
        [SerializeField] private Transform respawnPoint;

        public void RespawnPlayer()
        {
            // TODO things that do need to reset after death will be reset here.


            this.player.transform.position = this.respawnPoint.transform.position;
            Physics.SyncTransforms();
        }
    }
}