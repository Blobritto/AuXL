using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerStates 
{
    // Make sure these are protected so they can be accessed by child classes.
    [SerializeField] protected Rigidbody2D rb;
    [SerializeField] protected SpriteRenderer renderer;
    [SerializeField] protected float _walkSpeed;
    [SerializeField] protected float _walkAccel;
    [SerializeField] protected float _jumpHeight;
    [SerializeField] protected float _airSpeed;
    [SerializeField] protected float _airAccel;
    [SerializeField] protected bool _isGrounded;
    [SerializeField] protected Transform groundCheck;
    [SerializeField] protected Transform groundCheckL;
    [SerializeField] protected Transform groundCheckR;
    [SerializeField] protected PlayerStates currentState;

    public virtual void handleInput(PlayerController thisObject) { }

    // Set all of the variables referenced within the player class.
    public void SetComponents(Rigidbody2D _rb, SpriteRenderer _renderer, PlayerStates _currentState, Transform _groundCheck, Transform _groundCheckL, Transform _groundCheckR, float __walkSpeed, float __jumpHeight, float __airSpeed)
    {
        rb = _rb;
        renderer = _renderer;
        currentState = _currentState;
        _walkSpeed = __walkSpeed;
        _walkAccel = __walkSpeed;
        _jumpHeight = __jumpHeight;
        _airSpeed = __airSpeed;
        _airAccel = __airSpeed;
        groundCheck = _groundCheck;
        groundCheckL = _groundCheckL;
        groundCheckR = _groundCheckR;
    }
};

public class RunningState : PlayerStates
{
    public override void handleInput(PlayerController thisObject)
    {
        // Do walk movement.
        if (Input.GetKey("d") || Input.GetKey("right"))
        {
            _walkSpeed = _walkAccel;
            thisObject.renderer.flipX = false;
        }
        else if (Input.GetKey("a") || Input.GetKey("left"))
        {
            _walkSpeed = -_walkAccel;
            thisObject.renderer.flipX = true;
        }
        else
        {
            _walkSpeed = 0.0f;
        }
        if (Input.GetKey("space"))
        {
            // Jump the player into the air and switch their state so they have properties of being in the air.
            thisObject.rb.velocity = new Vector2(thisObject.rb.velocity.x, 30.0f);
            thisObject.currentState = new JumpState();
            thisObject.currentState.SetComponents(rb, renderer, currentState, groundCheck, groundCheckL, groundCheckR, _walkAccel, _jumpHeight, _airAccel);
        }

        thisObject.rb.velocity = new Vector2(_walkSpeed, thisObject.rb.velocity.y);
    }
}

public class JumpState : PlayerStates
{
    public override void handleInput(PlayerController thisObject)
    {
        // If a linecast towards the ground sees the player is touching the loor, then they are grounded.
        if (Physics2D.Linecast(thisObject.transform.position, thisObject.groundCheck.position, 1 << LayerMask.NameToLayer("Floor")) ||
        Physics2D.Linecast(thisObject.transform.position, thisObject.groundCheckL.position, 1 << LayerMask.NameToLayer("Floor")) ||
        Physics2D.Linecast(thisObject.transform.position, thisObject.groundCheckR.position, 1 << LayerMask.NameToLayer("Floor")))
        {
            // If they are grounded, it switches back to running state, and the components have to be reassigned.
            thisObject.currentState = new RunningState();
            thisObject.currentState.SetComponents(rb, renderer, currentState, groundCheck, groundCheckL, groundCheckR, _walkAccel, _jumpHeight, _airAccel);
        }
        // Do air movement, slightly faster than walk movement.
        else
        {
            if (Input.GetKey("d") || Input.GetKey("right"))
            {
                _airSpeed = _airAccel;
                thisObject.renderer.flipX = false;
            }
            else if (Input.GetKey("a") || Input.GetKey("left"))
            {
                _airSpeed = -_airAccel;
                thisObject.renderer.flipX = true;
            }
            else
            {
                _airSpeed = 0.0f;
            }
        }
        thisObject.rb.velocity = new Vector2(_airSpeed, thisObject.rb.velocity.y);
    }
}

public class PlayerController : MonoBehaviour
{
    // Global fields to parse into the player variables.
    // Player Components.
    public Rigidbody2D rb;
    public SpriteRenderer renderer;
    // Movement variables.
    public float _walkSpeed;
    public float _jumpHeight;
    public float _airSpeed;
    // Grounded checking objects.
    public Transform groundCheck;
    public Transform groundCheckL;
    public Transform groundCheckR;
    // The player itself
    public PlayerStates currentState;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        renderer = GetComponent<SpriteRenderer>();
        // By default, the player is walking on the ground.
        currentState = new RunningState();
        // Uses the previously defined values and components to be useable within the player finite state machine.
        currentState.SetComponents(rb, renderer, currentState, groundCheck, groundCheckL, groundCheckR, _walkSpeed, _jumpHeight, _airSpeed);
    }

    private void FixedUpdate()
    {
        // Input and all derivitive actions take place every frame.
        currentState.handleInput(this);
    }
}