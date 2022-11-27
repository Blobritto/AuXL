using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakCamera : MonoBehaviour
{
    Rigidbody2D rb;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        // The camera floats by default.
        rb.gravityScale = 0f;
    }
    void OnCollisionEnter2D(Collision2D col)
    {
        // When the camera breaks, it falls to the ground, and the beam is destroyed.
        if (col.gameObject.tag == "Coin" || col.gameObject.tag == "Player")
        {
            rb.gravityScale = 3f;
            GameObject.Destroy(transform.GetChild(0).gameObject);
        }
        // When it hits the floor, it destroys itself.
        if (col.gameObject.tag == "Floor" && transform.childCount == 0)
        {
            GameObject.Destroy(gameObject);
        }
    }
}
