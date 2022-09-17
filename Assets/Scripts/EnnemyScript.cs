using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnnemyScript : MonoBehaviour
{

    public ScriptableEnnemy scriptableEnnemy;
    public float HP;

    public RaftTile raftTileToAttack = null;
    public SpotEnnemiAttack spotEnnemiAttack = null;
    public List<SpotEnnemiAttack> spotEnnemisAll;

    float timeRefreshTarget = 0;
    float timeBetweenRefreshTarget = 1.5f;

    bool isInWater = true;

    float timeSinceLastAttack = 10f;

    public Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        HP = scriptableEnnemy.health;
        spotEnnemisAll = new List<SpotEnnemiAttack>();
    }

    // Update is called once per frame
    void Update()
    {
        timeRefreshTarget += Time.deltaTime;
        if(timeRefreshTarget> timeBetweenRefreshTarget || raftTileToAttack == null || raftTileToAttack.hp<=0f || spotEnnemiAttack==null || spotEnnemiAttack.raftTile==null || spotEnnemiAttack.raftTile.hp<=0f || raftTileToAttack!= spotEnnemiAttack.raftTile)
        {
            timeRefreshTarget = Random.Range(0f, 0.5f);
            RefreshTarget();
        }

        if(HP>0)
        {
            timeSinceLastAttack += Time.deltaTime;
            if (timeSinceLastAttack > 0.5f) animator.SetInteger("Attack", 0);
            if (raftTileToAttack!=null && spotEnnemiAttack!=null && spotEnnemiAttack.raftTile == raftTileToAttack)
            {
                Vector3 positionRaftSpotToAttack = spotEnnemiAttack.GetVector3();
                positionRaftSpotToAttack.y = this.transform.position.y;

                if (Vector3.SqrMagnitude(positionRaftSpotToAttack - this.transform.position) < scriptableEnnemy.range * scriptableEnnemy.range)
                {
                    this.transform.LookAt(new Vector3(raftTileToAttack.transform.position.x, this.transform.position.y, raftTileToAttack.transform.position.z));

                    if (timeSinceLastAttack > 1 / scriptableEnnemy.attackSpeed)
                    {
                        int attackToDo = 1 + Random.Range(0, scriptableEnnemy.nbAttack);
                        timeSinceLastAttack = 0f;
                        animator.SetInteger("Attack", attackToDo);
                    }
                }
                else
                {
                    if (isInWater)
                    {
                        Vector3 positionRaftSpot = spotEnnemiAttack.GetVector3();

                        positionRaftSpot = new Vector3(positionRaftSpot.x, scriptableEnnemy.offsetYSwimming, positionRaftSpot.z);

                        float distanceToGoodPositionMiddle = positionRaftSpot.x * positionRaftSpot.x + positionRaftSpot.z * positionRaftSpot.z;
                        float distanceToMiddle = this.transform.position.x * this.transform.position.x + this.transform.position.z * this.transform.position.z;

                        Vector3 positionToGo = Vector3.zero;

                        Debug.Log((positionRaftSpot.x * this.transform.position.x + positionRaftSpot.z * this.transform.position.z) / Mathf.Sqrt(distanceToGoodPositionMiddle * distanceToMiddle));

                        if((positionRaftSpot.x * this.transform.position.x + positionRaftSpot.z * this.transform.position.z) / Mathf.Sqrt(distanceToGoodPositionMiddle * distanceToMiddle) > 0.8f)
                        {
                            positionToGo = positionRaftSpot;
                        }
                        else
                        {
                            float angleAroundNow = Mathf.Atan2(this.transform.position.z, this.transform.position.x);
                            float angleAroundSpotPosition = Mathf.Atan2(positionRaftSpot.z, positionRaftSpot.x);

                            if (angleAroundNow - angleAroundSpotPosition > Mathf.PI/2f) angleAroundNow -= Mathf.PI;
                            else if (angleAroundNow - angleAroundSpotPosition < -Mathf.PI / 2f) angleAroundNow += Mathf.PI;

                            float angleMiddle = (angleAroundNow + angleAroundSpotPosition) / 2f;

                            positionToGo = Mathf.Sqrt(distanceToMiddle) * new Vector3(Mathf.Cos(angleMiddle), 0, Mathf.Sin(angleMiddle));

                        }
                        
                        positionToGo = new Vector3(positionToGo.x, scriptableEnnemy.offsetYSwimming, positionToGo.z);
                        this.transform.position = Vector3.MoveTowards(new Vector3(this.transform.position.x, scriptableEnnemy.offsetYSwimming, this.transform.position.z), positionToGo, scriptableEnnemy.speedMoveInWater * Time.deltaTime);

                        Quaternion beforeRotation = this.transform.rotation;
                        this.transform.LookAt(positionToGo);
                        this.transform.rotation = Quaternion.Slerp(beforeRotation, this.transform.rotation, 3*Time.deltaTime);
                    }
                }
            }
            else
            {
                float distanceToMiddle = this.transform.position.x * this.transform.position.x + this.transform.position.z * this.transform.position.z;
                Vector3 positionToGo = this.transform.position.normalized * Raft.instance.radiusMaxRaft;
                positionToGo = new Vector3(positionToGo.x, scriptableEnnemy.offsetYSwimming, positionToGo.z);

                this.transform.position = Vector3.MoveTowards(new Vector3(this.transform.position.x, scriptableEnnemy.offsetYSwimming, this.transform.position.z), positionToGo, scriptableEnnemy.speedMoveInWater * Time.deltaTime);

                Quaternion beforeRotation = this.transform.rotation;
                this.transform.LookAt(positionToGo);
                this.transform.rotation = Quaternion.Slerp(beforeRotation, this.transform.rotation, 3 * Time.deltaTime);
            }
        }

        

        if(HP<=0)
        {
            animator.SetBool("Death", true);
            Destroy(this.gameObject, 5f);
            GameManager.instance.ennemies.Remove(this);
            if (spotEnnemiAttack != null) spotEnnemiAttack.ennemyCurrent = null;
        }

    }

    public void RefreshTarget()
    {
        GetOutPrio();

        SpotEnnemiAttack closestRaftTileSpot = null;
        float distanceMin = 10000000f;

        foreach(RaftTile raftTile in Raft.instance.allRaftTiles)
        {
            if(raftTile.hp>0f)
            {
                List<SpotEnnemiAttack> spotEnnemis = raftTile.GetListSpot();

                foreach(SpotEnnemiAttack spotEnnemy in spotEnnemis)
                {
                    float distance = Vector3.SqrMagnitude(raftTile.transform.position - this.transform.position);

                    if (distance < distanceMin)
                    {
                        bool isOkSpot = true;

                        SpotEnnemiAttack spotToCheck = spotEnnemy;
                        for (int i=0; i<= scriptableEnnemy.sizeSpotAttack && isOkSpot; i++)
                        {
                            if(spotToCheck.ennemyCurrent!=null)
                            {
                                if(scriptableEnnemy.valuePrioSpot<= spotToCheck.ennemyCurrent.scriptableEnnemy.valuePrioSpot)
                                {
                                    isOkSpot = false;
                                }
                            }


                            if (spotToCheck.spotEnnemyNext != null) spotToCheck = spotToCheck.spotEnnemyNext;
                        }

                        spotToCheck = spotEnnemy;
                        for (int i = 0; i <= scriptableEnnemy.sizeSpotAttack && isOkSpot; i++)
                        {
                            if (spotToCheck.ennemyCurrent != null)
                            {
                                if (scriptableEnnemy.valuePrioSpot <= spotToCheck.ennemyCurrent.scriptableEnnemy.valuePrioSpot)
                                {
                                    isOkSpot = false;
                                }
                            }


                            if (spotToCheck.spotEnnemyPrev != null) spotToCheck = spotToCheck.spotEnnemyPrev;
                        }


                        if(isOkSpot)
                        {
                            distanceMin = distance;
                            closestRaftTileSpot = spotEnnemy;
                        }
                        
                    }
                }

                
            }
        }
        spotEnnemisAll.Clear();

        if(closestRaftTileSpot!=null)
        {
            SpotEnnemiAttack spotToCheckEnd = closestRaftTileSpot;
            for (int i = 0; i <= scriptableEnnemy.sizeSpotAttack; i++)
            {
                if (spotToCheckEnd.ennemyCurrent != null)
                {
                    spotToCheckEnd.ennemyCurrent.GetOutPrio();
                }
                spotToCheckEnd.ennemyCurrent = this;
                spotEnnemisAll.Add(spotToCheckEnd);
                spotToCheckEnd = spotToCheckEnd.spotEnnemyNext;
            }

            spotToCheckEnd = closestRaftTileSpot;
            for (int i = 0; i <= scriptableEnnemy.sizeSpotAttack; i++)
            {
                if (spotToCheckEnd.ennemyCurrent != null)
                {
                    spotToCheckEnd.ennemyCurrent.GetOutPrio();
                }
                spotToCheckEnd.ennemyCurrent = this;
                if (!spotEnnemisAll.Contains(spotToCheckEnd)) spotEnnemisAll.Add(spotToCheckEnd);

                spotToCheckEnd = spotToCheckEnd.spotEnnemyPrev;
            }


            spotEnnemiAttack = closestRaftTileSpot;
            raftTileToAttack = spotEnnemiAttack.raftTile;
        }
        else
        {
            spotEnnemiAttack = null;
            raftTileToAttack = null;
        }

        
    }


    public void DoDamage(int attackID)
    {
       if(raftTileToAttack!=null)
        {
            Raft.instance.DoDamage(raftTileToAttack, scriptableEnnemy.areaEffect, scriptableEnnemy.damage);
        }
    }

    public void GetOutPrio()
    {
        foreach(SpotEnnemiAttack spotEnnemi in spotEnnemisAll)
        {
            spotEnnemi.ennemyCurrent = null;
        }
        spotEnnemisAll.Clear();
        spotEnnemiAttack = null;
        raftTileToAttack = null;
    }
}
