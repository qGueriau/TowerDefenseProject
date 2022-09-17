using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuReward : MonoBehaviour
{

    public float timeToOpen = 1f;
    float timeOpenning = 0f;
    bool shouldOpen = false;

    public RectTransform panelReward;
    public Vector2 sizeNormalPanel;

    public TMPro.TextMeshProUGUI textWaveClear;

    public RectTransform positionHorizontalGroupReward;

    public int choiceMade = 0;

    int[] raftPartReward = new int[3];

    public Transform[] parentImageRewardRaftPart;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(shouldOpen)
        {
            timeOpenning += Time.deltaTime;

            if(timeOpenning< timeToOpen/2f)
            {
                panelReward.sizeDelta = new Vector2(50f, 50 + (sizeNormalPanel.y - 50) * (timeOpenning) / (timeToOpen / 2f));
            }
            else if (timeOpenning < timeToOpen)
            {
                panelReward.sizeDelta = new Vector2(50 + (sizeNormalPanel.x - 50) * (timeOpenning - timeToOpen / 2f) / (timeToOpen / 2f), sizeNormalPanel.y);

                textWaveClear.fontSize = 98f * (timeOpenning - timeToOpen / 2f) / (timeToOpen);
            }
            else if (timeOpenning < 1.5f * timeToOpen)
            {
                panelReward.sizeDelta = sizeNormalPanel;
                textWaveClear.fontSize = 98f * (timeOpenning - timeToOpen / 2f) / (timeToOpen);
            }
            else
            {
                panelReward.sizeDelta = sizeNormalPanel;
                shouldOpen = false;
            }
        }

        if(choiceMade==1)
        {
            positionHorizontalGroupReward.anchoredPosition = new Vector2(Mathf.Lerp(positionHorizontalGroupReward.anchoredPosition.x, 1700f, 5*Time.deltaTime), positionHorizontalGroupReward.anchoredPosition.y);
        }

    }

    public void OpenMenu()
    {
        this.gameObject.SetActive(true);

        raftPartReward = new int[3];
        for(int i=0; i<3; i++)
        {
            bool isOkTetris = false;
            while(!isOkTetris)
            {
                int randomTetris = Random.Range(0, Raft.instance.nbTypeTetris);
                isOkTetris = true;
                for (int j = 0; j < i; j++)
                {
                    if (raftPartReward[j] == randomTetris) isOkTetris = false;
                }

                raftPartReward[i] = randomTetris;

            }
            

            for (int j = 0; j < parentImageRewardRaftPart[i].childCount; j++)
            {
                parentImageRewardRaftPart[i].GetChild(j).gameObject.SetActive(j == raftPartReward[i]);
            }

        }




        shouldOpen = true;
        timeOpenning = 0f;

        panelReward.sizeDelta = new Vector2(50, 50);

        textWaveClear.fontSize = 0;
        choiceMade = 0;

        positionHorizontalGroupReward.anchoredPosition = new Vector2(0, positionHorizontalGroupReward.anchoredPosition.y);
    }
    public void ChoiceRewardType(int choice)
    {
        
        choiceMade = choice;
    }

    public void ChoiceRaftPart(int choiceRaftPart)
    {
        this.gameObject.SetActive(false);

        Raft.instance.AddChoicePart((Raft.TypeTetris)(raftPartReward[choiceRaftPart]));
        
    }
}
