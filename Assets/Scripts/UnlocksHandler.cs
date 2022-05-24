using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class UnlocksHandler : MonoBehaviour
{
    public enum EPowers
    {
        Default, // Should not occur
        Movement,
        Jump,
        Breath,
        Dash,
        Crawl,
        OpenDoor,
        AttackEnemy,
        PickUpBox,
        PushButton,
        EatingCake
    }

    [Tooltip("starting with all powers unlocked? (used for testing)")]
    [SerializeField] private bool InitAllPowerActive = false;

    [HideInInspector] public bool[] IsPowerActive;

    // Events that will invoked in other places where a power is gained.
    [HideInInspector] public UnityEvent<EPowers> GainedPower = new UnityEvent<EPowers>();

    [Tooltip("time it takes the player to die from not breathing")]
    [SerializeField] private float BreathingTime;
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
    }

    void Update()
    {
        if (!IsPowerActive[(int)EPowers.Breath])
        {
            if (BreathingTime > StartBreathingTime - Time.time)
            {
                // TODO add death UI here.
                // TODO add death animation here for player.

                GainedPowers(EPowers.Breath);
            }
        }
    }

    private void GainedPowers(EPowers power)
    {
        // In case that no power was given and yet the function was called.
        if (power == EPowers.Default)
        {
            Debug.LogWarning("Unkown power was invoked in function GainedPowers in UnlocksHandler.");
            return;
        }

        // Gained Power
        IsPowerActive[(int)power] = true;

        // TODO add power change in UI here
    }
}