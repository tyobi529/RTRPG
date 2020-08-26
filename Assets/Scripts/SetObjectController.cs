using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetObjectController : MonoBehaviour
{
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



    void Start()
    {
        gameController = GameObject.Find("GameController(Clone)").GetComponent<GameController>();
    }

    void Update()
    {

    }


    void OnMouseDown()
    {
        if (gameController.canSummon)
        {
            //最初の位置を保存
            firstPos_world = transform.position;
            //オブジェクトの座標をスクリーン座標へ
            this.firstPos_screen = Camera.main.WorldToScreenPoint(transform.position);
            //ワールド空間においてオブジェクトとクリックした座標との差をとる
            this.offset = firstPos_world - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, firstPos_screen.z));
        }


    }

    void OnMouseDrag()
    {
        if (gameController.canSummon)
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
        if (gameController.canSummon)
        {
            endPos_screen = new Vector3(Input.mousePosition.x, Input.mousePosition.y, firstPos_screen.z);

            //選んだマス
            select_x = (int)(endPos_screen.x / 150f);
            select_y = (int)((endPos_screen.y - 284f) / 150f);



            //範囲内
            if (0 <= select_x && select_x < 5 && 0 <= select_y && select_y < 7)
            {
                //対象にオブジェクトが無ければ
                if (gameController.MasuObject[select_x, select_y] == null)
                    gameController.GenerateEnemy(objectId, select_x, select_y);
            }

            transform.position = firstPos_world;

        }

    }

}