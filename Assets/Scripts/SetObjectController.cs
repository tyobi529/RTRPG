using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetObjectController : MonoBehaviour
{
    int yoko;
    int tate;

    private float width;

    int footer;



    //オブジェクトの移動前の位置
    public Vector3 firstPos_world;
    public Vector3 firstPos_screen;

    //オブジェクトの移動後の位置
    public Vector3 endPos_world;
    public Vector3 endPos_screen;



    //マスの幅（スクリーン）
    //private int width = 150;
    //private int height = 150;


    private Vector3 offset;

    private GameController gameController;


    public int select_x;
    public int select_y;

    public int objectId;

    //public bool cansummon = false;

    public int summon_phase = 1;

    public float cost = 10f;
    public float summon_cost = 10f;



    void Start()
    {
        FieldGenerator fieldGenerator = GameObject.Find("FieldGenerator").GetComponent<FieldGenerator>();
        yoko = fieldGenerator.yoko;
        tate = fieldGenerator.tate;
        width = Screen.width / yoko;


        gameController = GameObject.Find("GameController(Clone)").GetComponent<GameController>();

        //GameObject FieldGenerator = GameObject.Find("FieldGenerator");
        //tate = FieldGenerator.GetComponent<FieldGenerator>().tate;

        footer = GameObject.Find("FieldGenerator").GetComponent<FieldGenerator>().footer;
    }

    void Update()
    {
        if (summon_phase == 0)
        {
            cost += Time.deltaTime;
            this.GetComponent<SpriteRenderer>().color = Color.gray;

            if (cost > summon_cost)
            {
                summon_phase = 1;
                this.GetComponent<SpriteRenderer>().color = Color.white;
            }
        }

    }


    void OnMouseDown()
    {
        if (summon_phase == 1)
        {
            //最初の位置を保存
            firstPos_world = transform.position;
            //オブジェクトの座標をスクリーン座標へ
            this.firstPos_screen = Camera.main.WorldToScreenPoint(transform.position);
            //ワールド空間においてオブジェクトとクリックした座標との差をとる
            this.offset = firstPos_world - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, firstPos_screen.z));

            summon_phase++;
        }


    }

    void OnMouseDrag()
    {
        if (summon_phase == 2)
        {
            //マウスを離した点のスクリーン座標
            Vector3 currentfirstPos_screen = new Vector3(Input.mousePosition.x, Input.mousePosition.y, firstPos_screen.z);
            //離した点から、オブジェクトとの差の分を加える
            Vector3 currentPosition = Camera.main.ScreenToWorldPoint(currentfirstPos_screen) + this.offset;
            transform.position = currentPosition;
        }
    

    }

    void OnMouseUp()
    {
        if (summon_phase == 2)
        {
            endPos_screen = new Vector3(Input.mousePosition.x, Input.mousePosition.y, firstPos_screen.z);

            //選んだマス
            select_x = (int)(endPos_screen.x / width);
            select_y = (int)((endPos_screen.y - footer) / width);



            //範囲内
            if ((0 <= select_x && select_x < yoko && tate - 2 <= select_y && select_y < tate) && gameController.MasuObject[select_x, select_y] == null)
            {
                //対象にオブジェクトが無ければ
                //if (gameController.MasuObject[select_x, select_y] == null)
                    gameController.Summon(objectId, select_x, select_y);
            }
            else
            {
                summon_phase = 1;
            }

            transform.position = firstPos_world;

        }

    }

}