using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class BallBuff : ScriptableObject
{
    public int damageAdder = 0; //球的傷害
    public int refelctAdder = 0; //反彈次數
    public float speedMultiplier = 1; //球的速度
    public float fireRateMultiplier = 1; //砲台發射頻率
}
