using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class GameController : MonoBehaviourPunCallbacks
{
    public GameObject[,] MasuObject = new GameObject[5, 7];

    public GameObject PlayerPrefab1;
    public GameObject PlayerPrefab2;

    public GameObject[] PlayerPrefab = new GameObject[10];

    //private GameObject[] Player1 = new GameObject[3];
    //private GameObject[] Player2 = new GameObject[3];
    List<GameObject> Player1 = new List<GameObject>();
    List<GameObject> Player2 = new List<GameObject>();


    public int playerId;

    private int objectId = 0;

    private int width = 150;
    private int height = 150;

    //public bool gamestart = false;


    //// Start is called before the first frame update
    void Start()
    {
        playerId = PhotonNetwork.LocalPlayer.ActorNumber;

        //プレイヤー２側から生成する。
        if (playerId == 2)
        {
            //合計6個
            //int number = 0;
            for (int i = 0; i < 3; i++)
            //while (number < 6)
            {
                bool isok = false;

                while (!isok)
                {
                    int masu_x = Random.Range(0, 5);
                    int masu_y = Random.Range(0, 7);

                    if (MasuObject[masu_x, masu_y] != null)
                        continue;

                    //MasuObject配列にいれる
                    photonView.RPC(nameof(SetMasuObject), RpcTarget.All, 1, masu_x, masu_y);

                    isok = true;
                }

                isok = false;

                while (!isok)
                {
                    int masu_x = Random.Range(0, 5);
                    int masu_y = Random.Range(0, 7);

                    if (MasuObject[masu_x, masu_y] != null)
                        continue;

                    photonView.RPC(nameof(SetMasuObject), RpcTarget.All, 2, masu_x, masu_y);

                    isok = true;

                }



            }

        }


    
        

    }

    //// Update is called once per frame
    void Update()
    {


        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("盤面の情報");
            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 7; j++)
                {
                    if (MasuObject[i, j] != null)
                    {
                        Debug.Log(i);
                        Debug.Log(j);
                        Debug.Log(MasuObject[i, j]);
                    }

                }
            }
        }

    }

    [PunRPC]
    public void SetMasuObject(int Id, int Masu_x, int Masu_y)
    {

        GameObject PlayerPrefab = null;

        if (Id == 1)
        {
            PlayerPrefab = Instantiate(PlayerPrefab1) as GameObject;

        }
        //Player1.Add(Player);
        else if (Id == 2)
        {
            PlayerPrefab = Instantiate(PlayerPrefab2) as GameObject;

        }

        MasuObject[Masu_x, Masu_y] = PlayerPrefab;

        PlayerController playerController = PlayerPrefab.GetComponent<PlayerController>();

        playerController.playerId = Id;
        //playerController.objectId = i;


        playerController.masu_x = Masu_x;
        playerController.masu_y = Masu_y;
        playerController.objectId = objectId++;

        Vector3 GeneratePos_screen = new Vector3(width / 2 + width * Masu_x, 284 + height / 2 + height * Masu_y, 0);
        Vector3 GeneratePos_world = Camera.main.ScreenToWorldPoint(GeneratePos_screen);

        GeneratePos_world = new Vector3(GeneratePos_world.x, GeneratePos_world.y, 0);

        PlayerPrefab.transform.position = GeneratePos_world;
    }


    [PunRPC]
    public void Move_RPC(int Masu_x, int Masu_y, int Select_x, int Select_y)
    {

        MasuObject[Select_x, Select_y] = MasuObject[Masu_x, Masu_y];

        MasuObject[Masu_x, Masu_y] = null;

        Vector3 GeneratePos_screen = new Vector3(width / 2 + width * Select_x, 284 + height / 2 + height * Select_y, 0);
        Vector3 GeneratePos_world = Camera.main.ScreenToWorldPoint(GeneratePos_screen);

        GeneratePos_world = new Vector3(GeneratePos_world.x, GeneratePos_world.y, 0);


        MasuObject[Select_x, Select_y].transform.position = GeneratePos_world;

        MasuObject[Select_x, Select_y].GetComponent<PlayerController>().canmove = false;

    }


    public void Move(int Masu_x, int Masu_y, int Select_x, int Select_y)
    {
        photonView.RPC(nameof(Move_RPC), RpcTarget.All, Masu_x, Masu_y, Select_x, Select_y);
    }


    [PunRPC]
    public void Attack_RPC(int Attack_x, int Attack_y, int Damage_x, int Damage_y)
    {
        Debug.Log("ダメージ");
        MasuObject[Damage_x, Damage_y].GetComponent<PlayerController>().hp -= MasuObject[Attack_x, Attack_y].GetComponent<PlayerController>().power;




        MasuObject[Attack_x, Attack_y].GetComponent<PlayerController>().canmove = false;

        if (MasuObject[Damage_x, Damage_y].GetComponent<PlayerController>().hp <= 0)
        {
            Destroy(MasuObject[Damage_x, Damage_y].GetComponent<PlayerController>().hpText);
            Destroy(MasuObject[Damage_x, Damage_y].GetComponent<PlayerController>().timeText);
            Destroy(MasuObject[Damage_x, Damage_y]);
            MasuObject[Damage_x, Damage_y] = null;
        }

        else
        {
            //ダメージを受けると色変える
            StartCoroutine(DamageColor(Damage_x, Damage_y));
        }
    }


    public void Attack(int Attack_x, int Attack_y, int Damage_x, int Damage_y)
    {
        photonView.RPC(nameof(Attack_RPC), RpcTarget.All, Attack_x, Attack_y, Damage_x, Damage_y);
    }


    //ダメージで色を変える
    private IEnumerator DamageColor(int Damage_X, int Damage_Y) //コルーチン関数の名前
    {
        MasuObject[Damage_X, Damage_Y].GetComponent<SpriteRenderer>().color = Color.blue;

        yield return new WaitForSeconds(0.5f);

        MasuObject[Damage_X, Damage_Y].GetComponent<SpriteRenderer>().color = Color.white;


    }


    //3秒間矢印を出す

    //public void Attack(int Attack_x, int Attack_y, int Damage_x, int Damage_y)
    //{
    //    MasuObject[Damage_x, Damage_y].GetComponent<PlayerController>().hp -= MasuObject[Attack_x, Attack_y].GetComponent<PlayerController>().power;
    //}




    //[PunRPC]
    //public void UpdateMasu(int Id, int ObjectId, int Masu_x, int Masu_y, int Select_x, int Select_y)
    //{
    //    //PlayerController playerController = Player.GetComponent<PlayerController>();
    //    MasuObject[Masu_x, Masu_y] = null;

    //    //playerController.masu_x = Select_x;
    //    //playerController.masu_y = Select_y;

    //    if (Id == 1)
    //        MasuObject[Select_x, Select_y] = Player1[ObjectId];
    //    else if (Id == 2)
    //        MasuObject[Select_x, Select_y] = Player2[ObjectId];




    //}

    // データを送受信するメソッド
    //void IPunObservable.OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    //{
    //    if (stream.IsWriting)
    //    {
    //        // 自身側が生成したオブジェクトの場合は
    //        // 色相値と移動中フラグのデータを送信する
    //        stream.SendNext(gamestart);
    //    }
    //    else
    //    {
    //        // 他プレイヤー側が生成したオブジェクトの場合は
    //        // 受信したデータから色相値と移動中フラグを更新する
    //        gamestart = (bool)stream.ReceiveNext();

    //    }
    //}

    //移動先にオブジェクトがあるかチェック
    //存在しなければ移動する
    //public GameObject MasuCheck(GameObject Player, int Masu_x, int Masu_y)
    //{
    //    //if (!(0 <= Masu_x && Masu_x < 5 && 0 <= Masu_y && Masu_y < 7))
    //    //{
    //    //    Debug.Log("範囲外");
    //    //    return;
    //    //}

    //    //if (MasuObject[Masu_x, Masu_y] == null)
    //    //{
    //    //    //Debug.Log("aaa");
    //    //    ////移動
    //    //    //photonView.RPC(nameof(Move), RpcTarget.All, Player, Masu_x, Masu_y);
    //    //    return null;
    //    //}
    //    //else
    //    //{
    //    //    //攻撃
    //    //    return Masu
    //    //}
    //}


    //[PunRPC]
    //public void Move(GameObject Player, int Masu_x, int Masu_y)
    //{
    //    PlayerController playerController = Player.GetComponent<PlayerController>();

    //    MasuObject[playerController.masu_x, playerController.masu_y] = null;
    //    MasuObject[Masu_x, Masu_y] = Player;

    //    Vector3 Pos_screen = new Vector3(width / 2 + width * Masu_x, 284 + height / 2 + height * Masu_y, 0);
    //    Vector3 Pos_world = Camera.main.ScreenToWorldPoint(Pos_screen);
    //    Pos_world = new Vector3(Pos_world.x, Pos_world.y, 0);

    //    Player.transform.position = Pos_world;

    //}




}
