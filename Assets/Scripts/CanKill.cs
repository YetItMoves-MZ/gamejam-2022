using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanKill : MonoBehaviour
{
    [Header("Defining Features")]
    [Tooltip("What power is unlocked when the player dies by this object")]
    [SerializeField] private UnlocksHandler.EPowers GivingPower;

    [Tooltip("Is the object killing when using it? (true) or being in its collider will just kill the player (false)")]
    [SerializeField] private bool interactable;

    [Header("Scripts")]
    [Tooltip("The UnlockHandler script that is in UnlockHandler object")]
    [SerializeField] private UnlocksHandler unlocksHandler;

    [Header("Animators")]
    [Tooltip("Player animator")]
    [SerializeField] private Animator animator;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {

            if (!interactable)
            {
                // TODO add death UI here.
                // TODO add death animation here for player.
                unlocksHandler.GainedPower.Invoke(GivingPower);
            }
        }
    }




}
