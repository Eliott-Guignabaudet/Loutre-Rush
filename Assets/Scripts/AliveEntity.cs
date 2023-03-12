using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AliveEntity : MonoBehaviourPunCallbacks
{
    protected float _hp;
    protected float _maxHp;

    public  virtual void TakeDamage(float damageCount)
    {
        _hp -= damageCount;
        if (_hp <= 0)
        {
            Die();
        }
    }

    public virtual void TakeDamage(float damageCount, GameObject launcher)
    {
        _hp -= damageCount;
        if (_hp <= 0)
        {
            Die(launcher);
        }
    }

    public abstract void Die();
    public abstract void Die(GameObject killer);


}
