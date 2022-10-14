using MoreMountains.NiceVibrations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] public float health;
    

    public event Action<Enemy> OnDeath;
    public event Action<float> OnDamaged;

    [SerializeField] public GameObject deathEffect;
    [SerializeField] public GameObject damageEffect;


    public virtual void GetDamage(float damage ,Vector3 collisionPos, Action<GameObject> Passing)
    {
        health -= damage;
        if (health < 0)
        {
            OnDeath?.Invoke(this);
         //   MMVibrationManager.Haptic(HapticTypes.Failure, false, true, this);
            var effect = Instantiate(deathEffect);
            effect.transform.position = this.transform.position;
            Passing(this.gameObject);
        }
        else
        {
            var effect = Instantiate(damageEffect);
            effect.transform.position = collisionPos;
        }

        OnDamaged?.Invoke(damage);
    }

  

}
