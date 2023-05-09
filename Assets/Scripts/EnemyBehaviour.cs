using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class EnemyBehaviour : MonoBehaviour
{
    // Variables
    [SerializeField]
    private float _enemySpeed = 1f;
    [SerializeField]
    private float _thrustPower = 0.1f; // power of thrust

    [SerializeField]
    private float _pushApart = 0.1f; // Enemies pushing each other apart

    [SerializeField]
    public int _hp = 20;
    [SerializeField] // for debugging only
    private Vector2 _center = Vector2.zero;
    [SerializeField]
    private Rigidbody2D RB;

    //private Rigidbody2D RB;


    // Start is called before the first frame update
    void Start()
    {
        //StartCoroutine(Accelerate());
    }

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
            // TODO: find a simpler way
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
            Destroy(this.gameObject);
            // Debug.Log("ded");

        }

        if (other.CompareTag("Planet"))
        {
            //other.GetComponent<PlayerScript>().Damage();
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

    // TODO: Enemies that shoot:
    IEnumerator Shoot() 
    {
        // TODO add a shoot coroutine here later
        //float _delay = 1 / _rateOfFire;
        Vector2 target = _center;

        while (true)
        {
            // Aim at center of screen
            Debug.Log(message: "target aquired: " + target);
            if (target != null)
            {

                // shoot




                //yield return new WaitForSeconds(_delay);

            }

        }
    }

    // TODO: Enemy health system

    // TODO: Enemy death 

}
