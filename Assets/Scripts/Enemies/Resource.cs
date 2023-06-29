using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Resource : MonoBehaviour
{
    private void FixedUpdate()
    {
        Vector2 center = Vector2.zero;
        Vector2 centerDirection = center - (Vector2)(transform.position);

        // Increase pull the closer it gets to the center
        float distance = centerDirection.magnitude;
        float scaledGravity = GameControl.control.gravityStrength * (1f / distance);

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        rb.AddForce(centerDirection.normalized * scaledGravity);
    }
}
