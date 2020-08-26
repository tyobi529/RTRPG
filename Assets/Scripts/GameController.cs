using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;

public class GameController : MonoBehaviourPunCallbacks
{
    public GameObject[,] MasuObject = new GameObject[5, 7];

    private FieldGenerator fieldGenerator;

    //List<int[,]> MasuKind = new List<int[,]>();

    //マスの属性
    public int[,] MasuKind = new int[5, 7];

    public GameObject Yusha;
    public GameObject Enemy1;
    public GameObject Enemy2;
    public GameObject Enemy3;

    public GameObject Maou;

    //召喚コスト
    public float cost;
    public float max_cost = 30f;

    public float summon_cost = 20f;
    //trueなら召喚できる
    public bool canSummon = false;

    public int playerId;

    //private int objectId = 0;

    private int width = 150;
    private int height = 150;

    //public bool gamestart = false;

    public Text costText;


    public float MasuUpdateTime = 20;
    private float masu_update_time = 0;

    //特殊ます
    int special_x = 0;
    int special_y = 0;



    //// Start is called before the first frame update
    void Start()
    {
        playerId = PhotonNetwork.LocalPlayer.ActorNumber;

        fieldGenerator = GameObject.Find("FieldGenerator").GetComponent<FieldGenerator>();

        if (playerId == 1)
        {
            GameObject CostText = GameObject.Find("CostText");
            CostText.SetActive(false);
        }
        //プレイヤー２側から生成する。
        if (playerId == 2)
        {

            //勇者生成
            photonView.RPC(nameof(SetMasuObject), RpcTarget.All, 1, 1, 2, 0);

            //魔王生成
            photonView.RPC(nameof(SetMasuObject), RpcTarget.All, 2, 99, 2, 6);


            cost = 0;
            costText = GameObject.Find("CostText").GetComponent<Text>();


            //マス属性
            //int x = Random.Range(0, 5);
            //int y = Random.Range(0, 7);
            ////宿
            ////Debug.Log(x + " " + y);
            ////fieldGenerator.ChangeField(MasuKind[x, y], x, y);
            //photonView.RPC(nameof(ChangeField), RpcTarget.All, 1, x, y);
        }

    }

    //// Update is called once per frame
    void Update()
    {


        if (playerId == 2)
        {
            cost += Time.deltaTime;

            if (cost > max_cost)
                cost = max_cost;


            costText.text = cost.ToString("F0");

            if (cost >= summon_cost)
            {
                canSummon = true;
            }
            else
            {
                canSummon = false;
            }


            masu_update_time += Time.deltaTime;

            if (masu_update_time > MasuUpdateTime)
            {
                masu_update_time = 0;
                ChangeField();

            }
        }



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
    public void SetMasuObject(int Id, int ObjectId, int Masu_x, int Masu_y)
    {

        GameObject PlayerPrefab = null;

        if (Id == 1)
        {
            PlayerPrefab = Instantiate(Yusha) as GameObject;

        }
        //Player1.Add(Player);
        else if (Id == 2)
        {
            cost -= summon_cost;

            switch (ObjectId)
            {
                case 1:
                    PlayerPrefab = Instantiate(Enemy1) as GameObject;
                    break;
                case 2:
                    PlayerPrefab = Instantiate(Enemy2) as GameObject;
                    break;
                case 3:
                    PlayerPrefab = Instantiate(Enemy3) as GameObject;
                    break;
                case 99:
                    PlayerPrefab = Instantiate(Maou) as GameObject;
                    break;
                default:
                    Debug.Log("範囲外");
                    break;
            }

        }


        MasuObject[Masu_x, Masu_y] = PlayerPrefab;

        PlayerController playerController = PlayerPrefab.GetComponent<PlayerController>();

        playerController.playerId = Id;
        playerController.objectId = ObjectId;


        playerController.masu_x = Masu_x;
        playerController.masu_y = Masu_y;
        //playerController.objectId = objectId++;

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

        //マスの効果
        //１：宿
        if (MasuObject[Select_x, Select_y].GetComponent<PlayerController>().playerId == 1)
        {
            switch (MasuKind[Select_x, Select_y])
            {
                case 0:
                    return;
                case 1:
                    MasuObject[Select_x, Select_y].GetComponent<PlayerController>().hp = MasuObject[Select_x, Select_y].GetComponent<PlayerController>().maxhp;
                    break;
                case 2:
                    break;
            }

            MasuKind[Select_x, Select_y] = 0;
            ChangeField(0, Select_x, Select_y);


        }

    }


    public void Move(int Masu_x, int Masu_y, int Select_x, int Select_y)
    {
        photonView.RPC(nameof(Move_RPC), RpcTarget.All, Masu_x, Masu_y, Select_x, Select_y);
    }


    [PunRPC]
    public void Attack_RPC(int Attack_x, int Attack_y, int Damage_x, int Damage_y)
    {
        MasuObject[Damage_x, Damage_y].GetComponent<PlayerController>().hp -= MasuObject[Attack_x, Attack_y].GetComponent<PlayerController>().power;

        MasuObject[Attack_x, Attack_y].GetComponent<PlayerController>().canmove = false;

        if (MasuObject[Damage_x, Damage_y].GetComponent<PlayerController>().hp <= 0)
        {
            Destroy(MasuObject[Damage_x, Damage_y].GetComponent<PlayerController>().hpText);
            Destroy(MasuObject[Damage_x, Damage_y].GetComponent<PlayerController>().timeText);
            Destroy(MasuObject[Damage_x, Damage_y]);
            MasuObject[Damage_x, Damage_y] = null;

            //経験値獲得
            if (MasuObject[Attack_x, Attack_y].GetComponent<PlayerController>().playerId == 1)
            {
                MasuObject[Attack_x, Attack_y].GetComponent<PlayerController>().exp += 20;
                //レベルアップ
                if (MasuObject[Attack_x, Attack_y].GetComponent<PlayerController>().exp >= MasuObject[Attack_x, Attack_y].GetComponent<PlayerController>().levelup_exp)
                {
                    MasuObject[Attack_x, Attack_y].GetComponent<PlayerController>().exp = 0;

                    //プレイヤー２から実行する
                    if (playerId == 2)
                    {
                        LevelUp(Attack_x, Attack_y);
                    }
                }
            }
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

    [PunRPC]
    public void ChangeField(int Kind, int Masu_x, int Masu_y)
    {
        MasuKind[Masu_x, Masu_y] = Kind;
        fieldGenerator.ChangeField(Kind, Masu_x, Masu_y);

    }

    //特殊マスの生成
    public void ChangeField()
    {
        //前のマスのクリア
        photonView.RPC(nameof(ChangeField), RpcTarget.All, 0, special_x, special_y);

        //生成個数
        //int num = Random.Range(1, 3);
        int num = 1;

        for (int i = 0; i < num; i++)
        {
            //生成位置
            //int x = Random.Range(0, 5);
            //int y = Random.Range(0, 7);

            special_x = Random.Range(0, 5);
            special_y = Random.Range(0, 7);
            //マスの種類
            int kind = 1;
            photonView.RPC(nameof(ChangeField), RpcTarget.All, kind, special_x, special_y);
        }

    }




    public void GenerateEnemy(int ObjectId, int Select_x, int Select_y)
    {
        //photonView.RPC(nameof(GenerateEnemy_RPC), RpcTarget.All, num, Select_x, Select_y);

        photonView.RPC(nameof(SetMasuObject), RpcTarget.All, 2, ObjectId, Select_x, Select_y);

    }


    [PunRPC]
    public void LevelUp(int Masu_x, int Masu_y, int Hp_up, int Power_up, int Speed_up)
    {
        PlayerController playerController = MasuObject[Masu_x, Masu_y].GetComponent<PlayerController>();

        playerController.hp += Hp_up;
        playerController.power += Power_up;
        playerController.speed += Speed_up;
    }


    public void LevelUp(int Masu_x, int Masu_y)
    {
        //上昇値決定
        int hp_up = Random.Range(10, 21);
        int power_up = Random.Range(10, 21);
        int speed_up = Random.Range(10, 21);

        photonView.RPC(nameof(LevelUp), RpcTarget.All, Masu_x, Masu_y, hp_up, power_up, speed_up);


    }


    //ダメージで色を変える
    private IEnumerator DamageColor(int Damage_X, int Damage_Y) //コルーチン関数の名前
    {
        MasuObject[Damage_X, Damage_Y].GetComponent<SpriteRenderer>().color = Color.blue;

        yield return new WaitForSeconds(0.5f);

        MasuObject[Damage_X, Damage_Y].GetComponent<SpriteRenderer>().color = Color.white;


    }




}
