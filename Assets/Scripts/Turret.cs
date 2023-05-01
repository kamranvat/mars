using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Turret : MonoBehaviour
{

    //vars
    [SerializeField]
    private int _range = 100;
    [SerializeField]
    private GameObject _bulletPrefab;
    [SerializeField]
    private float _rateOfFire = 10;
    [SerializeField]
    private float _rateOfScan = 10; // if no enemy is present, how often per s should we check for enemies

    void Start()
    {
        StartCoroutine(Shoot());
    }

    void Update()
    {
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

    IEnumerator Shoot()
    {
        float _fireDelay = 1 / _rateOfFire;
        float _scanDelay = 1 / _rateOfScan;

        while (true)
        {
            // Aim at closest Enemy
            GameObject closestEnemy = GetClosestEnemy();
            if (closestEnemy != null)
            {

                // Calculate the direction to the closest enemy
                Vector3 direction = closestEnemy.transform.position - transform.position;

                // Rotate the turret towards the closest object on the z-axis
                // TODO: convert this all to a vector2 thing and make the following line simpler
                Quaternion rotation = Quaternion.Euler(0f, 0f, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f);
                transform.rotation = rotation;

                // Spawn the bullet slightly above the turret
                //Vector3 spawnPos = transform.position + direction.normalized * 2f;

                // Spawn the bullet in the turret for now - to avoid bad aim
                // Note: aim is still bad. TODO find out why
                Vector3 spawnPos = transform.position;
                Instantiate(_bulletPrefab, spawnPos, transform.rotation);

                yield return new WaitForSeconds(_fireDelay);

            }
            else 
            {
                Debug.Log(message: "No Enemy found");
                yield return new WaitForSeconds(_scanDelay);
            }


        }

    }
}
