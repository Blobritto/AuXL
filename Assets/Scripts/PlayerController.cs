using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
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
    [SerializeField] protected bool _groundedJump;
    [SerializeField] protected float _coyoteTime;
    [SerializeField] protected float _coyoteTimeCounter;
    [SerializeField] protected float _jumpBufferTime;
    [SerializeField] protected float _jumpBufferTimeCounter;
    [SerializeField] protected Transform groundCheck;
    [SerializeField] protected Transform groundCheckL;
    [SerializeField] protected Transform groundCheckR;
    [SerializeField] protected GameObject spawnPoint;
    [SerializeField] protected GameObject coin;
    [SerializeField] protected bool _cthrow;
    [SerializeField] protected PlayerStates currentState;
    public virtual void handleInput(PlayerController thisObject) { }
    // Set all of the variables referenced within the player class.
    public void SetComponents(Rigidbody2D _rb, SpriteRenderer _renderer, PlayerStates _currentState, Transform _groundCheck, Transform _groundCheckL, Transform _groundCheckR, float __walkSpeed, float __jumpHeight, float __airSpeed, bool __jumped, float __coyoteTime, float __coyoteTimeCounter, float __jumpBufferTime, float __jumpBufferTimeCounter, bool __jumpReset, GameObject _coin, bool __cthrow, bool __groundedJump, GameObject _spawnPoint)
    {
        rb = _rb;
        renderer = _renderer;
        currentState = _currentState;
        coin = _coin;
        _cthrow = __cthrow;
        _walkSpeed = __walkSpeed;
        _walkAccel = __walkSpeed;
        _airSpeed = __airSpeed;
        _airAccel = __airSpeed;        
        groundCheck = _groundCheck;
        groundCheckL = _groundCheckL;
        groundCheckR = _groundCheckR;       
        _jumped = __jumped;
        _jumpHeight = __jumpHeight;
        _jumpReset = __jumpReset;
        _groundedJump = __groundedJump;
        _coyoteTime = __coyoteTime;
        _coyoteTimeCounter = __coyoteTimeCounter;
        _jumpBufferTime = __jumpBufferTime;
        _jumpBufferTimeCounter = __jumpBufferTimeCounter;
        spawnPoint = _spawnPoint;
    }
    // Utilises the speed of the update function inside the rigidity of fixed update.
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
    // If a linecast towards the ground sees the player is touching the floor, then they are grounded.
    public bool isGrounded()
    {
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
    // Do walk movement.
    public void Move(float speed, float accel, float drag, int state, bool _cthrow)
    {
        // Horizontal movement
        if (Input.GetKey("d") || Input.GetKey("right"))
        {
            if (state == 1)
            {
                speed = accel;
            }
            else if (state == 2)
            {
                speed = Mathf.MoveTowards(rb.velocity.x, accel, drag * Time.deltaTime);
            }
            renderer.flipX = false;
        }
        else if (Input.GetKey("a") || Input.GetKey("left"))
        {
            if (state == 1)
            {
                speed = -accel;
            }
            else if (state == 2)
            {
                speed = Mathf.MoveTowards(rb.velocity.x, -accel, drag * Time.deltaTime);
            }
            renderer.flipX = true;
        }
        else
        {
            speed = Mathf.MoveTowards(rb.velocity.x, 0, 45f * Time.deltaTime);
        }
        if (Input.GetKey("r"))
        {
            rb.transform.position = spawnPoint.transform.position;
            if (GameObject.FindWithTag("Coin") != null)
            {
                GameObject.Destroy(GameObject.FindWithTag("Coin").gameObject);
            }
        }
        // Capping something or other.
        if (speed > accel)
        {
            speed = accel;
        }
        if (speed < -accel)
        {
            speed = -accel;
        }
        // Cap fall speed.
        if (rb.velocity.y < -35f)
        {
            rb.velocity = new Vector2(speed, -35f);
        }
        else
        {
            rb.velocity = new Vector2(speed, rb.velocity.y);
        }
    }
};
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
public class RunningState : PlayerStates
{
    public override void handleInput(PlayerController thisObject)
    {
        // If grounded, then the coyote time is set to max, only when in the air does it count down.
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
        // Move the player horizontally.
        Move(_walkSpeed, _walkAccel, 45f, 1, _cthrow);
        if (_jumped && _coyoteTimeCounter > 0f)
        {
            // Jump the player into the air and switch their state so they have properties of being in the air.
            thisObject.rb.velocity = new Vector2(thisObject.rb.velocity.x, _jumpHeight);
            thisObject.currentState = new JumpState();
            _coyoteTimeCounter = 0f;
            _jumpBufferTimeCounter = 0f;
            _walkSpeed = 0f;
            _jumpReset = false;
            _groundedJump = true;
            thisObject.currentState.SetComponents(rb, renderer, currentState, groundCheck, groundCheckL, groundCheckR, _walkAccel, _jumpHeight, _airAccel, _jumped, _coyoteTime, _coyoteTimeCounter, _jumpBufferTime, _jumpBufferTimeCounter, _jumpReset, coin, _cthrow, _groundedJump, spawnPoint);
        }
        // If the left mouse button is clicked / coin is thrown.
        if (_cthrow)
        {
            thisObject.currentState = new ThrowState();
            thisObject.currentState.SetComponents(rb, renderer, currentState, groundCheck, groundCheckL, groundCheckR, _walkAccel, _jumpHeight, _airAccel, _jumped, _coyoteTime, _coyoteTimeCounter, _jumpBufferTime, _jumpBufferTimeCounter, _jumpReset, coin, _cthrow, _groundedJump, spawnPoint);
        }
    }
}
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
public class ThrowState : PlayerStates
{
    public override void handleInput(PlayerController thisObject)
    {
        // Lots of maths to throw the coin in the direction of the mouse in relation to the player.
        Vector2 mousePosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        Vector2 mouseWorldPosition = Camera.main.ScreenToWorldPoint(mousePosition);
        Vector2 dir = mouseWorldPosition - new Vector2(thisObject.transform.position.x, thisObject.transform.position.y);
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        Vector2 knockback = Quaternion.AngleAxis(angle, Vector3.forward) * Vector3.right;
        Vector3 knockback3 = new Vector3(knockback.x, knockback.y, 0);
        GameObject existingCoin;
        existingCoin = GameObject.FindWithTag("Coin");
        // ExistingCoin is to make sure it can only be thrown if no other coin exists in the scene.
        if (_cthrow && existingCoin == null)
        {
           // Had a bug where if grounded and shot straight up, the effect would be massive, so I nerfed it in these circumstances.
            if (isGrounded())
            {
                knockback = new Vector2(knockback.x, knockback.y / 2);
            }
            thisObject.rb.velocity = knockback * -50;          
            _jumped = false;
            // Creates the new coin to be thrown.
            GameObject newCoin;
            newCoin = Object.Instantiate(coin, thisObject.rb.transform.position + (knockback3), Quaternion.identity);
            newCoin.GetComponent<Rigidbody2D>().velocity = knockback3 * 30;
            
            if (knockback.x > 0)
            {
                newCoin.GetComponent<Rigidbody2D>().angularVelocity = -1000f;
            }
            else
            {
                newCoin.GetComponent<Rigidbody2D>().angularVelocity = 1000f;
            }
        }
        _cthrow = false;
        // Throwing state only last a single frame, the player is returned to one of its other states immediately after.
        if (isGrounded())
        {
            thisObject.currentState = new RunningState();
            thisObject.currentState.SetComponents(rb, renderer, currentState, groundCheck, groundCheckL, groundCheckR, _walkAccel, _jumpHeight, _airAccel, _jumped, _coyoteTime, _coyoteTimeCounter, _jumpBufferTime, _jumpBufferTimeCounter, _jumpReset, coin, _cthrow, _groundedJump, spawnPoint);
        }
        else
        {
            thisObject.currentState = new JumpState();
            thisObject.currentState.SetComponents(rb, renderer, currentState, groundCheck, groundCheckL, groundCheckR, _walkAccel, _jumpHeight, _airAccel, _jumped, _coyoteTime, _coyoteTimeCounter, _jumpBufferTime, _jumpBufferTimeCounter, _jumpReset, coin, _cthrow, _groundedJump, spawnPoint);
        }
    }
}
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
public class JumpState : PlayerStates
{
    public override void handleInput(PlayerController thisObject)
    {
        // if the jump has been reset, then the next jump is not from on the ground.
        if (_jumpReset)
        {
            _groundedJump = false;
        }       
        // If jump is pressed, if player is grounded within timeframe, then the jump will count, it is buffered.
        if (_jumped && _groundedJump == false)
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
                _groundedJump = false;
                thisObject.currentState.SetComponents(rb, renderer, currentState, groundCheck, groundCheckL, groundCheckR, _walkAccel, _jumpHeight, _airAccel, _jumped, _coyoteTime, _coyoteTimeCounter, _jumpBufferTime, _jumpBufferTimeCounter, _jumpReset, coin, _cthrow, _groundedJump, spawnPoint);
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
            // Moves the player horizontally.
            Move(_airSpeed, _airAccel, 300f, 2, _cthrow);
            // If the left mouse button is clicked / coin is thrown.
            if (_cthrow)
            {
                thisObject.currentState = new ThrowState();
                thisObject.currentState.SetComponents(rb, renderer, currentState, groundCheck, groundCheckL, groundCheckR, _walkAccel, _jumpHeight, _airAccel, _jumped, _coyoteTime, _coyoteTimeCounter, _jumpBufferTime, _jumpBufferTimeCounter, _jumpReset, coin, _cthrow, _groundedJump, spawnPoint);
            }
        }
    }
}
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
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
    public bool _groundedJump;
    public float _coyoteTime;
    public float _coyoteTimeCounter;
    public float _jumpBufferTime;
    public float _jumpBufferTimeCounter;
    // Grounded checking objects.
    public Transform groundCheck;
    public Transform groundCheckL;
    public Transform groundCheckR;
    // Spawn area
    public GameObject spawnPoint;
    // Coin.
    public GameObject coin;
    public bool _cthrow;
    // The player itself
    public PlayerStates currentState;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        renderer = GetComponent<SpriteRenderer>();
        spawnPoint = GameObject.FindWithTag("Spawn");
        // By default, the player is walking on the ground.
        currentState = new RunningState();
        // Uses the previously defined values and components to be useable within the player finite state machine.
        currentState.SetComponents(rb, renderer, currentState, groundCheck, groundCheckL, groundCheckR, _walkSpeed, _jumpHeight, _airSpeed, _jumped, _coyoteTime, _coyoteTimeCounter, _jumpBufferTime, _jumpBufferTimeCounter, _jumpReset, coin , _cthrow, _groundedJump, spawnPoint);
    }
    private void FixedUpdate()
    {
        // Input and all derivitive actions take place every frame.
        currentState.handleInput(this);
    }
    private void Update()
    {
        if (Input.GetKeyDown("space") && SceneManager.GetActiveScene().buildIndex > 0)
        {
            currentState.SetJumped();
        }
        if (Input.GetKeyUp("space"))
        {
            currentState.ReleaseJump();
        }
        if (Input.GetMouseButtonDown(0) && SceneManager.GetActiveScene().buildIndex > 1)
        {
            currentState.CoinThrown();
        }

    }
    // When interacting with a coin, if it is moving, the player cannot collect it, so they can be hit by and jump on it, but when it is stationary, the player can retrieve their coin.
    void OnTriggerEnter2D(Collider2D col)
    {
        if (Mathf.Abs(col.gameObject.GetComponent<Rigidbody2D>().velocity.x) < 100f && Mathf.Abs(col.gameObject.GetComponent<Rigidbody2D>().velocity.y) < 100f && Mathf.Abs(col.gameObject.GetComponent<Rigidbody2D>().angularVelocity) < 200f && col.tag == "Coin")
        {
            GameObject.Destroy(col.gameObject);
        }               
    }
}