using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldGenerator : MonoBehaviour
{


    //横の数
    public int yoko = 7;
    public int tate = 7;
    private float width;


    //下の部分の高さ
    public int footer = 300;

    public GameObject MasuPrefab;

    public Sprite NormalImage;
    public Sprite SelectImage;
    public Sprite AttackImage;

    public Sprite PosImage;








    public GameObject[,] Masu;




    // Start is called before the first frame update
    void Start()
    {
        width = Screen.width / yoko;
        Masu = new GameObject[yoko, tate];

        for (int i = 0; i < yoko; i++)
        {
            for (int j = 0; j < tate; j++)
            {

                Masu[i, j] = Instantiate(MasuPrefab) as GameObject;



                Masu[i, j].transform.parent = this.transform;
                Masu[i, j].transform.position = ScreenToWorld(new Vector3(width / 2 + width * i, footer + width / 2 + width * j, 0));
            }
        }


    }


    public void ChangeField(int Kind, int Masu_x, int Masu_y)
    {
        //Destroy(Masu[Masu_x, Masu_y]);

        //Debug.Log(Masu_x);
        //Debug.Log(Masu_y);

        switch (Kind)
        {

            case 0:
                Masu[Masu_x, Masu_y].GetComponent<SpriteRenderer>().sprite = NormalImage;
                break;
            case 1:
                Masu[Masu_x, Masu_y].GetComponent<SpriteRenderer>().sprite = SelectImage;
                break;
            case 2:
                Masu[Masu_x, Masu_y].GetComponent<SpriteRenderer>().sprite = AttackImage;
                break;
            case 3:
                Masu[Masu_x, Masu_y].GetComponent<SpriteRenderer>().sprite = PosImage;
                break;
            default:
                Debug.Log("エラー");
                break;
                    
        }

       
    }


    //スクリーン座標をワールド座標に変えてz座標を0にする。
    Vector3 ScreenToWorld(Vector3 Pos_screen)
    {
        Vector3 Pos_world = Camera.main.ScreenToWorldPoint(Pos_screen);
        Pos_world = new Vector3(Pos_world.x, Pos_world.y, 0);
        return Pos_world;
    }
}
