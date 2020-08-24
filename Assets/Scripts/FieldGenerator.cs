using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldGenerator : MonoBehaviour
{
    public GameObject MasuPrefab;

    public GameObject PlayerPrefab;

    //public GameObject Field;

    // Start is called before the first frame update
    void Start()
    {
        //GameObject player1 = Instantiate(PlayerPrefab) as GameObject;
        //GameObject player2 = Instantiate(PlayerPrefab) as GameObject;

        //Debug.Log()
        for (int i = 0; i < 7; i++)
        {
            for (int j = 0; j < 5; j++)
            {
                GameObject masu = Instantiate(MasuPrefab) as GameObject;
                masu.transform.parent = this.transform;
                masu.transform.position = Camera.main.ScreenToWorldPoint(new Vector3(75 + j * 150, 1259 - i * 150, 0));
                masu.transform.position = new Vector3(masu.transform.position.x, masu.transform.position.y, 0);
            }
        }
    }


}
