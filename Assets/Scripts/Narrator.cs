using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Narrator : MonoBehaviour
{
    [Header("Timers")]
    [Tooltip("The time all messages will be shown")]
    [SerializeField] private float TimeForEachMessage;

    [Header("UI")]
    [Tooltip("The UI text of the narrator")]
    [SerializeField] private Text text;
    [Tooltip("The UI of the whole text of the narrator (including its background)")]
    [SerializeField] private GameObject textObject;

    [Header("Messeges")]
    [Tooltip("The narrator first message when the game starts")]
    [SerializeField] private string StartingMessege;

    private float StartTimeForEachMessage = 0;

    private bool IsNarrating = false;

    [HideInInspector]
    public UnityEvent<string> Narrate = new UnityEvent<string>();

    // Start is called before the first frame update
    void Start()
    {
        Narrate.AddListener(Narration);
        Narration(StartingMessege);
    }

    // Update is called once per frame
    void Update()
    {
        if (IsNarrating && TimeForEachMessage < Time.time - StartTimeForEachMessage)
        {
            // text.enabled = false;
            textObject.SetActive(false);
            IsNarrating = false;
        }
    }
    private void Narration(string messege)
    {
        StartTimeForEachMessage = Time.time;
        textObject.SetActive(true);
        // text.enabled = true;
        text.text = messege;
        IsNarrating = true;

        // TODO add sound effects for this?
    }
}
