using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class PlayerController : MonoBehaviour {

    public float velocity = 1f;
    public int health = 3;
    public GameObject bulletPrefab;
    public float bulletVelocity = 1;
    public float bulletPeriod = 0.5f;
    public Text healthUI;
    public GameObject deathMenuUI;
    public AudioClip shootSound;


    protected float nextShootTime = 0.0f;
    protected float _velocity;
    protected int _health;
    protected float upBound, downBound, leftBound, rightBound;
    protected float spriteOffSetOnX, spriteOffSetOnY;
    protected float velocityOnX, velocityOnY;
    protected bool alive = true;
    protected Rigidbody2D rb;
    protected SpriteRenderer sr;
    protected Animator animator;
    protected AudioSource audioSource;


    protected virtual void Start () {
		_velocity = velocity;
        _health = health;

        healthUI.text = _health.ToString();

        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();

        spriteOffSetOnX = sr.size.x;
        spriteOffSetOnY = sr.size.y;

        upBound = GameObject.FindGameObjectsWithTag("UpBound")[0].transform.position.y;
        downBound = GameObject.FindGameObjectsWithTag("DownBound")[0].transform.position.y;
        leftBound = GameObject.FindGameObjectsWithTag("LeftBound")[0].transform.position.x;
        rightBound = GameObject.FindGameObjectsWithTag("RightBound")[0].transform.position.x;

        audioSource = GetComponent<AudioSource>();
    }

    protected virtual void Update () {
        if(alive){
            //Movement 
            //X
            if(transform.position.x - spriteOffSetOnX <= leftBound)
            {
                if (Input.GetAxisRaw("Horizontal") < 0)
                {
                    velocityOnX = 0;
                }
                else
                {
                    velocityOnX = Input.GetAxisRaw("Horizontal");
                }
            }
            else if (transform.position.x + spriteOffSetOnX >= rightBound)
            {
                if (Input.GetAxisRaw("Horizontal") > 0)
                {
                    velocityOnX = 0;
                }
                else
                {
                    velocityOnX = Input.GetAxisRaw("Horizontal");
                }
            }
            else
            {
                velocityOnX = Input.GetAxisRaw("Horizontal");
            }

            //Y
            if (transform.position.y - spriteOffSetOnY <= downBound)
            {
                if (Input.GetAxisRaw("Vertical") < 0)
                {
                    velocityOnY = 0;
                }
                else
                {
                    velocityOnY = Input.GetAxisRaw("Vertical");
                }
            }
            else if (transform.position.y + spriteOffSetOnY >= upBound)
            {
                if (Input.GetAxisRaw("Vertical") > 0)
                {
                    velocityOnY = 0;
                }
                else
                {
                    velocityOnY = Input.GetAxisRaw("Vertical");
                }
            }
            else
            {
                velocityOnY = Input.GetAxisRaw("Vertical");
            }

            rb.velocity = new Vector2(velocityOnX, velocityOnY) * _velocity;

            //Animation
            if (Input.GetAxisRaw("Horizontal") > 0)
            {
                animator.SetBool("Right", true);
                animator.SetBool("Left", false);
            }
            else if (Input.GetAxisRaw("Horizontal") < 0)
            {
                animator.SetBool("Left", true);
                animator.SetBool("Right", false);
            }
            else
            {
                animator.SetBool("Right", false);
                animator.SetBool("Left", false);
            }

            //Shoot
            if (Input.GetKey(KeyCode.Space))
            {
                if (nextShootTime > bulletPeriod)
                {
                    GameObject bullet = PoolManager.Instantiate(bulletPrefab, new Vector3(this.transform.position.x,this.transform.position.y + 0.15f,this.transform.position.z));
                    bullet.GetComponent<Rigidbody2D>().velocity = Vector2.up * bulletVelocity;
                    nextShootTime = 0;
                    audioSource.clip = shootSound;
                    audioSource.Play();
                }
            }
            nextShootTime += Time.deltaTime;
        } else{
            rb.velocity = Vector2.zero;
        }
    }

    public void Damage()
    {
        if(alive){
            if (_health <= 0)
            {
                alive = false;
                animator.SetTrigger("Dead");
                StartCoroutine(WaitForDeathAnimation());
            } else{
                _health--;
                healthUI.text = _health.ToString();
                animator.SetTrigger("Damaged");
            }
        }

    }

    protected IEnumerator WaitForDeathAnimation()
    {

        yield return new WaitForSeconds(1.0f);
        Destroy(gameObject);
        deathMenuUI.SetActive(true);
        Time.timeScale = 0f;


    }

    void OnDestroy()
    {
        PlayerPrefs.SetFloat("highscore", HUDManager.highScore);
        PlayerPrefs.Save();
    }
}
