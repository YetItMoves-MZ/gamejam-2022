using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UnlocksHandler : MonoBehaviour
{
    public enum EPowers
    {
        Default, // Should not occur
        Movement,
        Breath,
        Jump,
        Crawl,
        PushButton,
        Dash,
        PickUpBox,
        AttackEnemy,
        Key,
        EatingCake
    }

    [Header("Debug")]
    [Tooltip("starting with all powers unlocked? (used for testing)")]
    [SerializeField] private bool InitAllPowerActive = false;

    [Header("UI")]
    [Tooltip("insert them in order!")]
    [SerializeField] private Image[] DisabledPowersUI;
    [Tooltip("insert them in order!")]
    [SerializeField] private Image[] EnabledPowersUI;

    [Tooltip("The narrator script in the narrator game object")]

    [Header("Narrator")]
    [SerializeField] private Narrator narrator;
    [Tooltip("The list of all messeges the narrator say when the player gain a power. (Have to be in the following order: Movement, Breath, Jump, Crawl, PushButton, Dash, PickUpBox, AttackEnemy, Key, EatingCake)")]
    [SerializeField] private string[] NarratorMesseges;


    [HideInInspector] public bool[] IsPowerActive;

    // Events that will invoked in other places where a power is gained.
    [HideInInspector] public UnityEvent<EPowers> GainedPower = new UnityEvent<EPowers>();
    [HideInInspector] public float StartBreathingTime = 0;


    // Start is called before the first frame update
    void Start()
    {
        // Initialize array with starting value according to InitAllPowerActive
        IsPowerActive = new bool[(int)EPowers.EatingCake + 1];
        for (int i = 0; i < (int)EPowers.EatingCake + 1; i++)
        {
            IsPowerActive[i] = InitAllPowerActive;
        }

        GainedPower.AddListener(GainedPowers);
        StartBreathingTime = Time.time;
    }

    private void GainedPowers(EPowers power)
    {
        Debug.Log("New power get! " + power);
        // In case that no power was given and yet the function was called.
        if (power == EPowers.Default)
        {
            Debug.LogWarning("Unkown power was invoked in function GainedPowers in UnlocksHandler.");
            return;
        }

        // Gained Power
        IsPowerActive[(int)power] = true;

        DisabledPowersUI[(int)power - 1].enabled = false;
        EnabledPowersUI[(int)power - 1].enabled = true;
        narrator.Narrate.Invoke(NarratorMesseges[(int)power - 1]);
    }
}