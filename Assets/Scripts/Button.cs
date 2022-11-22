using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button : MonoBehaviour
{
    public Rigidbody2D rb;
    public SpriteRenderer renderer;
    public GameObject door;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        renderer = GetComponent<SpriteRenderer>();
        renderer.color = new Color(1, 0, 0, 1);
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.tag == "Coin" || col.gameObject.tag == "Player")
        {
            renderer.color = new Color(0, 1, 0, 1);
            GameObject.Destroy(door.gameObject);
        }
    }
}
