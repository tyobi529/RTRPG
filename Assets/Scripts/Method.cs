using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Method : MonoBehaviour
{


    public void Attack(GameObject AttackObject, GameObject DamageObject)
    {
        int damage = AttackObject.GetComponent<PlayerController>().attack - DamageObject.GetComponent<PlayerController>().defence;

        if (damage < 0)
            damage = 1;

        DamageObject.GetComponent<PlayerController>().hp -= damage;

        if (DamageObject.GetComponent<PlayerController>().hp <= 0)
        {
            //Timerなどの削除
            DamageObject.GetComponent<PlayerController>().Destroy();
            //Destroy(DamageObject);
        }

        else
            StartCoroutine(DamageColor(DamageObject));
    }


    //ダメージで色を変える
    private IEnumerator DamageColor(GameObject Object) //コルーチン関数の名前
    {
        Object.GetComponent<SpriteRenderer>().color = Color.blue;

        yield return new WaitForSeconds(0.5f);

        Object.GetComponent<SpriteRenderer>().color = Color.white;


    }
}
