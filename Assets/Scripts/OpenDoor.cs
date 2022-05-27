using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenDoor : MonoBehaviour
{
    private Vector3 OriginalPosition;

    private Vector3 OriginalLocalPosition;

    private float MaxMove;

    [SerializeField]
    private bool IsOpening = false;

    // [SerializeField]
    private float Speed = 0.1f;

    // Start is called before the first frame update
    void Start()
    {
        this.OriginalPosition = transform.position;
        this.OriginalLocalPosition = transform.localPosition;
        this.MaxMove = transform.localScale.x * 4;
    }

    // Update is called once per frame
    void Update()
    {
        if (
            IsOpening &&
            transform.localPosition.x < (this.OriginalLocalPosition.x + MaxMove)
        )
        {
            this.Move();
        }
    }

    private void Move()
    {
        transform.localPosition =
            new Vector3((transform.localPosition.x + this.Speed),
                transform.localPosition.y,
                transform.localPosition.z);
    }

    public void StartOpenning()
    {
        this.IsOpening = true;
    }

    public void ResetPlacement()
    {
        this.IsOpening = false;
        transform.localPosition = this.OriginalLocalPosition;
    }
}
