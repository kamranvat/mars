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

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per 
    void Update()
    {
        Shoot();
    }

    private void Shoot()
    {
        // Aim at closest Enemy
        GameObject closestEnemy = GetClosestEnemy();
        if (closestEnemy != null)
        {
            // Calculate the direction to the closest enemy
            Vector3 direction = closestEnemy.transform.position - transform.position;
            // Rotate the turret to point at the closest enemy
            transform.rotation = Quaternion.LookRotation(direction);
            Debug.Log(message: "bang");
            // Instantiate a bullet
            Instantiate(_bulletPrefab, transform.position, Quaternion.LookRotation(direction));
        }
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
}
