using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.WSA;

public class Missile : MonoBehaviour
{
    [SerializeField]
    private float _thrustPower = 0.2f;
    [SerializeField]
    private float _scanRate = 10f;
    [SerializeField]
    private float _range = 50;
    [SerializeField]
    private float _launchPower = 100;
    [SerializeField]
    private float _launchDelay = 1f;
    private float timer = 0;
    private bool timerReached = false;
    [SerializeField]
    private float _missileSpeed = 10f;
    [SerializeField]
    private GameObject _explosionPrefab;


    [SerializeField]
    private Rigidbody2D RB;

    private Vector2 startingPosition;

    private void Start()
    {
        // Store the starting position of the bullet
        startingPosition = transform.position;
        // Launch outwards
        RB.AddForce(startingPosition * _launchPower);
        new WaitForSeconds(_launchDelay);
        StartCoroutine(Aim());
    }


    void Update()
    {
        LaunchDelay(_launchDelay);
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

    IEnumerator Aim()
    {

        float _scanDelay = 1 / _scanRate;

        while (true)
        {
            GameObject closestEnemy = GetClosestEnemy();

            if (closestEnemy != null)
            {

                // Direction to closest enemy
                Vector3 direction = closestEnemy.transform.position - transform.position;

                // Rotate towards direction on the z-axis
                Quaternion rotation = Quaternion.Euler(0f, 0f, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f);
                transform.rotation = rotation;

                yield return new WaitForSeconds(_scanDelay);

            }
            else
            {
                Destroy(gameObject);
                yield return null;
            }

        }

    }

    private void Movement()
    {
        // start moving after a delay
        if (timerReached)
        {
            //transform.position += transform.up * Time.deltaTime * _missileSpeed;
            RB.AddForce(transform.up * _thrustPower);
        }

        // add top speed maybe

    }

    void Explode()
    {
        ParticleSystem exp = GetComponent<ParticleSystem>();
        exp.Play();
        Destroy(gameObject, exp.main.duration);
    }

    private void OnDestroy()
    {
        Instantiate(_explosionPrefab, transform.position, transform.rotation);
        //DestroyExplosion();
    }

    private void LaunchDelay(float delay)
    {
        if (!timerReached)
        {
            timer += Time.deltaTime;
        }

        if (!timerReached && timer > delay)
        {
            timerReached = true;
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
