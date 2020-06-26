using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LR211 : MonoBehaviour
{
    [SerializeField] GameObject Menu;
    [SerializeField] GameObject Init;
    [SerializeField] GameObject Led;

    //Переменные
    private bool isOn = false;
    //Напряжения источника, В
    private double source = 42;
    //Частота источника
    private double freq = 50;
    //Номинал резистора, Ом
    private double R = 710;
    //Номиналы конденсатором, мкФ
    private double[] cap = new double[] { 1, 6, 11 };
    //Катушка индуктивности, мГн
    private double L = 30;
    //Объекты сцены
    public Dropdown xVal;
    public Dropdown xCap;
    public Text currentS;
    public Text VoltageIn;
    public Text VoltageOut;
    //Переменные для расчетов
    private double C;
    private double w;
    private double Im;
    private double I;
    private double Um;
    private double Ur;
    private double Uc;
    private double Ul;
    private double Ulc;
    private double Z;

    //Возврат в меню
    void Back()
    {
        SceneManager.LoadScene("Menu", LoadSceneMode.Single);
    }
    //Обработчик кнопки включить источник
    void Enable()
    {
        if (isOn == false)
        {
            isOn = true;
            Init.GetComponentInChildren<Text>().text = "Выключить";
            VoltageIn.text = source.ToString();
            Um = source;
            w = 2 * Math.PI * freq;
            getZ();
            changeCurrentS();
            Led.GetComponent<Image>().color = new Color32(0, 255, 0, 100);
        }
        else if (isOn == true)
        {
            isOn = false;
            VoltageIn.text = "0";
            VoltageOut.text = "0";
            currentS.text = "0";
            Init.GetComponentInChildren<Text>().text = "Включить";
            Led.GetComponent<Image>().color = new Color32(255, 255, 255, 100);
        }
    }

    void getZ()
    {
        //Преобразуем в СИ
        if (C >=1) C = C * Math.Pow(10, -6);
        Z = Math.Sqrt(Math.Pow(R, 2) + Math.Pow((w * L - (1 / (w * C))), 2));
        UnityEngine.Debug.Log("Z="+Z+" Ом");
    }

    void changeCurrentS()
    {
        //Амплитудное значение
        Im = Um / Z;
        //Действующее
        I = Im;
        UnityEngine.Debug.Log("Im=" + Im + " А");
        Ur = I * R;
        UnityEngine.Debug.Log("Ur=" + Ur + " В");
        Uc = I / (w * C);
        UnityEngine.Debug.Log("Uc=" + Uc + " В");
        Ul = I * w * L;
        UnityEngine.Debug.Log("Ul=" + Ul + " В");
        Ulc = Uc-Ul;
        UnityEngine.Debug.Log("Ul=" + Ul + " В");
        if (isOn == true)
            currentS.text = Math.Round(Im*1000, 3).ToString();
        mes(xVal);
    }

    //Функция выбора емкости конденсатора
    void xC(Dropdown DSX)
    {
        var val = DSX.value;
        C = cap[val];
        UnityEngine.Debug.Log("C="+C+" мкФ");
        getZ();
        changeCurrentS();
    }

    //Функция выбора объекта измерения
    void mes(Dropdown DSX)
    {
        var val = DSX.value;
        switch (val)
        {
            case 0:
                if (isOn == true)
                    VoltageOut.text = Math.Round(Uc, 3).ToString();
                break;
            case 1:
                if (isOn == true)
                    VoltageOut.text = Math.Round(Ul, 3).ToString();
                break;
            case 2:
                if (isOn == true)
                    VoltageOut.text = Math.Round(Ur, 3).ToString();
                break;
            case 3:
                if (isOn == true)
                    VoltageOut.text = Math.Round(Ulc, 3).ToString();
                break;
            default:
                UnityEngine.Debug.Log("Ошибка при выборе объекта измерений");
                break;
        }
    }

    //Перед первым кадром
    async void Start()
    {
        //События для кнопок
        Menu.GetComponent<Button>().onClick.AddListener(Back);
        Init.GetComponent<Button>().onClick.AddListener(Enable);
        //Стандартное значение
        C = cap[0];
        //События для дропдаун меню
        xVal.onValueChanged.AddListener(delegate
        {
            mes(xVal);
        });
        xCap.onValueChanged.AddListener(delegate
        {
            xC(xCap);
        });
        //Преобразуем в СИ
        L = L * Math.Pow(10, -3);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
