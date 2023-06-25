using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Bullet : MonoBehaviour
{
    [SerializeField]
    private float _bulletSpeed = 1f;
    [SerializeField]
    private float _range;

    [SerializeField]
    private Rigidbody2D RB;

    [SerializeField]
    private float maxFireAngle = 90;
        
    private Vector2 direction;
    private Vector2 startingPosition;

    private void Start()
    {
        //Aim();
        // Store starting position for MaxRange()
        startingPosition = transform.position;
    }


    void Update()
    {
        transform.position += transform.up * Time.deltaTime * _bulletSpeed;
        MaxRange();
    }

    void MaxRange()
    {
        // Destroy after it reaches this range
        if (Vector2.Distance(startingPosition, (Vector2)transform.position) >= _range)
        {
            Destroy(gameObject);
        }
    }

    void Aim()
    {

        // Aim at closest Enemy
        GameObject closestEnemy = GetClosestEnemyInArc();
        if (closestEnemy != null)
        {
            // Calculate the direction to the closest enemy
            Vector3 direction = closestEnemy.transform.position - transform.position;

            // Rotate the bullet towards the closest object on the z-axis
            Quaternion rotation = Quaternion.Euler(0f, 0f, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f);
            transform.rotation = rotation;
        }

    }

    public GameObject GetClosestEnemyInArc()
    {
        float closest = _range;
        GameObject closestEnemy = null;
        GameObject[] allEnemies = GameObject.FindGameObjectsWithTag("Enemy");
        Debug.Log("get called it wants its enemy back");

        // Iterate through all enemies, compare distances, return the closest one
        foreach (GameObject enemy in allEnemies)
        {
            // use dot product to compare enemy direction with turret direction
            float dot = Vector2.Dot(transform.forward, enemy.transform.position);
            Debug.Log("dot : " + dot);
            float distance = Vector3.Distance(enemy.transform.position, transform.position);
            if ((distance < closest) && (dot < 0))
            {
                closest = distance;
                closestEnemy = enemy;
            }
        }
        return closestEnemy;
    }

    // Gizmos for debugging
    void OnDrawGizmos()
    {
        Color color;
        color = Color.green;
        // local up
        DrawHelperAtCenter(this.transform.up, color, 2f);
    }

    private void DrawHelperAtCenter(
                   Vector3 direction, Color color, float scale)
    {
        Gizmos.color = color;
        Vector3 destination = transform.position + direction * scale;
        Gizmos.DrawLine(transform.position, destination);
    }



}
