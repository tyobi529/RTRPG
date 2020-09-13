using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;

public class GameController : MonoBehaviourPunCallbacks
{
    //生成時のステータス
    //public int[] enemy_hp = new int[3];
    //public int[] enemy_attack = new int[3];
    //public float[] enemy_speed = new float[3];

    //public int hero_hp = 0;
    //public int hero_attack = 0;
    //public float hero_speed = 0;

    //public int maou_hp = 0;
    //public int maou_attack = 0;
    //public float maou_speed = 0;

    //public int[] hero_hpup = new int[2];
    public int[] hero_attackup = new int[2];
    public int[] hero_defenceup = new int[2];

    public int[] hero_speedup = new int[2];

    //public int[] enemy_lower_hpup = new int[3];
    //public int[] enemy_upper_hpup = new int[3];
    //public int[] enemy_lower_attackup = new int[3];
    //public int[] enemy_upper_attackup = new int[3];
    //public int[] enemy_lower_speedup = new int[3];
    //public int[] enemy_upper_speedup = new int[3];

    public int[] enemy_attackup = new int[3];
    public int[] enemy_defenceup = new int[3];
    public int[] enemy_speedup = new int[3];


    //勇者のステータス上昇値
    //０：HP
    //１：攻撃
    //２：スピード
    int[] powerup = new int[3] { 0, 0, 0 };

    //横の数
    int yoko;
    int tate;
    //private int width = Screen.width / 6;
    private float width;


    //下の部分の高さ
    private int footer;


    public GameObject[,] MasuObject;

    //勇者の位置
    private int hero_x = 0;
    private int hero_y = 0;

    private FieldGenerator fieldGenerator;

    //List<int[,]> MasuKind = new List<int[,]>();

    //マスの属性
    public int[,] MasuKind;

    //特殊マスを事前にいれておく
    //private int[] special_masu_x = new int[10];
    //private int[] special_masu_y = new int[10];
    //private int[] special_masu_kind = new int[10];

    //特技の種類
    int special_kind = 0;


    PlayerStatus playerStatus;


    public GameObject Hero;
    public GameObject Enemy1;
    public GameObject Enemy2;
    public GameObject Enemy3;

    public GameObject Maou;

    public GameObject Siro;

    //召喚コスト
    //public float cost;
    //public float max_cost = 30f;

    //public float summon_cost = 20f;
    //trueなら召喚できる
    //public bool canSummon = false;

    public int playerId;

    //private int objectId = 0;



    //public bool gamestart = false;

    //public Text costText;


    public float MasuUpdateTime = 20;
    //private float masu_update_time = 0;

    //特殊ます
    //int special_x = 0;
    //int special_y = 0;


    //int summon_count = 0;


    //特技ボタン
    //private Button SpecialButton1;
    //private Button SpecialButton2;
    //private Button SpecialButton3;

    //private GameObject SpecialButton1;
    //private GameObject SpecialButton2;
    //private GameObject SpecialButton3;

    private GameObject[] SpecialButton = new GameObject[3];



    private Method method;

    //bool gamestart = false;
    //float limitTime = 0f;
    //Slider limiter;

    public GameObject timeController;

    public bool doubleAttack = true;


    //// Start is called before the first frame update
    void Start()
    {
        fieldGenerator = GameObject.Find("FieldGenerator").GetComponent<FieldGenerator>();
        yoko = fieldGenerator.yoko;
        tate = fieldGenerator.tate;
        width = Screen.width / yoko;
        MasuObject = new GameObject[yoko, tate];
        MasuKind = new int[yoko, tate];

        footer = GameObject.Find("FieldGenerator").GetComponent<FieldGenerator>().footer;


        playerId = PhotonNetwork.LocalPlayer.ActorNumber;


        playerStatus = GameObject.Find("PlayerStatus").GetComponent<PlayerStatus>();


        method = GameObject.Find("Method").GetComponent<Method>();

        //limiter = GameObject.Find("Limiter").GetComponent<Slider>();

        timeController = GameObject.Find("TimeController");

        if (playerId == 1)
        {

            //SpecialButton[0] = GameObject.Find("SpecialButton1");
            //SpecialButton[1] = GameObject.Find("SpecialButton2");
            //SpecialButton[2] = GameObject.Find("SpecialButton3");

            //for (int i = 0; i < 3; i++)
            //{
            //    SpecialButton[i].SetActive(false);
            //}
        }

        //プレイヤー２側から生成する。
        if (playerId == 2)
        {

            //勇者生成
            //photonView.RPC(nameof(SetMasuObject), RpcTarget.AllViaServer, 1, 1, 0, 0);
            photonView.RPC(nameof(SetMasuObject), RpcTarget.AllViaServer, 1, 1, 2, 1);

            //城生成
            for (int i = 0; i < yoko; i++)
                photonView.RPC(nameof(SetMasuObject), RpcTarget.AllViaServer, 1, 2, i, 0);


            //魔王生成
            //photonView.RPC(nameof(SetMasuObject), RpcTarget.AllViaServer, 2, 99, yoko - 1, tate - 1);
            photonView.RPC(nameof(SetMasuObject), RpcTarget.AllViaServer, 2, 99, 2, tate - 1);



            //ステージ生成

            //SetUpFirstField();



            DecidePowerUpValue(0);
            DecidePowerUpValue(1);
            DecidePowerUpValue(2);



            DecideSpecial();

            int x = Random.Range(0, yoko);
            int y = Random.Range(1, tate - 2);
            photonView.RPC(nameof(ChangeField), RpcTarget.AllViaServer, 1, x, y);


            //ゲーム開始
            photonView.RPC(nameof(GameStart), RpcTarget.AllViaServer);

        }

    }

    //// Update is called once per frame
    void Update()
    {


        //if (playerId == 2)
        //{

        //    masu_update_time += Time.deltaTime;

        //    if (masu_update_time > MasuUpdateTime)
        //    {
        //        masu_update_time = 0;
        //        ResetField();

        //    }
        //}



        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("盤面の情報");
            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 7; j++)
                {
                    //if (MasuObject[i, j] != null)
                    //{
                    //    Debug.Log(i);
                    //    Debug.Log(j);
                    //    //Debug.Log(MasuObject[i, j]);

                    //}

                    if (MasuKind[i, j] != 0)
                    {
                        Debug.Log(i);
                        Debug.Log(j);
                        Debug.Log(MasuKind[i, j]);
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
            switch (ObjectId)
            {
                case 1:
                    PlayerPrefab = Instantiate(Hero) as GameObject;

                    PlayerPrefab.GetComponent<PlayerController>().hp = playerStatus.hero_hp;
                    PlayerPrefab.GetComponent<PlayerController>().attack = playerStatus.hero_attack;
                    PlayerPrefab.GetComponent<PlayerController>().defence = playerStatus.hero_defence;
                    PlayerPrefab.GetComponent<PlayerController>().speed = playerStatus.hero_speed;
                    break;
                case 2:
                    PlayerPrefab = Instantiate(Siro) as GameObject;

                    PlayerPrefab.GetComponent<PlayerController>().hp = playerStatus.siro_hp;
                    break;
                default:
                    Debug.Log("エラー");
                    break;
            }
            

        }

        else if (Id == 2)
        {

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

            //魔物
            if (ObjectId != 99)
            {
                PlayerPrefab.GetComponent<PlayerController>().hp = playerStatus.enemy_hp[ObjectId - 1];
                PlayerPrefab.GetComponent<PlayerController>().attack = playerStatus.enemy_attack[ObjectId - 1];
                PlayerPrefab.GetComponent<PlayerController>().defence = playerStatus.enemy_defence[ObjectId - 1];
                PlayerPrefab.GetComponent<PlayerController>().speed = playerStatus.enemy_speed[ObjectId - 1];

                //summon_count++;
                ////５回召喚したらレベルアップ
                //if (summon_count % 5 == 0 && Id == 2)
                //{
                //    EnemyLevelUp();
                //}
            }
            //魔王
            else
            {
                PlayerPrefab.GetComponent<PlayerController>().hp = playerStatus.maou_hp;
                PlayerPrefab.GetComponent<PlayerController>().attack = playerStatus.maou_attack;
                PlayerPrefab.GetComponent<PlayerController>().defence = playerStatus.maou_defence;
                PlayerPrefab.GetComponent<PlayerController>().speed = playerStatus.maou_speed;
            }
           

        }


        MasuObject[Masu_x, Masu_y] = PlayerPrefab;

        PlayerController playerController = PlayerPrefab.GetComponent<PlayerController>();

        playerController.playerId = Id;
        playerController.objectId = ObjectId;


        playerController.masu_x = Masu_x;
        playerController.masu_y = Masu_y;


        PlayerPrefab.transform.position = ScreenToWorld(new Vector3(width / 2 + width * Masu_x, footer + width / 2 + width * Masu_y, 0));


    }


    [PunRPC]
    public void Move_RPC(int Masu_x, int Masu_y, int Select_x, int Select_y)
    {

        MasuObject[Select_x, Select_y] = MasuObject[Masu_x, Masu_y];

        MasuObject[Masu_x, Masu_y] = null;

        MasuObject[Select_x, Select_y].transform.position = ScreenToWorld(new Vector3(width / 2 + width * Select_x, footer + width / 2 + width * Select_y, 0));

        MasuObject[Select_x, Select_y].GetComponent<PlayerController>().masu_x = Select_x;
        MasuObject[Select_x, Select_y].GetComponent<PlayerController>().masu_y = Select_y;



        //HP表示
        //MasuObject[Select_x, Select_y].GetComponent<PlayerController>().hpText.transform.position = new Vector3(width / 2 + width * Select_x, footer + width / 4 + width * Select_y, 0);
        //MasuObject[Select_x, Select_y].GetComponent<PlayerController>().hpText.GetComponent<Text>().text = MasuObject[Select_x, Select_y].GetComponent<PlayerController>().hp.ToString();

        //MasuObject[Select_x, Select_y].GetComponent<PlayerController>().phase = 0;

        ////時間表示
        //MasuObject[Select_x, Select_y].GetComponent<PlayerController>().timer.SetActive(true);
        //MasuObject[Select_x, Select_y].GetComponent<PlayerController>().timer.transform.position = new Vector3(width / 4 * 3 + width * Select_x, footer + width / 4 * 3 + width * Select_y, 0);

        CoolTime(MasuObject[Select_x, Select_y]);

        //マスの効果
        //１：宿
        if (MasuObject[Select_x, Select_y].GetComponent<PlayerController>().playerId == 1)
        {
 
            hero_x = Select_x;
            hero_y = Select_y;


            SpecialMasu(Select_x, Select_y);

        }

    }


    //攻撃or移動後にクールタイム
    void CoolTime(GameObject Object)
    {
        Object.GetComponent<PlayerController>().phase = 0;

        //時間表示
        Object.GetComponent<PlayerController>().timer.SetActive(true);
        Object.GetComponent<PlayerController>().timer.transform.position = new Vector3(width / 4 * 3 + width * Object.GetComponent<PlayerController>().masu_x, footer + width / 4 * 3 + width * Object.GetComponent<PlayerController>().masu_y, 0);
    }


    void SpecialMasu(int Select_x, int Select_y)
    {
        switch (MasuKind[Select_x, Select_y])
        {
            case 0:
                return;
            case 1:
                //MasuObject[Select_x, Select_y].GetComponent<PlayerController>().hp = MasuObject[Select_x, Select_y].GetComponent<PlayerController>().maxhp;
                BedMasu(Select_x, Select_y);
                break;
            case 2:
                //MasuObject[Select_x, Select_y].GetComponent<PlayerController>().maxhp += powerup[0];
                //HpUp(Select_x, Select_y);
                AttackUp(Select_x, Select_y);

                break;
            case 3:
                //MasuObject[Select_x, Select_y].GetComponent<PlayerController>().attack += powerup[1];
                DefenceUp(Select_x, Select_y);

                break;
            case 4:
                //MasuObject[Select_x, Select_y].GetComponent<PlayerController>().speed += powerup[2];
                SpeedUp(Select_x, Select_y);
                break;
            //case 6:
            //    if (playerId == 1)
            //        GetSpecial(special_kind);
            //    break;
            default:
                break;
        }

        int kind = MasuKind[Select_x, Select_y];

        MasuKind[Select_x, Select_y] = 0;
        ChangeField(0, Select_x, Select_y);

        //新しくマスを出現させる
        if (playerId == 2)
            SetUpField(kind);
       
    }


    void BedMasu(int Select_x, int Select_y)
    {
        MasuObject[Select_x, Select_y].GetComponent<PlayerController>().hp = MasuObject[Select_x, Select_y].GetComponent<PlayerController>().maxhp;

    }

    //void HpUp(int Select_x, int Select_y)
    //{
    //    MasuObject[Select_x, Select_y].GetComponent<PlayerController>().maxhp += powerup[0];

    //    if (playerId == 2)
    //        DecidePowerUpValue(0);
    //}

    void AttackUp(int Select_x, int Select_y)
    {
        MasuObject[Select_x, Select_y].GetComponent<PlayerController>().attack += powerup[0];

        if (playerId == 2)
            DecidePowerUpValue(0);
    }

    void DefenceUp(int Select_x, int Select_y)
    {
        MasuObject[Select_x, Select_y].GetComponent<PlayerController>().defence += powerup[1];

        if (playerId == 2)
            DecidePowerUpValue(1);
    }

    void SpeedUp(int Select_x, int Select_y)
    {
        MasuObject[Select_x, Select_y].GetComponent<PlayerController>().speed += powerup[2];

        if (playerId == 2)
            DecidePowerUpValue(2);
    }


    //void GetSpecial(int Kind)
    //{
    //    GameObject special_button = null;

    //    for (int i = 0; i < 3; i++)
    //    {
    //        if (!SpecialButton[i].activeSelf)
    //        {
    //            SpecialButton[i].SetActive(true);
    //            special_button = SpecialButton[i];
    //            break;
    //        }
    //    }

    //    if (special_button == null)
    //    {
    //        Debug.Log("特技いっぱい");
    //        return;
    //    }

    //    switch(Kind)
    //    {
    //        case 1:
    //            special_button.transform.GetChild(0).gameObject.GetComponent<Text>().text = "四方斬り";
    //            special_button.GetComponent<Button>().onClick.AddListener(Sihogiri);
    //            break;
    //        case 2:
    //            special_button.transform.GetChild(0).gameObject.GetComponent<Text>().text = "ななめ斬り";
    //            special_button.GetComponent<Button>().onClick.AddListener(Nanamegiri);
    //            break;
    //        case 3:
    //            special_button.transform.GetChild(0).gameObject.GetComponent<Text>().text = "回転斬り";
    //            special_button.GetComponent<Button>().onClick.AddListener(Kaitengiri);
    //            break;
    //        case 4:
    //            special_button.transform.GetChild(0).gameObject.GetComponent<Text>().text = "上斬り";
    //            special_button.GetComponent<Button>().onClick.AddListener(Uegiri);
    //            break;
    //        case 5:
    //            special_button.transform.GetChild(0).gameObject.GetComponent<Text>().text = "下斬り";
    //            special_button.GetComponent<Button>().onClick.AddListener(Sitagiri);
    //            break;
    //        case 6:
    //            special_button.transform.GetChild(0).gameObject.GetComponent<Text>().text = "右斬り";
    //            special_button.GetComponent<Button>().onClick.AddListener(Migigiri);
    //            break;
    //        case 7:
    //            special_button.transform.GetChild(0).gameObject.GetComponent<Text>().text = "左斬り";
    //            special_button.GetComponent<Button>().onClick.AddListener(Hidarigiri);
    //            break;
    //        default:
    //            break;
    //    }

    //    //次の特技を決定する
    //    DecideSpecial();
    //}

    //ステータスの上昇値を決める
    public void DecidePowerUpValue(int num)
    {
        switch (num)
        {
            case 0:
                powerup[0] = Random.Range(hero_attackup[0], hero_attackup[1] - 1);
                break;
            case 1:
                powerup[1] = Random.Range(hero_defenceup[0], hero_defenceup[1] - 1);
                break;
            case 2:
                powerup[2] = Random.Range(hero_speedup[0], hero_speedup[1] - 1);
                break;
            default:
                Debug.Log("エラー");
                break;
        }

        //プレイヤー１にも共有
        photonView.RPC(nameof(SendValue), RpcTarget.AllViaServer, num, powerup[num]);
    }

    [PunRPC]
    public void SendValue(int num, int Value)
    {
        powerup[num] = Value;
    }

    //手に入れる特技を決める
    public void DecideSpecial()
    {
        special_kind = Random.Range(1, 8);
        //special_kind = 7;

        //プレイヤー１にも共有
        photonView.RPC(nameof(SendSpecialValue), RpcTarget.AllViaServer, special_kind);

    }

    [PunRPC]
    public void SendSpecialValue(int Special)
    {
        special_kind = Special;
    }



    [PunRPC]
    public void ChangeField(int Kind, int Masu_x, int Masu_y)
    {
        MasuKind[Masu_x, Masu_y] = Kind;
        fieldGenerator.ChangeField(Kind, Masu_x, Masu_y);

    }

    //最初のフィールドを決定する
    public void SetUpFirstField()
    {
        //生成個数
        //int num = Random.Range(1, 3);
        int num = 6;
        int kind = 0;
        int special_x = 0;
        int special_y = 0;
        //bool isok = false;

        //int stop_count = 0;

        Random.InitState(System.DateTime.Now.Millisecond);

        List<int> a = new List<int>();
        for (int i = 0; i < 25; i++)
        {
            a.Add(i);

        }

        a.RemoveAt(10);

        for (int i = 0; i < num; i++)
        {
            //stop_count = 0;

            //do
            //{

            //    Random.InitState(System.DateTime.Now.Millisecond);

            //    //if (Input.GetKeyDown(KeyCode.Space))
            //    //    break;

            //    stop_count++;

            //    if (stop_count > 100)
            //    {
            //        Debug.Log("無限ループ");
            //        break;

            //    }

            //    //isok = true;

            //    special_x = Random.Range(0, yoko);

            //    //if (special_x >= 2)
            //    //    special_y = Random.Range(0, tate - special_x + 1);
            //    //else
            //    //    special_y = Random.Range(0, tate);

            //    special_y = Random.Range(0, tate - 2);



            //    if (special_x == 2 && special_y == 0)
            //        continue;


            //    //if (MasuObject[special_x, special_y] != null)
            //    //    continue;


            //    if (i == 0)
            //        break;

            //    for (int j = 0; j < i; j++)
            //    {
            //        if (special_x == special_masu_x[j] && special_y == special_masu_y[j])
            //            continue;


            //        if (j == i - 1)
            //            break;
            //    }


            //} while (true);


            int b = Random.Range(0, a.Count);
            special_x = a[b] / 5;
            special_y = a[b] % 5;
            a.RemoveAt(b);

            kind = i + 1;

            photonView.RPC(nameof(ChangeField), RpcTarget.AllViaServer, kind, special_x, special_y);


            //special_masu_x[i] = special_x;
            //special_masu_y[i] = special_y;
            //special_masu_kind[i] = kind;


        }

        //パラメータマスを追加
        for (int i = 1; i < 4; i++)
        {
            int b = Random.Range(0, a.Count);
            special_x = a[b] / 5;
            special_y = a[b] % 5;
            a.RemoveAt(b);

            kind = i + 1;

            photonView.RPC(nameof(ChangeField), RpcTarget.AllViaServer, kind, special_x, special_y);
        }
    }

    //次のフィールドの決定
    public void SetUpField(int Kind)
    {
        int x = 0;
        int y = 0;

        (x, y) = SearchNoneField();

        photonView.RPC(nameof(ChangeField), RpcTarget.AllViaServer, Kind, x, y);


        
    }

    //オブジェクトと特殊効果がないマスを探す。
    (int X, int Y) SearchNoneField()
    {
        int special_x = 0;
        int special_y = 0;

        do
        {
            if (Input.GetKeyDown(KeyCode.Space))
                break;

            special_x = Random.Range(0, yoko);

            //if (special_x >= 2)
            //    special_y = Random.Range(0, tate - special_x + 1);
            //else
            //    special_y = Random.Range(0, tate);

            special_y = Random.Range(0, tate - 2);



            //if (special_x == 0 && special_y == 0)
            //    continue;

            if (special_x == 2 && special_y == 0)
                continue;

            //勇者のマスには出ない
            if (MasuObject[special_x, special_y] != null)
            {
                if (MasuObject[special_x, special_y].GetComponent<PlayerController>().playerId == 1)
                    continue;

            }

            if (MasuKind[special_x, special_y] != 0)
                continue;


            break;

        } while (true);

        return (special_x, special_y);
    }


    [PunRPC]
    public void CheckMasu_RPC(int Masu_x, int Masu_y, int Select_x, int Select_y)
    {
        //オブジェクトが無ければ移動する
        if (MasuObject[Select_x, Select_y] == null)
            Move_RPC(Masu_x, Masu_y, Select_x, Select_y);


        //相手のオブジェクトなら攻撃する。
        else if (MasuObject[Masu_x, Masu_y].GetComponent<PlayerController>().playerId != MasuObject[Select_x, Select_y].GetComponent<PlayerController>().playerId)
        {
            MasuObject[Masu_x, Masu_y].transform.position = ScreenToWorld(new Vector3(width / 2 + width * Masu_x, footer + width / 2 + width * Masu_y, 0));

            //StartCoroutine(DamageColor(Select_x, Select_y));

            //Attack_RPC(Masu_x, Masu_y, Select_x, Select_y);
            method.Attack(MasuObject[Masu_x, Masu_y], MasuObject[Select_x, Select_y]);

            //MasuObject[Masu_x, Masu_y].GetComponent<PlayerController>().phase = 0;

            ////時間表示
            //MasuObject[Masu_x, Masu_y].GetComponent<PlayerController>().timer.SetActive(true);
            //MasuObject[Masu_x, Masu_y].GetComponent<PlayerController>().timer.transform.position = new Vector3(width / 4 * 3 + width * Masu_x, footer + width / 4 * 3 + width * Masu_y, 0);

            CoolTime(MasuObject[Masu_x, Masu_y]);

            if (MasuObject[Select_x, Select_y] != null && doubleAttack)
                method.Attack(MasuObject[Select_x, Select_y], MasuObject[Masu_x, Masu_y]);


        }

        //自分のオブジェクトならキャンセルする。
        else
            MasuObject[Masu_x, Masu_y].transform.position = ScreenToWorld(new Vector3(width / 2 + width * Masu_x, footer + width / 2 + width * Masu_y, 0));
    }


    public void CheckMasu(int Masu_x, int Masu_y, int Select_x, int Select_y)
    {
        photonView.RPC(nameof(CheckMasu_RPC), RpcTarget.AllViaServer, Masu_x, Masu_y, Select_x, Select_y);

    }

    [PunRPC]
    public void Summon_RPC(int ObjectId, int Select_x, int Select_y)
    {
        if (MasuObject[Select_x, Select_y] == null)
        {
            //photonView.RPC(nameof(SetMasuObject), RpcTarget.All, 2, ObjectId, Select_x, Select_y);
            SetMasuObject(2, ObjectId, Select_x, Select_y);


            if (playerId == 2)
            {
                SetObjectController objectController = GameObject.Find("Enemy" + ObjectId.ToString()).GetComponent<SetObjectController>();
                objectController.summon_phase = 0;
                objectController.cost = 0;
            }


        }
    }

    //召喚可能かを判定する
    public void Summon(int ObjectId, int Select_x, int Select_y)
    {
        photonView.RPC(nameof(Summon_RPC), RpcTarget.AllViaServer, ObjectId, Select_x, Select_y);
    }




    //[PunRPC]
    //public void LevelUp(int Masu_x, int Masu_y, int Hp_up, int attack_up, int Speed_up)
    //{
    //    PlayerController playerController = MasuObject[Masu_x, Masu_y].GetComponent<PlayerController>();

    //    playerController.hp += Hp_up;
    //    playerController.attack += attack_up;
    //    playerController.speed += Speed_up;
    //}


    //public void LevelUp(int Masu_x, int Masu_y)
    //{
    //    //上昇値決定
    //    int hp_up = Random.Range(hero_attackup[0], hero_attackup[1]);
    //    int attack_up = Random.Range(hero_defenceup[0], hero_defenceup[1]);
    //    int speed_up = Random.Range(hero_speedup[0], hero_speedup[1]);

    //    photonView.RPC(nameof(LevelUp), RpcTarget.AllViaServer, Masu_x, Masu_y, hp_up, attack_up, speed_up);


    //}

    //[PunRPC]
    public void EnemyLevelUp()
    {
        for (int i = 0; i < 3; i++)
        {
            playerStatus.enemy_attack[i] += enemy_attackup[i];
            playerStatus.enemy_defence[i] += enemy_defenceup[i];
            playerStatus.enemy_speed[i] += enemy_speedup[i];

        }
    }


    //public void EnemyLevelUp()
    //{
    //    int[] hp_up = new int[3];
    //    int[] attack_up = new int[3];
    //    int[] speed_up = new int[3];

    //    //上昇値決定
    //    for (int i = 0; i < 3; i++)
    //    {
    //        hp_up[i] = Random.Range(enemy_lower_hpup[i], enemy_upper_hpup[i]);
    //        attack_up[i] = Random.Range(enemy_lower_attackup[i], enemy_upper_attackup[i]);
    //        speed_up[i] = Random.Range(enemy_lower_speedup[i], enemy_upper_speedup[i]);
    //    }


    //    photonView.RPC(nameof(EnemyLevelUp), RpcTarget.AllViaServer, hp_up[0], hp_up[1], hp_up[2], attack_up[0], attack_up[1], attack_up[2], speed_up[0], speed_up[1], speed_up[2]);


    //}


    /// <summary>
    /// 特技
    /// </summary>

    //public void Sihogiri()
    //{
    //    photonView.RPC(nameof(Sihogiri_RPC), RpcTarget.AllViaServer);
    //}

    //[PunRPC]
    //public void Sihogiri_RPC()
    //{
    //    GameObject Hero = MasuObject[hero_x, hero_y];
    //    GameObject damageObject1 = null;
    //    GameObject damageObject2 = null;
    //    GameObject damageObject3 = null;
    //    GameObject damageObject4 = null;

    //    if (0 <= hero_x - 1)
    //        damageObject1 = MasuObject[hero_x - 1, hero_y];

    //    if (hero_x + 1 < yoko)
    //        damageObject2 = MasuObject[hero_x + 1, hero_y];

    //    if (0 <= hero_y - 1)
    //        damageObject3 = MasuObject[hero_x, hero_y - 1];

    //    if (hero_y + 1 < tate)
    //        damageObject4 = MasuObject[hero_x, hero_y + 1];

    //    method.Sihogiri(Hero, damageObject1, damageObject2, damageObject3, damageObject4);
    //}


    //public void Nanamegiri()
    //{
    //    photonView.RPC(nameof(Nanamegiri_RPC), RpcTarget.AllViaServer);
    //}

    //[PunRPC]
    //public void Nanamegiri_RPC()
    //{
    //    //Debug.Log("ななめ斬り");

    //    GameObject Hero = MasuObject[hero_x, hero_y];
    //    GameObject damageObject1 = null;
    //    GameObject damageObject2 = null;
    //    GameObject damageObject3 = null;
    //    GameObject damageObject4 = null;

    //    if (0 <= hero_x - 1 && 0 <= hero_y - 1)
    //        damageObject1 = MasuObject[hero_x - 1, hero_y - 1];

    //    if (0 <= hero_x - 1 && hero_y + 1 < tate)
    //        damageObject2 = MasuObject[hero_x - 1, hero_y + 1];


    //    if (hero_x + 1 < yoko && 0 <= hero_y - 1)
    //        damageObject3 = MasuObject[hero_x + 1, hero_y - 1];


    //    if (hero_x + 1 < yoko && hero_y + 1 < tate)
    //        damageObject4 = MasuObject[hero_x + 1, hero_y + 1];

    //    method.Nanamegiri(Hero, damageObject1, damageObject2, damageObject3, damageObject4);

    //}


    //public void Kaitengiri()
    //{
    //    photonView.RPC(nameof(Kaitengiri_RPC), RpcTarget.AllViaServer);
    //}

    //[PunRPC]
    //public void Kaitengiri_RPC()
    //{
    //    //Debug.Log("回転斬り");

    //    GameObject Hero = MasuObject[hero_x, hero_y];
    //    GameObject damageObject1 = null;
    //    GameObject damageObject2 = null;
    //    GameObject damageObject3 = null;
    //    GameObject damageObject4 = null;

    //    GameObject damageObject5 = null;
    //    GameObject damageObject6 = null;
    //    GameObject damageObject7 = null;
    //    GameObject damageObject8 = null;

    //    //四方

    //    if (0 <= hero_x - 1)
    //        damageObject1 = MasuObject[hero_x - 1, hero_y];

    //    if (hero_x + 1 < yoko)
    //        damageObject2 = MasuObject[hero_x + 1, hero_y];

    //    if (0 <= hero_y - 1)
    //        damageObject3 = MasuObject[hero_x, hero_y - 1];

    //    if (hero_y + 1 < tate)
    //        damageObject4 = MasuObject[hero_x, hero_y + 1];


    //    //斜め

    //    if (0 <= hero_x - 1 && 0 <= hero_y - 1)
    //        damageObject5 = MasuObject[hero_x - 1, hero_y - 1];

    //    if (0 <= hero_x - 1 && hero_y + 1 < tate)
    //        damageObject6 = MasuObject[hero_x - 1, hero_y + 1];


    //    if (hero_x + 1 < yoko && 0 <= hero_y - 1)
    //        damageObject7 = MasuObject[hero_x + 1, hero_y - 1];


    //    if (hero_x + 1 < yoko && hero_y + 1 < tate)
    //        damageObject8 = MasuObject[hero_x + 1, hero_y + 1];

    //    method.Kaitengiri(Hero, damageObject1, damageObject2, damageObject3, damageObject4, damageObject5, damageObject6, damageObject7, damageObject8);

    //}


    //public void Uegiri()
    //{
    //    photonView.RPC(nameof(Uegiri_RPC), RpcTarget.AllViaServer);
    //}

    //[PunRPC]
    //public void Uegiri_RPC()
    //{

    //    GameObject Hero = MasuObject[hero_x, hero_y];
    //    GameObject damageObject1 = null;
    //    GameObject damageObject2 = null;
    //    GameObject damageObject3 = null;

    //    if (0 <= hero_x - 1 && hero_y + 1 < tate)
    //        damageObject1 = MasuObject[hero_x - 1, hero_y + 1];

    //    if (hero_y + 1 < tate)
    //        damageObject2 = MasuObject[hero_x, hero_y + 1];

    //    if (hero_x + 1 < yoko && hero_y + 1 < tate)
    //        damageObject3 = MasuObject[hero_x + 1, hero_y + 1];

    //    method.Uegiri(Hero, damageObject1, damageObject2, damageObject3);

    //}


    //public void Sitagiri()
    //{
    //    photonView.RPC(nameof(Sitagiri_RPC), RpcTarget.AllViaServer);
    //}

    //[PunRPC]
    //public void Sitagiri_RPC()
    //{

    //    GameObject Hero = MasuObject[hero_x, hero_y];
    //    GameObject damageObject1 = null;
    //    GameObject damageObject2 = null;
    //    GameObject damageObject3 = null;

    //    if (0 <= hero_x - 1 && 0 <= hero_y - 1)
    //        damageObject1 = MasuObject[hero_x - 1, hero_y - 1];

    //    if (0 <= hero_y - 1)
    //        damageObject2 = MasuObject[hero_x, hero_y - 1];

    //    if (hero_x + 1 < yoko && 0 <= hero_y - 1)
    //        damageObject3 = MasuObject[hero_x + 1, hero_y - 1];

    //    method.Sitagiri(Hero, damageObject1, damageObject2, damageObject3);

    //}

    //public void Migigiri()
    //{
    //    photonView.RPC(nameof(Migigiri_RPC), RpcTarget.AllViaServer);
    //}

    //[PunRPC]
    //public void Migigiri_RPC()
    //{

    //    GameObject Hero = MasuObject[hero_x, hero_y];
    //    GameObject damageObject1 = null;
    //    GameObject damageObject2 = null;
    //    GameObject damageObject3 = null;

    //    if (hero_x + 1 < yoko && 0 <= hero_y - 1)
    //        damageObject1 = MasuObject[hero_x + 1, hero_y - 1];

    //    if (hero_x + 1 < yoko)
    //        damageObject2 = MasuObject[hero_x + 1, hero_y];

    //    if (hero_x + 1 < yoko && hero_y + 1 < tate)
    //        damageObject3 = MasuObject[hero_x + 1, hero_y + 1];

    //    method.Migigiri(Hero, damageObject1, damageObject2, damageObject3);

    //}


    //public void Hidarigiri()
    //{
    //    photonView.RPC(nameof(Hidarigiri_RPC), RpcTarget.AllViaServer);
    //}

    //[PunRPC]
    //public void Hidarigiri_RPC()
    //{

    //    GameObject Hero = MasuObject[hero_x, hero_y];
    //    GameObject damageObject1 = null;
    //    GameObject damageObject2 = null;
    //    GameObject damageObject3 = null;

    //    if (0 <= hero_x - 1 && 0 <= hero_y - 1)
    //        damageObject1 = MasuObject[hero_x - 1, hero_y - 1];

    //    if (0 <= hero_x - 1)
    //        damageObject2 = MasuObject[hero_x - 1, hero_y];

    //    if (0 <= hero_x - 1 && hero_y + 1 < tate)
    //        damageObject3 = MasuObject[hero_x - 1, hero_y + 1];

    //    method.Hidarigiri(Hero, damageObject1, damageObject2, damageObject3);

    //}


    //ダメージで色を変える
    private IEnumerator DamageColor(int Damage_X, int Damage_Y) //コルーチン関数の名前
    {
        MasuObject[Damage_X, Damage_Y].GetComponent<SpriteRenderer>().color = Color.blue;

        yield return new WaitForSeconds(0.5f);

        MasuObject[Damage_X, Damage_Y].GetComponent<SpriteRenderer>().color = Color.white;


    }

    [PunRPC]
    public void GameStart()
    {
        timeController.GetComponent<TimeController>().gamestart = true;
    }



    //スクリーン座標をワールド座標に変えてz座標を0にする。
    Vector3 ScreenToWorld(Vector3 Pos_screen)
    {
        Vector3 Pos_world = Camera.main.ScreenToWorldPoint(Pos_screen);
        Pos_world = new Vector3(Pos_world.x, Pos_world.y, 0);
        return Pos_world;
    }

    


}
