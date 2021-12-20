using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    Animator anim;
    Rigidbody2D rigid;
    SpriteRenderer renderer;

    public float jumpPower;
    public float movePower;

    void Awake() {
        renderer = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        rigid = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        Move();
        Jump();
    }

    Vector3 movement;
    void Move()
    {
        Vector3 moveVelocity = Vector3.zero;

        if (Input.GetAxisRaw("Horizontal") < 0)
        {
            moveVelocity = Vector3.left;
            renderer.flipX = true;
        }

        else if (Input.GetAxisRaw("Horizontal") > 0)
        {
            moveVelocity = Vector3.right;
            renderer.flipX = false;
        }

        transform.position += moveVelocity * movePower * Time.deltaTime;

        if (moveVelocity == Vector3.zero) // isWalking false
            anim.SetBool("isWalking", false);
        else // true
            anim.SetBool("isWalking", true);
    }

    void Jump()
    {
        if(Input.GetKeyDown(KeyCode.Z)) {
            rigid.velocity = Vector2.zero;

            Vector2 jumpVelocity = new Vector2(0, jumpPower);
            rigid.AddForce(jumpVelocity, ForceMode2D.Impulse);
        }
    }
}
