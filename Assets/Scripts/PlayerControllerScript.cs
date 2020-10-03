using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(AudioSource))]
public class PlayerControllerScript : BaseUnit {
    public float jumpSpeed = 8;
    public float killHeight = -30f;
    public int numberOfJumps = 2;
    public AudioClip coinSound;
    public AudioClip deathSound;
    public float baseSpeed;

    // State Machine Vars
    public PlayerState playerState;
    // Moving Vars
    public bool isGrounded;
    public float moveSpeed = 5;
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
    private int _floatCount;
    // Jumping Vars
    public KeyCode jumpKey;
    private int _jumpCount;

    private Transform _cam;
    private bool _isAlive = true;
    private AudioSource _aSource;


    private void Start() {
        _cam = Camera.main.transform;
        _aSource = GetComponent<AudioSource>();
        //InvokeRepeating("OverloadCPU", 5, 5);
    }

    private void Update() {
        if (!_isAlive)
            return;

        //StateMachine
        StateMachine();


        //Ground Checking
        isGrounded = IsGrounded(raycastOffset) || IsGrounded(-raycastOffset);
        Debug.Log("isGrounded: " + isGrounded);


        //Jumping
        if (Input.GetKeyDown(KeyCode.Space)) {
            Jump();
        }

        if (transform.position.y < killHeight) {
            Die();
        }
    }

    // State Machine
    void StateMachine() {

        switch (playerState) {
            case PlayerState.Moving:
                if (isGrounded) {
                    Move(moveSpeed);
                    _jumpCount = 0;
                    _floatCount = 0;
                }
                cdReseter();
                break;

            case PlayerState.Jumping:
                Jump();
                playerState = PlayerState.Moving;
                break;

            case PlayerState.Floating:
                // if(!buttonReleased)
                Float(floatSpeed);
                playerState = PlayerState.Moving;
                break;

            case PlayerState.Dashing:
                Dash(moveSpeed * 3);
                playerState = PlayerState.Moving;
                _dashInCooldown = true;
                _dashCDTick = 0;
                break;

            case PlayerState.Stopping:
                Stop();
                playerState = PlayerState.Moving;
                _stopInCooldown = true;
                _stopCDTick = 0;
                break;

        }
        //TODO DIAGRAM STATES LIKE A HUMAN



    }

    void cdReseter() {
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
    }





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

    private void KillEnemy(GameObject enemy) {
        _RB.velocity = new Vector2(_RB.velocity.x, jumpSpeed * 0.5f);
        Destroy(enemy);
        Globals.instance.KillCount++;
    }

    private void Die() {
        StartCoroutine(DeathCoroutine());
    }

    private void Jump(float jumpMult = 1) {
        if (_jumpCount < numberOfJumps) {
            _RB.velocity = new Vector2(_RB.velocity.x, jumpSpeed * jumpMult);
            _jumpCount++;
        }
    }
    private void Float(float floatSpeed = 0) {
        if (_floatCount < 1) {
            _RB.velocity = new Vector2(_RB.velocity.x, floatSpeed);
            _floatCount++;
        }
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

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }



}
