using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{

    public Transform FXparent;
    public Transform ennemyParentTemplate;
    public Transform ennemyParent;

    public static GameManager instance;

    public List<EnnemyScript> ennemies;

    public Transform parentIndicatorPosition;
    public Transform parentIndicatorHPennemy;

    

    Camera mainCamera;

    public int scrapAmount = 0;
    public TMPro.TextMeshProUGUI textRessource;
    public TMPro.TextMeshProUGUI textDayCount;
    public Transform parentScrap;
    int nbScrapPossible = 0;
    List<Scrap> scraps = new List<Scrap>();

    float simulateSeconds = 200;
    public float probaNewScrapBySecond = 0.5f;
    public Transform scrapIndicatorParent;

    int dayCount = 0;
    public GameObject buttonSkipTheNight;
    public TMPro.TextMeshProUGUI textSkipTheNight;

    bool isWaveOver = false;
    float timeNight = 60f;

    public float durationNight = 60f;

    public float waveBasePoint = 2f;
    public float powerEachDayMoreWavePoint = 0.5f;
    public float eachDayMoreWavePoint =2f;

    public MenuReward menuReward;

    // Start is called before the first frame update
    void Awake()
    {
        instance = this;
        mainCamera = Camera.main;

        nbScrapPossible = parentScrap.childCount;


    }
    void Start()
    {
        GenerateScrapInit();

        isWaveOver = true;
        timeNight = durationNight;

    }


    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            AddEnnemyRandom();
        }

        DisplayPositionEnnemy();
        DisplayHPEnnemy();
        UpdateScrap();

        textRessource.text = "Scrap : " + scrapAmount;
        textDayCount.text = "Day : " + dayCount;

        buttonSkipTheNight.SetActive(isWaveOver);
        textSkipTheNight.text = "Skip the night (" + Mathf.FloorToInt(timeNight) + ")s";
        if (isWaveOver && !menuReward.gameObject.activeInHierarchy)
        {
            timeNight -= Time.deltaTime;
            if(timeNight < 1f)
            {
                GoNextWave();
            }
        }

        if(!isWaveOver && ennemies.Count==0)
        {
            menuReward.OpenMenu();
            isWaveOver = true;
        }

    }

    public void GoNextWave()
    {
        isWaveOver = false;
        timeNight = durationNight;

        float pointNextWave = waveBasePoint + Mathf.Pow(dayCount * eachDayMoreWavePoint, powerEachDayMoreWavePoint);

        bool canKeepAddingCreature = true;

        float angleFromWave = Random.Range(0, 360f);
        float maxAngleAttackWave = Mathf.Clamp(Mathf.Sqrt(pointNextWave), 10f, 45f);

        while(canKeepAddingCreature)
        {
            List<int> idCreaturesPossible = new List<int>();
            for(int i=0; i< ennemyParentTemplate.childCount; i++)
            {
                if(ennemyParentTemplate.GetChild(i).GetComponent<EnnemyScript>().scriptableEnnemy.costPointInWave<= pointNextWave)
                {
                    idCreaturesPossible.Add(i);
                }
            }

            if (idCreaturesPossible.Count == 0) canKeepAddingCreature = false;
            else
            {
                int idEnnemyToAdd = idCreaturesPossible[Random.Range(0, idCreaturesPossible.Count)];
                pointNextWave -= ennemyParentTemplate.GetChild(idEnnemyToAdd).GetComponent<EnnemyScript>().scriptableEnnemy.costPointInWave;
                AddEnnemy(idEnnemyToAdd, angleFromWave, maxAngleAttackWave);
            }

        }


        dayCount++;
    }

   

    public void CreateFX(int idFX, float timeBeforeDestruction)
    {
        GameObject fx = GameObject.Instantiate(FXparent.GetChild(idFX).gameObject, this.transform);
        Destroy(fx, timeBeforeDestruction);
    }

    public void AddEnnemyRandom()
    {
        AddEnnemy( 2 - Random.Range(1, 11)/10,0,30f);
        //AddEnnemy(Random.Range(1, ennemyParentTemplate.childCount),0,30f);
        //AddEnnemy(2,0,180f);
    }

    public void AddEnnemy(int ennemyType, float maxAngleAttackWave, float maxAngleDiff)
    {
        //GameObject ennemyRandom = GameObject.Instantiate(ennemyParentTemplate.GetChild(Random.Range(0, ennemyParentTemplate.childCount)).gameObject, ennemyParent);
        GameObject ennemyRandom = GameObject.Instantiate(ennemyParentTemplate.GetChild(ennemyType).gameObject, ennemyParent);

        float distance = Random.Range(80f, 90f);
        distance = 50f;
        float angle = maxAngleAttackWave + Random.Range(-maxAngleDiff, maxAngleDiff);

        ennemyRandom.transform.position = distance * (Quaternion.Euler(angle * Vector3.up) * Vector3.forward);

        ennemies.Add(ennemyRandom.GetComponent<EnnemyScript>());
    }

    public void DisplayPositionEnnemy()
    {

        
        



        int i = 0;
        foreach(EnnemyScript ennemy in ennemies)
        {
            float distanceToEnnemy = Vector3.Distance(mainCamera.transform.position, ennemy.transform.position);

            Vector2 position = mainCamera.WorldToViewportPoint(ennemy.transform.position);

            float scalar = Vector3.Dot(ennemy.transform.position - mainCamera.transform.position, mainCamera.transform.rotation * Vector3.forward);

            if(scalar<0)
            {
                //position.y = Mathf.Clamp(position.y, Mathf.Min(0, -(Mathf.Abs(position.y))), -10f);
                position.y = -3f;
            }

            if(position.x>0f && position.x<1f && position.y>0f &&  position.y<1f)
            {

            }
            else
            {
                if (i >= parentIndicatorPosition.childCount)
                {
                    GameObject.Instantiate(parentIndicatorPosition.GetChild(0).gameObject, parentIndicatorPosition);
                }
                parentIndicatorPosition.GetChild(i).gameObject.SetActive(true);


                
                float angle = 0;

                float distanceToEdge = 300;
                if (position.x < 0) distanceToEdge = 0;
                else if (position.x > 1) distanceToEdge = 0;
                else distanceToEdge = Mathf.Min(distanceToEdge, 0.5f * 1920 - Mathf.Abs(position.x - 0.5f) * 1920);

                if (position.y < 0) distanceToEdge = 0;
                else if (position.y > 1) distanceToEdge = 0;
                else distanceToEdge = Mathf.Min(distanceToEdge, 0.5f * 1080 - Mathf.Abs(position.y - 0.5f) * 1080);

                angle = 90 + Mathf.Atan2(position.y - 0.5f, position.x - 0.5f) * 180f / Mathf.PI;

                float maxDistance = Mathf.Abs(position.x - 0.5f);
                bool isDistanceX = true;
                if (maxDistance < Mathf.Abs(position.y - 0.5f))
                {
                    isDistanceX = false;
                    maxDistance = Mathf.Abs(position.y - 0.5f);
                }

                if (maxDistance > 0.5f)
                {
                    if (isDistanceX)
                    {
                        position = new Vector2(Mathf.Clamp(position.x, 0f, 1f), Mathf.Clamp(0.5f + (position.y - 0.5f) / (maxDistance / 0.5f), 0f, 1f));
                    }
                    else
                    {
                        position = new Vector2(Mathf.Clamp(0.5f + (position.x - 0.5f) / (maxDistance / 0.5f), 0f, 1f), Mathf.Clamp(position.y, 0f, 1f));
                    }
                }

                if (angle > 180f) angle -= 360f;

                float factorSize = Mathf.Clamp(1 - Mathf.Max(0, distanceToEnnemy - 25) / 100, 0.5f, 1f);

                
                parentIndicatorPosition.GetChild(i).transform.eulerAngles = new Vector3(0, 0, Mathf.Max(0f, 0.01f * (100 - distanceToEdge / factorSize)) * angle);
                parentIndicatorPosition.GetChild(i).transform.localScale = factorSize * Vector3.one;
                parentIndicatorPosition.GetChild(i).GetComponent<RectTransform>().anchoredPosition = new Vector2(1920 * position.x, 1080 * position.y);

                i++;
            }

            
        }
        for (int j = i; j < parentIndicatorPosition.childCount; j++) parentIndicatorPosition.GetChild(j).gameObject.SetActive(false);
    }
    
    public void DisplayHPEnnemy()
    {

        

        int i = 0;
        foreach(EnnemyScript ennemy in ennemies)
        {
            float distanceToEnnemy = Vector3.Distance(mainCamera.transform.position, ennemy.transform.position);

            Vector2 position = mainCamera.WorldToViewportPoint(ennemy.transform.position);



            if(position.x>0f && position.x<1f && position.y>0f &&  position.y<1f)
            {

            
                if (i >= parentIndicatorHPennemy.childCount)
                {
                    GameObject.Instantiate(parentIndicatorHPennemy.GetChild(0).gameObject, parentIndicatorHPennemy);
                }
                parentIndicatorHPennemy.GetChild(i).gameObject.SetActive(true);

                float factorSize = Mathf.Clamp(1 - Mathf.Max(0, distanceToEnnemy - 25) / 100, 0.5f, 1f);


                parentIndicatorHPennemy.GetChild(i).transform.localScale = factorSize * Vector3.one;
                parentIndicatorHPennemy.GetChild(i).GetComponent<RectTransform>().anchoredPosition = new Vector2(1920 * position.x, 1080 * position.y + ennemy.scriptableEnnemy.offsetYHPBar* factorSize*factorSize);


                float HPEnnemy = ennemy.scriptableEnnemy.health;

                float sizeBarRatio = Mathf.Clamp(150 + (ennemy.scriptableEnnemy.health - 10) * 350f / 40f, 150, 500f);
                float sizeBarHP = Mathf.Clamp((sizeBarRatio - 15f) * ennemy.HP / ennemy.scriptableEnnemy.health,0, sizeBarRatio - 15f);
                parentIndicatorHPennemy.GetChild(i).GetComponent<RectTransform>().sizeDelta = new Vector2(sizeBarRatio, 40f);
                parentIndicatorHPennemy.GetChild(i).GetChild(0).GetComponent<RectTransform>().sizeDelta = new Vector2(sizeBarHP, parentIndicatorHPennemy.GetChild(i).GetChild(0).GetComponent<RectTransform>().sizeDelta.y);

                i++;
            }

            
        }
        for (int j = i; j < parentIndicatorHPennemy.childCount; j++) parentIndicatorHPennemy.GetChild(j).gameObject.SetActive(false);
    }

    public void UpdateScrap()
    {

        if (Random.Range(0, 1f) < probaNewScrapBySecond * Time.deltaTime)
        {
            Scrap scrap = generateNewScrap();
            
        }


        for (int s=0; s<scraps.Count;s++)
        {
            Scrap scrap = scraps[s];
            scrap.transform.position += new Vector3(scrap.directionSpeed.x, 0, scrap.directionSpeed.y) * Time.deltaTime;
            scrap.transform.eulerAngles += new Vector3(0, scrap.rotationSpeed, 0) * Time.deltaTime;

            if (Mathf.Abs(scrap.transform.position.x) > 150f || Mathf.Abs(scrap.transform.position.z) > 150f) Destroy(scrap.gameObject);

        }

        int i = 0;
        foreach (Scrap scrap in scraps)
        {
            float ratioHP = scrap.getRatioHP();
            if (ratioHP != 1f)
            {
                if (i >= scrapIndicatorParent.childCount)
                {
                    GameObject.Instantiate(scrapIndicatorParent.GetChild(0).gameObject, scrapIndicatorParent);
                }
                scrapIndicatorParent.GetChild(i).gameObject.SetActive(true);

                float distanceToScrap = Vector3.Distance(mainCamera.transform.position, scrap.transform.position);

                Vector2 position = mainCamera.WorldToViewportPoint(scrap.transform.position);


                float factorSize = Mathf.Clamp(1 - Mathf.Max(0, distanceToScrap - 25) / 100, 0.5f, 1f);

                Transform scrapIndicator = scrapIndicatorParent.GetChild(i);
                scrapIndicator.localScale = factorSize * Vector3.one;
                scrapIndicator.GetComponent<RectTransform>().anchoredPosition = new Vector2(1920 * position.x, 1080 * position.y);

                scrapIndicator.GetChild(0).GetChild(1).GetComponent<Image>().fillAmount = 1-ratioHP;
                scrapIndicator.GetChild(0).GetChild(2).GetComponent<Image>().fillAmount = 1-ratioHP;

                i++;
            }

            
        }
        for (int j = i; j < scrapIndicatorParent.childCount; j++) scrapIndicatorParent.GetChild(j).gameObject.SetActive(false);


    }

    public void GenerateScrapInit()
    {
        for(int i=0; i< simulateSeconds; i++)
        {
            if(Random.Range(0,1f)<probaNewScrapBySecond)
            {
                Scrap scrap = generateNewScrap();
                scrap.transform.position += new Vector3(scrap.directionSpeed.x ,0, scrap.directionSpeed.y) * (simulateSeconds - i);
            }
        }

    }

    public Scrap generateNewScrap()
    {
        GameObject scrapGo = GameObject.Instantiate(parentScrap.GetChild(Random.Range(0, nbScrapPossible)).gameObject, parentScrap);
        scrapGo.transform.localEulerAngles = new Vector3(0,Random.Range(0f, 360f), 0);
        scrapGo.gameObject.SetActive(true);

        Scrap scrap = scrapGo.GetComponent<Scrap>();


        Vector3 positionInitiale = 10 * new Vector3(Random.Range(-1f, 1f), 0f, Random.Range(-1f, 1f));
        float magnitudeSpeed = Random.Range(0.1f, 0.5f);
        Vector2 directionSpeed = magnitudeSpeed * new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;

        positionInitiale -= (100f / magnitudeSpeed) * new Vector3(directionSpeed.x, 0, directionSpeed.y);

        scrap.directionSpeed = directionSpeed;
        scrap.rotationSpeed = Random.Range(-5f, 5f);
        scrapGo.transform.position = new Vector3(positionInitiale.x, scrapGo.transform.position.y, positionInitiale.z);

        scraps.Add(scrap);
        return scrap;
    }

    public void GetScrap(Scrap scrap)
    {
        scrapAmount++;
        scraps.Remove(scrap);
        Raft.instance.RefreshTowerMenu();
    }

    public void UseScrap(int scrap)
    {
        scrapAmount -= scrap;
        Raft.instance.RefreshTowerMenu();
    }

    

    public void DoDamageArea(Vector3 positionTarget, float damage, float areaEffect, float ratioDamageMaxArea)
    {
        foreach(EnnemyScript ennemy in ennemies)
        {
            float ratioDistanceArea = Mathf.Max(0, (Vector3.SqrMagnitude(positionTarget - ennemy.transform.position) - ennemy.scriptableEnnemy.radiusEnnemy * ennemy.scriptableEnnemy.radiusEnnemy) / (areaEffect * areaEffect));

            if(ratioDistanceArea<1f)
            {
                ennemy.HP -= damage * ratioDamageMaxArea * (1 - ratioDistanceArea);
            }
        }
    }

}
