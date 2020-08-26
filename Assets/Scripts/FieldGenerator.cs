using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldGenerator : MonoBehaviour
{
    public GameObject MasuPrefab;

    public GameObject Bed;


    public GameObject[,] Masu = new GameObject[5, 7];


    private int width = 150;
    private int height = 150;

    // Start is called before the first frame update
    void Start()
    {
        //GameObject player1 = Instantiate(PlayerPrefab) as GameObject;
        //GameObject player2 = Instantiate(PlayerPrefab) as GameObject;

        //Debug.Log()
        for (int i = 0; i < 5; i++)
        {
            for (int j = 0; j < 7; j++)
            {
                Masu[i, j] = Instantiate(MasuPrefab) as GameObject;
                Masu[i, j].transform.parent = this.transform;
                //Masu[i, j].transform.position = Camera.main.ScreenToWorldPoint(new Vector3(75 + i * 150, 284 + j * 150, 0));
                Masu[i, j].transform.position = Camera.main.ScreenToWorldPoint(new Vector3(width / 2 + width * i, 284 + height / 2 + height * j, 0));

                Masu[i, j].transform.position = new Vector3(Masu[i, j].transform.position.x, Masu[i, j].transform.position.y, 0);
            }
        }
    }


    public void ChangeField(int Kind, int Masu_x, int Masu_y)
    {
        Destroy(Masu[Masu_x, Masu_y]);

        switch (Kind)
        {
            case 0:
                Masu[Masu_x, Masu_y] = Instantiate(MasuPrefab) as GameObject;
                break;
            case 1:
                Masu[Masu_x, Masu_y] = Instantiate(Bed) as GameObject;
                break;
            default:
                Debug.Log("エラー");
                break;
                    
        }

        Masu[Masu_x, Masu_y].transform.parent = this.transform;
        Masu[Masu_x, Masu_y].transform.position = Camera.main.ScreenToWorldPoint(new Vector3(width / 2 + width * Masu_x, 284 + height / 2 + height * Masu_y, 0));

        //Masu[Masu_x, Masu_y].transform.position = Camera.main.ScreenToWorldPoint(new Vector3(75 + Masu_x * 150, 284 + Masu_y * 150, 0));
        Masu[Masu_x, Masu_y].transform.position = new Vector3(Masu[Masu_x, Masu_y].transform.position.x, Masu[Masu_x, Masu_y].transform.position.y, 0);
    }

}
