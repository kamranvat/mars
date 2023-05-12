using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileTurret : MonoBehaviour
{
    //vars
    [SerializeField]
    private int _range = 59;
    [SerializeField]
    private GameObject _bulletPrefab;
    [SerializeField]
    private float _rateOfFire = 1;
    [SerializeField]
    private float _rateOfScan = 1; // if no enemy is present, how often per s should we check for enemies

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

                // Spawn bullet above turret
                Vector3 spawnPos = transform.position;
                Instantiate(_bulletPrefab, spawnPos, transform.rotation);

                yield return new WaitForSeconds(_fireDelay);

            }
            else
            {
                yield return new WaitForSeconds(_scanDelay);
            }


        }

    }
}
