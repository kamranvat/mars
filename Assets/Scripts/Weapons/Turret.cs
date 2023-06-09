using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Turret : MonoBehaviour
{

    // This was intended to load different types of turrets, but I ran out of time so there is only one type
    public TurretType turretType = TurretType.Normal;

    public int turretLevel = 0;
    [SerializeField]
    private int range = 60;
    [SerializeField]
    private GameObject _bulletLevel0;
    [SerializeField]
    private GameObject _bulletLevel1;
    [SerializeField]
    private GameObject _bulletLevel2;
    private GameObject bullet;
    [SerializeField]
    private float rateOfFire = 10;
    [SerializeField]
    private float _rateOfScan = 10; // if no enemy is present, how often per s should we check for enemies

    void Start()
    {
        SetStats();
        StartCoroutine(Shoot());
    }

    void Update()
    {
    }


    public GameObject GetClosestEnemyInArc()
    {
        float closest = range;
        GameObject closestEnemy = null;
        GameObject[] allEnemies = GameObject.FindGameObjectsWithTag("Enemy");

        // Iterate through all enemies, compare distances, return the closest one
        foreach (GameObject enemy in allEnemies)
        {  
            // use dot product to compare enemy direction with turret direction
            // (avoids shooting through planet)
            float dot = Vector2.Dot(transform.position, enemy.transform.position);
            float distance = Vector3.Distance(enemy.transform.position, transform.position);
            if ((distance < closest) && (dot > 0)) 
            {
                closest = distance;
                closestEnemy = enemy;
            }
        }
        return closestEnemy;
    }

    private void SetStats()
    {
        switch (turretLevel)
        {
            case 0:
                range = 10;
                bullet = _bulletLevel0;
                rateOfFire = 4;
                break;
            case 1:
                range = 15;
                bullet = _bulletLevel1;
                rateOfFire = 7;
                break;
            case 2:
                range = 25;
                bullet = _bulletLevel2;
                rateOfFire = 11;
                break;
            default:
                break;
        }
    }

    public void SetTurretLevel(int level)
    {
        turretLevel = level;
    }


    IEnumerator Shoot()
    {
        float _fireDelay = 1 / rateOfFire;
        float _scanDelay = 1 / _rateOfScan;

        while (true)
        {
            // Aim at closest Enemy
            GameObject closestEnemy = GetClosestEnemyInArc();
            if (closestEnemy != null)
            {

                // Calculate the direction to the closest enemy
                Vector3 direction = closestEnemy.transform.position - transform.position;

                // Rotate the turret towards the closest object on the z-axis
                Quaternion rotation = Quaternion.Euler(0f, 0f, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f);
                transform.rotation = rotation;

                // Spawn bullet above turret
                Vector3 spawnPos = transform.position + transform.forward.normalized;
                Instantiate(bullet, spawnPos, transform.rotation);

                yield return new WaitForSeconds(_fireDelay);

            }
            else 
            {
                yield return new WaitForSeconds(_scanDelay);
            }


        }

    } 

    
    IEnumerator ShootAtCursor()
    {
        float _fireDelay = 1 / rateOfFire;
        float _scanDelay = 1 / _rateOfScan;

        while (true)
        {
            Vector3 direction = Input.mousePosition;
            direction.z = -Camera.main.transform.position.z;
            direction = Camera.main.ScreenToWorldPoint(direction) - transform.position;
            transform.rotation = Quaternion.LookRotation(transform.forward, direction);

            Vector3 spawnPos = transform.position + transform.forward.normalized;
            Instantiate(bullet, spawnPos, transform.rotation);

            yield return new WaitForSeconds(_fireDelay);

        }

    }
}
