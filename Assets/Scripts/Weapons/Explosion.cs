using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Explosion : MonoBehaviour
{
    [SerializeField]
    private float _duration = 0.3f;
    [SerializeField]
    private float _damage = 10;
    [SerializeField]
    private float _bypass = 0f;
    [SerializeField]
    private bool _emp = false;

    void Start()
    {
        Invoke("DestroyExplosion", _duration);        
    }

    private void DestroyExplosion()
    {
        Destroy(gameObject);
    }

    public Tuple<float, float, bool> GetDamageStats()
    {
        return Tuple.Create(_damage, _bypass, _emp);
    }
}
