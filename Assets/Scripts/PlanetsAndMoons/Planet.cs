using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;
using Random = UnityEngine.Random;

public class Planet : MonoBehaviour
{
    // Script for the planet

    // Attachments:
    [SerializeField]
    private GameObject[] attachedObjectTypes; // Pool of object types to attach to the planet
    [SerializeField]
    private GameObject[] attachmentSlots;

    // Rotation:
    [SerializeField]
    private GameObject target; // Point to rotate around
    [SerializeField]
    private float _degPerSec;

    void Start()
    {
        randomAttach();
    }

    void Update()
    {
        transform.RotateAround(target.transform.position, Vector3.forward, _degPerSec * Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {

        // When an enemy hits, get their damage stats and apply them
        if (other.CompareTag("Enemy"))
        {
            EnemyBehaviour enemy = other.gameObject.GetComponent<EnemyBehaviour>();
            if (enemy != null)
            {
                GameControl.control.DamagePlayer(enemy.GetDamageStats());
                Destroy(other.gameObject);
            }        
        }

        if (other.CompareTag("Resource"))
        {
            GameControl.control.CollectResource();
            Destroy(other.gameObject);
        }

        if (other.CompareTag("Intel"))
        {
            GameControl.control.CollectIntel();
            Destroy(other.gameObject);
        }


    }


    // Randomly populate with turrets (TODO change for player selection)
    void randomAttach()
    {
        for (int i = 0; i < attachmentSlots.Length; i++)
        {
            // Instantiate a random selection from the 6 types to predefined locations on the wall
            int randomType = Random.Range(0, attachedObjectTypes.Length);
            AttachToPlanet(i, attachedObjectTypes[randomType]);
        }
    }

    // Add a specified object to the planet at a specified position
    void AttachToPlanet(int position, GameObject objToPlace)
    {
        attachmentSlots[position] = Instantiate(objToPlace, attachmentSlots[position].transform);
    }
}