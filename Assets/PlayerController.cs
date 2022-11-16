using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerStates : MonoBehaviour
{
    public struct Variables
    {
        public Rigidbody2D rb;
        public SpriteRenderer renderer;

        public float _walkSpeed;
        public float _jumpHeight;
        public float _airSpeed;
        public Transform groundCheck;
        public Transform groundCheckL;
        public Transform groundCheckR;

        public PlayerStates currentState;
    }

    // common base class for sharing stuff (e.g. static counter variables)
    // also forces people to implement minimal functionality
    
    
    public virtual void handleInput(PlayerController thisObject) { }
    
    public void Awake(PlayerController thisObject)
    {
        Variables var;

        var.rb = GameObject.GetComponent<Rigidbody2D>();
        var.renderer = GameObject.GetComponent<SpriteRenderer>();
        var.currentState = new RunningState();
    }
};

public class RunningState : PlayerStates
{

    public override void handleInput(PlayerController thisObject)
    {
        Variables var;

        if (Input.GetKey("d") || Input.GetKey("right"))
        {
            var._walkSpeed = 10.0f;
            var.renderer.flipX = false;
        }
        else if (Input.GetKey("a") || Input.GetKey("left"))
        {
            var._walkSpeed = -10.0f;
            var.renderer.flipX = true;
        }
        else
        {
            var._walkSpeed = 0.0f;
        }
        if (Input.GetKey("space"))
        {
            var.rb.velocity = new Vector2(var.rb.velocity.x, var._jumpHeight);
            thisObject.currentState = new JumpState();
        }

        var.rb.velocity = new Vector2(var._walkSpeed, var.rb.velocity.y);
    }
}

public class JumpState : PlayerStates
{
    public override void handleInput(PlayerController thisObject)
    {
        Variables var;


        while (Physics2D.Linecast(thisObject.transform.position, var.groundCheck.position, 1 << LayerMask.NameToLayer("Floor")) ||
        Physics2D.Linecast(thisObject.transform.position, var.groundCheckL.position, 1 << LayerMask.NameToLayer("Floor")) ||
        Physics2D.Linecast(thisObject.transform.position, var.groundCheckR.position, 1 << LayerMask.NameToLayer("Floor")))
        {
            if (Input.GetKey("d") || Input.GetKey("right"))
            {
                var._airSpeed = 15.0f;
                var.renderer.flipX = false;
            }
            else if (Input.GetKey("a") || Input.GetKey("left"))
            {
                var._airSpeed = -15.0f;
                var.renderer.flipX = true;
            }
        }

        var.rb.velocity = new Vector2(var._airSpeed, var.rb.velocity.y);
        thisObject.currentState = new RunningState();
    }
}




public class PlayerController : MonoBehaviour
{
    //Animator animator;
    Rigidbody2D rb;
    SpriteRenderer renderer;

    public float _walkSpeed = 10.0f;
    public float _jumpHeight = 30.0f;
    public float _airSpeed = 10.0f;
    public Transform groundCheck;
    public Transform groundCheckL;
    public Transform groundCheckR;


    public PlayerStates currentState;


    bool isGrounded;


    // Start is called before the first frame update
    void Start()
    {

        rb = GetComponent<Rigidbody2D>();
        renderer = GetComponent<SpriteRenderer>();
        currentState = new RunningState();

        currentState.Awake(this);

    }


    private void FixedUpdate()
    {
        currentState.handleInput(this);
    }
}
