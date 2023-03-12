using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : AliveEntity
{
    private float _xpGain;

    private void Awake()
    {
        _maxHp = 100;
        _hp = _maxHp;
        _xpGain = 100;
    }

    public override void Die()
    {
        Destroy( this.gameObject );
    }

    public override void Die(GameObject killer)
    {
        if (killer.GetComponent<PlayerController>() != null)
        {
            killer.GetComponent<PlayerController>().xp += _xpGain;
        }
        Die();
    }
}
