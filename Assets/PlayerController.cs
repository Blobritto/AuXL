using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerStates 
{
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

    public PlayerStates currentState;


    public virtual void handleInput(PlayerController thisObject) { }

    public void SetComponents(Rigidbody2D _rb, SpriteRenderer _renderer, PlayerStates _currentState, Transform _groundCheck, Transform _groundCheckL, Transform _groundCheckR, float __walkSpeed, float __jumpHeight, float __airSpeed, bool __isGrounded)
    {
        rb = _rb;
        renderer = _renderer;
        currentState = _currentState;
        _walkSpeed = __walkSpeed;
        _walkAccel = __walkSpeed;
        _jumpHeight = __jumpHeight;
        _airSpeed = __airSpeed;
        _airAccel = __airSpeed;
        _isGrounded = __isGrounded;
        groundCheck = _groundCheck;
        groundCheckL = _groundCheckL;
        groundCheckR = _groundCheckR;
    }
};

public class RunningState : PlayerStates
{
    public override void handleInput(PlayerController thisObject)
    {
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
            thisObject.rb.velocity = new Vector2(thisObject.rb.velocity.x, 30.0f);
            Debug.Log("Jumped");
            _isGrounded = false;
            thisObject.currentState = new JumpState();
        }

        thisObject.rb.velocity = new Vector2(_walkSpeed, thisObject.rb.velocity.y);
    }
}

public class JumpState : PlayerStates
{
    public override void handleInput(PlayerController thisObject)
    {
        if (Physics2D.Linecast(thisObject.transform.position, thisObject.groundCheck.position, 1 << LayerMask.NameToLayer("Floor")) ||
        Physics2D.Linecast(thisObject.transform.position, thisObject.groundCheckL.position, 1 << LayerMask.NameToLayer("Floor")) ||
        Physics2D.Linecast(thisObject.transform.position, thisObject.groundCheckR.position, 1 << LayerMask.NameToLayer("Floor")))
        {
            _isGrounded = true;
        }
        else
        {
            _isGrounded = false;
        }

        if (Input.GetKey("d") || Input.GetKey("right"))
        {
            _airSpeed = 15.0f;
            thisObject.renderer.flipX = false;
        }
        else if (Input.GetKey("a") || Input.GetKey("left"))
        {
            _airSpeed = -15.0f;
            thisObject.renderer.flipX = true;
        }
        else
        {
            _airSpeed = 0.0f;
        }

        thisObject.rb.velocity = new Vector2(_airSpeed, thisObject.rb.velocity.y);

       /* if (Mathf.Abs(thisObject.rb.velocity.x + _airSpeed) > Mathf.Abs(_airSpeed))
        {
            thisObject.rb.velocity = new Vector2(_airSpeed, thisObject.rb.velocity.y);
        }
        else
        {
            thisObject.rb.velocity = new Vector2(thisObject.rb.velocity.x + _airSpeed, thisObject.rb.velocity.y);
        }*/

        if (_isGrounded == true)
        {
            thisObject.currentState = new RunningState();
        }
        //thisObject.currentState = new RunningState();
    }
}

public class PlayerController : MonoBehaviour
{
    //Animator animator;
    public Rigidbody2D rb;
    public SpriteRenderer renderer;
    public float _walkSpeed;
    public float _jumpHeight;
    public float _airSpeed;
    public bool _isGrounded;
    public Transform groundCheck;
    public Transform groundCheckL;
    public Transform groundCheckR;
    public PlayerStates currentState;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        renderer = GetComponent<SpriteRenderer>();
        currentState = new RunningState();
        currentState.SetComponents(rb, renderer, currentState, groundCheck, groundCheckL, groundCheckR, _walkSpeed, _jumpHeight, _airSpeed, _isGrounded);
    }

    private void FixedUpdate()
    {
        currentState.handleInput(this);
    }
}
