using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(AudioSource))]
public class Beario : BaseUnit
{
    public float jumpSpeed = 12;
    public float killHeight = -30f;
    public int numberOfJumps = 2;
    public AudioClip coinSound;
    public AudioClip deathSound;

    private Transform _Cam;
    private bool _IsAlive = true;
    private AudioSource _ASource;
    private int _JumpCount;


    private void Start()
    {
        _Cam = Camera.main.transform;
        _ASource = GetComponent<AudioSource>();
        //InvokeRepeating("OverloadCPU", 5, 5);
    }

    private void Update()
    {
        if (!_IsAlive)
            return;

        //Movement
        float horizontalInput = Input.GetAxis("Horizontal");
        Move(horizontalInput);

        //Ground Checking
        bool isGrounded = IsGrounded(raycastOffset) || IsGrounded(-raycastOffset);
        if (isGrounded) _JumpCount = 0;

        //Animate Jumping or grounded
        _Anim.SetBool("Jumping", !isGrounded);

        //Jumping
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Jump();
        }

        if (transform.position.y < killHeight)
        {
            Die();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Coin"))
        {
            ++Globals.instance.Score;
            _ASource.PlayOneShot(coinSound);
            Destroy(other.gameObject);
        }
        else if (other.CompareTag("EnemyTag"))
        {
            KillEnemy(other.gameObject);
        }

    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.transform.CompareTag("EnemyTag"))
        {
            Die();
        }
    }

    private void KillEnemy(GameObject enemy)
    {
        _RB.velocity = new Vector2(_RB.velocity.x, jumpSpeed * 0.5f);
        Destroy(enemy);
        ++Globals.instance.KillCount;
    }

    private void Die()
    {
        StartCoroutine(DeathCoroutine());
    }

    private void Jump(float jumpMult = 1)
    {
        if (_JumpCount < numberOfJumps)
        {
            _RB.velocity = new Vector2(_RB.velocity.x, jumpSpeed * jumpMult);
            ++_JumpCount;
        }
    }



    IEnumerator DeathCoroutine()
    {
        _IsAlive = false;
        transform.SetParent(null);
        _ASource.PlayOneShot(deathSound);
        _Anim.SetBool("IsAlive", false);
        _Cam.SetParent(null);
        _RB.velocity = new Vector2(0, jumpSpeed * 1.8f);
        foreach (var item in GetComponents<Collider2D>())
        {
            item.enabled = false;
        }
        //GetComponent<Collider2D>().enabled = false;

        //yield return null; //waits 1 frame
        //yield return new WaitForSeconds(3); //Affected by timescale
        //yield break; //Exits the coroutine
        yield return new WaitForSecondsRealtime(3);
        
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }


    void OverloadCPU()
    {
        print("OverloadCPU");
        for (int i = 0; i < 1000000; i++)
        {
            //GameObject tmp = new GameObject();
            //tmp.isStatic = false;
            //tmp.SetActive(false);
            int j = ((i + 1 - 1) * 1 / 1) + 0;
            j = (int)(Mathf.Sqrt(i) * Mathf.Sqrt(i));
        }
    }


}
