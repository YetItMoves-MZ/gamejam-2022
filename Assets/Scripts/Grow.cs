using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grow : MonoBehaviour
{
    [SerializeField] private float Speed = 2f;
    private float MaxSize = 20f;
    private bool isGrown = false;

    // Start is called before the first frame update
    void Start()
    {
        transform.localScale = Vector3.zero;
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.localScale.x <= MaxSize && !isGrown)
        {
            transform.localScale = new Vector3(transform.localScale.x + this.Speed * Time.deltaTime, transform.localScale.x + this.Speed * Time.deltaTime, transform.localScale.x + this.Speed * Time.deltaTime);
            this.isGrown = transform.localScale.x >= MaxSize;
        }
        // if (transform.localScale.x > 0 && isGrown)
        // {
        //     // transform.localScale = new Vector3(transform.localScale.x - this.Speed * Time.deltaTime, transform.localScale.x - this.Speed * Time.deltaTime, transform.localScale.x - this.Speed * Time.deltaTime);
        // }
    }
}