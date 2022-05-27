using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Descend : MonoBehaviour
{
    private Vector3 OriginalPosition;

    [SerializeField]
    private bool IsDesending = false;

    // [SerializeField]
    private float Speed = 0.01f;

    // Start is called before the first frame update
    void Start()
    {
        this.OriginalPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (IsDesending && transform.position.y > -1)
        {
            this.Move();
        }
    }

    private void Move()
    {
        transform.position =
            new Vector3(transform.position.x,
                (transform.position.y - this.Speed),
                transform.position.z);
    }

    public void StartDesending()
    {
        this.IsDesending = true;
    }

    public void ResetPlacement()
    {
        this.IsDesending = false;
        transform.position = this.OriginalPosition;
    }
}
