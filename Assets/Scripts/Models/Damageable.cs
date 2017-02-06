using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Damageable
{
    // FIXME: trusting child classes to assign MAxHEalth to health
    public virtual int Health { get; protected set; }
    public abstract int MaxHealth { get; }


    /// <summary>
    /// Reduce the health. Call the Death function if Health less than 0
    /// </summary>
    /// <param name="amount">The amount to reduce health by</param>
    public virtual void TakeDamage(int amount)
    {
        Health -= amount;
        if(Health <= 0)
        {
            Death();
        }
    }

    /// <summary>
    /// A function that should kill the troop (usually involves calling some callbacks or diabling gameObject
    /// </summary>
    public abstract void Death();
}
