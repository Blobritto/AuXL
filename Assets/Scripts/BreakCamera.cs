using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakCamera : MonoBehaviour
{
    Rigidbody2D rb;
    //BoxCollider2D bc;

    // Start is called before the first frame update
    void Start()
    {
        //bc = GetComponent<BoxCollider2D>();
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
    }

    // Update is called once per frame
    void Update()
    {
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.tag == "Coin" || col.gameObject.tag == "Player")
        {
            rb.gravityScale = 3f;
            GameObject.Destroy(transform.GetChild(0).gameObject);
            //bc.isTrigger = true;
        }

        if (col.gameObject.tag == "Floor" && transform.childCount == 0)
        {
            GameObject.Destroy(gameObject);
        }
    }
}
