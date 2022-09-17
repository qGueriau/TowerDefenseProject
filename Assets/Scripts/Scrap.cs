using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scrap : MonoBehaviour
{
    public Vector2 directionSpeed;
    public float rotationSpeed;
    public float healthMax = 10f;
    float health = 10f;

    public float clicDamage = 1f;

    public float rehealRecentClick = 2.5f;

    public bool caught = false;
    public Vector3 positionToGoCaught;
    public Vector3 middleBezier;
    public Vector3 positionStartCaught;

    float timeSinceCaught = 0;
    public float timeCaughtAnimation = 2f;

    // Start is called before the first frame update
    void Start()
    {
        health = healthMax;
    }

    // Update is called once per frame
    void Update()
    {
        health += rehealRecentClick * Time.deltaTime;
        if (health > healthMax) health = healthMax;

        if(caught)
        {
            timeSinceCaught += Time.deltaTime;

            float tBezier = timeSinceCaught/ timeCaughtAnimation;
            float tBezierMinus = 1 - tBezier;


            Vector3 positionCaught = tBezierMinus * tBezierMinus * tBezierMinus * positionStartCaught + tBezierMinus * tBezier * (tBezierMinus + tBezier) * middleBezier + tBezier * tBezier * tBezier * positionToGoCaught;

            this.transform.position = positionCaught;

            if (timeSinceCaught>timeCaughtAnimation)
            {
                GameManager.instance.GetScrap(this);
                Destroy(this.gameObject);
            }
        }
    }
    
    public float getRatioHP()
    {
        if (caught) return 1f;
        else return health / healthMax;
    }

    public void OnMouseDown()
    {
        health -= clicDamage;

        if(health<0f && !caught)
        {
            caught = true;
            positionToGoCaught = Vector3.zero;
            middleBezier = (this.transform.position + positionToGoCaught) / 2f;
            middleBezier.y = 10f;

            positionStartCaught = this.transform.position;

            timeSinceCaught = 0f;
        }
    }
}
