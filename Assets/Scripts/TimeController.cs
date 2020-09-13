using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimeController : MonoBehaviour
{
    public bool gamestart = false;
    public GameObject Limiter;

    public float timeLimit = 180f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (gamestart)
            Limiter.GetComponent<Slider>().value += Time.deltaTime / 180f;
    }


}
