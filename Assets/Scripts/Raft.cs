using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Raft : MonoBehaviour
{


    public Transform parentRaftToCopy;
    public Transform parentRaft;
    public Transform parentRaftGrid;

    int widthGrid = 100;
    int heightGrid = 100;

    int posXMiddle;
    int posYMiddle;

    public int posXMin = 10000;
    public int posYMin = 10000;
    public int posXMax = 0;
    public int posYMax = 0;

    int dimensionMaxRaft = 0;
    public float radiusMaxRaft = 0;

    RaftTile[,] raftTiles;

    float ratioGoodBorderStart = 0.8f;

    public List<RaftTile> allRaftTiles = new List<RaftTile>();

    public static Raft instance;

    bool isConstructionTetris = false;
    bool isConstructionTower = false;

    TypeTetris tetrisBuild;
    int rotationTetris = 0;

    public GameObject currentBuildingTetris;

    public Transform cameraParent;
    public float angleXMin = 20f;
    public float angleXMax = 90f;
    float currentAngleX = 45f;
    float targetAngleX = 45f;
    float currentAngleY = 45f;
    float targetAngleY = 45f;

    public float speedMoveCameraAngle = 10f;

    public float distanceMin = 3f;
    public float zoomMin = 0.25f;
    public float zoomMax = 3f;
    float currentZoom = 1f;
    float targetZoom = 1f;
    public float speedZoom = 0.1f;

    public float speedMovementWithSize = 0.75f;

    public float distanceWithEachSizeMaxRaft = 0.5f;

    Vector2 moveInput;

    public List<ScriptableTower> towersPossible;

    public RectTransform menuTower;
    int towerSelected = -1;
    public Material materialWireFrame;


    public Transform towerBuildParent;
    Renderer[] materialTowerBuild;

    public int nbTypeTetris = 5;
    public enum TypeTetris
    {
        SMALL_L, BIG_L, CROSS, KEYS, LINE
    }
    private void Awake()
    {
        instance = this;
    }

    public int nbSpotEachBorder = 5;


    // Start is called before the first frame update
    void Start()
    {
        raftTiles = new RaftTile[widthGrid, heightGrid];
        posXMiddle = widthGrid / 2;
        posYMiddle = heightGrid / 2;

        GenerateRaftStart();

        GenerateMenuTower();
    }

    public void GenerateRaftStart()
    {
        TypeTetris firstTetris = (TypeTetris)(Random.Range(0, 5));
        

        int randomOrientation = Random.Range(0, 4);

        
        int[,] positionTiles = getPositionTilesOfTetris(firstTetris, randomOrientation);

       

        for (int j=0; j< positionTiles.GetLength(0); j++)
        {
            RaftTile raftTile = GenerateRaftTile(posXMiddle + positionTiles[j, 0], posYMiddle + positionTiles[j, 1]);
            raftTiles[posXMiddle + positionTiles[j, 0], posYMiddle + positionTiles[j, 1]] = raftTile;
            allRaftTiles.Add(raftTile);


            if (posXMiddle + positionTiles[j, 0] < posXMin) posXMin = posXMiddle + positionTiles[j, 0];
            if (posXMiddle + positionTiles[j, 0] > posXMax) posXMax = posXMiddle + positionTiles[j, 0];

            if (posYMiddle + positionTiles[j, 1] < posYMin) posYMin = posYMiddle + positionTiles[j, 1];
            if (posYMiddle + positionTiles[j, 1] > posYMax) posYMax = posYMiddle + positionTiles[j, 1];


        }

        CheckBorder();

        AddModuleRandom();


        CheckBorder();




        cameraParent.transform.localPosition = new Vector3((posXMin+posXMax)/2f - posXMiddle,0, (posYMin + posYMax) / 2f - posYMiddle);
        dimensionMaxRaft = Mathf.Max(posXMax - posXMin, posYMax - posYMin);
        radiusMaxRaft = Mathf.Sqrt(Mathf.Pow(Mathf.Max(posXMax - widthGrid / 2, posXMin - widthGrid / 2), 2) + Mathf.Pow(Mathf.Max(posYMax - heightGrid / 2, posYMin - heightGrid / 2), 2));

        cameraParent.transform.GetChild(0).localPosition = new Vector3(0, 0, -distanceMin - distanceWithEachSizeMaxRaft * (dimensionMaxRaft* dimensionMaxRaft));
        cameraParent.transform.localEulerAngles = new Vector3(currentAngleX, currentAngleY, 0);
    }

    public int[,] getPositionTilesOfTetris(TypeTetris tetris, int orientation)
    {
        int[,] tetrisPosition = null;

        if (tetris == TypeTetris.SMALL_L)
        {
            tetrisPosition = new int[3, 2];

            tetrisPosition[0, 0] = 0;
            tetrisPosition[0, 1] = 0;

            if(orientation==0)
            {
                tetrisPosition[1, 0] = 0;
                tetrisPosition[1, 1] = 1;

                tetrisPosition[2, 0] = 1;
                tetrisPosition[2, 1] = 0;
            }
            else if (orientation == 1)
            {
                tetrisPosition[1, 0] = 1;
                tetrisPosition[1, 1] = 0;

                tetrisPosition[2, 0] = 0;
                tetrisPosition[2, 1] = -1;
            }
            else if (orientation == 2)
            {
                tetrisPosition[1, 0] = 0;
                tetrisPosition[1, 1] = -1;

                tetrisPosition[2, 0] = -1;
                tetrisPosition[2, 1] = 0;
            }
            else if (orientation == 3)
            {
                tetrisPosition[1, 0] = -1;
                tetrisPosition[1, 1] = 0;

                tetrisPosition[2, 0] = 0;
                tetrisPosition[2, 1] = 1;
            }

        }
        else if (tetris == TypeTetris.CROSS)
        {
            tetrisPosition = new int[5, 2];

            tetrisPosition[0, 0] = 0;
            tetrisPosition[0, 1] = 0;

            tetrisPosition[1, 0] = 1;
            tetrisPosition[1, 1] = 0;

            tetrisPosition[2, 0] = 0;
            tetrisPosition[2, 1] = 1;

            tetrisPosition[3, 0] = -1;
            tetrisPosition[3, 1] = 0;

            tetrisPosition[4, 0] = 0;
            tetrisPosition[4, 1] = -1;
        }
        else if (tetris == TypeTetris.KEYS)
        {
            tetrisPosition = new int[4, 2];

            tetrisPosition[0, 0] = 0;
            tetrisPosition[0, 1] = 0;

            if(orientation==0)
            {
                tetrisPosition[1, 0] = 1;
                tetrisPosition[1, 1] = 0;

                tetrisPosition[2, 0] = 0;
                tetrisPosition[2, 1] = 1;

                tetrisPosition[3, 0] = -1;
                tetrisPosition[3, 1] = 0;
            }
            else if (orientation == 1)
            {
                tetrisPosition[1, 0] = 1;
                tetrisPosition[1, 1] = 0;

                tetrisPosition[2, 0] = 0;
                tetrisPosition[2, 1] = 1;

                tetrisPosition[3, 0] = 0;
                tetrisPosition[3, 1] = -1;
            }
            else if (orientation == 2)
            {
                tetrisPosition[1, 0] = 1;
                tetrisPosition[1, 1] = 0;

                tetrisPosition[2, 0] = -1;
                tetrisPosition[2, 1] = 0;

                tetrisPosition[3, 0] = 0;
                tetrisPosition[3, 1] = -1;
            }
            else if (orientation == 3)
            {
                tetrisPosition[1, 0] = 0;
                tetrisPosition[1, 1] = 1;

                tetrisPosition[2, 0] = -1;
                tetrisPosition[2, 1] = 0;

                tetrisPosition[3, 0] = 0;
                tetrisPosition[3, 1] = -1;
            }
        }
        else if (tetris == TypeTetris.LINE)
        {
            tetrisPosition = new int[4, 2];

            tetrisPosition[0, 0] = 0;
            tetrisPosition[0, 1] = 0;

            if (orientation == 0)
            {
                tetrisPosition[1, 0] = 1;
                tetrisPosition[1, 1] = 0;

                tetrisPosition[2, 0] = -1;
                tetrisPosition[2, 1] = 0;

                tetrisPosition[3, 0] = -2;
                tetrisPosition[3, 1] = 0;
            }
            else if (orientation == 1)
            {
                tetrisPosition[1, 0] = 0;
                tetrisPosition[1, 1] = 1;

                tetrisPosition[2, 0] = 0;
                tetrisPosition[2, 1] = 2;

                tetrisPosition[3, 0] = 0;
                tetrisPosition[3, 1] = -1;
            }
            else if (orientation == 2)
            {
                tetrisPosition[1, 0] = 1;
                tetrisPosition[1, 1] = 0;

                tetrisPosition[2, 0] = 2;
                tetrisPosition[2, 1] = 0;

                tetrisPosition[3, 0] = -1;
                tetrisPosition[3, 1] = 0;
            }
            else if (orientation == 3)
            {
                tetrisPosition[1, 0] = 0;
                tetrisPosition[1, 1] = -1;

                tetrisPosition[2, 0] = 0;
                tetrisPosition[2, 1] = -2;

                tetrisPosition[3, 0] = 0;
                tetrisPosition[3, 1] = 1;
            }
        }
        else if (tetris == TypeTetris.BIG_L)
        {
            tetrisPosition = new int[4, 2];

            tetrisPosition[0, 0] = 0;
            tetrisPosition[0, 1] = 0;

            if (orientation == 0)
            {
                tetrisPosition[1, 0] = 1;
                tetrisPosition[1, 1] = 0;

                tetrisPosition[2, 0] = -1;
                tetrisPosition[2, 1] = 0;

                tetrisPosition[3, 0] = -1;
                tetrisPosition[3, 1] = 1;
            }
            else if (orientation == 1)
            {
                tetrisPosition[1, 0] = 0;
                tetrisPosition[1, 1] = -1;

                tetrisPosition[2, 0] = 0;
                tetrisPosition[2, 1] = 1;

                tetrisPosition[3, 0] = 1;
                tetrisPosition[3, 1] = 1;
            }
            else if (orientation == 2)
            {
                tetrisPosition[1, 0] = -1;
                tetrisPosition[1, 1] = 0;

                tetrisPosition[2, 0] = 1;
                tetrisPosition[2, 1] = 0;

                tetrisPosition[3, 0] = 1;
                tetrisPosition[3, 1] = -1;
            }
            else if (orientation == 3)
            {
                tetrisPosition[1, 0] = 0;
                tetrisPosition[1, 1] = 1;

                tetrisPosition[2, 0] = 0;
                tetrisPosition[2, 1] = -1;

                tetrisPosition[3, 0] = -1;
                tetrisPosition[3, 1] = -1;
            }
        }

        return tetrisPosition;
    }

    public RaftTile GenerateRaftTile(int posX, int posY)
    {
        int randomRaftTile = Random.Range(0, parentRaftToCopy.childCount);

        GameObject raftTile = GameObject.Instantiate(parentRaftToCopy.GetChild(randomRaftTile).gameObject, parentRaft);
        raftTile.transform.position = new Vector3(posX - posXMiddle, 0.5f, posY-posYMiddle);
        raftTile.transform.localEulerAngles = new Vector3(0,90 * Random.Range(0,4),0);

        raftTile.GetComponent<RaftTile>().parentBorder.transform.eulerAngles = Vector3.zero;
        raftTile.GetComponent<RaftTile>().parentBorderGo.transform.eulerAngles = Vector3.zero;

        RaftTile raftTileScript =  raftTile.GetComponent<RaftTile>();
        raftTileScript.posX = posX;
        raftTileScript.posY = posY;

        return raftTileScript;
    }

    // Update is called once per frame
    void Update()
    {

        if(Input.GetKeyDown(KeyCode.X))
        {
            AddModuleRandom();
            foreach (RaftTile raftTile in allRaftTiles) raftTile.ShowBorder(isConstructionTetris);
        }

        MoveCamera();




        targetAngleX = Mathf.Clamp(targetAngleX, angleXMin, angleXMax);

        currentAngleY = Mathf.Lerp(currentAngleY, targetAngleY, 3*Time.deltaTime);
        currentAngleX = Mathf.Lerp(currentAngleX, targetAngleX, 3*Time.deltaTime);

        cameraParent.localEulerAngles = new Vector3(currentAngleX,currentAngleY,0);

        

        currentZoom = Mathf.Lerp(currentZoom, targetZoom, 3 * Time.deltaTime);
        cameraParent.transform.GetChild(0).localPosition = new Vector3(0, 0, -distanceMin - distanceWithEachSizeMaxRaft * currentZoom*(Mathf.Pow(dimensionMaxRaft,1.5f)));


        if(isConstructionTower)
        {
            menuTower.anchoredPosition = new Vector2(0f, Mathf.Lerp(menuTower.anchoredPosition.y, 20f, 3 * Time.deltaTime));
        }
        else 
        {
            menuTower.anchoredPosition = new Vector2(0f, Mathf.Lerp(menuTower.anchoredPosition.y, -450f, 3 * Time.deltaTime));
        }

        
        CheckBuildTower();


        CheckBuildTetris();
        
    }

    public void MoveCamera()
    {
        Vector3 move = moveInput;

        move = Quaternion.Euler(0,cameraParent.transform.localEulerAngles.y, 0) * new Vector3(move.x,0,move.y);

        float ratioSpeed = dimensionMaxRaft * speedMovementWithSize * Time.deltaTime;

        cameraParent.localPosition += new Vector3(move.x * ratioSpeed, 0, move.z * ratioSpeed);
        cameraParent.localPosition = new Vector3(Mathf.Clamp(cameraParent.localPosition.x, posXMin - posXMiddle, posXMax - posXMiddle), 0, Mathf.Clamp(cameraParent.localPosition.z, posYMin - posYMiddle, posYMax - posYMiddle));

    }

    public void Zoom(InputAction.CallbackContext callBackMouseZoom)
    {
       if(callBackMouseZoom.performed)
        targetZoom = Mathf.Clamp(targetZoom - callBackMouseZoom.ReadValue<float>() * speedZoom, zoomMin, zoomMax);
    }

    public void RotateLeft(InputAction.CallbackContext callBackRotate)
    {
        if (callBackRotate.performed)
            Rotate(-90f);
    }
    public void RotateRight(InputAction.CallbackContext callBackRotate)
    {
        if (callBackRotate.performed)
            Rotate(90f);
    }

    public void Rotate(float angleRotate)
    {
        
        //targetAngleY += angleRotate;
    }

    public void moveCameraAngleX(InputAction.CallbackContext callBackMoveCamera)
    {
        if(Input.GetMouseButton(1))
        {
            targetAngleX -= callBackMoveCamera.ReadValue<float>() * speedMoveCameraAngle;
            
        }
    }
    public void moveCameraAngleY(InputAction.CallbackContext callBackMoveCamera)
    {
        if (Input.GetMouseButton(1))
        {
            targetAngleY += callBackMoveCamera.ReadValue<float>() * speedMoveCameraAngle;
            
        }
    }

    public void moveCameraChangeInput(InputAction.CallbackContext callBackMoveCamera)
    {
        moveInput = callBackMoveCamera.ReadValue<Vector2>();
    }

    public void Build()
    {
        isConstructionTetris = !isConstructionTetris;
        foreach (RaftTile raftTile in allRaftTiles) raftTile.ShowBorder(isConstructionTetris);
    }

    public void Build(bool activeBuild)
    {
        isConstructionTower = false;

        isConstructionTetris = activeBuild;
        foreach (RaftTile raftTile in allRaftTiles) raftTile.ShowBorder(isConstructionTetris);
    }

    public void TowerBuilding()
    {
        if(!isConstructionTetris)
        {
            isConstructionTower = !isConstructionTower;
        }
        
    }

    public void AddChoicePart(TypeTetris typeTetris)
    {

        Build(true);

        tetrisBuild = typeTetris;

        rotationTetris = 0;
    }


    public void AddModuleRandom()
    {
        TypeTetris tetrisRandom = (TypeTetris)(Random.Range(0, 5));

        int saveRot = 0;
        int nbBorduresMinDistance = 1000000;

        int posXSave = 0;
        int posYSave = 0;

        for (int k = 0; k < 4; k++)
        {
            int[,] positionTiles2 = getPositionTilesOfTetris(tetrisRandom, k);

            for (int x = posXMin-3; x <= posXMax+3; x++)
            {
                for (int y = posYMin-3; y <= posYMax+3; y++)
                {
                    int nbBordure = 0;
                    int distance = 0;
                    bool isPossible = true;

                    int[,] position = new int[widthGrid, heightGrid];
                    for (int i = 0; i < widthGrid; i++)
                    {
                        for (int j = 0; j < heightGrid; j++)
                        {
                            if (raftTiles[i, j] != null) position[i, j] = 1;
                        }
                    }

                    for (int j = 0; j < positionTiles2.GetLength(0); j++)
                    {
                        if (position[x + positionTiles2[j, 0], y + positionTiles2[j, 1]] == 1) isPossible = false;

                        if(raftTiles[x + positionTiles2[j, 0]+1, y + positionTiles2[j, 1]]!=null && !raftTiles[x + positionTiles2[j, 0] + 1, y + positionTiles2[j, 1]].borderOk[3]) isPossible = false;
                        if(raftTiles[x + positionTiles2[j, 0]-1, y + positionTiles2[j, 1]]!=null && !raftTiles[x + positionTiles2[j, 0] - 1, y + positionTiles2[j, 1]].borderOk[1]) isPossible = false;
                        if(raftTiles[x + positionTiles2[j, 0], y + positionTiles2[j, 1]+1]!=null && !raftTiles[x + positionTiles2[j, 0] , y + positionTiles2[j, 1]+1].borderOk[2]) isPossible = false;
                        if(raftTiles[x + positionTiles2[j, 0], y + positionTiles2[j, 1]-1]!=null && !raftTiles[x + positionTiles2[j, 0] , y + positionTiles2[j, 1]-1].borderOk[0]) isPossible = false;

                        position[x + positionTiles2[j, 0], y + positionTiles2[j, 1]] = 1;
                    }

                    for (int i = 0; i < widthGrid; i++)
                    {
                        for (int j = 0; j < heightGrid; j++)
                        {
                            if (position[i, j] == 1)
                            {
                                if (position[i - 1, j] == 0) nbBordure++;
                                if (position[i + 1, j] == 0) nbBordure++;
                                if (position[i, j - 1] == 0) nbBordure++;
                                if (position[i, j + 1] == 0) nbBordure++;

                                distance += Mathf.Abs(i - posXMiddle) + Mathf.Abs(j - posYMiddle);

                            }
                        }
                    }

                    if (isPossible && nbBordure*20+ distance < nbBorduresMinDistance)
                    {
                        nbBorduresMinDistance = nbBordure*20+ distance;
                        saveRot = k;
                        posXSave = x;
                        posYSave = y;
                    }


                }
            }

        }


        CreateRaftTetrisModule(posXSave, posYSave, tetrisRandom, saveRot);




        
    }

    public void CreateRaftTetrisModule(int posX, int posY, TypeTetris tetrisType, int rot)
    {
        int[,] positionTiles2Real = getPositionTilesOfTetris(tetrisType, rot);



        for (int j = 0; j < positionTiles2Real.GetLength(0); j++)
        {
            RaftTile raftTile = GenerateRaftTile(posX + positionTiles2Real[j, 0], posY + positionTiles2Real[j, 1]);

            raftTiles[posX + positionTiles2Real[j, 0], posY + positionTiles2Real[j, 1]] = raftTile;

            allRaftTiles.Add(raftTile);

            if (posX + positionTiles2Real[j, 0] < posXMin) posXMin = posX + positionTiles2Real[j, 0];
            if (posX + positionTiles2Real[j, 0] > posXMax) posXMax = posX + positionTiles2Real[j, 0];

            if (posY + positionTiles2Real[j, 1] < posYMin) posYMin = posY + positionTiles2Real[j, 1];
            if (posY + positionTiles2Real[j, 1] > posYMax) posYMax = posY + positionTiles2Real[j, 1];

        }


        dimensionMaxRaft = Mathf.Max(posXMax - posXMin, posYMax - posYMin);
        radiusMaxRaft = Mathf.Sqrt(Mathf.Pow(Mathf.Max(posXMax - widthGrid / 2, posXMin - widthGrid / 2), 2) + Mathf.Pow(Mathf.Max(posYMax - heightGrid / 2, posYMin - heightGrid / 2), 2));


        CheckBorder();
    }


    public void CheckBorder()
    {
        for (int i = 0; i < widthGrid; i++)
        {
            for (int j = 0; j < heightGrid; j++)
            {
                if (raftTiles[i, j] != null)
                {
                    raftTiles[i, j].borderExist[0] = false;
                    raftTiles[i, j].borderExist[1] = false;
                    raftTiles[i, j].borderExist[2] = false;
                    raftTiles[i, j].borderExist[3] = false;

                    raftTiles[i, j].spotEnnemiTile = new SpotEnnemiAttack[4, nbSpotEachBorder];

                    if (raftTiles[i, j + 1] == null)
                    {
                        if (!raftTiles[i, j].borderGenerateOnce)
                        {
                            raftTiles[i, j].borderOk[0] = Random.Range(0f, 1f) < ratioGoodBorderStart;

                           

                        }
                        raftTiles[i, j].borderExist[0] = true;
                        for (int s = 0; s < nbSpotEachBorder; s++) raftTiles[i, j].spotEnnemiTile[0, s] = new SpotEnnemiAttack(raftTiles[i, j]);
                        for (int s = 0; s < nbSpotEachBorder; s++)
                        {
                            if (s > 0) raftTiles[i, j].spotEnnemiTile[0, s].spotEnnemyPrev = raftTiles[i, j].spotEnnemiTile[0, s - 1];
                            if (s < nbSpotEachBorder - 1) raftTiles[i, j].spotEnnemiTile[0, s].spotEnnemyNext = raftTiles[i, j].spotEnnemiTile[0, s + 1];

                        }
                    }
                    if (raftTiles[i + 1, j] == null)
                    {
                        if (!raftTiles[i, j].borderGenerateOnce)
                        {
                            raftTiles[i, j].borderOk[1] = Random.Range(0f, 1f) < ratioGoodBorderStart;

                            

                        }
                        raftTiles[i, j].borderExist[1] = true;
                        for (int s = 0; s < nbSpotEachBorder; s++) raftTiles[i, j].spotEnnemiTile[1, s] = new SpotEnnemiAttack(raftTiles[i, j]);
                        for (int s = 0; s < nbSpotEachBorder; s++)
                        {
                            if (s > 0) raftTiles[i, j].spotEnnemiTile[1, s].spotEnnemyPrev = raftTiles[i, j].spotEnnemiTile[1, s - 1];
                            if (s < nbSpotEachBorder - 1) raftTiles[i, j].spotEnnemiTile[1, s].spotEnnemyNext = raftTiles[i, j].spotEnnemiTile[1, s + 1];

                        }
                    }
                    if (raftTiles[i, j - 1] == null)
                    {
                        if (!raftTiles[i, j].borderGenerateOnce)
                        {
                            raftTiles[i, j].borderOk[2] = Random.Range(0f, 1f) < ratioGoodBorderStart;

                            
                        }
                        raftTiles[i, j].borderExist[2] = true;
                        for (int s = 0; s < nbSpotEachBorder; s++) raftTiles[i, j].spotEnnemiTile[2, s] = new SpotEnnemiAttack(raftTiles[i, j]);
                        for (int s = 0; s < nbSpotEachBorder; s++)
                        {
                            if (s > 0) raftTiles[i, j].spotEnnemiTile[2, s].spotEnnemyPrev = raftTiles[i, j].spotEnnemiTile[2, s - 1];
                            if (s < nbSpotEachBorder - 1) raftTiles[i, j].spotEnnemiTile[2, s].spotEnnemyNext = raftTiles[i, j].spotEnnemiTile[2, s + 1];

                        }
                    }
                    if (raftTiles[i - 1, j] == null)
                    {
                        if (!raftTiles[i, j].borderGenerateOnce)
                        {
                            raftTiles[i, j].borderOk[3] = Random.Range(0f, 1f) < ratioGoodBorderStart;

                            
                        }
                        raftTiles[i, j].borderExist[3] = true;
                        for (int s = 0; s < nbSpotEachBorder; s++) raftTiles[i, j].spotEnnemiTile[3, s] = new SpotEnnemiAttack(raftTiles[i, j]);
                        for (int s = 0; s < nbSpotEachBorder; s++)
                        {
                            if (s > 0) raftTiles[i, j].spotEnnemiTile[3, s].spotEnnemyPrev = raftTiles[i, j].spotEnnemiTile[3, s - 1];
                            if (s < nbSpotEachBorder - 1) raftTiles[i, j].spotEnnemiTile[3, s].spotEnnemyNext = raftTiles[i, j].spotEnnemiTile[3, s + 1];

                        }
                    }
                    raftTiles[i, j].SetBorderReady();
                }
            }
        }

        for (int i = 0; i < widthGrid; i++)
        {
            for (int j = 0; j < heightGrid; j++)
            {
                if(raftTiles[i, j] != null)
                {
                    raftTiles[i, j].RefreshSpot();

                    if (raftTiles[i, j + 1] == null)
                    {
                        if (raftTiles[i + 1, j + 1] != null)
                        {
                            raftTiles[i, j].spotEnnemiTile[0, nbSpotEachBorder - 1].spotEnnemyNext = raftTiles[i + 1, j + 1].spotEnnemiTile[3, 0];
                            raftTiles[i + 1, j + 1].spotEnnemiTile[3, 0].spotEnnemyPrev = raftTiles[i, j].spotEnnemiTile[0, nbSpotEachBorder - 1];
                        }
                        else if (raftTiles[i + 1, j] != null)
                        {
                            raftTiles[i, j].spotEnnemiTile[0, nbSpotEachBorder - 1].spotEnnemyNext = raftTiles[i + 1, j].spotEnnemiTile[0, 0];
                            raftTiles[i + 1, j].spotEnnemiTile[0, 0].spotEnnemyPrev = raftTiles[i, j].spotEnnemiTile[0, nbSpotEachBorder - 1];
                        }
                        else
                        {
                            raftTiles[i, j].spotEnnemiTile[0, nbSpotEachBorder - 1].spotEnnemyNext = raftTiles[i, j].spotEnnemiTile[1, 0];
                            raftTiles[i, j].spotEnnemiTile[1, 0].spotEnnemyPrev = raftTiles[i, j].spotEnnemiTile[0, nbSpotEachBorder - 1];
                        }

                        if (raftTiles[i - 1, j + 1] != null)
                        {
                            raftTiles[i, j].spotEnnemiTile[0, 0].spotEnnemyPrev = raftTiles[i - 1, j + 1].spotEnnemiTile[1, nbSpotEachBorder - 1];
                            raftTiles[i - 1, j + 1].spotEnnemiTile[1, nbSpotEachBorder - 1].spotEnnemyNext = raftTiles[i, j].spotEnnemiTile[0, 0];
                        }
                        else if (raftTiles[i - 1, j] != null)
                        {
                            raftTiles[i, j].spotEnnemiTile[0, 0].spotEnnemyPrev = raftTiles[i - 1, j].spotEnnemiTile[0, nbSpotEachBorder - 1];
                            raftTiles[i - 1, j].spotEnnemiTile[0, nbSpotEachBorder - 1].spotEnnemyNext = raftTiles[i, j].spotEnnemiTile[0, 0];
                        }
                        else
                        {
                            raftTiles[i, j].spotEnnemiTile[0, 0].spotEnnemyPrev = raftTiles[i, j].spotEnnemiTile[3, nbSpotEachBorder - 1];
                            raftTiles[i, j].spotEnnemiTile[3, nbSpotEachBorder - 1].spotEnnemyNext = raftTiles[i, j].spotEnnemiTile[0, 0];
                        }



                    }

                    if (raftTiles[i + 1, j] == null)
                    {
                        if (raftTiles[i + 1, j + 1] != null)
                        {
                            raftTiles[i, j].spotEnnemiTile[1, 0].spotEnnemyPrev = raftTiles[i + 1, j + 1].spotEnnemiTile[2, nbSpotEachBorder - 1];
                            raftTiles[i + 1, j + 1].spotEnnemiTile[2, nbSpotEachBorder - 1].spotEnnemyNext = raftTiles[i, j].spotEnnemiTile[1, 0];
                        }
                        else if (raftTiles[i, j + 1] != null)
                        {
                            raftTiles[i, j].spotEnnemiTile[1, 0].spotEnnemyPrev = raftTiles[i, j + 1].spotEnnemiTile[1, nbSpotEachBorder - 1];
                            raftTiles[i, j + 1].spotEnnemiTile[1, nbSpotEachBorder - 1].spotEnnemyNext = raftTiles[i, j].spotEnnemiTile[1, 0];
                        }
                        else
                        {
                            raftTiles[i, j].spotEnnemiTile[1, 0].spotEnnemyPrev = raftTiles[i, j].spotEnnemiTile[0, nbSpotEachBorder - 1];
                            raftTiles[i, j].spotEnnemiTile[0, nbSpotEachBorder - 1].spotEnnemyNext = raftTiles[i, j].spotEnnemiTile[1, 0];
                        }


                        if (raftTiles[i + 1, j - 1] != null)
                        {
                            raftTiles[i, j].spotEnnemiTile[1, nbSpotEachBorder - 1].spotEnnemyNext = raftTiles[i + 1, j - 1].spotEnnemiTile[0, 0];
                            raftTiles[i + 1, j - 1].spotEnnemiTile[0, 0].spotEnnemyPrev = raftTiles[i, j].spotEnnemiTile[1, nbSpotEachBorder - 1];
                        }
                        else if (raftTiles[i, j - 1] != null)
                        {
                            raftTiles[i, j].spotEnnemiTile[1, nbSpotEachBorder - 1].spotEnnemyNext = raftTiles[i, j - 1].spotEnnemiTile[1, 0];
                            raftTiles[i, j - 1].spotEnnemiTile[1, 0].spotEnnemyPrev = raftTiles[i, j].spotEnnemiTile[1, nbSpotEachBorder - 1];
                        }
                        else
                        {
                            raftTiles[i, j].spotEnnemiTile[1, nbSpotEachBorder - 1].spotEnnemyNext = raftTiles[i, j].spotEnnemiTile[2, 0];
                            raftTiles[i, j].spotEnnemiTile[2, 0].spotEnnemyPrev = raftTiles[i, j].spotEnnemiTile[1, nbSpotEachBorder - 1];
                        }
                    }

                    if (raftTiles[i, j - 1] == null)
                    {
                        if (raftTiles[i + 1, j - 1] != null)
                        {
                            raftTiles[i, j].spotEnnemiTile[2, 0].spotEnnemyPrev = raftTiles[i + 1, j - 1].spotEnnemiTile[3, nbSpotEachBorder - 1];
                            raftTiles[i + 1, j - 1].spotEnnemiTile[3, nbSpotEachBorder - 1].spotEnnemyNext = raftTiles[i, j].spotEnnemiTile[2, 0];
                        }
                        else if (raftTiles[i + 1, j] != null)
                        {
                            raftTiles[i, j].spotEnnemiTile[2, 0].spotEnnemyPrev = raftTiles[i + 1, j].spotEnnemiTile[2, nbSpotEachBorder - 1];
                            raftTiles[i + 1, j].spotEnnemiTile[2, nbSpotEachBorder - 1].spotEnnemyNext = raftTiles[i, j].spotEnnemiTile[2, 0];
                        }
                        else
                        {
                            raftTiles[i, j].spotEnnemiTile[2, 0].spotEnnemyPrev = raftTiles[i, j].spotEnnemiTile[1, nbSpotEachBorder - 1];
                            raftTiles[i, j].spotEnnemiTile[1, nbSpotEachBorder - 1].spotEnnemyNext = raftTiles[i, j].spotEnnemiTile[2, 0];
                        }

                        if (raftTiles[i - 1, j - 1] != null)
                        {
                            raftTiles[i, j].spotEnnemiTile[2, nbSpotEachBorder - 1].spotEnnemyNext = raftTiles[i - 1, j - 1].spotEnnemiTile[1, 0];
                            raftTiles[i - 1, j - 1].spotEnnemiTile[1, 0].spotEnnemyPrev = raftTiles[i, j].spotEnnemiTile[2, nbSpotEachBorder - 1];
                        }
                        else if (raftTiles[i - 1, j] != null)
                        {
                            raftTiles[i, j].spotEnnemiTile[2, nbSpotEachBorder - 1].spotEnnemyNext = raftTiles[i - 1, j].spotEnnemiTile[2, 0];
                            raftTiles[i - 1, j].spotEnnemiTile[2, 0].spotEnnemyPrev = raftTiles[i, j].spotEnnemiTile[2, nbSpotEachBorder - 1];
                        }
                        else
                        {
                            raftTiles[i, j].spotEnnemiTile[2, nbSpotEachBorder - 1].spotEnnemyNext = raftTiles[i, j].spotEnnemiTile[3, 0];
                            raftTiles[i, j].spotEnnemiTile[3, 0].spotEnnemyPrev = raftTiles[i, j].spotEnnemiTile[2, nbSpotEachBorder - 1];
                        }
                    }

                    if (raftTiles[i - 1, j] == null)
                    {
                        if (raftTiles[i - 1, j - 1] != null)
                        {
                            raftTiles[i, j].spotEnnemiTile[3, 0].spotEnnemyPrev = raftTiles[i - 1, j - 1].spotEnnemiTile[0, nbSpotEachBorder - 1];
                            raftTiles[i - 1, j - 1].spotEnnemiTile[0, nbSpotEachBorder - 1].spotEnnemyNext = raftTiles[i, j].spotEnnemiTile[3, 0];
                        }
                        else if (raftTiles[i, j - 1] != null)
                        {
                            raftTiles[i, j].spotEnnemiTile[3, 0].spotEnnemyPrev = raftTiles[i, j - 1].spotEnnemiTile[3, nbSpotEachBorder - 1];
                            raftTiles[i, j - 1].spotEnnemiTile[3, nbSpotEachBorder - 1].spotEnnemyNext = raftTiles[i, j].spotEnnemiTile[3, 0];
                        }
                        else
                        {
                            raftTiles[i, j].spotEnnemiTile[3, 0].spotEnnemyPrev = raftTiles[i, j].spotEnnemiTile[2, nbSpotEachBorder - 1];
                            raftTiles[i, j].spotEnnemiTile[2, nbSpotEachBorder - 1].spotEnnemyNext = raftTiles[i, j].spotEnnemiTile[3, 0];
                        }

                        if (raftTiles[i - 1, j + 1] != null)
                        {
                            raftTiles[i, j].spotEnnemiTile[3, nbSpotEachBorder - 1].spotEnnemyNext = raftTiles[i - 1, j + 1].spotEnnemiTile[2, 0];
                            raftTiles[i - 1, j + 1].spotEnnemiTile[2, 0].spotEnnemyPrev = raftTiles[i, j].spotEnnemiTile[3, nbSpotEachBorder - 1];
                        }
                        else if (raftTiles[i, j + 1] != null)
                        {
                            raftTiles[i, j].spotEnnemiTile[3, nbSpotEachBorder - 1].spotEnnemyNext = raftTiles[i, j + 1].spotEnnemiTile[3, 0];
                            raftTiles[i, j + 1].spotEnnemiTile[3, 0].spotEnnemyPrev = raftTiles[i, j].spotEnnemiTile[3, nbSpotEachBorder - 1];
                        }
                        else
                        {
                            raftTiles[i, j].spotEnnemiTile[3, nbSpotEachBorder - 1].spotEnnemyNext = raftTiles[i, j].spotEnnemiTile[0, 0];
                            raftTiles[i, j].spotEnnemiTile[0, 0].spotEnnemyPrev = raftTiles[i, j].spotEnnemiTile[3, nbSpotEachBorder - 1];
                        }
                    }
                }
                   
            }
        }

    }

    public void RefreshTowerMenu()
    {
        GenerateMenuTower();
    }

    public void GenerateMenuTower()
    {
        for(int i=0; i<towersPossible.Count; i++)
        {
            if(i>= menuTower.GetChild(0).childCount)
            {
                GameObject.Instantiate(menuTower.GetChild(0).GetChild(0).gameObject, menuTower.GetChild(0));
            }
            Transform childTowerUI = menuTower.GetChild(0).GetChild(i);

            childTowerUI.GetChild(3).GetComponent<TMPro.TextMeshProUGUI>().text = towersPossible[i].nameTower[0];
            childTowerUI.GetChild(4).GetComponent<TMPro.TextMeshProUGUI>().text = "Cost : "+towersPossible[i].cost;
            if (GameManager.instance.scrapAmount < towersPossible[i].cost) childTowerUI.GetChild(4).GetComponent<TMPro.TextMeshProUGUI>().color = Color.red;
            else childTowerUI.GetChild(4).GetComponent<TMPro.TextMeshProUGUI>().color = Color.white;
            

            for (int j=0; j<3; j++)
            {
                if (j < towersPossible[i].damageUI) childTowerUI.GetChild(6).GetChild(0).GetChild(1).GetChild(j).GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);
                else childTowerUI.GetChild(6).GetChild(0).GetChild(1).GetChild(j).GetComponent<Image>().color = new Color(0.75f, 0.75f, 0.75f, 0.5f);

                if (j < towersPossible[i].rangeUI) childTowerUI.GetChild(6).GetChild(1).GetChild(1).GetChild(j).GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);
                else childTowerUI.GetChild(6).GetChild(1).GetChild(1).GetChild(j).GetComponent<Image>().color = new Color(0.75f, 0.75f, 0.75f, 0.5f);
            }
            int idTower = i;

            childTowerUI.GetComponent<Button>().onClick.RemoveAllListeners();
            childTowerUI.GetComponent<Button>().onClick.AddListener(delegate { SelectTower(idTower); });

        }
    }

    public void SelectTower(int idTower)
    {
        if(towerSelected!=-1)
        {
            menuTower.GetChild(0).GetChild(towerSelected).GetChild(2).gameObject.SetActive(false);
        }

        if(idTower!= towerSelected)
        {
            towerSelected = idTower;
            menuTower.GetChild(0).GetChild(towerSelected).GetChild(2).gameObject.SetActive(true);
            if (towerBuildParent.childCount != 0)
            {
                Destroy(towerBuildParent.GetChild(0).gameObject);
            }
        }
        else
        {
            towerSelected = -1;
        }
        
    }

    public void CheckBuildTower()
    {
        if (isConstructionTower && towerSelected != -1)
        {
            if (towerBuildParent.childCount == 0)
            {
                GameObject towerCreated = GameObject.Instantiate(towersPossible[towerSelected].towerGO[0],towerBuildParent);
                materialTowerBuild = towerCreated.GetComponentsInChildren<Renderer>();
                foreach(Renderer renderer in materialTowerBuild)
                {
                    Material[] materialTowerWireFrame = new Material[renderer.materials.Length];
                    for (int i = 0; i < renderer.materials.Length; i++) materialTowerWireFrame[i] = materialWireFrame;
                    //renderer.material = materialWireFrame;
                    renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                    renderer.materials = materialTowerWireFrame;
                }
            }
            GameObject towerGO = towerBuildParent.GetChild(0).gameObject;

            
            int layerMask = 1 << 6;

            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
            {
                towerGO.SetActive(true);
                RaftTile raftTile = hit.transform.GetComponent<RaftTile>();
                if(raftTile!=null)
                {
                    towerGO.SetActive(true);

                    if(raftTile.towerOnIt==null && towersPossible[towerSelected].cost<=GameManager.instance.scrapAmount)
                    {
                        foreach (Renderer renderer in materialTowerBuild)
                        {
                            Material[] materialTowerWireFrame = new Material[renderer.materials.Length];
                            for (int i = 0; i < renderer.materials.Length; i++)
                            {
                                materialTowerWireFrame[i] = materialWireFrame;
                                materialTowerWireFrame[i].SetColor("_WireColor", new Color(0.5f, 1.5f, 0.5f));
                            }
                            renderer.materials = materialTowerWireFrame;
                        }


                        if(Input.GetMouseButtonDown(0))
                        {
                            GameObject towerCreated = GameObject.Instantiate(towersPossible[towerSelected].towerGO[0], raftTile.transform);
                            towerCreated.transform.position = new Vector3(raftTile.transform.position.x, towersPossible[towerSelected].offsetY , raftTile.transform.position.z);
                            raftTile.towerOnIt = towerCreated.GetComponent<Tower>();
                            towerCreated.GetComponent<Tower>().enabled = true;

                            GameManager.instance.UseScrap(towersPossible[towerSelected].cost);

                        }
                    }
                    else
                    {
                        foreach (Renderer renderer in materialTowerBuild)
                        {
                            Material[] materialTowerWireFrame = new Material[renderer.materials.Length];
                            for (int i = 0; i < renderer.materials.Length; i++)
                            {
                                materialTowerWireFrame[i] = materialWireFrame;
                                materialTowerWireFrame[i].SetColor("_WireColor", new Color(1.5f, 0.5f, 0.5f));
                            }
                            renderer.materials = materialTowerWireFrame;
                            
                        }
                    }

                    towerGO.transform.position = new Vector3(raftTile.transform.position.x, towersPossible[towerSelected].offsetY , raftTile.transform.position.z);
                }
                else
                {
                    towerGO.SetActive(false);
                }
            }
            else
            {
                towerGO.SetActive(false);
            }
        }
        else
        {
            if(towerBuildParent.childCount!=0)
            {
                Destroy(towerBuildParent.GetChild(0).gameObject);
            }
        }
    }

    public void RotateTetrisConstructionRotateLeft(InputAction.CallbackContext callBackRotate)
    {
        if (callBackRotate.performed)
        {
            rotationTetris += 1;
            if (rotationTetris > 3) rotationTetris -= 4;
        }
            
    }

    public void CheckBuildTetris()
    {
        if (isConstructionTetris)
        {
            for (int i = 1; i < currentBuildingTetris.transform.childCount; i++)
            {
                Destroy(currentBuildingTetris.transform.GetChild(i).gameObject);
            }


            int layerMask = 1 << 4;

            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
            {
                currentBuildingTetris.SetActive(true);

                int posX = Mathf.RoundToInt(hit.point.x);
                int posZ = Mathf.RoundToInt(hit.point.z);

                Vector3 positionRaftCentral = new Vector3(posX, 0, posZ);

                if(posX>-widthGrid/2 && posX< widthGrid / 2 && posZ>-heightGrid/2 && posZ<heightGrid/2)
                {
                    posX = posX + widthGrid / 2;
                    posZ = posZ + heightGrid / 2;

                    bool isCloseToATile = false;
                    bool isOnARedBorder = false;
                    bool isOnATileExisting = false;

                    int[,] positionTilesNewTetris = getPositionTilesOfTetris(tetrisBuild, rotationTetris);

                    for (int i = 0; i < positionTilesNewTetris.GetLength(0); i++)
                    {
                        

                        if (raftTiles[posX + positionTilesNewTetris[i, 0], posZ + positionTilesNewTetris[i, 1]] != null) isOnATileExisting = true;

                        if (raftTiles[posX + positionTilesNewTetris[i, 0] + 1, posZ + positionTilesNewTetris[i, 1]] != null)
                        {
                            isCloseToATile = true;
                            if (!raftTiles[posX + positionTilesNewTetris[i, 0] + 1, posZ + positionTilesNewTetris[i, 1]].borderOk[3]) isOnARedBorder = true;
                        }
                        if (raftTiles[posX + positionTilesNewTetris[i, 0] - 1, posZ + positionTilesNewTetris[i, 1]] != null)
                        {
                            isCloseToATile = true;
                            if (!raftTiles[posX + positionTilesNewTetris[i, 0] - 1, posZ + positionTilesNewTetris[i, 1]].borderOk[1]) isOnARedBorder = true;
                        }
                        if (raftTiles[posX + positionTilesNewTetris[i, 0], posZ + positionTilesNewTetris[i, 1] + 1] != null)
                        {
                            isCloseToATile = true;
                            if (!raftTiles[posX + positionTilesNewTetris[i, 0], posZ + positionTilesNewTetris[i, 1] + 1].borderOk[2]) isOnARedBorder = true;
                        }
                        if (raftTiles[posX + positionTilesNewTetris[i, 0], posZ + positionTilesNewTetris[i, 1] - 1] != null)
                        {
                            isCloseToATile = true;
                            if (!raftTiles[posX + positionTilesNewTetris[i, 0], posZ + positionTilesNewTetris[i, 1] - 1].borderOk[0]) isOnARedBorder = true;
                        }


                    }

                    bool isPositionOK = isCloseToATile && !isOnARedBorder && !isOnATileExisting;

                    for (int i = 0; i < positionTilesNewTetris.GetLength(0); i++)
                    {
                        GameObject instanceRaft = GameObject.Instantiate(currentBuildingTetris.transform.GetChild(0).gameObject, currentBuildingTetris.transform);
                        instanceRaft.SetActive(true);
                        instanceRaft.transform.position = new Vector3(positionRaftCentral.x + positionTilesNewTetris[i, 0], instanceRaft.transform.position.y, positionRaftCentral.z + positionTilesNewTetris[i, 1]);

                        /*bool isCloseToATileThisTile = false;
                        bool isOnARedBorderThisTile = false;
                        bool isOnATileExistingThisTile = false;

                        if (raftTiles[posX + positionTilesNewTetris[i, 0], posZ + positionTilesNewTetris[i, 1]] != null) isOnATileExistingThisTile = true;

                        if (raftTiles[posX + positionTilesNewTetris[i, 0] + 1, posZ + positionTilesNewTetris[i, 1]] != null)
                        {
                            isCloseToATileThisTile = true;
                            if (!raftTiles[posX + positionTilesNewTetris[i, 0] + 1, posZ + positionTilesNewTetris[i, 1]].borderOk[3]) isOnARedBorderThisTile = true;
                        }
                        if (raftTiles[posX + positionTilesNewTetris[i, 0] - 1, posZ + positionTilesNewTetris[i, 1]] != null)
                        {
                            isCloseToATileThisTile = true;
                            if (!raftTiles[posX + positionTilesNewTetris[i, 0] - 1, posZ + positionTilesNewTetris[i, 1]].borderOk[1]) isOnARedBorderThisTile = false;
                        }
                        if (raftTiles[posX + positionTilesNewTetris[i, 0], posZ + positionTilesNewTetris[i, 1] + 1] != null)
                        {
                            isCloseToATileThisTile = true;
                            if (!raftTiles[posX + positionTilesNewTetris[i, 0], posZ + positionTilesNewTetris[i, 1] + 1].borderOk[2]) isOnARedBorderThisTile = false;
                        }
                        if (raftTiles[posX + positionTilesNewTetris[i, 0], posZ + positionTilesNewTetris[i, 1] - 1] != null)
                        {
                            isCloseToATileThisTile = true;
                            if (!raftTiles[posX + positionTilesNewTetris[i, 0], posZ + positionTilesNewTetris[i, 1] - 1].borderOk[0]) isOnARedBorderThisTile = false;
                        }

                        bool isThisTileOk = !isOnARedBorder && !isOnATileExisting;
                        */

                        if (isPositionOK)
                        {
                            instanceRaft.GetComponent<Renderer>().material.SetColor("_WireColor", new Color(0.25f, 1f, 0.25f));
                        }
                        else
                        {
                            
                            instanceRaft.GetComponent<Renderer>().material.SetColor("_WireColor", new Color(1f, 0.25f, 0.25f));
                            
                        }

                    }

                    if(isPositionOK)
                    {
                        if (Input.GetMouseButtonDown(0))
                        {
                            CreateRaftTetrisModule(posX, posZ, tetrisBuild, rotationTetris);

                            Build(false);
                            for (int i = 1; i < currentBuildingTetris.transform.childCount; i++)
                            {
                                Destroy(currentBuildingTetris.transform.GetChild(i).gameObject);
                            }
                        }
                    }
                }

                


                /*if (raftTile.towerOnIt == null)
                    {
                        foreach (Renderer renderer in materialTowerBuild)
                        {
                            renderer.material.SetColor("_WireColor", new Color(0.5f, 1.5f, 0.5f));
                        }


                        if (Input.GetMouseButtonDown(0))
                        {
                            GameObject towerCreated = GameObject.Instantiate(towersPossible[towerSelected].towerGO[0], raftTile.transform);
                            towerCreated.transform.position = new Vector3(raftTile.transform.position.x, towersPossible[towerSelected].offsetY, raftTile.transform.position.z);
                            raftTile.towerOnIt = towerCreated.GetComponent<Tower>();
                            towerCreated.GetComponent<Tower>().enabled = true;
                        }
                    }
                    else
                    {
                        foreach (Renderer renderer in materialTowerBuild)
                        {
                            renderer.material.SetColor("_WireColor", new Color(1.5f, 0.5f, 0.5f));
                        }
                    }

                    towerGO.transform.position = new Vector3(raftTile.transform.position.x, towersPossible[towerSelected].offsetY, raftTile.transform.position.z);
                */
            }
        }
        else
        {
            for(int i=1; i< currentBuildingTetris.transform.childCount; i++)
            {
                Destroy(currentBuildingTetris.transform.GetChild(i).gameObject);
            }
        }
    }


    public void DoDamage(RaftTile raftTileToAttack, float areaEffect, float damage)
    {
        raftTileToAttack.DoDamge(damage);

        if(areaEffect>1f)
        {
            int posX = raftTileToAttack.posX;
            int posY = raftTileToAttack.posY;


            for(int i=posX - Mathf.RoundToInt(areaEffect); i< posX + Mathf.RoundToInt(areaEffect); i++)
            {
                for (int j = posY - Mathf.RoundToInt(areaEffect); j < posY + Mathf.RoundToInt(areaEffect); j++)
                {
                    float distance = (i - posX) * (i - posX) + (j - posY) * (j - posY);
                    if(i>=0 && i<widthGrid && j>=0 && j<heightGrid && distance<= areaEffect* areaEffect)
                    {
                        if (raftTiles[i, j] != null) raftTiles[i, j].DoDamge(damage);
                    }
                }
            }

        }

    }
}
