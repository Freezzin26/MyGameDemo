using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EagleEnemy : MonoBehaviour, IEnemy
{
    public Transform topPoint, bottomPoint;
    public float Speed;
    private AudioSource deathAudio;
    private bool Forward;
    private float topY, bottomY;
    private Rigidbody2D rb;
    private Animator Anim;

    // Start is called before the first frame update
    void Start()
    {
        deathAudio = gameObject.GetComponent<AudioSource>();
        Anim = gameObject.GetComponent<Animator>();
        rb = gameObject.GetComponent<Rigidbody2D>();
        topY = topPoint.transform.position.y;
        bottomY = bottomPoint.transform.position.y;
        Destroy(topPoint.gameObject);
        Destroy(bottomPoint.gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        Move();
    }

    void Move(){
        if (transform.position.y > topY)
        {
            Forward = false;
            //rb.velocity = new Vector2(0, 0);
        }
        else if (transform.position.y < bottomY)
        {
            Forward = true;
            //rb.velocity = new Vector2(0, 0);
        }
        if (Forward){
            rb.velocity = new Vector2(rb.velocity.x, Speed);
            
        }
        else{
            rb.velocity = new Vector2(rb.velocity.x, -Speed);
            
        }
    }
    
    public void Death(){
        Destroy(gameObject);
    }

    public void JumpOn(){
        GetComponent<Collider2D>().enabled = false;
        Anim.SetTrigger("death");
        deathAudio.Play();
    }
}
