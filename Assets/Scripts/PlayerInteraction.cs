using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    [SerializeField] private UnlocksHandler unlocksHandler;
    [SerializeField] private GameObject Key;
    [SerializeField] private Animator EnemyAnimator;
    [SerializeField] private float EnemyDeathtime;
    [SerializeField] private GameObject PlayerBox;

    // When did the enemy die
    private float EnemyDiedTime = -5;
    private GameObject Enemy;

    [HideInInspector] public bool holdBox = false;

    private void Start()
    {
        EnemyDiedTime = -EnemyDeathtime;
    }
    private void Update()
    {
        HandleEnemyDeath();
    }
    private void OnTriggerStay(Collider other)
    {
        GameObject otherObject = other.gameObject;
        switch (otherObject.tag)
        {
            case "Enemy":
                if (unlocksHandler.IsPowerActive[(int)UnlocksHandler.EPowers.AttackEnemy] && Input.GetAxis("Use") != 0)
                {
                    //TODO insert enemy death animation here

                    EnemyDiedTime = Time.time;
                    Enemy = otherObject;

                    otherObject.SetActive(false);
                    Key.SetActive(true);
                }
                break;
            case "Key":
                if (Input.GetAxis("Use") != 0)
                {
                    otherObject.SetActive(false);
                    unlocksHandler.GainedPower.Invoke(UnlocksHandler.EPowers.Key);
                }
                break;
            case "BoxWithButton":
                if (unlocksHandler.IsPowerActive[(int)UnlocksHandler.EPowers.PushButton] && Input.GetAxis("Use") != 0)
                {
                    otherObject.SetActive(false);
                    // TODO add explosion animation here

                    unlocksHandler.GainedPower.Invoke(UnlocksHandler.EPowers.PickUpBox);
                    // TODO add death UI here.
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
                    // TODO insert door animation here.

                }
                break;
            case "Cake":
                if (unlocksHandler.IsPowerActive[(int)UnlocksHandler.EPowers.EatingCake] && Input.GetAxis("Use") != 0)
                {
                    // TODO add win game here.

                }
                break;
            case "Cage":
                if (unlocksHandler.IsPowerActive[(int)UnlocksHandler.EPowers.Key] && Input.GetAxis("Use") != 0)
                {
                    // TODO insert cage animation here.
                }
                break;
            default:
                break;
        }
    }

    private void HandleEnemyDeath()
    {
        if (EnemyDiedTime > Time.time - EnemyDeathtime)
        {
            Enemy.SetActive(false);
            Key.SetActive(true);
        }
    }
}
