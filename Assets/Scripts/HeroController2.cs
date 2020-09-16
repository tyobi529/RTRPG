using Photon.Pun;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using DG.Tweening;

// MonoBehaviourPunCallbacksを継承すると、photonViewプロパティが使えるようになる
public class HeroController2 : MonoBehaviourPunCallbacks, IPunObservable
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


    int playerId;

    Animator animator;

    GameObject Canvas;
    public GameObject HPSlider;

    bool canmove = true;

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



        playerId = PhotonNetwork.LocalPlayer.ActorNumber;

        startPosition = transform.position;
        endPosition = transform.position;




        animator = GetComponent<Animator>();

        Canvas = GameObject.Find("Canvas");

        HPSlider = Instantiate(HPSlider) as GameObject;
        HPSlider.transform.parent = Canvas.transform;


        if (photonView.IsMine)
        {
            //MainCamera = GameObject.Find("Main Camera");
            //MainCamera.GetComponent<Camera>().orthographicSize = 3f;
        }


        //初期位置
        //Vector3 screenPos = Camera.main.WorldToScreenPoint(transform.position);
        //masu_x = (int)(screenPos.x / width);
        //masu_y = (int)((screenPos.y - footer - 60f) / width);

        masu_x = 3;
        masu_y = 0;
        transform.position = ScreenToWorld(new Vector2(width * masu_x + width / 2, footer + width * masu_y + width / 2));

    }




    private void Update()
    {


        Vector2 Pos = Camera.main.WorldToScreenPoint(transform.position);
        Pos = new Vector2(Pos.x, Pos.y + 50f);

        HPSlider.transform.position = Pos;

        HPSlider.GetComponent<Slider>().value = (float)(hp) / (float)(maxhp);


        if (gameStart)
        {


            //Vector3 screenPos = Camera.main.WorldToScreenPoint(transform.position);

            //masu_x = (int)(screenPos.x / width);
            ////40fは補正
            //masu_y = (int)((screenPos.y - footer - 60f) / width);

            //if (first_x != masu_x || first_y != masu_y)
            //{
            //    ChangeField(0, first_x, first_y);

            //    first_x = masu_x;
            //    first_y = masu_y;

            //    ChangeField(3, first_x, first_y);


            //    //移動さきに敵がいたらレベルアップ
            //    if (gameController.MasuObject[masu_x, masu_y] != null)
            //        gameController.LevelUp(masu_x, masu_y);
            //}


            if (Input.GetKeyDown(KeyCode.Space))
            {
                //Debug.Log("masu_x" + masu_x);
                //Debug.Log("masu_y" + masu_y);
                Debug.Log("direct_x" + direct_x);
                Debug.Log("direct_y" + direct_y);


            }


            // 自身が生成したオブジェクトだけに移動処理を行う
            if (photonView.IsMine)
            {
                //カメラ
                //MainCamera.transform.position = new Vector3(transform.position.x, transform.position.y, -10);


                float dv_x = 0f;
                float dv_y = 0f;

                if (Input.GetKey(KeyCode.LeftArrow))
                {
                    dv_x = -1;
                    dv_y = 0;
                    direct_x = -1;
                    direct_y = 0;

                    Vector2 lscale = transform.localScale;
                    if (lscale.x > 0)
                        lscale.x *= -1;

                    transform.localScale = lscale;

                    animator.SetInteger("Walk", 2);
                }

                else if (Input.GetKey(KeyCode.RightArrow))
                {
                    dv_x = 1;
                    dv_y = 0;
                    direct_x = 1;
                    direct_y = 0;

                    Vector2 lscale = transform.localScale;
                    if (lscale.x < 0)
                        lscale.x *= -1;

                    transform.localScale = lscale;

                    animator.SetInteger("Walk", 2);

                }

                else if (Input.GetKey(KeyCode.UpArrow))
                {
                    dv_x = 0;
                    dv_y = 1;
                    direct_x = 0;
                    direct_y = 1;

                    animator.SetInteger("Walk", 0);

                }

                else if (Input.GetKey(KeyCode.DownArrow))
                {
                    dv_x = 0;
                    dv_y = -1;
                    direct_x = 0;
                    direct_y = -1;

                    animator.SetInteger("Walk", 1);

                }

                //var direction = new Vector2(dv_x, dv_y) * Time.deltaTime * speed / 100f;

                //transform.Translate(direction);

                //masu_x += direct_x;
                //masu_y += direct_y;


                Move(direct_x, direct_y);

                //Vector2 MovePos = Camera.main.ScreenToWorldPoint(width * masu_x + width / 2, footer + width * masu_y)



            }

            else
            {
                // 受信時の座標から受信した座標へ補間移動する
                //elapsedTime += Time.deltaTime;
                //transform.position = Vector3.Lerp(startPosition, endPosition, elapsedTime / InterpolationDuration);
            }

        }
    }

    IEnumerator Move(int Direct_x, int Direct_y)
    {
        Debug.Log(Direct_x);
        Debug.Log(Direct_y);
        canmove = false;
        Vector2 MovePos = Camera.main.ScreenToWorldPoint(new Vector2(width * (masu_x + Direct_x) + width / 2, footer + width * (masu_y + Direct_y)));


        transform.DOMove(MovePos, moveTime);
        yield return new WaitForSeconds(moveTime);

        masu_x += Direct_x;
        masu_y += Direct_y;
        canmove = true;
    }

    [PunRPC]
    void ChangeField(int Kind, int Masu_x, int Masu_y)
    {
        fieldGenerator.ChangeField(Kind, Masu_x, Masu_y);
    }

    IEnumerator ChangeField(int Masu_x, int Masu_y)
    {
        //赤色にする
        photonView.RPC(nameof(ChangeField), RpcTarget.AllViaServer, 2, Masu_x, Masu_y);

        //1秒停止
        yield return new WaitForSeconds(1);

        //元の色にする
        photonView.RPC(nameof(ChangeField), RpcTarget.AllViaServer, 0, Masu_x, Masu_y);
    }


    IEnumerator ChangeColor()
    {
        GetComponent<SpriteRenderer>().color = Color.red;

        //1秒停止
        yield return new WaitForSeconds(1);

        //元の色にする
        GetComponent<SpriteRenderer>().color = Color.white;
    }









    void IPunObservable.OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(hp);

            //stream.SendNext(transform.position);

            stream.SendNext(direct_x);
            stream.SendNext(direct_y);

        }
        else
        {
            //// 受信時の座標を、補間の開始座標にする
            //startPosition = transform.position;
            //// 受信した座標を、（transfrom.positionへ直接反映させずに）補間の終了座標にする
            //endPosition = (Vector3)stream.ReceiveNext();
            //elapsedTime = 0f;

            hp = (int)stream.ReceiveNext();


            direct_x = (int)stream.ReceiveNext();
            direct_y = (int)stream.ReceiveNext();
        }
    }






    public void GameStart()
    {
        gameController = GameObject.Find("GameController(Clone)").GetComponent<GameController>();







        gameStart = true;
    }


    Vector3 ScreenToWorld(Vector3 Screen_Pos)
    {
        Vector3 World_Pos = Camera.main.ScreenToWorldPoint(Screen_Pos);
        World_Pos = new Vector3(World_Pos.x, World_Pos.y, 0);

        return World_Pos;
    }
}