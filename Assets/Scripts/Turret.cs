using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Turret : MonoBehaviour
{

    //vars
    [SerializeField]
    private int _range = 100;
    [SerializeField]
    private GameObject _bulletPrefab;
    [SerializeField]
    private float _rateOfFire = 10;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Shoot());
    }

    // Update is called once per 
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
        float _delay = 1 / _rateOfFire;
        while (true)
        {
            // Aim at closest Enemy
            GameObject closestEnemy = GetClosestEnemy();
            if (closestEnemy != null)
            {

                // Calculate the direction to the closest enemy
                Vector3 direction = closestEnemy.transform.position - transform.position;

                // Rotate the turret towards the closest object on the z-axis
                Quaternion rotation = Quaternion.Euler(0f, 0f, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f);
                transform.rotation = rotation;

                // Spawn the bullet slightly above the turret
                Vector3 spawnPos = transform.position + direction.normalized * 2f;
                Instantiate(_bulletPrefab, spawnPos, transform.rotation);

                yield return new WaitForSeconds(_delay);

            }

        }

        //yield return null;

    }
}
