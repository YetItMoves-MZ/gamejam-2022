using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenCage : MonoBehaviour
{
    private Quaternion OriginalLocalRotation;

    private float MaxMove;

    [SerializeField]
    private bool IsOpening = false;

    // Start is called before the first frame update
    void Start()
    {
        this.MaxMove = 0.9f;
        this.OriginalLocalRotation = transform.localRotation;
    }

    // Update is called once per frame
    void Update()
    {
        if (
            IsOpening &&
            transform.localRotation.y <= MaxMove &&
            transform.localRotation.y >= 0
        )
        {
            this.Move();
        }
    }

    private void Move()
    {
        transform
            .RotateAround(transform.position,
            transform.up,
            Time.deltaTime * 90f);
    }

    public void StartOpenning()
    {
        this.IsOpening = true;
    }

    public void ResetPlacement()
    {
        this.IsOpening = false;
        transform.localRotation = this.OriginalLocalRotation;
    }
}
