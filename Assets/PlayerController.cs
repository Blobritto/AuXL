using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerStates
{
    public Rigidbody2D rb;
    public SpriteRenderer renderer;

    public float _walkSpeed = 10.0f;
    public float _jumpHeight = 30.0f;
    public float _airSpeed = 10.0f;
    public Transform groundCheck;
    public Transform groundCheckL;
    public Transform groundCheckR;
    // common base class for sharing stuff (e.g. static counter variables)
    // also forces people to implement minimal functionality
    public virtual void handleInput(PlayerController thisObject) { }
    public void Awake()
    {
        rb = GameObject.GetComponent<Rigidbody2D>();
        renderer = GameObject.GetComponent<SpriteRenderer>();
        currentState = new RunningState();
    }
};

public class RunningState : PlayerStates
{

    public override void handleInput(PlayerController thisObject)
    {
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
        if (Input.GetKey("space"))
        {
            rb.velocity = new Vector2(rb.velocity.x, _jumpHeight);
            thisObject.currentState = new JumpState();
        }

        rb.velocity = new Vector2(_walkSpeed, rb.velocity.y);
    }
}

public class JumpState : PlayerStates
{
    public GameObject player1;

    public override void handleInput(PlayerController thisObject)
    {
        while (Physics2D.Linecast(thisObject.transform.position, groundCheck.position, 1 << LayerMask.NameToLayer("Floor")) ||
        Physics2D.Linecast(thisObject.transform.position, groundCheckL.position, 1 << LayerMask.NameToLayer("Floor")) ||
        Physics2D.Linecast(thisObject.transform.position, groundCheckR.position, 1 << LayerMask.NameToLayer("Floor")))
        {
            if (Input.GetKey("d") || Input.GetKey("right"))
            {
                _airSpeed = 10.0f;
                renderer.flipX = false;
            }
            else if (Input.GetKey("a") || Input.GetKey("left"))
            {
                _airSpeed = -10.0f;
                renderer.flipX = true;
            }
        }

        rb.velocity = new Vector2(_airSpeed, rb.velocity.y);
        thisObject.currentState = new RunningState();
    }
}




public class PlayerController : MonoBehaviour
{
    //Animator animator;
    /*    Rigidbody2D rb;
        SpriteRenderer renderer;

        public float _walkSpeed = 10.0f;
        public float _jumpHeight = 30.0f;
        public float _airSpeed = 10.0f;
        public Transform groundCheck;
        public Transform groundCheckL;
        public Transform groundCheckR;*/


    public PlayerStates currentState;


    bool isGrounded;


    // Start is called before the first frame update
    void Start()
    {

        /* rb = GetComponent<Rigidbody2D>();
         renderer = GetComponent<SpriteRenderer>();
         currentState = new RunningState();*/

    }

    // Update is called once per frame
    void Update()
    {
    }

    private void FixedUpdate()
    {
        currentState.Awake();
        currentState.handleInput(this);
    }
}
