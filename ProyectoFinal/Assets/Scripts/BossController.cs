using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossController : MonoBehaviour
{

    public float velocity = 1f;
    public int health = 1;
    public GameObject bulletPrefab;
    public float bulletVelocity = 1;
    public float bulletPeriod = 0.25f;
    public GameObject missilePrefab;
    public float missileVelocity = 1;
    public float missilePeriod = 0.25f;
    public float offset = 20;
    public int scoreValue = 100;
    public AudioClip shootSound;


    private float nextShootTime = 0.0f;
    private float nextMissileTime = 0.0f;
    private float _velocity;
    private int _health;
    private float upBound;
    private int _scoreValue = 100;
    private bool damaged;
    private bool alive = true;
    private float nextDamageTime = 0.0f;
    private float damagedPeriod = 0.2f;
    private GameObject player;
    private AudioSource audioSource;
    private SpriteRenderer sr;
    private Animator animator;
    private Transform bulletSpawn, bulletSpawn1, missileSpawn;

    void Start()
    {
        _velocity = velocity;
        _health = health;
        _scoreValue = scoreValue;

        player = GameObject.FindWithTag("Player");
        audioSource = GetComponent<AudioSource>();
        sr = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();

        bulletSpawn = transform.Find("BulletSpawn");
        Debug.Log(bulletSpawn);
        bulletSpawn1 = transform.Find("BulletSpawn1");
        missileSpawn = transform.Find("MissileSpawn");


        upBound = GameObject.FindGameObjectsWithTag("UpBound")[0].transform.position.y;
    }

    void Update()
    {
        if(alive)
        {
            if (transform.position.y >= upBound - offset)
            {
                //Movement 
                GetComponent<Rigidbody2D>().velocity = new Vector2(-transform.up.x, -transform.up.y) * _velocity;
            }
            else
            {
                GetComponent<Rigidbody2D>().velocity = Vector2.zero;

                //Shoot
                ShootGuided();
                Shoot();
            }

            if (damaged)
            {
                if (nextDamageTime > damagedPeriod)
                {
                    damaged = false;
                    nextDamageTime = 0;
                }
                nextDamageTime += Time.deltaTime;
            }
            else
            {
                sr.color = Color.white;
            }
        } else
        {
            GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        }
    }

    protected void Shoot()
    {
        if (nextShootTime > bulletPeriod)
        {
            GameObject bullet = GameObject.Instantiate(bulletPrefab, bulletSpawn.position, Quaternion.Euler(Vector2.down));
            bullet.GetComponent<Rigidbody2D>().velocity = Vector2.down * bulletVelocity;
            bullet = GameObject.Instantiate(bulletPrefab, bulletSpawn1.position, Quaternion.Euler(Vector2.down));
            bullet.GetComponent<Rigidbody2D>().velocity = Vector2.down * bulletVelocity;
            nextShootTime = 0;
        }

        nextShootTime += Time.deltaTime;
    }

    protected void ShootGuided()
    {
        if (nextMissileTime > bulletPeriod)
        {
            Vector2 diff = player.transform.position - transform.position;
            diff.Normalize();
            float rot_z = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;

            GameObject missile = GameObject.Instantiate(missilePrefab, missileSpawn.position, Quaternion.Euler(0f, 0f, rot_z - 90));
            missile.GetComponent<Rigidbody2D>().velocity = diff.normalized * missileVelocity;

            audioSource.clip = shootSound;
            audioSource.Play();
            nextMissileTime = 0;
        }

        nextMissileTime += Time.deltaTime;
    }

    public void Damage()
    {
        if(alive)
        {
            if (_health <= 0)
            {
                alive = false;
                HUDManager.score += _scoreValue;
                animator.SetTrigger("Dead");
                StartCoroutine(WaitForDeathAnimation());
            } else
            {
                _health--;
                damaged = true;
                sr.color = Color.red;
            }
        }
 
    }

    private IEnumerator WaitForDeathAnimation()
    {

        yield return new WaitForSeconds(0.75f);
        Destroy(gameObject);

    }

    public int GetHealth()
    {
        return _health;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == Layers.Player)
        {
            other.GetComponent<PlayerController>().Damage();
        }
    }
}
