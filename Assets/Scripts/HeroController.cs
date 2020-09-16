using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using DG.Tweening;

// MonoBehaviourPunCallbacksを継承すると、photonViewプロパティが使えるようになる
public class HeroController : MonoBehaviour
{
    public int maxhp;
    public int hp;
    public int power;
    public int defence;
    public float speed;

    public float moveTime;



    int tate;
    int yoko;

    int width;

    int footer;


    GameController gameController;
    FieldGenerator fieldGenerator;
    PlayerStatus playerStatus;

    bool gameStart = true;

    // 補間にかける時間
    private const float InterpolationDuration = 0.2f;

    private Vector3 startPosition;
    private Vector3 endPosition;
    private float elapsedTime = 0f;


    int first_x;
    int first_y;

    public int masu_x;
    public int masu_y;

    public int direct_x;
    public int direct_y;


    public int playerId;

    Animator animator;

    GameObject Canvas;
    public GameObject HPSlider;

    public bool canmove = true;

    public bool canheal = true;
    public float healTime = 0f;

    public Text healText;

    //GameObject MainCamera;

    private void Start()
    {
        fieldGenerator = GameObject.Find("FieldGenerator").GetComponent<FieldGenerator>();
        footer = fieldGenerator.footer;
        yoko = fieldGenerator.yoko;
        tate = fieldGenerator.tate;
        width = Screen.width / yoko;


        playerStatus = GameObject.Find("PlayerStatus").GetComponent<PlayerStatus>();
        maxhp = playerStatus.hero_maxhp;
        hp = maxhp;
        power = playerStatus.hero_power;
        defence = playerStatus.hero_defence;
        speed = playerStatus.hero_speed;



        //playerId = PhotonNetwork.LocalPlayer.ActorNumber;

        startPosition = transform.position;
        endPosition = transform.position;




        animator = GetComponent<Animator>();

        Canvas = GameObject.Find("Canvas");

        HPSlider = Instantiate(HPSlider) as GameObject;
        HPSlider.transform.SetParent(Canvas.transform);



        //初期位置
        //Vector3 screenPos = Camera.main.WorldToScreenPoint(transform.position);
        //masu_x = (int)(screenPos.x / width);
        //masu_y = (int)((screenPos.y - footer - 60f) / width);

        masu_x = 3;
        masu_y = 0;
        transform.position = ScreenToWorld(new Vector2(width * masu_x + width / 2, footer + width * masu_y + width / 2));


        gameStart = true;
    }




    private void Update()
    {
        if (hp <= 0)
        {
            Destroy(HPSlider);
            Destroy(this.gameObject);

        }


        if (gameStart)
        {


            Vector2 Pos = Camera.main.WorldToScreenPoint(transform.position);
            Pos = new Vector2(Pos.x, Pos.y + 50f);

            HPSlider.transform.position = Pos;

            HPSlider.GetComponent<Slider>().value = (float)(hp) / (float)(maxhp);


            if (playerId == 1)
            {
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    OnAttackButton();

                }

                if (playerId == 1 && canmove)
                {
                    if (Input.GetKeyDown(KeyCode.LeftArrow))
                    {
                        gameController.Check_Masu(-1, 0);
                        //canmove = false;
                    }
                    else if (Input.GetKeyDown(KeyCode.RightArrow))
                    {
                        gameController.Check_Masu(1, 0);
                        //canmove = false;
                    }
                    else if (Input.GetKeyDown(KeyCode.DownArrow))
                    {
                        gameController.Check_Masu(0, -1);
                        //canmove = false;
                    }
                    else if (Input.GetKeyDown(KeyCode.UpArrow))
                    {
                        gameController.Check_Masu(0, 1);
                        //canmove = false;
                    }

                }



                if (!canheal)
                {
                    healTime -= Time.deltaTime;

                    healText.text = healTime.ToString("F0");

                    if (healTime <= 0)
                    {
                        healText.text = "回復";
                        canheal = true;
                    }
                }


                if (Input.GetKeyDown(KeyCode.A))
                    OnHealButton();
            }
            





        }
    }

    public void Move(int Direct_x, int Direct_y)
    {
        StartCoroutine(Move_Coroutine(Direct_x, Direct_y));
    }

    IEnumerator Move_Coroutine(int Direct_x, int Direct_y)
    {


        canmove = false;


        Vector2 MovePos = ScreenToWorld(new Vector2(width * (masu_x + direct_x) + width / 2, footer + width * (masu_y + direct_y) + width / 2));


        transform.DOMove(MovePos, moveTime);
        yield return new WaitForSeconds(moveTime);

        masu_x += direct_x;
        masu_y += direct_y;
        canmove = true;
    }

    public void ChangeAnimation(int Direct_x, int Direct_y)
    {
        //向きの変更
        direct_x = Direct_x;
        direct_y = Direct_y;


        //アニメーション
        if (direct_y == 1)
            animator.SetInteger("Walk", 0);
        else if (direct_y == -1)
            animator.SetInteger("Walk", 1);
        else if (direct_x == -1)
        {
            Vector2 lscale = transform.localScale;
            if (lscale.x > 0)
                lscale.x *= -1;

            transform.localScale = lscale;

            animator.SetInteger("Walk", 2);

        }
        else if (direct_x == 1)
        {
            Vector2 lscale = transform.localScale;
            if (lscale.x < 0)
                lscale.x *= -1;

            transform.localScale = lscale;

            animator.SetInteger("Walk", 2);
        }
    }

    IEnumerator ChangeColor()
    {
        GetComponent<SpriteRenderer>().color = Color.red;

        //1秒停止
        yield return new WaitForSeconds(1);

        //元の色にする
        GetComponent<SpriteRenderer>().color = Color.white;
    }


    public void GameStart()
    {
        gameController = GameObject.Find("GameController(Clone)").GetComponent<GameController>();

        gameStart = true;
    }



    public void OnAttackButton()
    {
        gameController.Attack(direct_x, direct_y);
    }



    public void OnHealButton()
    {
        if (canheal)
            gameController.Heal();
    }

    Vector3 ScreenToWorld(Vector3 Screen_Pos)
    {
        Vector3 World_Pos = Camera.main.ScreenToWorldPoint(Screen_Pos);
        World_Pos = new Vector3(World_Pos.x, World_Pos.y + 0.3f, 0);

        return World_Pos;
    }
}