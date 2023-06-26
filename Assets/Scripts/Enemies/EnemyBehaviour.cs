using System.Collections;
using System.Collections.Generic;
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

    [SerializeField]
    public int _hp = 20;

    [SerializeField] // for debugging only, center of screen
    private Vector2 _center = Vector2.zero;

    [SerializeField]
    private Rigidbody2D RB;

    [SerializeField]
    private float _dmg = 10; // how much damage this enemy causes
    [SerializeField]
    private float _bypass = 0.1f;


    // Update is called once per frame
    void Update()
    {
        Movement();
        
    }

    void Movement()
    {
        // TODO: Convert gameobj transform into target vector 
        // (allows me to set arbitrary things as target)
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
        // Test collisions
        if (other.CompareTag("Bullet"))
        {
            Destroy(other.gameObject);
            handleEnemyDeath();
        }

        if (other.CompareTag("Fuse"))
        {
            Destroy(other.gameObject);
        }

        if (other.CompareTag("Explosion"))
        {
            Destroy(this.gameObject);
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

    private void handleEnemyDeath()
    {
        GameControl.control.enemiesRemaining--;
        Destroy(this.gameObject);
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

    // TODO: Enemy health system

    // TODO: Enemy death 

}
