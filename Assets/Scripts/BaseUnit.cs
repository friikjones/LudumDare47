using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Animator))]
public class BaseUnit : MonoBehaviour {
    public float acceleration = 1f;
    public float raycastOffset = 0.4f;
    public float raycastDistance = 0.1f;


    protected Rigidbody2D _RB;
    protected SpriteRenderer _SR;
    protected Animator _Anim;

    private void Awake() {
        _RB = GetComponent<Rigidbody2D>();
        _SR = GetComponent<SpriteRenderer>();
        _Anim = GetComponent<Animator>();
    }

    protected void Move(float moveSpeed) {
        //Flips the orientation of the sprite
        // if (moveSpeed < 0) _SR.flipX = true;
        // else if (moveSpeed > 0) _SR.flipX = false;

        //Animate running based on absolute value of moveSpeed
        // _Anim.SetFloat("MoveSpeed", Mathf.Abs(moveSpeed));

        //apply velocity so it moves
        if (_RB.velocity.x < moveSpeed && _RB.velocity.x > -moveSpeed) {
            _RB.velocity += new Vector2(moveSpeed * acceleration, _RB.velocity.y);
        }

    }

    protected void Dash(float moveSpeed) {

        //apply velocity so it moves
        _RB.velocity = new Vector2(moveSpeed, _RB.velocity.y);

    }

    protected void Stop() {

        //apply velocity so it moves
        _RB.velocity = new Vector2(0, _RB.velocity.y);

    }



    protected bool IsGrounded(float offsetX) {
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

        Debug.Log("hitInfo: " + hitInfo.distance);

        return hitInfo;
    }

}
