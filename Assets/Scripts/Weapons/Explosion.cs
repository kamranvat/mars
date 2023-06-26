using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    [SerializeField]
    private float _duration = 1f;
    [SerializeField]
    private float _damage;

    void Start()
    {
        Invoke("DestroyExplosion", _duration);        
    }

    private void DestroyExplosion()
    {
        Destroy(gameObject);
    }

    public float GetDamage()
    {
        return _damage;
    }

}
