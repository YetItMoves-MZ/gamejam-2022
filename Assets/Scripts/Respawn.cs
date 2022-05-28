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
        [SerializeField] private GameObject InvisibleBox;
        [SerializeField] private GameObject Enemy;
        [SerializeField] private GameObject Key;
        [SerializeField] private GameObject WallBehindCrusher;

        [Header("Images")]
        [SerializeField] private Image KeyImage;

        [Header("Respawn Locations")]
        [SerializeField] private Transform PlayerSpawner;
        [SerializeField] private Transform BoulderSpawner;

        [Header("UI")]
        [SerializeField] private GameObject RespawnUI;

        [Header("Scripts")]
        [Tooltip("Player interaction script that is located in the player game object")]
        [SerializeField] private PlayerInteraction playerInteraction;
        [Tooltip("unlocks handler script that is located in the unlocks handler game object")]
        [SerializeField] private UnlocksHandler unlocksHandler;
        [Tooltip("Descend script that is located in Ceiling")]
        [SerializeField] private Descend CielingDescend;
        [Tooltip("Open door Scripts that are located in Door + Door Crusher")]
        [SerializeField] private OpenDoor[] openDoor;
        [Tooltip("Open cage Script that is located in Cage Pivot")]
        [SerializeField] private OpenCage openCage;

        [Header("Animations")]
        [Tooltip("Player animator")]
        [SerializeField] private Animator PlayerAnimator;
        [Tooltip("Enemy animator")]
        [SerializeField] private Animator EnemyAnimator;


        public void RespawnPlayer()
        {
            this.player.transform.position = PlayerSpawner.transform.position;

            this.CielingBeneathBoulder.SetActive(true);
            this.BoxBeneathBoulder.SetActive(true);
            this.PlacedBox.SetActive(false);
            this.Enemy.SetActive(true);
            this.Key.SetActive(false);
            this.playerBox.SetActive(false);
            this.WallBehindCrusher.SetActive(false);
            this.InvisibleBox.SetActive(true);
            this.KeyImage.enabled = false;

            unlocksHandler.IsPowerActive[(int)UnlocksHandler.EPowers.Key] = false;

            PlayerAnimator.SetFloat("Speed", 0);
            PlayerAnimator.SetBool("isDead", false);

            // BUG revert box animations here

            this.EnemyAnimator.SetBool("isDead", false);
            this.EnemyAnimator.SetBool("canAttack", false);

            CielingDescend.ResetPlacement();
            openCage.ResetPlacement();
            for (int i = 0; i < openDoor.Length; i++)
            {
                openDoor[i].ResetPlacement();
            }

            Physics.SyncTransforms();

            RespawnUI.SetActive(false);

            playerInteraction.ResetStats();

            Cursor.lockState = CursorLockMode.Locked;

        }
    }
}