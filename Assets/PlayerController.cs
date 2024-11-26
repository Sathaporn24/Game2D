using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public float speed = 1f;
    public float jumpSpeed = 9f;
    public float maxSpeed = 10f;
    public float JumpPower = 20f;
    public bool grounded;
    public float jumpRate = 1f;
    public float nextJumpPress = 0.0f;
    public float fireRate = 0.2f;
    public float nextFireRate = 0.0f;
    private Rigidbody2D ridigBody2D;
    private Physics2D physic2D;
    Animator animator;
    public int healthbar = 100;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ridigBody2D = this.gameObject.GetComponent<Rigidbody2D>();
        animator = this.gameObject.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        animator.SetBool("Grounded", true);
        animator.SetFloat("Speed", Mathf.Abs(Input.GetAxis("Horizontal")));
        if (Input.GetAxis("Horizontal") < -0.1f)
        {
            transform.Translate(Vector2.right * speed * Time.deltaTime);
            transform.eulerAngles = new Vector2(0, 180);
        }
        else if (Input.GetAxis("Horizontal") > 0.1f)
        {
            transform.Translate(Vector2.right * speed * Time.deltaTime);
            transform.eulerAngles = new Vector2(0, 0);
        }
        if(Input.GetButtonDown("Jump") && Time.time > nextJumpPress){
            animator.SetBool("Jump",true);
            nextJumpPress = Time.time + jumpRate;
            ridigBody2D.AddForce(jumpSpeed*(Vector2.up * JumpPower));
        }else{
            animator.SetBool("Jump",false);
        }

        if(Input.GetKey(KeyCode.X) && Time.time > nextFireRate){
            nextFireRate = Time.time + fireRate;
            animator.SetBool("Attack1",true);
        }else{
            animator.SetBool("Attack1",false);
        }
    }
}
