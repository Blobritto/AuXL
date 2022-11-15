using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    //Animator animator;
    Rigidbody2D rb;
    SpriteRenderer renderer;

    [SerializeField] private float _walkSpeed = 10.0f;
    [SerializeField] private float _jumpHeight = 30.0f;
    [SerializeField] Transform groundCheck;
    [SerializeField] Transform groundCheckL;
    [SerializeField] Transform groundCheckR;


    bool isGrounded;


    // Start is called before the first frame update
    void Start()
    {

        rb = GetComponent<Rigidbody2D>();
        renderer = GetComponent<SpriteRenderer>();
        
    }

    // Update is called once per frame
    void Update()
    { 
    }

    private void FixedUpdate()
    {
        if (Physics2D.Linecast(transform.position, groundCheck.position, 1 << LayerMask.NameToLayer("Floor")) ||
            Physics2D.Linecast(transform.position, groundCheckL.position, 1 << LayerMask.NameToLayer("Floor")) ||
            Physics2D.Linecast(transform.position, groundCheckR.position, 1 << LayerMask.NameToLayer("Floor")))
        {
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }
       


        if (Input.GetKey("d") || Input.GetKey("right"))
        {
            _walkSpeed = 10.0f;
            renderer.flipX = false;
        }
        else if (Input.GetKey("a") || Input.GetKey("left"))
        {
            _walkSpeed = -10.0f;
            renderer.flipX = true;
        }
        else
        {
            _walkSpeed = 0.0f;
        }
        if (Input.GetKey("space") && isGrounded)
        {
            rb.velocity = new Vector2(rb.velocity.x, _jumpHeight);
        }
        
        
        
        rb.velocity = new Vector2(_walkSpeed, rb.velocity.y);

    }
}
