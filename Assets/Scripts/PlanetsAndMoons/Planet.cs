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
    public Turret[] turrets; // Pool of turrets to attach to the planet
    [SerializeField]
    private GameObject[] attachmentSlots;

    // Rotation:
    [SerializeField]
    private GameObject target; // Point to rotate around
    [SerializeField]
    private float _degPerSec;

    void Start()
    {
        AttachAllObjects();
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
                GameControl.Instance.DamagePlayer(enemy.GetDamageStats());
                Destroy(other.gameObject);
            }        
        }

        if (other.CompareTag("Resource"))
        {
            GameControl.Instance.CollectResource();
            Destroy(other.gameObject);
        }

        if (other.CompareTag("Intel"))
        {
            GameControl.Instance.CollectIntel();
            Destroy(other.gameObject);
        }


    }

    public void SetTurretLevels(int[] levels)
    {
        for (int i = 0; i < attachmentSlots.Length - 1; i++)
        {
            turrets[i].turretLevel = levels[i];
        }
    }
    void AttachAllObjects()
    {
        for (int i = 0; i < attachmentSlots.Length-1; i++)
        { 
            AttachToPlanet(i, turrets[i].gameObject);
            Debug.Log("attached "+i);
        }
    }

    // Add a specified object to the planet at a specified position
    void AttachToPlanet(int position, GameObject objToPlace)
    {
        attachmentSlots[position] = Instantiate(objToPlace, attachmentSlots[position].transform);
    }
}