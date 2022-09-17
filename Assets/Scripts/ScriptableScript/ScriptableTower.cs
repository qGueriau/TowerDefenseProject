using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/ScriptableTower", order = 2)]
public class ScriptableTower : ScriptableObject
{
    public enum TypeEnnemyTarget
    {
        Ground, Fly, Ground_And_Fly
    }
   
    
    public enum TypeEffect
    {
       Defense_Hack, Net_Attack
    }


    public string[] nameTower;
    public int cost;
   

    public TypeEnnemyTarget type;

    
    public int range;
    public int rangeUI;

    public float attackSpeed;
    public int attackSpeedUI;

    public float damage;
    public int damageUI;

    public float areaEffectRadius;
    public float areaEffectRatioDamageMax;

    public List<TypeEffect> effects;

    public GameObject[] towerGO;
    public GameObject[] projectileGo;
    public bool hasFXEndProjectile = false;
    public GameObject[] FXEndProjectileGo;
    public float[] timeEndProjectileGo;

    public float offsetY;



    public float speedProjectile;
    public bool turnOnlyY;
    public bool orientationCorrection;
    public bool shouldLookAtTarget;
    public bool projectileGravity;

    public bool shouldAlignWithEnnemyToShoot;
    public float speedRotation;

    public float parabolicShootParameter;
}
