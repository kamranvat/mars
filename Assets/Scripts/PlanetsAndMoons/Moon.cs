using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Moon : MonoBehaviour
{
    // Script for Deimos and Phobos

    // Attachments:
    GameObject[] attachedObjects = new GameObject[3];
    [SerializeField]
    private GameObject[] attachedObjectTypes; // Your pool of object types to apply to the planet
    [SerializeField]
    private GameObject[] attachmentSlots;

    // Rotation:
    [SerializeField]
    private GameObject target;
    [SerializeField]
    private float _degPerSec;

    void Start()
    {
        AttachAllObjects();
    }

    private void FixedUpdate()
    {
        transform.RotateAround(target.transform.position, Vector3.forward, _degPerSec * Time.deltaTime);
    }


    void AttachAllObjects()
    {
        for (int i = 0; i < attachmentSlots.Length - 1; i++)
        {
            AttachToPlanet(i, attachedObjectTypes[i].gameObject);
            Debug.Log("attached " + i);
        }
    }

    // Add a specified object to the planet at a specified position
    void AttachToPlanet(int position, GameObject objToPlace)
    {
        Debug.Log("Attaching " + objToPlace);
        attachedObjects[position] = Instantiate(objToPlace, attachmentSlots[position].transform);
    }
}