using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;
using Random = UnityEngine.Random;

public class EnemyBehaviour : MonoBehaviour
{
    // Movement:
    [SerializeField]
    private float _thrustPower = 0.02f; // power of thrust
    [SerializeField]
    private float _pushApart = 0.1f; // Enemies getting repelled when spawning on top of each other
    [SerializeField] // for debugging only, center of screen
    private Vector2 _center = Vector2.zero;
    [SerializeField]
    private Rigidbody2D RB;

    // Health:
    [SerializeField]
    private float _shieldHp = 10;
    [SerializeField]
    public float maxHp = 20;
    private float _hp;
    

    // Damage:
    [SerializeField]
    private float _damage = 10; 
    [SerializeField]
    private float _bypass = 0.1f;
    [SerializeField]
    private bool _emp = false;

    // Drops:
    [SerializeField]
    private GameObject Resource;
    private int _resourceAmount;


    private void Start()
    {
        _hp = maxHp;
        _resourceAmount = Mathf.FloorToInt(maxHp / 10);
    }

    private void FixedUpdate()
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
                DamageEnemy(explosion.GetDamageStats());
            } 
        }

        if (other.CompareTag("Planet"))
        {
            
        }

        // If they spawn on top of each other, push each other away so that the groups look nice
        if (other.CompareTag("Enemy"))
        {
            Vector2 direction = other.gameObject.transform.position - transform.position;
            RB.AddForce(-direction.normalized * _pushApart);
            other.gameObject.GetComponent<Rigidbody2D>().velocity += direction.normalized * _pushApart;
        }

        // TODO: enemy health bar

    }

    public float EnemyShield(Tuple<float, float, bool> damageTuple)
    {

        float damage = damageTuple.Item1;
        float bypass = damageTuple.Item2;
        bool emp = damageTuple.Item3;
        // Takes damage and bypass, updates enemies _shieldHp
        // Returns how much hp the enemy loses after shielding

        float bypassAmount = damage * bypass;
        float shieldDamage = damage - bypassAmount;
        float enemyDamage = bypassAmount;

        if (emp)
        {
            // EMP only damages shields
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

    public void DamageEnemy(Tuple<float, float, bool> damageTuple)
    {
        _hp -= EnemyShield(damageTuple);

        if (_hp <= 0)
        {
            HandleEnemyDeath();
        }
    }

    private void HandleEnemyDeath()
    {
        GameControl.control.enemiesRemaining--;
        DropResources();
        Destroy(gameObject);
    }

    private void DropResources()
    {
        for (int i = 0; i < _resourceAmount; i++)
        {
            DropObject(Resource);
        }
    }

    void DropObject(GameObject prefab)
    {
        GameObject newObject = Instantiate(prefab, transform.position, transform.rotation);
        Rigidbody2D rigidbody2D = newObject.GetComponent<Rigidbody2D>();

        // Apply random velocity
        Vector2 randomVelocity = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f));
        rigidbody2D.velocity = randomVelocity;

        // Apply random rotation
        float randomRotation = Random.Range(0f, 360f);
        rigidbody2D.angularVelocity = randomRotation;
    }

    public Tuple<float, float, bool> GetDamageStats()
    {
        return Tuple.Create(_damage, _bypass, _emp);
    }

    // TODO: Enemies that shoot:
    IEnumerator Shoot() 
    {
        // TODO add a shoot coroutine here later
        //float _delay = 1 / rateOfFire;
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
