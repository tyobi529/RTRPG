using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaouController : MonoBehaviour
{
    FieldGenerator fieldGenerator;

    int yoko;
    int tate;
    float width;
    int footer;

    GameObject Hero;
    HeroController heroController;


    GameController gameController;
    bool gameStart = false;

    public int playerId;

    // Start is called before the first frame update
    void Start()
    {
        fieldGenerator = GameObject.Find("FieldGenerator").GetComponent<FieldGenerator>();
        yoko = fieldGenerator.yoko;
        tate = fieldGenerator.tate;
        width = Screen.width / yoko;
        footer = fieldGenerator.footer;

        Hero = GameObject.Find("Hero");
        heroController = Hero.GetComponent<HeroController>();


    }

    // Update is called once per frame
    void Update()
    {
        if (playerId == 2)
        {
            //Hinotama();

            Meteo();

            if (Input.GetKeyDown(KeyCode.A))
                StartCoroutine(Nami());

            if (Input.GetKeyDown(KeyCode.B))
                StartCoroutine(Maguma());
        }

    }





    //[PunRPC]
    void ChangeField(int Kind, int Masu_x, int Masu_y)
    {
        fieldGenerator.ChangeField(Kind, Masu_x, Masu_y);
    }



    //[PunRPC]
    public void Damage()
    {
        heroController.hp -= 30;
        StartCoroutine(DamageColor());
        Debug.Log(30 + "ダメージ");
    }


    public void Attack(int Masu_x, int Masu_y)
    {
        StartCoroutine(BlinkMasu(Masu_x, Masu_y));
    }

    void Hinotama()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePos = Input.mousePosition;
            int x = (int)(mousePos.x / width);
            int y = (int)((mousePos.y - footer) / width);

            gameController.MaouAttack(x, y);
        }
    }


    void Meteo()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePos = Input.mousePosition;
            int x = (int)(mousePos.x / width);
            int y = (int)((mousePos.y - footer) / width);

            gameController.MaouAttack(x, y);

            if (0 <= x - 1)
                gameController.MaouAttack(x - 1, y);

            if (0 <= y - 1)
                gameController.MaouAttack(x, y - 1);

            if (x + 1 < yoko)
                gameController.MaouAttack(x + 1, y);

            if (y + 1 < tate)
                gameController.MaouAttack(x, y + 1);



        }
    }


    IEnumerator Nami()
    {
        for (int i = 0; i < yoko; i++)
        {
            for (int j = 0; j < tate; j++)
                gameController.MaouAttack(i, j);

            yield return new WaitForSeconds(1);

        }

    }

    IEnumerator Maguma()
    {
        for (int i = 0; i < tate; i++)
        {
            for (int j = 0; j < yoko; j++)
                gameController.MaouAttack(j, i);

            yield return new WaitForSeconds(1);

        }

    }


    IEnumerator BlinkMasu(int Masu_x, int Masu_y)
    {
        //黄色にする
        fieldGenerator.ChangeField(1, Masu_x, Masu_y);

        //1秒停止
        yield return new WaitForSeconds(1);

        //赤色にする
        fieldGenerator.ChangeField(2, Masu_x, Masu_y);

        //ダメージ
        if (playerId == 2)
            gameController.Damage(Masu_x, Masu_y);


        yield return new WaitForSeconds(1);

        //戻す
        fieldGenerator.ChangeField(0, Masu_x, Masu_y);



    }






    //ダメージを受けると色を変える。
    IEnumerator DamageColor()
    {
        Hero.GetComponent<SpriteRenderer>().color = Color.red;

        yield return new WaitForSeconds(0.2f);

        Hero.GetComponent<SpriteRenderer>().color = Color.white;



    }


    public void GameStart()
    {
        gameController = GameObject.Find("GameController(Clone)").GetComponent<GameController>();

        gameStart = true;



    }
}
