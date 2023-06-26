using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{

    private float _duration = 1f;
    public float damage;

    void Start()
    {
        Invoke("DestroyExplosion", _duration);        
    }

    private void DestroyExplosion()
    {
        Destroy(gameObject);
    }

}
