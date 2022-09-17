using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpotEnnemiAttack
{
    public EnnemyScript ennemyCurrent;

    public SpotEnnemiAttack spotEnnemyPrev;
    public SpotEnnemiAttack spotEnnemyNext;

    public RaftTile raftTile;

    public SpotEnnemiAttack (RaftTile raftTile)
    {
        this.raftTile = raftTile;
    }

    public Vector3 GetVector3()
    {
        return raftTile.GetVector3SpotPosition(this);
        
    }
}
