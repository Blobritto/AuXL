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
    [SerializeField] protected bool _jumped;
    [SerializeField] protected bool _jumpReset;
    [SerializeField] protected float _coyoteTime;
    [SerializeField] protected float _coyoteTimeCounter;
    [SerializeField] protected float _jumpBufferTime;
    [SerializeField] protected float _jumpBufferTimeCounter;
    [SerializeField] protected Transform groundCheck;
    [SerializeField] protected Transform groundCheckL;
    [SerializeField] protected Transform groundCheckR;
    [SerializeField] protected Transform coin;
    [SerializeField] protected bool _cthrow;
    [SerializeField] protected PlayerStates currentState;

    public virtual void handleInput(PlayerController thisObject) { }

    // Set all of the variables referenced within the player class.
    public void SetComponents(Rigidbody2D _rb, SpriteRenderer _renderer, PlayerStates _currentState, Transform _groundCheck, Transform _groundCheckL, Transform _groundCheckR, float __walkSpeed, float __jumpHeight, float __airSpeed, bool __jumped, float __coyoteTime, float __coyoteTimeCounter, float __jumpBufferTime, float __jumpBufferTimeCounter, bool __jumpReset, Transform _coin, bool __cthrow)
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
        _jumped = __jumped;
        _jumpReset = __jumpReset;
        _coyoteTime = __coyoteTime;
        _coyoteTimeCounter = __coyoteTimeCounter;
        _jumpBufferTime = __jumpBufferTime;
        _jumpBufferTimeCounter = __jumpBufferTimeCounter;
        coin = _coin;
        _cthrow = __cthrow;
    }

    public void SetJumped()
    {
        _jumped = true;
    }

    public void ReleaseJump()
    {
        _jumped = false;
        _jumpReset = true;
    }

    public void CoinThrown()
    {
        _cthrow = true;
    }

    public bool isGrounded()
    {
        // If a linecast towards the ground sees the player is touching the floor, then they are grounded.
        if (Physics2D.Linecast(rb.transform.position, groundCheck.position, 1 << LayerMask.NameToLayer("Floor")) ||
        Physics2D.Linecast(rb.transform.position, groundCheckL.position, 1 << LayerMask.NameToLayer("Floor")) ||
        Physics2D.Linecast(rb.transform.position, groundCheckR.position, 1 << LayerMask.NameToLayer("Floor")))
        {
            return true;
        }
        else
        {
            return false;
        }
    }
};



/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
public class RunningState : PlayerStates
{
    public override void handleInput(PlayerController thisObject)
    {
        if (isGrounded())
        {
            _coyoteTimeCounter = _coyoteTime;
        }
        else
        {
            _coyoteTimeCounter -= Time.deltaTime;
        }
        // For sanity sake.
        if (_coyoteTimeCounter < 0.01f)
        {
            _coyoteTimeCounter = 0f;
            _jumped = false;
        }

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
            _walkSpeed = Mathf.MoveTowards(thisObject.rb.velocity.x, 0, 45f * Time.deltaTime);
        }
        if (_cthrow)
        {
            thisObject.currentState = new ThrowState();
            thisObject.currentState.SetComponents(rb, renderer, currentState, groundCheck, groundCheckL, groundCheckR, _walkAccel, _jumpHeight, _airAccel, _jumped, _coyoteTime, _coyoteTimeCounter, _jumpBufferTime, _jumpBufferTimeCounter, _jumpReset, coin, _cthrow);
        }

        if (_jumped && _coyoteTimeCounter > 0f)
        {
            // Jump the player into the air and switch their state so they have properties of being in the air.
            thisObject.rb.velocity = new Vector2(thisObject.rb.velocity.x, _jumpHeight);
            thisObject.currentState = new JumpState();
            _coyoteTimeCounter = 0f;
            _jumpBufferTimeCounter = 0f;
            _walkSpeed = 0f;
            _jumpReset = false;
            thisObject.currentState.SetComponents(rb, renderer, currentState, groundCheck, groundCheckL, groundCheckR, _walkAccel, _jumpHeight, _airAccel, _jumped, _coyoteTime, _coyoteTimeCounter, _jumpBufferTime, _jumpBufferTimeCounter, _jumpReset, coin, _cthrow);
        }
        if (_walkSpeed > _walkAccel)
        {
            _walkSpeed = _walkAccel;
        }
        if (_walkSpeed < -_walkAccel)
        {
            _walkSpeed = -_walkAccel;
        }
        
        // Cap fall speed.
        if (thisObject.rb.velocity.y < -35f)
        {
            thisObject.rb.velocity = new Vector2(thisObject.rb.velocity.x, -35f);
        }
        else
        {
            thisObject.rb.velocity = new Vector2(_walkSpeed, thisObject.rb.velocity.y);
        }
    }
}



/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
public class ThrowState : PlayerStates
{
    public override void handleInput(PlayerController thisObject)
    {
        Vector2 mousePosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        Vector2 mouseWorldPosition = Camera.main.ScreenToWorldPoint(mousePosition);
        Vector2 dir = mouseWorldPosition - new Vector2(thisObject.transform.position.x, thisObject.transform.position.y);
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        Vector2 knockback = Quaternion.AngleAxis(angle, Vector3.forward) * Vector3.right;
        Debug.Log(knockback);

        if (_cthrow)
        {
            if (isGrounded())
            {
                knockback = new Vector2(knockback.x, knockback.y / 2);
            }
            thisObject.rb.velocity = knockback * -50;
            _cthrow = false;
            _jumped = false;
        }

        if (isGrounded())
        {
            thisObject.currentState = new RunningState();
            thisObject.currentState.SetComponents(rb, renderer, currentState, groundCheck, groundCheckL, groundCheckR, _walkAccel, _jumpHeight, _airAccel, _jumped, _coyoteTime, _coyoteTimeCounter, _jumpBufferTime, _jumpBufferTimeCounter, _jumpReset, coin, _cthrow);
        }
        else
        {
            thisObject.currentState = new JumpState();
            thisObject.currentState.SetComponents(rb, renderer, currentState, groundCheck, groundCheckL, groundCheckR, _walkAccel, _jumpHeight, _airAccel, _jumped, _coyoteTime, _coyoteTimeCounter, _jumpBufferTime, _jumpBufferTimeCounter, _jumpReset, coin, _cthrow);
        }
    }
}



/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
public class JumpState : PlayerStates
{
    public override void handleInput(PlayerController thisObject)
    {
        // If jump is pressed, if player is grounded within timeframe, then the jump will count, it is buffered.
        if (_jumped)
        {
            _jumpBufferTimeCounter = _jumpBufferTime;
        }
        else
        {
            _jumpBufferTimeCounter -= Time.deltaTime;
        }
        // For sanity sake.
        if (_jumpBufferTimeCounter < 0.01f)
        {
            _jumpBufferTimeCounter = 0f;
        }
        
        // If falling, then not jumping.
        if (thisObject.rb.velocity.y <= 0f)
        {
            _jumped = false;
        }

        if (isGrounded())
        {
            // If jumping immediately upon landing, there is no reason to switch back into running state.
            if (_jumpBufferTimeCounter > 0f && _jumpReset)
            {
               if (_jumpReset)
               {
                    thisObject.rb.velocity = new Vector2(thisObject.rb.velocity.x, _jumpHeight);
                    _jumpBufferTimeCounter = 0f;
                    _jumpReset = false;
               }
                
               // If Input key was released before landing within buffer time, a full jump would happen, this checks if the key is still held down, and if not, small jump occurs.
               if (Input.GetKey("space"))
               {
                    _jumped = true;
               }
            }
            else
            {
                // If they are grounded, it switches back to running state, and the components have to be reassigned.
                thisObject.currentState = new RunningState();
                // Stops constant jumping.
                _jumped = false;
                _jumpReset = false;
                thisObject.currentState.SetComponents(rb, renderer, currentState, groundCheck, groundCheckL, groundCheckR, _walkAccel, _jumpHeight, _airAccel, _jumped, _coyoteTime, _coyoteTimeCounter, _jumpBufferTime, _jumpBufferTimeCounter, _jumpReset, coin, _cthrow);
            }
        }
        // Do air movement, slightly faster than walk movement.
        else
        {
            // If the player releases the jump button before the apex, they get a shorter total jump.
            if (_jumped == false)
            {
                thisObject.rb.AddForce(thisObject.transform.up * -1 * 500);
            }

            // Air Movement.
            if (Input.GetKey("d") || Input.GetKey("right"))
            {
                _airSpeed = Mathf.MoveTowards(thisObject.rb.velocity.x, _airAccel, 300f * Time.deltaTime);
                thisObject.renderer.flipX = false;
            }
            else if (Input.GetKey("a") || Input.GetKey("left"))
            {
                _airSpeed = Mathf.MoveTowards(thisObject.rb.velocity.x, -_airAccel, 300f * Time.deltaTime);
                thisObject.renderer.flipX = true;
            }
            else
            {
                _airSpeed = Mathf.MoveTowards(thisObject.rb.velocity.x, 0, 45f * Time.deltaTime);
            }
            if (_cthrow)
            {
                thisObject.currentState = new ThrowState();
                thisObject.currentState.SetComponents(rb, renderer, currentState, groundCheck, groundCheckL, groundCheckR, _walkAccel, _jumpHeight, _airAccel, _jumped, _coyoteTime, _coyoteTimeCounter, _jumpBufferTime, _jumpBufferTimeCounter, _jumpReset, coin, _cthrow);
            }
        }
        // Cap fall speed to 20 units.
        if (thisObject.rb.velocity.y < -35f)
        {
            thisObject.rb.velocity = new Vector2(_airSpeed, -35f);
        }
        else
        {
            // Move through the air.
            thisObject.rb.velocity = new Vector2(_airSpeed, thisObject.rb.velocity.y);
        }
    }
}



/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
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
    public bool _jumped;
    public bool _jumpReset;
    public float _coyoteTime;
    public float _coyoteTimeCounter;
    public float _jumpBufferTime;
    public float _jumpBufferTimeCounter;
    // Grounded checking objects.
    public Transform groundCheck;
    public Transform groundCheckL;
    public Transform groundCheckR;
    // Coin.
    public Transform coin;
    public bool _cthrow;
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
        currentState.SetComponents(rb, renderer, currentState, groundCheck, groundCheckL, groundCheckR, _walkSpeed, _jumpHeight, _airSpeed, _jumped, _coyoteTime, _coyoteTimeCounter, _jumpBufferTime, _jumpBufferTimeCounter, _jumpReset, coin , _cthrow);
    }

    private void FixedUpdate()
    {
        // Input and all derivitive actions take place every frame.
        currentState.handleInput(this);
    }

    private void Update()
    {
        if (Input.GetKeyDown("space"))
        {
            currentState.SetJumped();
        }
        if (Input.GetKeyUp("space"))
        {
            currentState.ReleaseJump();
        }
        if (Input.GetMouseButtonDown(0))
        {
            currentState.CoinThrown();
        }
    }
}