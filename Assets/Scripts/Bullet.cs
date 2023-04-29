using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField]
    private float _bulletSpeed = 20f;
    [SerializeField]
    private GameObject _prefab_sphere;
    [SerializeField]
    private float _range;

    public Vector3 direction;
    private Vector3 startingPosition; // The starting position of the bullet

    private void Start()
    {
        // Store the starting position of the bullet
        startingPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {

        transform.Translate(direction * Time.deltaTime * _bulletSpeed);
        MaxRange();
    }


    void MaxRange()
    {
        // TODO: kill after it reaches the range. hypothenuse
        if (Vector3.Distance(startingPosition, transform.position) >= _range)
        {
            Destroy(gameObject);
            Debug.Log(message: "bullet ded");
        }
    }
}
