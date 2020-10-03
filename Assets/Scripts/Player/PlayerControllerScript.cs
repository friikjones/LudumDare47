using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class PlayerControllerScript : MonoBehaviour {

    // GameObject Vars
    private Transform _cam;
    private Rigidbody2D _RB;
    private SpriteRenderer _SR;
    private Animator _Anim;
    public UnityEvent _levelReset;

    // State Machine Vars
    public PlayerState playerState;
    private bool _isAlive = true;

    // Moving Vars
    public float maxMoveSpeed = 5;
    public float acceleration = 1f;
    public float killHeight = -30f;
    public float raycastOffset = 0.4f;
    public float raycastDistance = 0.1f;
    private bool _isGrounded;

    // Jumping Vars
    public KeyCode jumpKey;
    public float jumpSpeed = 8;
    public int numberOfJumps = 2;
    private int _jumpCount;

    // Dashing Vars
    public KeyCode dashKey;
    public float dashCD;
    private int _dashCDTick;
    private bool _dashInCooldown;

    // Stopping Vars
    public KeyCode stopKey;
    public float stopCD;
    private int _stopCDTick;
    private bool _stopInCooldown;

    // Floating Vars
    public KeyCode floatKey;
    public float floatSpeed;
    public float floatMaxDuration;
    private int _floatTick;
    private int _floatCount;

    // Audio Vars
    public AudioClip coinSound;
    public AudioClip deathSound;
    private AudioSource _aSource;

    private void Awake() {
        _RB = GetComponent<Rigidbody2D>();
        _SR = GetComponent<SpriteRenderer>();
        _Anim = GetComponent<Animator>();
        _cam = Camera.main.transform;
        _aSource = GetComponent<AudioSource>();
        _levelReset = GameObject.Find("Globals").GetComponent<GameManagerScript>()._levelReset;
        _levelReset.AddListener(LevelReset);
    }

    private void Start() {

    }

    private void Update() {
        if (!_isAlive)
            return;

        // StateMachine
        StateMachine();
        // Animation transition control
        AnimatorTransitions();

        // Ground Checking
        _isGrounded = IsGrounded(raycastOffset) || IsGrounded(-raycastOffset);
        Debug.Log("isGrounded: " + _isGrounded);

        // killHeight Check
        if (transform.position.y < killHeight) {
            Die();
        }
    }

    // State Machine
    void StateMachine() {

        // State Machine
        switch (playerState) {

            // Moving state - default movementation state
            case PlayerState.Moving:
                if (_isGrounded) {
                    Move(maxMoveSpeed);
                    _jumpCount = 0;
                    _floatCount = 0;
                }
                // Transitions 
                if (Input.GetKeyDown(jumpKey)) {
                    playerState = PlayerState.Jumping;
                }
                if (Input.GetKeyDown(dashKey)) {
                    playerState = PlayerState.Dashing;
                    _Anim.SetTrigger("dashTrigger");
                }
                if (Input.GetKeyDown(stopKey)) {
                    playerState = PlayerState.Stopping;
                }
                if (Input.GetKeyDown(floatKey) && _floatCount < 1) {
                    playerState = PlayerState.Floating;
                }
                CdReseter();
                break;

            // Jumping state
            case PlayerState.Jumping:
                Jump();
                playerState = PlayerState.Moving;
                break;

            // Dashing state
            case PlayerState.Dashing:
                Dash(maxMoveSpeed * 3);
                playerState = PlayerState.Moving;
                _dashInCooldown = true;
                _dashCDTick = 0;
                _Anim.ResetTrigger("dashTrigger");
                break;

            // Stopping state
            case PlayerState.Stopping:
                Stop();
                playerState = PlayerState.Moving;
                _stopInCooldown = true;
                _stopCDTick = 0;
                break;

            // Floating state
            case PlayerState.Floating:
                Float(floatSpeed);
                _floatTick++;
                if (Input.GetKeyUp(floatKey) || _floatTick > floatMaxDuration * 50) {
                    playerState = PlayerState.Moving;
                    _floatCount++;
                }
                break;

        }
        //TODO DIAGRAM STATES LIKE A HUMAM


    }

    void AnimatorTransitions() {

        if (_RB.velocity.x > 0) {
            _Anim.SetBool("isRunning", true);
        } else {
            _Anim.SetBool("isRunning", false);
        }
        if (_RB.velocity.y > 0.1f) {
            _Anim.SetBool("isGoingUp", true);
        } else if (_RB.velocity.y < -0.1f) {
            _Anim.SetBool("isFalling", true);
        } else {
            _Anim.SetBool("isGoingUp", false);
            _Anim.SetBool("isFalling", false);
        }

    }

    void CdReseter() {
        // Dash CD reseter
        _dashCDTick++;
        if (_dashCDTick > dashCD * 50) {
            _dashInCooldown = false;
        }
        // Stopping reseter
        _stopCDTick++;
        if (_stopCDTick > stopCD * 50) {
            _stopInCooldown = false;
        }
        // Float counter reseter
        _floatTick = 0;
    }

    void LevelReset() {

    }

    // -------------------------- Movement Functions------------------------------

    private void Move(float moveSpeed) {
        //Flips the orientation of the sprite
        // if (moveSpeed < 0) _SR.flipX = true;
        // else if (moveSpeed > 0) _SR.flipX = false;

        //Animate running based on absolute value of moveSpeed
        // _Anim.SetFloat("MoveSpeed", Mathf.Abs(moveSpeed));

        //apply velocity so it moves
        if (_RB.velocity.x < (moveSpeed - acceleration) && _RB.velocity.x > -(moveSpeed - acceleration)) {
            _RB.velocity += new Vector2(acceleration, _RB.velocity.y);
        }

    }

    private void Jump(float jumpMult = 1) {
        if (_jumpCount < numberOfJumps) {
            _RB.velocity = new Vector2(_RB.velocity.x, jumpSpeed * jumpMult);
            _jumpCount++;
        }
    }

    private void Dash(float moveSpeed) {
        _RB.velocity = new Vector2(moveSpeed, _RB.velocity.y);
    }

    private void Stop() {
        _RB.velocity = new Vector2(0, _RB.velocity.y);
    }

    private void Float(float floatSpeed = 0) {
        _RB.velocity = new Vector2(_RB.velocity.x, floatSpeed);
    }

    // -------------------------- Collision Checks ------------------------------
    private void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Coin")) {
            Globals.instance.Score++;
            _aSource.PlayOneShot(coinSound);
            Destroy(other.gameObject);
        } else if (other.CompareTag("EnemyTag")) {
            KillEnemy(other.gameObject);
        }

    }

    private void OnCollisionEnter2D(Collision2D other) {
        if (other.transform.CompareTag("EnemyTag")) {
            Die();
        }
    }

    private bool IsGrounded(float offsetX) {
        //The point our raycast will start from
        Vector2 origin = transform.position;
        origin.x += offsetX;

        //Draw ray so we can see it, Just for debug Purposes
        Debug.DrawRay(origin, Vector3.down * raycastDistance);

        // Cast a ray and store if it hits something based on our previous values
        RaycastHit2D hitInfo = Physics2D.Raycast(origin, Vector2.down, raycastDistance);

        //If my raycast is not hitting something, then i dont have a parent
        if (hitInfo.collider == null) {
            transform.SetParent(null);
            return false;
        }

        //Parent unit if its hitting a moving platform
        if (hitInfo.collider.GetComponent<MovingPlatform>() != null) {
            transform.SetParent(hitInfo.transform);
        }

        return hitInfo;
    }

    // -------------------------- Interactions Functions------------------------------
    private void KillEnemy(GameObject enemy) {
        _RB.velocity = new Vector2(_RB.velocity.x, jumpSpeed * 0.5f);
        Destroy(enemy);
        Globals.instance.KillCount++;
    }

    private void Die() {
        StartCoroutine(DeathCoroutine());
    }

    IEnumerator DeathCoroutine() {
        _isAlive = false;
        transform.SetParent(null);
        _aSource.PlayOneShot(deathSound);
        _Anim.SetBool("IsAlive", false);
        _cam.SetParent(null);
        _RB.velocity = new Vector2(0, jumpSpeed * 1.8f);
        foreach (var item in GetComponents<Collider2D>()) {
            item.enabled = false;
        }
        yield return new WaitForSecondsRealtime(3);
        // TODO manager "game over" bool = true
    }

}
