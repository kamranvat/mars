using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Missile : MonoBehaviour
{
    [SerializeField]
    private float _thrustPower = 1f;
    [SerializeField]
    private float _scanRate = 10f;
    [SerializeField]
    private float _range;

    [SerializeField]
    private Rigidbody2D RB;

    private Vector2 startingPosition;

    private void Start()
    {
        // Store the starting position of the bullet
        startingPosition = transform.position;

    }


    void Update()
    {
        Movement();
        MaxRange();
    }

    public GameObject GetClosestEnemy()
    {
        float closest = _range;
        GameObject closestEnemy = null;
        GameObject[] allEnemies = GameObject.FindGameObjectsWithTag("Enemy");

        // Iterate through all enemies, compare distances, return the closest one
        foreach (GameObject enemy in allEnemies)
        {
            float distance = Vector3.Distance(enemy.transform.position, transform.position);
            if (distance < closest)
            {

                closest = distance;
                closestEnemy = enemy;
            }
        }
        return closestEnemy;
    }

    private void Movement()
    {
        // TODO: Convert gameobj transform into target vector 
        // (allows me to set arbitrary things as target)

        float _scanDelay = 1 / _scanRate;

        // Aim at closest Enemy
        GameObject closestEnemy = GetClosestEnemy();

        if (closestEnemy != null)
        {

            // Calculate the direction to the closest enemy
            Vector3 direction = closestEnemy.transform.position - transform.position;

            // Rotate  towards the closest object on the z-axis
            // TODO: convert this all to a vector2 thing and make the following line simpler
            Quaternion rotation = Quaternion.Euler(0f, 0f, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f);
            transform.rotation = rotation;

            RB.AddForce(direction * _thrustPower);
    
            new WaitForSeconds(_scanDelay);

        }
        else
        {
            Destroy(gameObject);
        }

        

    }

    void MaxRange()
    {
        // Destroy after it reaches this range
        if (Vector2.Distance(startingPosition, (Vector2)transform.position) >= _range)
        {
            Destroy(gameObject);
        }
    }
}
