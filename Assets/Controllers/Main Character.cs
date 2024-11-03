using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Controller : MonoBehaviour
{
    Rigidbody2D rigidbody2D;
   // Animator animator;
    public int state = 0;
    public int speed;
    public int JumpHeight = 10;
    public bool isJumping;



    void Start()
    {
    //    animator = GetComponent<Animator>();
        rigidbody2D = GetComponent<Rigidbody2D>();

  //      animator.SetFloat("Move X", 1);
   //     animator.SetFloat("Move Y", -1);

       
    }

    // Update is called once per frame
    void Update()
    {

        
        Vector2 position = transform.position;
        float move = Input.GetAxis("Horizontal");
        position.x= position.x + speed*Time.deltaTime * move;
        transform.position = position;

        if (!isJumping && Input.GetKeyDown(KeyCode.Space))
        {
            isJumping=true;
            rigidbody2D.velocity = Vector3.zero;
            rigidbody2D.AddForce(new Vector2(0, Mathf.Sqrt(-2 * Physics2D.gravity.y * JumpHeight)), ForceMode2D.Impulse);
        }
        

        if (move != 0)
        {
            state = move < 0 ? -1 : 1;
      //      animator.SetFloat("Move X", state);
        //    animator.SetFloat("Move Y", state);
            

        }
        else
        {
         //   animator.SetFloat("Move X", 0);
            

        }
        

    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (isJumping = true) isJumping = false;

    }

}
