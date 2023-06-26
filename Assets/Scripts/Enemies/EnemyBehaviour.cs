using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class EnemyBehaviour : MonoBehaviour
{
    // Variables
    //[SerializeField]
    //private float _enemySpeed = 1f; // speed to spawn at
    [SerializeField]
    private float _thrustPower = 0.02f; // power of thrust

    [SerializeField]
    private float _pushApart = 0.1f; // Enemies getting repelled when spawning on top of each other

    // Enemy stats:
    [SerializeField]
    private float _shieldHp = 10;
    [SerializeField]
    public float maxHp = 20;
    private float _hp;
    private int _resources;

    [SerializeField] // for debugging only, center of screen
    private Vector2 _center = Vector2.zero;

    [SerializeField]
    private Rigidbody2D RB;

    [SerializeField]
    private float _dmg = 10; // how much damage this enemy causes
    [SerializeField]
    private float _bypass = 0.1f;



    private void Start()
    {
        _hp = maxHp;
        _resources = Mathf.FloorToInt(maxHp / 10);
    }


    void Update()
    {
        Movement();      
    }

    void Movement()
    {
        Vector2 target = _center;

        if (target != null)
        {

            // Calculate the direction and thrust
            Vector2 direction = target - (Vector2)transform.position;
            Vector2 thrust = direction * _thrustPower;

            // Rotate into the direction. 
            Quaternion rotation = Quaternion.Euler(0f, 0f, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f);
            transform.rotation = rotation;

            // Accelerate toward the target
            RB.AddForce(thrust);
        }
    }
    
    // Collision with bullets and planets
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Bullet"))
        {
            Destroy(other.gameObject);
            HandleEnemyDeath();
        }

        if (other.CompareTag("Fuse"))
        {
            Destroy(other.gameObject);
        }

        if (other.CompareTag("Explosion"))
        {
            Explosion explosion = other.GetComponent<Explosion>();
            if (explosion != null)
            {
                float damage = explosion.GetDamage();
                Destroy(this.gameObject);
            } 
        }

        if (other.CompareTag("Planet"))
        {
            GameControl.control.DamagePlayer(_dmg, _bypass);
            Destroy(this.gameObject);
        }

        // If they spawn on top of each other, push each other away so that the groups look nice
        if (other.CompareTag("Enemy"))
        {
            Vector2 direction = other.gameObject.transform.position - transform.position;
            RB.AddForce(-direction.normalized * _pushApart);
            other.gameObject.GetComponent<Rigidbody2D>().velocity += direction.normalized * _pushApart;
        }

        // For now, destroy after collision 
        // TODO: enemy health bar
        //Destroy(gameObject);

    }

    public float EnemyShield(float damage, float bypass, bool emp)
    {
        // Calculate the amount of damage to bypass the shield
        float bypassAmount = damage * bypass;

        // Calculate the amount of damage to take by the shield
        float shieldDamage = damage - bypassAmount;

        float enemyDamage = bypassAmount;

        // Apply EMP effect if enabled
        if (emp)
        {
            // Take all the damage by the shield
            _shieldHp -= damage;

            if (_shieldHp <= 0)
            {
                _shieldHp = 0;
            }

            return 0;

        }
        else
        {
            if (shieldDamage >= _shieldHp)
            {
                _shieldHp = 0;
                // TODO reset recharge timer
                enemyDamage += shieldDamage - _shieldHp;
            }
            else if (shieldDamage < _shieldHp)
            {
                _shieldHp = _shieldHp - shieldDamage;
            }

            return enemyDamage;
        }


    }

    public void DamageEnemy(float damage, float bypass, bool emp)
    {
        _hp -= EnemyShield(damage, bypass, emp);

        if (_hp <= 0)
        {
            HandleEnemyDeath();
        }
    }

    private void HandleEnemyDeath()
    {
        GameControl.control.enemiesRemaining--;
        Destroy(gameObject);
    }

    // TODO: Enemies that shoot:
    IEnumerator Shoot() 
    {
        // TODO add a shoot coroutine here later
        //float _delay = 1 / _rateOfFire;
        Vector2 target = _center;

        while (GameControl.control.isPlayerAlive)
        {
            // Aim at center of screen
            Debug.Log(message: "target aquired: " + target);
            if (target != null)
            {

                // shoot




                yield return new WaitForSeconds(1);

            }

        }
    }
}
