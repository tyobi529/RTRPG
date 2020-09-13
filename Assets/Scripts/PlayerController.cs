using System.Collections;
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

    int yoko;
    //マスの幅（スクリーン）
    private int width;
    //private int height = 150;

    private int footer;



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
    public int maxhp;
    public int hp;
    public int attack;
    public int defence;
    public float speed;
    

    public int exp = 0;
    public int levelup_exp = 100;

    //private float waitTime = 300f;
    private float time = 1f;

    //１：勇者側
    //２：魔王側
    public int playerId = 0;

    //１：Enemy1
    public int objectId = 0;

    //public GameObject Canvas;
    public GameObject hpText;

    public GameObject TimerPrefab;
    public GameObject timer;


    private GameObject AttackText;
    private GameObject DefenceText;
    private GameObject SpeedText;




    //public bool canmove = true;
    public int phase = 1;

    void Start()
    {
        FieldGenerator fieldGenerator = GameObject.Find("FieldGenerator").GetComponent<FieldGenerator>();
        yoko = fieldGenerator.yoko;
        width = Screen.width / yoko;

        footer = GameObject.Find("FieldGenerator").GetComponent<FieldGenerator>().footer;

        gameController = GameObject.Find("GameController(Clone)").GetComponent<GameController>();


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

        hpText.GetComponent<Text>().color = Color.black;

        hpText.transform.position = new Vector3(width / 2 + width * masu_x, footer + width / 4 + width * masu_y, 0);
        hpText.GetComponent<Text>().text = hp.ToString();


        timer = Instantiate(TimerPrefab) as GameObject;
        timer.transform.SetParent(Canvas.transform);

        timer.transform.position = new Vector3(width / 4 * 3 + width * masu_x, footer + width / 4 * 3 + width * masu_y, 0);
        timer.SetActive(false);


        if (gameController.playerId == 1 && playerId == 1)
        {
            AttackText = GameObject.Find("AttackText");
            DefenceText = GameObject.Find("DefenceText");
            SpeedText = GameObject.Find("SpeedText");
        }
    }

    void Update()
    {
        Vector3 Pos = Camera.main.WorldToScreenPoint(transform.position);
        hpText.transform.position = new Vector3(Pos.x, Pos.y - width / 2, 0);
        hpText.GetComponent<Text>().text = hp.ToString();


        if (gameController.playerId == 1 && playerId == 1)
        {
            //ステータスの更新
            //HPText.GetComponent<Text>().text = "HP: " + hp + "/" + maxhp;
            AttackText.GetComponent<Text>().text = "AT: " + attack;
            DefenceText.GetComponent<Text>().text = "DE: " + defence;
            SpeedText.GetComponent<Text>().text = "SP: " + speed;
        }



        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            Debug.Log(masu_x);
            Debug.Log(masu_y);
        }

        if (phase == 0)
        {
            timer.GetComponent<Image>().fillAmount = time;
            time -= Time.deltaTime * speed / 300;

            if (time < 0f)
            {
                //canmove = true;
                phase = 1;
                time = 1f;
                timer.SetActive(false);
                //time = waitTime / speed;
            }
        }

    }


    void OnMouseDown()
    {
        if (playerId == gameController.playerId && phase == 1)
        {
            //最初の位置を保存
            firstPos_world = transform.position;
            //オブジェクトの座標をスクリーン座標へ
            this.firstPos_screen = Camera.main.WorldToScreenPoint(transform.position);
            //ワールド空間においてオブジェクトとクリックした座標との差をとる
            this.offset = firstPos_world - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, firstPos_screen.z));

            phase++;

        }





    }

    void OnMouseDrag()
    {
        if (playerId == gameController.playerId && phase == 2)
        {
            //マウスを離した点のスクリーン座標
            Vector3 currentfirstPos_screen = new Vector3(Input.mousePosition.x, Input.mousePosition.y, firstPos_screen.z);
            //離した点から、オブジェクトとの差の分を加える
            Vector3 currentPosition = Camera.main.ScreenToWorldPoint(currentfirstPos_screen) + this.offset;
            transform.position = currentPosition;

            GetComponent<SpriteRenderer>().color = Color.yellow;

            //phase++;
        }

    }

    void OnMouseUp()
    {

        if (playerId == gameController.playerId && phase == 2)
        {
            GetComponent<SpriteRenderer>().color = Color.white;

            endPos_screen = new Vector3(Input.mousePosition.x, Input.mousePosition.y, firstPos_screen.z);

            //選んだマス
            select_x = (int)(endPos_screen.x / width);
            select_y = (int)((endPos_screen.y - footer) / width);


            int move_x = Mathf.Abs(select_x - masu_x);
            int move_y = Mathf.Abs(select_y - masu_y);


            //範囲内
            if ((move_x == 0 && move_y == 1) || (move_x == 1 && move_y == 0))
                gameController.CheckMasu(masu_x, masu_y, select_x, select_y);
            else
                transform.position = firstPos_world;


            phase = 1;
        }



    }


    public void Destroy()
    {
        Destroy(hpText);
        Destroy(timer);
        Destroy(this.gameObject);
    }







}