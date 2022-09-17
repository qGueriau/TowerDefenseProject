using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : MonoBehaviour
{
    public ScriptableTower infos;

    public EnnemyScript targetennemy = null;
    float timeSinceLastShoot = 10f;

    List<GameObject> projectiles = new List<GameObject>();
    List<EnnemyScript> projectilesTarget = new List<EnnemyScript>();
    List<Vector3> directionShootProjectile = new List<Vector3>();
    public Transform startProjectile;

    public Transform mobilePart;
    public Transform mobilePartX;

    float angleMobileX = 0f;

    public Animator animator;



    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        RefreshTarget();

        timeSinceLastShoot += Time.deltaTime;

        if (animator != null) animator.SetBool("isShooting", timeSinceLastShoot < 0.1f);

        if (targetennemy!=null)
        {
            float distanceSqrt = Vector3.SqrMagnitude(this.transform.position - targetennemy.transform.position);
            
            if (distanceSqrt < infos.range * infos.range)
            {
                bool canMove = true;

                if(animator!=null)
                {
                    canMove = !animator.GetCurrentAnimatorStateInfo(0).IsName("Shooting");
                }

                if(canMove)
                {
                    float currentAngleMobile = mobilePart.transform.eulerAngles.y;

                    if (infos.turnOnlyY) mobilePart.transform.LookAt(new Vector3(targetennemy.transform.position.x, mobilePart.transform.position.y, targetennemy.transform.position.z));

                    float shouldAngleMonster = mobilePart.transform.eulerAngles.y;
                    if (infos.orientationCorrection)
                    {
                        mobilePart.transform.localEulerAngles = new Vector3(-90f, mobilePart.transform.localEulerAngles.y + 180f, 0f);
                        shouldAngleMonster = mobilePart.transform.eulerAngles.y;
                    }

                    while (shouldAngleMonster - currentAngleMobile > 180f) shouldAngleMonster -= 360f;
                    while (shouldAngleMonster - currentAngleMobile < -180f) shouldAngleMonster += 360f;

                    currentAngleMobile = Mathf.MoveTowards(currentAngleMobile, shouldAngleMonster, infos.speedRotation * Time.deltaTime);

                    if (infos.orientationCorrection) mobilePart.transform.eulerAngles = new Vector3(-90f, 0f, currentAngleMobile);
                    else mobilePart.transform.eulerAngles = new Vector3(mobilePart.transform.eulerAngles.x, currentAngleMobile, mobilePart.transform.eulerAngles.z);

                    if (mobilePartX != null)
                    {
                        angleMobileX = Mathf.MoveTowards(angleMobileX, distanceSqrt * infos.parabolicShootParameter, 2 * infos.speedRotation * Time.deltaTime);
                        mobilePartX.transform.localEulerAngles = new Vector3(angleMobileX, 0, 0);

                    }

                    if (Mathf.Abs(shouldAngleMonster - currentAngleMobile) < 5f)
                    {
                        if (timeSinceLastShoot > 1 / infos.attackSpeed)
                        {
                            if (animator == null)
                            {
                                GameObject projectile = GameObject.Instantiate(infos.projectileGo[0], this.transform);
                                projectiles.Add(projectile);
                                projectilesTarget.Add(targetennemy);
                                if(mobilePartX != null)
                                {
                                    directionShootProjectile.Add(mobilePartX.rotation * Vector3.forward);
                                    if (infos.orientationCorrection) directionShootProjectile.Add(mobilePartX.rotation * Vector3.up);

                                }
                                else directionShootProjectile.Add(Vector3.zero);
                                projectile.transform.position = startProjectile.position;
                            }



                            timeSinceLastShoot = 0f;
                        }
                    }
                }
                

                


            }
        }
        
        for(int i= projectiles.Count-1; i>=0; i--)
        {
            Transform projectile = projectiles[i].transform;
            if(projectilesTarget[i]==null)
            {
                Destroy(projectile.gameObject);
                projectiles.RemoveAt(i);
                directionShootProjectile.RemoveAt(i);
                projectilesTarget.RemoveAt(i);
            }
            else
            {
                EnnemyScript projectileEnnemiTarget = projectilesTarget[i];
                Transform projectileTarget = projectileEnnemiTarget.transform;
                Vector3 projectileDirectionStart = directionShootProjectile[i];
                Vector3 positionToGo = projectilesTarget[i].transform.position;

                if (infos.projectileGravity)
                {
                    float distanceToTower = Vector3.Distance(new Vector3(projectile.position.x, 0, projectile.position.z), new Vector3(startProjectile.position.x, 0, startProjectile.position.z));
                    float distanceToEnnemy = Vector3.Distance(new Vector3(projectile.position.x, 0, projectile.position.z), new Vector3(projectileTarget.position.x, 0, projectileTarget.position.z));

                    float distanceTot = (distanceToTower + distanceToEnnemy);
                    float tBezier = distanceToTower / distanceTot;
                    float MinustBezier = 1-tBezier;

                    positionToGo.y = MinustBezier * MinustBezier * MinustBezier * startProjectile.position.y + (tBezier * MinustBezier * MinustBezier + tBezier * tBezier * MinustBezier) * (startProjectile.position.y +  distanceTot * projectileDirectionStart.y) + tBezier * tBezier * tBezier * projectileTarget.position.y;
                }

                projectile.LookAt(positionToGo);

                projectile.position = Vector3.MoveTowards(projectile.position, positionToGo, infos.speedProjectile * Time.deltaTime);

                if (Vector3.SqrMagnitude(projectile.position - projectileTarget.position) - 0.25f * Mathf.Pow(projectileEnnemiTarget.scriptableEnnemy.radiusEnnemy,2) < 3 * infos.speedProjectile * Time.deltaTime * Time.deltaTime)
                {
                    Destroy(projectile.gameObject);
                    projectilesTarget[i].HP -= infos.damage;

                    if(infos.areaEffectRadius>0)
                    {
                        GameManager.instance.DoDamageArea(projectileTarget.position, infos.damage, infos.areaEffectRadius, infos.areaEffectRatioDamageMax);
                    }

                    if(infos.hasFXEndProjectile)
                    {
                        GameObject FXprojectile = GameObject.Instantiate(infos.FXEndProjectileGo[0]);
                        FXprojectile.transform.position = projectile.position;
                        Destroy(FXprojectile, infos.timeEndProjectileGo[0]);
                    }

                    projectiles.RemoveAt(i);
                    directionShootProjectile.RemoveAt(i);
                    projectilesTarget.RemoveAt(i);

                }
            }
            


        }

    }


    public void RefreshTarget()
    {
        if (targetennemy == null || targetennemy.HP <= 0f || Vector3.SqrMagnitude(this.transform.position - targetennemy.transform.position)>infos.range * infos.range)
        {
            EnnemyScript closestEnnemy = null;
            float distanceMin = 10000000f;

            foreach (EnnemyScript ennemy in GameManager.instance.ennemies)
            {
                if (ennemy.HP > 0f && canAttack(ennemy))
                {
                    float distance = Vector3.SqrMagnitude(ennemy.transform.position - this.transform.position);

                    if (distance < distanceMin)
                    {
                        distanceMin = distance;
                        closestEnnemy = ennemy;
                    }
                }
            }

            targetennemy = closestEnnemy;
        }
    }

    public bool canAttack(EnnemyScript ennemy)
    {
        if(ennemy.scriptableEnnemy.type == ScriptableEnnemy.TypeEnnemy.Fly)
        {
            if (infos.type == ScriptableTower.TypeEnnemyTarget.Fly || infos.type == ScriptableTower.TypeEnnemyTarget.Ground_And_Fly) return true;
        }
        else if (ennemy.scriptableEnnemy.type == ScriptableEnnemy.TypeEnnemy.Ground)
        {
            if (infos.type == ScriptableTower.TypeEnnemyTarget.Ground || infos.type == ScriptableTower.TypeEnnemyTarget.Ground_And_Fly) return true;
        }
        return false;
    }

    public void Shoot()
    {
        GameObject projectile = GameObject.Instantiate(infos.projectileGo[0], this.transform);
        projectiles.Add(projectile);
        projectilesTarget.Add(targetennemy);
        if (mobilePartX != null)
        {
            directionShootProjectile.Add(mobilePartX.rotation * Vector3.forward);
            if(infos.orientationCorrection) directionShootProjectile.Add(mobilePartX.rotation * Vector3.up);

        }
        else directionShootProjectile.Add(Vector3.zero);
        projectile.transform.position = startProjectile.position;
    }
}
