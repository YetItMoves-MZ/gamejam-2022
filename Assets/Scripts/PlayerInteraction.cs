using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{

    [Header("Timers")]
    [Tooltip("The time it takes for the death animation of the enemy")]
    [SerializeField] private float EnemyDeathTime;
    [Tooltip("The time it takes for the enemy attacking animation to finish")]
    [SerializeField] private float EnemyAttackTime;
    [Tooltip("The time it takes from the triggered death to the respawn UI")]
    [SerializeField] private float PlayerDeathTime;
    [Tooltip("The time it takes for the box to expload")]
    [SerializeField] private float BoxExplosionTime;
    [Tooltip("time it takes the player to die from not breathing")]
    [SerializeField] private float BreathingTime;

    [Header("Game Objects")]
    [Tooltip("The game object of the key")]
    [SerializeField] private GameObject Key;
    [Tooltip("The game object of the box that the player holds")]
    [SerializeField] private GameObject PlayerBox;
    [Tooltip("The game object of the almost invisible box that indicates where to place the box")]
    [SerializeField] private GameObject InvisibleBox;
    [Tooltip("The game object of the box that will be placed by the player")]
    [SerializeField] private GameObject PlacedBox;
    [Tooltip("game object that contains the box with button, the boxes near it and the wall behind it")]
    [SerializeField] private GameObject BoxWithButtonParent;
    [Tooltip("The game object of the explosion animation")]
    [SerializeField] private GameObject Explosion;


    [Header("UI")]
    [Tooltip("The respawn UI game object")]
    [SerializeField] private GameObject RespawnUI;
    [Tooltip("The win game UI game object")]
    [SerializeField] private GameObject WinGameUI;

    [Header("Animators")]
    [Tooltip("Enemy animator")]
    [SerializeField] private Animator EnemyAnimator;
    [Tooltip("Player animator")]
    [SerializeField] private Animator PlayerAnimator;

    [Header("Scripts")]
    [Tooltip("The unlocks handler script located in unlocks handler game object")]
    [SerializeField] private UnlocksHandler unlocksHandler;
    [Tooltip("Descend script that is located in Ceiling")]
    [SerializeField] private Descend CielingDescend;
    [Tooltip("Open door Scripts that are located in Door + Door Crusher")]
    [SerializeField] private OpenDoor[] openDoor;
    [Tooltip("Open cage Script that is located in Cage Pivot")]
    [SerializeField] private OpenCage openCage;

    // When did the enemy die
    private float StartEnemyDeathTime;
    private float StartEnemyAttackTime;
    private float StartPlayerDeathTime;
    private float StartBoxExplosionTime;
    private float StartBreathingTime;
    private GameObject Enemy;
    private bool IsEnemyDead = false;
    public bool IsPlayerDead = false;
    private bool IsEnemyAttacking = false;
    private bool IsBoxEploading = false;
    [HideInInspector] public bool killedOnlyOnce = false;

    [HideInInspector] public bool holdBox = false;

    private void Update()
    {
        HandleEnemyDeath();
        HandlePlayerDeath();
        HandleEnemyAttack();
        HandleBoxExplosion();
        HandlePlayerBreathing();
    }

    private void Start()
    {
        StartBreathingTime = Time.time;
    }
    private void OnTriggerStay(Collider other)
    {
        GameObject otherObject = other.gameObject;
        switch (otherObject.tag)
        {
            // special case of player death, also need to see the enemy attacking animation first.
            case "Enemy":
                this.IsEnemyAttacking = true;
                this.StartEnemyAttackTime = Time.time;
                this.EnemyAnimator.SetBool("CanAttack", true);
                break;

            case "BoxWithButton":
                if (unlocksHandler.IsPowerActive[(int)UnlocksHandler.EPowers.PushButton] && Input.GetAxis("Use") != 0)
                {
                    this.IsBoxEploading = true;
                    this.StartBoxExplosionTime = Time.time;
                    Explosion.SetActive(true);
                }
                break;

            // all cases that player will die
            case "Blade":
                PlayerKilled(UnlocksHandler.EPowers.Crawl);
                break;
            case "Anvil":
                PlayerKilled(UnlocksHandler.EPowers.Movement);
                break;
            case "Spike":
                PlayerKilled(UnlocksHandler.EPowers.Jump);
                break;
            case "Ceiling":
                PlayerKilled(UnlocksHandler.EPowers.PushButton);
                break;
            case "Boulder":
                PlayerKilled(UnlocksHandler.EPowers.Dash);
                break;
            // cieling is going down
            case "CrusherTrigger":
                CielingDescend.StartDesending();
                break;
            case "BoxPlacement":
                if (holdBox == true && Input.GetAxis("Use") != 0)
                {
                    holdBox = false;
                    PlayerBox.SetActive(false);
                    InvisibleBox.SetActive(false);
                    PlacedBox.SetActive(true);
                }
                break;
            case "HitEnemy":
                if (unlocksHandler.IsPowerActive[(int)UnlocksHandler.EPowers.AttackEnemy] && Input.GetAxis("Use") != 0)
                {
                    EnemyAnimator.SetBool("isDead", true);

                    StartEnemyDeathTime = Time.time;
                    Enemy = otherObject;
                    IsEnemyDead = true;
                }
                break;
            case "Key":
                if (Input.GetAxis("Use") != 0)
                {
                    otherObject.SetActive(false);
                    unlocksHandler.GainedPower.Invoke(UnlocksHandler.EPowers.Key);
                }
                break;
            case "Box":
                if (unlocksHandler.IsPowerActive[(int)UnlocksHandler.EPowers.PickUpBox] && Input.GetAxis("Use") != 0)
                {
                    PlayerBox.SetActive(true);
                    otherObject.SetActive(false);
                    holdBox = true;
                }
                break;
            case "Door":
                if (unlocksHandler.IsPowerActive[(int)UnlocksHandler.EPowers.PushButton] && Input.GetAxis("Use") != 0)
                {
                    for (int i = 0; i < openDoor.Length; i++)
                    {
                        openDoor[i].StartOpenning();
                    }
                }
                break;
            case "Cake":
                if (unlocksHandler.IsPowerActive[(int)UnlocksHandler.EPowers.EatingCake] && Input.GetAxis("Use") != 0)
                {
                    WinGameUI.SetActive(true);

                    // to prevent player from moving. also he is actualy dead in narative to i guess that checks out :P
                    IsPlayerDead = true;

                }
                break;
            case "Cage":
                if (unlocksHandler.IsPowerActive[(int)UnlocksHandler.EPowers.Key] && Input.GetAxis("Use") != 0)
                {
                    openCage.StartOpenning();
                }
                break;
            default:
                break;
        }
    }

    private void HandleEnemyDeath()
    {
        if (EnemyDeathTime < Time.time - StartEnemyDeathTime && IsEnemyDead)
        {
            Enemy.SetActive(false);
            Key.SetActive(true);
        }
    }
    private void HandlePlayerDeath()
    {
        if (PlayerDeathTime < Time.time - StartPlayerDeathTime && IsPlayerDead)
        {
            RespawnUI.SetActive(true);
        }
    }

    private void HandleEnemyAttack()
    {
        if (EnemyAttackTime < Time.time - StartEnemyAttackTime && IsEnemyAttacking)
        {
            PlayerKilled(UnlocksHandler.EPowers.AttackEnemy);
        }

    }

    private void HandleBoxExplosion()
    {
        if (BoxExplosionTime < Time.time - StartBoxExplosionTime && IsBoxEploading)
        {
            Explosion.SetActive(false);
            PlayerKilled(UnlocksHandler.EPowers.PickUpBox);
            this.BoxWithButtonParent.SetActive(false);
        }
    }

    private void HandlePlayerBreathing()
    {
        if (!unlocksHandler.IsPowerActive[(int)UnlocksHandler.EPowers.Breath] && unlocksHandler.IsPowerActive[(int)UnlocksHandler.EPowers.Movement])
        {
            if (BreathingTime < Time.time - StartBreathingTime)
            {
                PlayerKilled(UnlocksHandler.EPowers.Breath);
            }
        }
    }

    private void PlayerKilled(UnlocksHandler.EPowers unlockedPower)
    {
        if (!killedOnlyOnce)
        {
            killedOnlyOnce = true;
            PlayerAnimator.SetBool("isDead", true);

            this.StartPlayerDeathTime = Time.time;
            this.IsPlayerDead = true;

            unlocksHandler.GainedPower.Invoke(unlockedPower);

            Cursor.lockState = CursorLockMode.None;
        }
    }

    public void ResetStats()
    {
        IsEnemyDead = false;
        IsPlayerDead = false;
        IsEnemyAttacking = false;

        holdBox = false;
        killedOnlyOnce = false;

        StartBreathingTime = Time.time;
    }
}
