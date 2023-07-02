using System;
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
    private float _damage;
    [SerializeField]
    private float _bypass;
    [SerializeField]
    private bool _emp;

    [SerializeField]
    private Rigidbody2D RB;

    private Vector2 direction;
    private Vector2 startingPosition;

    private void Start()
    {
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

    public void SetDamageStats(Tuple<float, float, bool> damageTuple)
    {
        _damage = damageTuple.Item1;
        _bypass = damageTuple.Item2;
        _emp = damageTuple.Item3;
    }
    public Tuple<float, float, bool> GetDamageStats()
    {
        return Tuple.Create(_damage, _bypass, _emp);
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


}
