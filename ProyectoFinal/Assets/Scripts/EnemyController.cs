using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour {

    public float velocity = 1f;
    public int health = 1;
    public GameObject bulletPrefab;
    public bool guidedBullets = false;
    public float bulletVelocity = 1;
    public float bulletPeriod = 0.25f;
    public float bulletProbability = 0.25f;
    public int scoreValue = 100;
    public AudioClip shootSound;


    private float nextShootTime = 0.0f;
    private float _velocity;
    private int _health;
    private bool onPlay = false;
    private GameObject player;
    private bool alreadyShoot = false;
    private int _scoreValue = 100;
    private AudioSource audioSource;


    void Start () {
        _velocity = velocity;
        _health = health;
        _scoreValue = scoreValue;
        if (guidedBullets)
        {
            player = GameObject.FindWithTag("Player");
            audioSource = GetComponent<AudioSource>();
        }
    }

    void Update () {
        //Movement 
        GetComponent<Rigidbody2D>().velocity = new Vector2(-transform.up.x, -transform.up.y) * _velocity;

        //Shoot
        if (guidedBullets)
        {
            if (!alreadyShoot)
            {
                ShootGuided();
            }
        }
        else
        {
            Shoot();
        }

    }

    protected void Shoot()
    {
        if (nextShootTime > bulletPeriod)
        {
            float rand = Random.value;
            if (bulletProbability > rand && onPlay)
            {
                GameObject bullet = GameObject.Instantiate(bulletPrefab, new Vector3(this.transform.position.x, this.transform.position.y - 0.15f, this.transform.position.z), Quaternion.Euler(Vector2.down));
                bullet.GetComponent<Rigidbody2D>().velocity = Vector2.down * bulletVelocity;

            }
            nextShootTime = 0;
        }

        nextShootTime += Time.deltaTime;
    }

    protected void ShootGuided()
    {
        if (nextShootTime > bulletPeriod)
        {
            float rand = Random.value;
            if (bulletProbability > rand && onPlay)
            {
                Vector2 diff = player.transform.position - transform.position;
                diff.Normalize();
                float rot_z = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;

                GameObject bullet = GameObject.Instantiate(bulletPrefab, new Vector3(this.transform.position.x, this.transform.position.y - 0.15f, this.transform.position.z), Quaternion.Euler(0f, 0f, rot_z - 90));
                bullet.GetComponent<Rigidbody2D>().velocity = diff.normalized * bulletVelocity;
                _velocity = velocity * 2;
                alreadyShoot = true;
                audioSource.clip = shootSound;
                audioSource.Play();
            }
            nextShootTime = 0;
        }

        nextShootTime += Time.deltaTime;
    }

    public void Damage()
    {
        _health--;
        if (_health <= 0)
        {
            HUDManager.score += _scoreValue;
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == Layers.Player)
        {
            other.GetComponent<PlayerController>().Damage();
            Destroy(gameObject);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.layer == Layers.Bounds)
        {
            if (onPlay) {
                Destroy(gameObject);
            }
            
            onPlay = true;
        }
    }
}
