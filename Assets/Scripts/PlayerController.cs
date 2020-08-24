﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    //オブジェクトの移動前の位置
    public Vector3 firstPos_world;
    public Vector3 firstPos_screen;

    //オブジェクトの移動後の位置
    public Vector3 endPos_world;
    public Vector3 endPos_screen;

    //クリックした位置
    private Vector3 clickPos;


    //マスの幅（スクリーン）
    private int width = 150;
    private int height = 150;


    private Vector3 offset;

    private GameController gameController;

    //オブジェクトのマス
    public int masu_x;
    public int masu_y;

    public int select_x;
    public int select_y;


    //選んだます
    //int[] select_masu = new int[2];

    //タップしたかどうか
    //private bool istap = false;

    public int hp = 100;
    public int power = 50;
    public float speed = 10f;

    public float waitTime = 30f;
    public float time = 0f;

    public int playerId = 0;
    public int objectId = 0;

    //public GameObject Canvas;
    public GameObject hpText;
    public GameObject timeText;



    public bool canmove = true;

    void Start()
    {
        //PlayerPos = new Vector3(0, 0, 0);
        //transform.position = PlayerPos;
        gameController = GameObject.Find("GameController(Clone)").GetComponent<GameController>();
        //gameController.MasuObject[masu_x, masu_y] = this.gameObject;

        //firstPos_screen = new Vector3(width / 2 + width * masu_x, 284 + height / 2 + height * masu_y, 0);
        //firstPos_world = Camera.main.ScreenToWorldPoint(firstPos_screen);
        //firstPos_world = new Vector3(firstPos_world.x, firstPos_world.y, 0);
        //transform.position = firstPos_world;
        //Debug.Log(offset);

        //masu_x = 6;
        //masu_y = 2;

        firstPos_world = transform.position;
        firstPos_screen = Camera.main.WorldToScreenPoint(firstPos_world);

        GameObject Canvas = GameObject.Find("Canvas");

        //hp表示
        hpText = new GameObject("HPText");

        hpText.transform.parent = Canvas.transform;

        hpText.AddComponent<Text>();

        hpText.GetComponent<Text>().font = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
        hpText.GetComponent<Text>().fontSize = 50;
        hpText.GetComponent<Text>().fontStyle = FontStyle.Bold;

        hpText.GetComponent<Text>().color = Color.red;

        //待機時間表示
        timeText = new GameObject("timeText");

        timeText.transform.parent = Canvas.transform;

        timeText.AddComponent<Text>();

        timeText.GetComponent<Text>().font = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
        timeText.GetComponent<Text>().fontSize = 50;
        timeText.GetComponent<Text>().fontStyle = FontStyle.Bold;

        timeText.GetComponent<Text>().color = Color.yellow;

        time = waitTime / speed;
    }

    void Update()
    {
        hpText.transform.position = Camera.main.WorldToScreenPoint(transform.position);
        hpText.GetComponent<Text>().text = hp.ToString();

        timeText.transform.position = Camera.main.WorldToScreenPoint(new Vector3(transform.position.x, transform.position.y - 0.4f, 0));
        timeText.GetComponent<Text>().text = time.ToString();


        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            Debug.Log(masu_x);
            //Debug.Log("aa");
            Debug.Log(masu_y);
        }

        if (!canmove)
        {
            time -= Time.deltaTime;

            if (time < 0f)
            {
                canmove = true;
                time = waitTime / speed;
            }
        }
    }


    void OnMouseDown()
    {
        if (playerId == gameController.playerId && canmove)
        {
            Debug.Log("aa");
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
        if (playerId == gameController.playerId && canmove)
        {
            //マウスを離した点のスクリーン座標
            Vector3 currentfirstPos_screen = new Vector3(Input.mousePosition.x, Input.mousePosition.y, firstPos_screen.z);
            //離した点から、オブジェクトとの差の分を加える
            Vector3 currentPosition = Camera.main.ScreenToWorldPoint(currentfirstPos_screen) + this.offset;
            transform.position = currentPosition;

            GetComponent<SpriteRenderer>().color = Color.yellow;
        }

    }

    void OnMouseUp()
    {
        if (playerId == gameController.playerId && canmove)
        {
            GetComponent<SpriteRenderer>().color = Color.white;

            endPos_screen = new Vector3(Input.mousePosition.x, Input.mousePosition.y, firstPos_screen.z);

            //選んだマス
            select_x = (int)(endPos_screen.x / 150f);
            select_y = (int)((endPos_screen.y - 284f) / 150f);



            //int[] move = new int[2];
            //MasuCheck(SelectMasu()[0], SelectMasu()[1]);

            //int[] select = new int[2] { SelectMasu()[0], SelectMasu()[1] };

            int move_x = Mathf.Abs(select_x - masu_x);
            int move_y = Mathf.Abs(select_y - masu_y);

            //Debug.Log("masu_x" + masu_x);
            //Debug.Log("masu_y" + masu_y);
            //Debug.Log("select_x" + select_x);
            //Debug.Log("select_y" + select_y);

            //範囲内
            if ((move_x == 0 && move_y == 1) || (move_x == 1 && move_y == 0))
            {


                //オブジェクトが存在しなければ移動する。
                if (gameController.MasuObject[select_x, select_y] == null)
                {
                    Debug.Log("移動");
                    gameController.Move(masu_x, masu_y, select_x, select_y);
                    masu_x = select_x;
                    masu_y = select_y;
                    //Move(masu_x, masu_y);
                    //return;

                    //canmove = false;

                }

                //対象が相手のオブジェクトなら攻撃
                else if (playerId != gameController.MasuObject[select_x, select_y].GetComponent<PlayerController>().playerId)
                {
                    //攻撃
                    Debug.Log("攻撃");

                    gameController.Attack(masu_x, masu_y, select_x, select_y);

                    transform.position = firstPos_world;

                    //return;

                    //canmove = false;

                }

                else
                {
                    transform.position = firstPos_world;

                }


                //StartCoroutine("Wait");


            }

            else
            {
                Debug.Log("範囲外");

                transform.position = firstPos_world;

            }

        }



    }


    //3秒停止する
    //private IEnumerator Wait() //コルーチン関数の名前
    //{
    //    canmove = false;
    //    yield return new WaitForSeconds(3.0f);
    //    canmove = true;
    //}






}