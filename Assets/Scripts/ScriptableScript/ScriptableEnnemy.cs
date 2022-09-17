using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/ScriptableEnnemy", order = 1)]
public class ScriptableEnnemy : ScriptableObject
{
    public float costPointInWave = 1f;

    public enum TypeEnnemy
    {
        Ground, Fly 
    }
    
    public enum TypeEnnemyFight
    {
        Cac, Range
    }
    
    public enum TypeEffect
    {
       Defense_Hack, Net_Attack
    }


    public string[] nameEnnemy;

    public float health;

    public int sizeSpotAttack = 1;
    public float valuePrioSpot = 0;




    public float radiusEnnemy = 1;

    public TypeEnnemy type;

    public TypeEnnemyFight typeFight;
    public float range;
    public float attackSpeed;
    public float damage;
    public int areaEffect;

    public List<TypeEffect> effects;

    public float speedMoveInWater;
    public float speedMoveOnRaft;
    public bool canJumpOnRaft;


    public float offsetYSwimming;
    public float offsetYHPBar;

    public int nbAttack;

}
