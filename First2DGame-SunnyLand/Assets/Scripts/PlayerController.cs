using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    public float speed;
    public float jumpForce;
    public LayerMask layer;
    public Collider2D coll;
    public Collider2D displayColl;
    public Transform checkPoint;
    public GameObject clear;
    public float collection;
    public Text cherryNum;
    private int jumpCount;
    public AudioSource jumpAudio, hitAudio, getAudio, endAudio, deadAudio;
    private Animator anim;
    private Rigidbody2D rb;
    private bool isHurt;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!isHurt)
        {
            Move();
        }
        SwitchAnimation();
    }

    void Update()
    {
        #region 跳跃
        if (!anim.GetBool("crouching"))
        {
            if (Input.GetButtonDown("Jump") && jumpCount > 0)
            {
                jumpAudio.Play();
                rb.velocity = new Vector2(rb.velocity.x * Time.deltaTime, jumpForce);
                anim.SetBool("jumping", true);
                anim.SetBool("falling", false);
                jumpCount--;
            }
        }
        #endregion

        #region 下蹲
        if (!Physics2D.OverlapCircle(checkPoint.position, 0.2f, layer))
        {
            if (Input.GetButton("Crouch"))
            {
                anim.SetBool("crouching", true);
                displayColl.enabled = false;
            }
            else
            {
                anim.SetBool("crouching", false);
                displayColl.enabled = true;
            }
        }
        #endregion

        cherryNum.text = collection.ToString();
    }

    void Move()
    {
        #region 移动
        //var horizontal = Input.GetAxis("Horizontal");
        var direction = Input.GetAxisRaw("Horizontal");
        anim.SetFloat("running", Mathf.Abs(direction));
        
        rb.velocity = new Vector2(speed * direction * Time.fixedDeltaTime, rb.velocity.y);
        if (direction != 0)
        {
            transform.localScale = new Vector3(direction, 1, 1);
        }
        #endregion

    }

    void SwitchAnimation()
    {
        anim.SetBool("idle", false);
        if (rb.velocity.y < 0.1f && !coll.IsTouchingLayers(layer))
        {
            anim.SetBool("falling", true);
        }
        if (anim.GetBool("jumping"))
        {
            if (rb.velocity.y < 0)
            {
                anim.SetBool("jumping", false);
                anim.SetBool("falling", true);
            }
        }
        else if (coll.IsTouchingLayers(layer))
        {
            anim.SetBool("falling", false);
            anim.SetBool("idle", true);
            jumpCount = 2;
        }
        //受伤
        if (isHurt)
        {
            anim.SetBool("hurt", true);
            if (Mathf.Abs(rb.velocity.x) < 0.1f)
            {
                isHurt = false;
                anim.SetBool("hurt", false);
                anim.SetBool("idle", true);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        //收集
        if (other.tag == "Collection" && other.IsTouching(coll))
        {
            other.gameObject.GetComponent<Animator>().SetTrigger("Get");
            getAudio.Play();
        }
        //通关条件
        else if (other.tag == "Gem" && other.IsTouching(coll))
        {
            other.gameObject.GetComponent<Animator>().SetTrigger("Get");
            endAudio.Play();
            var pause = GameObject.FindGameObjectWithTag("GameEnd");
            pause.SetActive(false);
            clear.SetActive(true);
            Time.timeScale = 0;
        }
        //死亡
        else if (other.tag == "DeadLine")
        {
            gameObject.GetComponent<AudioSource>().enabled = false;
            deadAudio.Play();
            Invoke("Reload", 1);
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.tag == "Enemy")
        {
            if (anim.GetBool("falling"))
            {
                var enemy = other.gameObject.GetComponent<IEnemy>();
                enemy.JumpOn();
                rb.velocity = new Vector2(rb.velocity.x, jumpForce);
                anim.SetBool("jumping", true);
                anim.SetBool("falling", false);
            }
            else if (transform.position.x < other.gameObject.transform.position.x)
            {
                rb.velocity = new Vector2(-3.5f, rb.velocity.y);
                isHurt = true;
                hitAudio.Play();
            }
            else if (transform.position.x > other.gameObject.transform.position.x)
            {
                rb.velocity = new Vector2(3.5f, rb.velocity.y);
                isHurt = true;
                hitAudio.Play();
            }
        }

    }

    void Reload()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void Count()
    {
        collection++;
    }
}
