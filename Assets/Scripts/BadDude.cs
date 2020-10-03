using UnityEngine;

public class BadDude : BaseUnit
{
    private int direction = 1;

    private void Update()
    {
        //Check if the Right side is touching ground
        // if its not then move left
        if (!IsGrounded(raycastOffset))
            direction = -1;

        //Check if the Left side is touching ground
        // if its not then move Right
        if (!IsGrounded(-raycastOffset))
            direction = 1;

        Move(direction);
    }


    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.transform.CompareTag("EnemyTag"))
        {
            direction *= -1;
        }
    }
}
