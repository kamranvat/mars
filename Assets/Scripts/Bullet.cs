using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField]
    private float _bulletSpeed = 1f;
    [SerializeField]
    private float _range;

    [SerializeField]
    private Rigidbody2D RB;

    private Vector3 direction;
    private Vector3 startingPosition;

    private void Start()
    {
        // Store the starting position of the bullet
        startingPosition = transform.position;
        direction = transform.up;
    }


    void Update()
    {

        transform.Translate(direction * Time.deltaTime * _bulletSpeed);
        MaxRange();
    }


    void MaxRange()
    {
        // Destroy after it reaches this range
        if (Vector3.Distance(startingPosition, transform.position) >= _range)
        {
            Destroy(gameObject);
        }
    }


}
