using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Player
{
    public class Respawn : MonoBehaviour
    {
        [Header("Game Objects")]
        [SerializeField] private GameObject player;
        [SerializeField] private GameObject playerBox;
        [SerializeField] private GameObject Boulder;
        [SerializeField] private GameObject CielingBeneathBoulder;
        [SerializeField] private GameObject BoxBeneathBoulder;
        [Tooltip("make sure both doors are in the same order here as the respawn locations")]
        [SerializeField] private GameObject[] Doors;
        [SerializeField] private GameObject PlacedBox;
        [SerializeField] private GameObject Enemy;
        [SerializeField] private GameObject Key;

        [Header("Images")]
        [SerializeField] private Image KeyImage;

        [Header("Respawn Locations")]
        [SerializeField] private Transform PlayerSpawner;
        [SerializeField] private Transform BoulderSpawner;
        [Tooltip("make sure both doors are in the same order here as the game objects")]
        [SerializeField] private Transform[] DoorsSpawner;

        [Header("Scripts")]
        [Tooltip("Player interaction script that is located in the player game object")]
        [SerializeField] private PlayerInteraction playerInteraction;
        [SerializeField] private UnlocksHandler unlocksHandler;

        public void RespawnPlayer()
        {
            playerInteraction.ResetStats();

            this.player.transform.position = PlayerSpawner.transform.position;
            this.Boulder.transform.position = BoulderSpawner.transform.position;
            this.CielingBeneathBoulder.SetActive(true);
            this.BoxBeneathBoulder.SetActive(true);
            this.PlacedBox.SetActive(false);
            this.Enemy.SetActive(true);
            this.Key.SetActive(false);
            this.playerBox.SetActive(false);
            this.KeyImage.enabled = false;

            unlocksHandler.IsPowerActive[(int)UnlocksHandler.EPowers.Key] = false;

            //TODO reset animations

            for (int i = 0; i < Doors.Length; i++)
            {
                this.Doors[i].transform.position = this.DoorsSpawner[i].transform.position;
            }

            Physics.SyncTransforms();
        }
    }
}