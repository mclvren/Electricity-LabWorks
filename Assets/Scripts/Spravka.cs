using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Spravka : MonoBehaviour
{
    public GameObject[] Canvas;

    //void switchCanvas2_2(int n)
    //{
    //    switch (n)
    //    {
    //        case 0:
    //            Canvas[1].SetActive(false);
    //            Canvas[2].SetActive(false);
    //            Canvas[0].SetActive(true);
    //            break;
    //        case 1:
    //            Canvas[0].SetActive(false);
    //            Canvas[1].SetActive(true);
    //            break;
    //        case 2:
    //            Canvas[0].SetActive(false);
    //            Canvas[2].SetActive(true);
    //            break;
    //        default:
    //            UnityEngine.Debug.Log("Ошибка при выборе схемы");
    //            break;
    //    }
    //}

    public void switch_to_Spravka_Canvas()
    {
        Canvas[0].SetActive(false);
        Canvas[1].SetActive(true);
    }

    public void switchCanvasback()
    {
        Canvas[1].SetActive(false);
        Canvas[0].SetActive(true);
    }

    public void switch_to_Spravka_Canvas_211_22()
    {
        Canvas[0].SetActive(false);
        Canvas[1].SetActive(false);
        Canvas[2].SetActive(true);
    }

    public void switchCanvasback_211_22()
    {
        Canvas[2].SetActive(false);
        Canvas[1].SetActive(true);
        Canvas[0].SetActive(true);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
