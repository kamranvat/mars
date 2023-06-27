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

    private int _lives = 5;

    void Start()
    {
        randomAttach();
    }

    private void FixedUpdate()
    {
        transform.RotateAround(target.transform.position, Vector3.forward, _degPerSec * Time.deltaTime);
    }


    // Randomly populate with turrets (TODO change for player selection)
    void randomAttach()
    {
        for (int i = 0; i < attachedObjects.Length; i++)
        {
            // Instantiate a random selection from the 6 types to predefined locations on the wall
            int randomType = Random.Range(0, attachedObjectTypes.Length);
            AttachToPlanet(i, attachedObjectTypes[randomType]);
        }
    }

    // Add a specified object to the planet at a specified position
    void AttachToPlanet(int position, GameObject objToPlace)
    {
        Debug.Log("Attaching " + objToPlace);
        attachedObjects[position] = Instantiate(objToPlace, attachmentSlots[position].transform);
    }
}