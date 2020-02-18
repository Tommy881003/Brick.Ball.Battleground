using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu]
public class BallProperty : ScriptableObject
{
    public int damage = 1; //球的傷害(原本是1)
    public int reflect = 3; //球的反彈
    public float speed = 25; //球的速度
    public float fireRate = 12.5f; //砲台發射頻率

    public void Initialize()
    {
        BallProperty Base = CreateInstance<BallProperty>();
        damage = Base.damage;
        reflect = Base.reflect;
        speed = Base.speed;
        fireRate = Base.fireRate;
    }
    public void SetValue(BallBuff buff)
    {
        damage += buff.damageAdder;
        reflect += buff.refelctAdder;
        speed *= buff.speedMultiplier;
        if (buff.damageAdder == 0)
            fireRate *= buff.fireRateMultiplier;
        else
            fireRate *= Mathf.Min(0.55f + 0.1f * damage, 1f);
    }
}
