using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldGenerator : MonoBehaviour
{


    //横の数
    public int yoko = 6;
    public int tate = 7;
    private float width;


    //下の部分の高さ
    public int footer = 284;

    public GameObject MasuPrefab;

    public GameObject Maou_Masu;


    //public GameObject Bed;

    //public GameObject Takara;

    //public GameObject Tokugi;

    public Sprite NormalImgae;
    public Sprite YushaImgae;

    public Sprite BedImgae;

    public Sprite HPImgae;
    public Sprite PowerImgae;
    public Sprite DefenceImgae;
    public Sprite SpeedImgae;

    public Sprite TakaraImgae;
    public Sprite TokugiImgae;
    public Sprite EXPImgae;





    public GameObject[,] Masu;



    //private int width = 150;

    // Start is called before the first frame update
    void Start()
    {
        width = Screen.width / yoko;
        Masu = new GameObject[yoko, tate];


        for (int i = 0; i < yoko; i++)
        {
            for (int j = 0; j < tate; j++)
            {
                if (j >= 5)
                    Masu[i, j] = Instantiate(Maou_Masu) as GameObject;
                else
                    Masu[i, j] = Instantiate(MasuPrefab) as GameObject;

                //if (i == 2 && j == 0)
                //    Masu[i, j].GetComponent<SpriteRenderer>().sprite = YushaImgae;


                Masu[i, j].transform.parent = this.transform;
                Masu[i, j].transform.position = ScreenToWorld(new Vector3(width / 2 + width * i, footer + width / 2 + width * j, 0));
            }
        }


    }


    public void ChangeField(int Kind, int Masu_x, int Masu_y)
    {
        //Destroy(Masu[Masu_x, Masu_y]);

        switch (Kind)
        {
            case 0:
                Masu[Masu_x, Masu_y].GetComponent<SpriteRenderer>().sprite = NormalImgae;
                break;
            case 1:
                Masu[Masu_x, Masu_y].GetComponent<SpriteRenderer>().sprite = BedImgae;
                break;
            case 2:
                Masu[Masu_x, Masu_y].GetComponent<SpriteRenderer>().sprite = PowerImgae;
                break;
            case 3:
                Masu[Masu_x, Masu_y].GetComponent<SpriteRenderer>().sprite = DefenceImgae;
                break;
            case 4:
                Masu[Masu_x, Masu_y].GetComponent<SpriteRenderer>().sprite = SpeedImgae;
                break;
            case 5:
                Masu[Masu_x, Masu_y].GetComponent<SpriteRenderer>().sprite = TakaraImgae;
                break;
            case 6:
                Masu[Masu_x, Masu_y].GetComponent<SpriteRenderer>().sprite = TokugiImgae;
                break;
            default:
                Debug.Log("エラー");
                break;
                    
        }

        //Masu[Masu_x, Masu_y].transform.parent = this.transform;
        //Masu[Masu_x, Masu_y].transform.position = ScreenToWorld(new Vector3(width / 2 + width * Masu_x, footer + width / 2 + width * Masu_y, 0));

        //Masu[Masu_x, Masu_y].transform.position = Camera.main.ScreenToWorldPoint(new Vector3(75 + Masu_x * 150, 284 + Masu_y * 150, 0));
        //Masu[Masu_x, Masu_y].transform.position = new Vector3(Masu[Masu_x, Masu_y].transform.position.x, Masu[Masu_x, Masu_y].transform.position.y, 0);
    }


    //スクリーン座標をワールド座標に変えてz座標を0にする。
    Vector3 ScreenToWorld(Vector3 Pos_screen)
    {
        Vector3 Pos_world = Camera.main.ScreenToWorldPoint(Pos_screen);
        Pos_world = new Vector3(Pos_world.x, Pos_world.y, 0);
        return Pos_world;
    }
}
