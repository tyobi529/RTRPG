using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;


public class GameController : MonoBehaviourPunCallbacks
{
    int yoko;
    int tate;
    float width;
    int footer;

    public int up_power_hero;
    public int up_defence_hero;
    public int up_speed_hero;

    public int up_power_enemy;
    public int up_defence_enemy;

    FieldGenerator fieldGenerator;

    int playerId;

    public GameObject[,] MasuObject;

    public GameObject Enemy;

    PlayerStatus playerStatus;

    GameObject Hero;
    HeroController heroController;
    GameObject Maou;
    MaouController maouController;

    int hero_x;
    int hero_y;



    // Start is called before the first frame update
    void Start()
    {
        fieldGenerator = GameObject.Find("FieldGenerator").GetComponent<FieldGenerator>();
        yoko = fieldGenerator.yoko;
        tate = fieldGenerator.tate;
        width = Screen.width / yoko;
        footer = fieldGenerator.footer;

        playerStatus = GameObject.Find("PlayerStatus").GetComponent<PlayerStatus>();

        playerId = PhotonNetwork.LocalPlayer.ActorNumber;

        MasuObject = new GameObject[yoko, tate];

        if (playerId == 2)
        {
            photonView.RPC(nameof(GameStart), RpcTarget.AllViaServer);


        }




        //GameObject a = Instantiate(Hero) as GameObject;

    }


    (int, int) DecideEnemyPos(int Pos_num)
    {
        int x = 0;
        int y = 0;

        do
        {
            switch (Pos_num)
            {
                case 0:
                    x = Random.Range(0, (yoko - 1) / 2);
                    y = Random.Range(0, (tate - 1) / 2);
                    break;
                case 1:
                    x = Random.Range(0, (yoko - 1) / 2);
                    y = Random.Range((tate + 1) / 2, tate);
                    break;
                case 2:
                    x = Random.Range((yoko + 1) / 2, yoko);
                    y = Random.Range(0, (tate - 1) / 2);
                    break;
                case 3:
                    x = Random.Range((yoko + 1) / 2, yoko);
                    y = Random.Range((tate + 1) / 2, tate);
                    break;
                default:
                    Debug.Log("エラー");
                    break;

            }

            if (MasuObject[x, y] == null)
                break;


            if (x == hero_x && y == hero_y)
                break;

        } while (true);


        return (x, y);
    }

    [PunRPC]
    void GenerateEnemy_RPC(int Pos_num, int Masu_x, int Masu_y)
    {
        //Debug.Log("x" + Masu_x);
        //Debug.Log("y" + Masu_y);
        GameObject enemy = Instantiate(Enemy) as GameObject;
        //Debug.Log("bb");
        enemy.GetComponent<EnemyStatus>().maxhp = playerStatus.enemy_maxhp;
        enemy.GetComponent<EnemyStatus>().hp = enemy.GetComponent<EnemyStatus>().maxhp;
        enemy.GetComponent<EnemyStatus>().power = playerStatus.enemy_power;
        enemy.GetComponent<EnemyStatus>().defence = playerStatus.enemy_defence;

        enemy.GetComponent<EnemyStatus>().enemyPos = Pos_num;
        enemy.transform.position = ScreenToWorld(new Vector2(width * Masu_x + width / 2f, footer + width * Masu_y + width / 2f));
        MasuObject[Masu_x, Masu_y] = enemy;
    }




    Vector2 ScreenToWorld(Vector2 Screen_Pos)
    {
        Vector2 World_Pos = Camera.main.ScreenToWorldPoint(Screen_Pos);

        return new Vector2(World_Pos.x, World_Pos.y);
    }

    void GenerateEnemy(int Pos_num)
    {

        int x = 0;
        int y = 0;
        (x, y) = DecideEnemyPos(Pos_num);



        photonView.RPC(nameof(GenerateEnemy_RPC), RpcTarget.AllViaServer, Pos_num, x, y);




    }


    public void LevelUp()
    {

        heroController.power += up_power_hero;
        heroController.defence += up_defence_hero;
        heroController.speed += up_speed_hero;
    }


    public void Enemy_LevelUp()
    {
        playerStatus.enemy_power += up_power_enemy;
        playerStatus.enemy_defence += up_defence_enemy;
    }

    [PunRPC]
    public void Attack_RPC(int Masu_x, int Masu_y)
    {
        EnemyStatus enemyStatus = MasuObject[Masu_x, Masu_y].GetComponent<EnemyStatus>();
        //ダメージ
        int enemy_damage = heroController.power - enemyStatus.defence;
        int hero_damage = enemyStatus.power - heroController.defence;

        enemyStatus.hp -= enemy_damage;
        heroController.hp -= hero_damage;

        if (enemyStatus.hp <= 0)
        {
            int enemyPos = MasuObject[Masu_x, Masu_y].GetComponent<EnemyStatus>().enemyPos;

            //Destroy(MasuObject[Masu_x, Masu_y]);
            MasuObject[Masu_x, Masu_y].GetComponent<EnemyStatus>().Destroy();
            //レベルアップ
            LevelUp();

            Enemy_LevelUp();

            //次の敵生成
            if (playerId == 2)
                GenerateEnemy(enemyPos);

            
        }
    }



    public void Attack(int Direct_x, int Direct_y)
    {
        if (MasuObject[hero_x + Direct_x, hero_y + Direct_y] == null)
            return;


        photonView.RPC(nameof(Attack_RPC), RpcTarget.AllViaServer, hero_x + Direct_x, hero_y + Direct_y);

        StartCoroutine(ChangeSpriteColor());

    }


    [PunRPC]
    void Damage_Enemy(GameObject Object, int Damage)
    {
        Debug.Log(Damage + "ダメージ");
        Object.GetComponent<EnemyStatus>().hp -= Damage;
    }

    [PunRPC]
    void Damage_Hero(GameObject Object, int Damage)
    {
        Debug.Log(Damage + "ダメージ");
        Object.GetComponent<HeroController>().hp -= Damage;
    }


    //レベルアップ時
    IEnumerator ChangeSpriteColor()
    {
        Hero.GetComponent<SpriteRenderer>().color = Color.blue;

        //1秒停止
        yield return new WaitForSeconds(1);

        //元の色にする
        Hero.GetComponent<SpriteRenderer>().color = Color.white;
    }


    [PunRPC]
    void Move_RPC(int Direct_x, int Direct_y)
    {
        hero_x += Direct_x;
        hero_y += Direct_y;

        heroController.Move(Direct_x, Direct_y);
    }

    [PunRPC]
    void ChangeDirection_RPC(int Direct_x, int Direct_y)
    {
        //アニメーションの変更
        heroController.ChangeAnimation(Direct_x, Direct_y);

    }

    public void Check_Masu(int Direct_x, int Direct_y)
    {
        //向きの変更
        photonView.RPC(nameof(ChangeDirection_RPC), RpcTarget.AllViaServer, Direct_x, Direct_y);



        //何もなければ移動する
        if (MasuObject[hero_x + Direct_x, hero_y + Direct_y] == null)
            photonView.RPC(nameof(Move_RPC), RpcTarget.AllViaServer, Direct_x, Direct_y);
        else
        {
            //Debug.Log("aa");
            //heroController.canmove = true;
            //Debug.Log("canmove" + heroController.canmove);
        }

    }



    /// <summary>
    /// 魔王の攻撃
    /// </summary>

    [PunRPC]
    public void MaouAttack_RPC(int Masu_x, int Masu_y)
    {
        maouController.Attack(Masu_x, Masu_y);
    }

    [PunRPC]
    public void Damage_RPC()
    {
        maouController.Damage();
    }



    public void Damage(int Masu_x, int Masu_y)
    {
        if (hero_x == Masu_x && hero_y == Masu_y)
            photonView.RPC(nameof(Damage_RPC), RpcTarget.AllViaServer);


    }


    public void MaouAttack(int Masu_x, int Masu_y)
    {
        photonView.RPC(nameof(MaouAttack_RPC), RpcTarget.AllViaServer, Masu_x, Masu_y);

    }


    [PunRPC]
    private void Heal_RPC()
    {
        heroController.hp = heroController.maxhp;
        
    }

    public void Heal()
    {
        photonView.RPC(nameof(Heal_RPC), RpcTarget.AllViaServer);
        heroController.canheal = false;
        heroController.healTime = 10f;

    }


    //ゲーム開始
    [PunRPC]
    void GameStart()
    {
        //Hero = GameObject.Find("Hero(Clone)");
        Hero = GameObject.Find("Hero");
        heroController = Hero.GetComponent<HeroController>();
        heroController.playerId = playerId;
        heroController.GameStart();


        Maou = GameObject.Find("Maou");
        maouController = Maou.GetComponent<MaouController>();
        maouController.playerId = playerId;
        maouController.GameStart();


        if (playerId == 2)
        {
            for (int i = 0; i < 4; i++)
            {
                GenerateEnemy(i);
            }

        }


        hero_x = heroController.masu_x;
        hero_y = heroController.masu_y;
    }



}
