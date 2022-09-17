using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaftTile : MonoBehaviour
{
    //Top, right, bot, left
    public bool[] borderOk = new bool[4];
    public bool[] borderExist = new bool[4];
    public bool borderGenerateOnce = false;

    public Transform parentBorder;
    public Transform parentBorderGo;

    public Tower towerOnIt = null;

    public float hpMax = 25;
    public float hp = 25;

    public int posX;
    public int posY;

    public SpotEnnemiAttack[,] spotEnnemiTile;

    List<SpotEnnemiAttack> spots = new List<SpotEnnemiAttack>();

    // Start is called before the first frame update
    void Start()
    {
        hp = hpMax;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Vector3 GetVector3SpotPosition(SpotEnnemiAttack spotRaftTile)
    {
        Vector3 positionSpot = this.transform.position;

        float nbSpotMax = (float)(Raft.instance.nbSpotEachBorder);

        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < nbSpotMax; j++)
            {
                if(spotEnnemiTile[i,j] == spotRaftTile)
                {
                    if (i == 0) positionSpot += new Vector3(j/(nbSpotMax-1)-0.5f, 0f, 0.5f);
                    else if (i == 1) positionSpot += new Vector3(0.5f,0,j/(nbSpotMax-1) - 0.5f);
                    else if (i == 2) positionSpot += new Vector3(-(j / (nbSpotMax - 1) - 0.5f), 0f, -0.5f);
                    else if (i == 3) positionSpot += new Vector3(-0.5f, 0, -(j / (nbSpotMax - 1) - 0.5f));
                }
            }
        }

        return positionSpot;
    }
    public void RefreshSpot()
    {
        spots = new List<SpotEnnemiAttack>();

        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < Raft.instance.nbSpotEachBorder; j++)
            {
                if (spotEnnemiTile[i, j] != null) spots.Add(spotEnnemiTile[i, j]);
            }
        }
    }

    public List<SpotEnnemiAttack> GetListSpot()
    {
        

        return spots;
    }

    public void SetBorderReady()
    {
        borderGenerateOnce = true;
        for (int i=0; i<4; i++)
        {
            parentBorder.GetChild(i).gameObject.SetActive(borderExist[i]);
            parentBorderGo.GetChild(i).gameObject.SetActive(borderOk[i]);
            
            if(borderOk[i]) parentBorder.GetChild(i).GetComponent<Renderer>().material.color = new Color(0f, 0.6f, 0.0f, 0.6f);
            else parentBorder.GetChild(i).GetComponent<Renderer>().material.color = new Color(0.6f, 0.0f, 0.0f, 0.6f);
        }
    }

    public void ShowBorder(bool showBorder)
    {
        parentBorder.gameObject.SetActive(showBorder);
    }

    public void DoDamge(float damage)
    {
        hp -= damage;

        float ratioHP = Mathf.Clamp(hp / hpMax,0,1f);

        if(ratioHP>0.8f) this.GetComponent<Renderer>().material.SetColor("_OutlineColor", new Color(0f, 0f, 0f));
        else this.GetComponent<Renderer>().material.SetColor("_OutlineColor", new Color(0.75f*(1f-ratioHP), 0f, 0f));
    }
}
