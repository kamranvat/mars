using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class RailgunRound : MonoBehaviour
{
    [SerializeField]
    private float _bulletSpeed = 10f;
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


}
