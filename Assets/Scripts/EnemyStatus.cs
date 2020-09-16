using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyStatus : MonoBehaviour
{
    public int maxhp;
    public int hp;
    public int power;
    public int defence;

    public int enemyPos;

    GameObject Canvas;
    public GameObject HPSlider;

    private void Start()
    {
        Canvas = GameObject.Find("Canvas");

        HPSlider = Instantiate(HPSlider) as GameObject;
        HPSlider.transform.SetParent(Canvas.transform);

        Vector2 Pos = Camera.main.WorldToScreenPoint(transform.position);
        Pos = new Vector2(Pos.x, Pos.y + 50f);

        HPSlider.transform.position = Pos;
    }


    private void Update()
    {


        HPSlider.GetComponent<Slider>().value = (float)(hp) / (float)(maxhp);
    }


    public void Destroy()
    {
        Destroy(HPSlider);
        Destroy(this.gameObject);
    }
}
