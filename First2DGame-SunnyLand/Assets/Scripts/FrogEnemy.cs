using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrogEnemy : MonoBehaviour, IEnemy
{
    private Rigidbody2D rb;
    public Transform leftPoint, rightPoint;
    public LayerMask layer;
    private Animator Anim;
    private Collider2D Coll;
    private float leftX, rightX;
    public float Speed;
    public float jumpForce;
    private bool Face = true;
    private AudioSource audioSource;
    

    // Start is called before the first frame update
    void Start()
    {
        audioSource = gameObject.GetComponent<AudioSource>();
        rb = gameObject.GetComponent<Rigidbody2D>();
        Anim = gameObject.GetComponent<Animator>();
        Coll = gameObject.GetComponent<Collider2D>();
        //transform.DetachChildren();
        leftX = leftPoint.transform.position.x;
        rightX = rightPoint.transform.position.x;
        Destroy(leftPoint.gameObject);
        Destroy(rightPoint.gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        Move();
        SwitchAnim();
    }

    void Move(){
        if (transform.position.x < leftX)
        {
            transform.localScale = new Vector3(-1, 1, 1);
            rb.velocity = new Vector2(0, 0);
            Face = false;
        }
        else if (transform.position.x > rightX)
        {
            transform.localScale = new Vector3(1, 1, 1);
            rb.velocity = new Vector2(0, 0);
            Face = true;
        }
        if (Face){
            if (Coll.IsTouchingLayers(layer)){
                Anim.SetBool("jumping", true);
                rb.velocity = new Vector2(-Speed, jumpForce);
            }else{
                rb.velocity = new Vector2(-Speed, rb.velocity.y);
            }
            
        }
        else{
            if (Coll.IsTouchingLayers(layer)){
                Anim.SetBool("jumping", true);
                rb.velocity = new Vector2(Speed, jumpForce);
            }else{
                rb.velocity = new Vector2(Speed, rb.velocity.y);
            }
            
        }
    }

    void SwitchAnim(){
        if (Anim.GetBool("jumping"))
        {
            if (rb.velocity.y < 0)
            {
                Anim.SetBool("jumping", false);
                Anim.SetBool("falling", true);
            }
        }
        if (Coll.IsTouchingLayers(layer) && Anim.GetBool("falling"))
        {
            Anim.SetBool("falling", false);
        }
    }

    public void Death(){
        Destroy(gameObject);
    }

    public void JumpOn(){
        GetComponent<CircleCollider2D>().enabled = false;
        GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
        Anim.SetTrigger("death");
        audioSource.Play();
    }
}
