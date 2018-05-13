using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour {

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == Layers.Enemies)
        {
            if(other.GetComponent<EnemyController>() != null)
            {
                other.GetComponent<EnemyController>().Damage();
            }
            else
            {
                other.GetComponent<BossController>().Damage();
            }
            Destroy(gameObject);
        } else if(other.gameObject.layer == Layers.Player)
        {
            other.GetComponent<PlayerController>().Damage();
            Destroy(gameObject);
        } 
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.layer == Layers.Bounds)
        {
            PoolManager.Despawn(gameObject);
        }
    }
}
