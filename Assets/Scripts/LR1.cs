using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using System.Linq;

public class LR1 : MonoBehaviour
{
 //Присваиваем объекты к переменным
    //Кнопки
    [SerializeField] GameObject Init;
    [SerializeField] GameObject DropShift;
    [SerializeField] GameObject Menu;
    [SerializeField] GameObject Plus;
    [SerializeField] GameObject Plus10;
    [SerializeField] GameObject Minus;
    [SerializeField] GameObject Minus10;
    [SerializeField] GameObject Zero;
    [SerializeField] GameObject Polar;
    //Стрелка
    [SerializeField] GameObject Arrow;
    //Текст
    [SerializeField] GameObject Deg;
    [SerializeField] GameObject Current;
    //Затемнение компаса
    [SerializeField] GameObject Blackout;
    

    Coroutine coroutine;
    private bool isOn = false;
    private float mA = 0;
    private Text cur;
    bool goRotatePlr = false;
    private float currentAngle = 0f;
    private float currentValue;
    private bool inverseState = false;
    private Dropdown DS;

    void Start()
    {
        //События для кнопок
        Menu.GetComponent<Button>().onClick.AddListener(Back);
        Init.GetComponent<Button>().onClick.AddListener(Enable);
        Plus.GetComponent<Button>().onClick.AddListener(PlusX);
        Plus10.GetComponent<Button>().onClick.AddListener(Plus10X);
        Minus.GetComponent<Button>().onClick.AddListener(MinusX);
        Minus10.GetComponent<Button>().onClick.AddListener(Minus10X);
        Zero.GetComponent<Button>().onClick.AddListener(ZeroX);
        Polar.GetComponent<Button>().onClick.AddListener(Inverse);
        DS = DropShift.GetComponent<Dropdown>();
        DS.onValueChanged.AddListener(delegate
        {
            Shifter(DS);
        });
        //Кол-во градусов
        Deg.GetComponent<Text>().text = "0°";
        cur = Current.GetComponent<Text>();
        //Установка нуля
        rttclockwise(0);
        DS.interactable = false;
    }
    //Аппроксимация функции. Угол по току
    //По данным с https://planetcalc.ru/5992/ % ошибки меньше всего у кубической регрессии
    //Нахождения кубической регрессии с помощью http://mathhelpplanet.com/static.php?p=onlayn-mnk-i-regressionniy-analiz

    /* Исходные данные:
     *   mA 85	130	210	380 455
     * Угол 20	30	40	50  60
    */
    // Средняя ошибка аппроксимации (погрешность) = 0.00789056316568377374 %
    float GetA(float i)
    {
        //Коэффициенты кубической регрессии, найденные в http://mathhelpplanet.com/static.php?p=onlayn-mnk-i-regressionniy-analiz
        double a, b, c, d;
        a = 0.00000174794000391636;
        b = -0.00152270563400624948;
        c = 0.48827583735541679744;
        d = -11.57761239131832553539;
        //Формула кубической регрессии
        float A = (float)(a * Math.Pow(i, 3) + b * Math.Pow(i, 2) + c * i + d);
        return A;
    }
    //Средняя ошибка аппроксимации = 0.00000000654452368306 %
    float GetB(float i)
    {
        //Коэффициенты кубической регрессии, найденные в http://mathhelpplanet.com/static.php?p=onlayn-mnk-i-regressionniy-analiz
        double a, b, c, d;
        a = 0.00000064251689367231;
        b = -0.00085084745762031844;
        c = 0.38255389388416460861;
        d = -5.76429378412285586819;
        //Формула кубической регрессии
        float A = (float)(a * Math.Pow(i, 3) + b * Math.Pow(i, 2) + c * i + d);
        return A;
    }
    //Прибавление тока
    void PlusX()
    {
        mA += 15;
        currentValue = mA;
        cur.text = mA.ToString();
        StopAllCoroutines();
        if (isOn) rttclockwise(GetA(mA));
    }

    void Plus10X()
    {
        mA += 10;
        currentValue = mA;
        cur.text = mA.ToString();
        StopAllCoroutines();
        if (isOn) rttclockwise(GetA(mA));
    }
    //Убавление
    void MinusX()
    {
        if (currentValue > 0)
        {
            mA -= 15;
            currentValue = mA;
            cur.text = mA.ToString();
            StopAllCoroutines();
            if (currentValue < 0)
            {
                ZeroX();
            }
            else
            {
                if (isOn) rttcounterclockwise(GetA(mA));
            }
        }
    }
    void Minus10X()
    {
        if (currentValue > 0)
        {
            mA -= 10;
            currentValue = mA;
            cur.text = mA.ToString();
            StopAllCoroutines();
            if (currentValue < 0)
            {
                ZeroX();
            }
            else
            {
                if (isOn) rttcounterclockwise(GetA(mA));
            }
        }
    }
    //Задать 0 мА
    void ZeroX()
    {
        mA = 0;
        StopAllCoroutines();
        if (mA > currentValue)
        {
            currentValue = mA;
            cur.text = mA.ToString();
            if (isOn) rttclockwise(0); 
        }
        else
        {
            currentValue = mA;
            cur.text = mA.ToString();
            if (isOn) rttcounterclockwise(0);
        }
    }
    //Смена полярности
    void Inverse()
    {
        float k;
        mA *= -1;
        inverseState = !inverseState;
        StopAllCoroutines();
        if (inverseState) k = GetB(Math.Abs(mA));
        else k = GetA(Math.Abs(mA));
        if (mA > currentValue)
        {
            currentValue = mA;
            cur.text = mA.ToString();
            if (isOn) rttclockwise(k);
        }
        else if (mA < currentValue)
        {
            k *= -1;
            currentValue = mA;
            cur.text = mA.ToString();
            if (isOn) rttcounterclockwise(k);
        }
    }
    void ShifterH(float tAngle)
    {
        if (tAngle > currentAngle)
        {
            if (isOn) rttclockwise(tAngle);
        }
        else if (tAngle < currentAngle)
        {
            if (isOn) rttcounterclockwise(tAngle);
        }
        
    }
    //Смещение
    private float bAngle;
    private bool Shifter_Check_If_Changed = false;
    void Shifter(Dropdown DSX)
    {
        var val = DSX.value;
        float tAngle;
        if ((bAngle != null)&&(bAngle != 0f)) tAngle = bAngle;
        else tAngle = currentAngle;
        UnityEngine.Debug.Log("currentAngle=" + currentAngle);
        if (!Shifter_Check_If_Changed) bAngle = currentAngle;
        //
        switch (val)
        {
            case 0:
                tAngle = bAngle;
                break;
            case 1:
                tAngle += 2f;
                break;
            case 2:
                tAngle -= 1f;
                break;
            case 3:
                tAngle -= 3f;
                break;
            case 4:
                tAngle -= 8f;
                break;
            case 5:
                tAngle -= 16f;
                break;
            default:
                UnityEngine.Debug.Log("Смещение задано не верно");
                break;
        }
        ShifterH(tAngle);
        UnityEngine.Debug.Log("tAngle="+tAngle);
        Shifter_Check_If_Changed = true;
        UnityEngine.Debug.Log("bAngle="+bAngle);
    }
    //Функции после запуска (покадровые)
    void Update()
    {
        if (isOn&&(mA != 0))
        {
            toggleBlackout(true);
            DS.interactable = true;
        }
        else
        {
            toggleBlackout(false);
            DS.interactable = false;
        }
        //Временное решение, чтобы нужно было перезапустить после выбранного смещения для изменения тока
        if (Shifter_Check_If_Changed)
        {
            Plus.GetComponent<Button>().interactable = false;
            Plus10.GetComponent<Button>().interactable = false;
            Minus.GetComponent<Button>().interactable = false;
            Minus10.GetComponent<Button>().interactable = false;
            Zero.GetComponent<Button>().interactable = false;
            Polar.GetComponent<Button>().interactable = false;
        }
        else
        {
            Plus.GetComponent<Button>().interactable = true;
            Plus10.GetComponent<Button>().interactable = true;
            Minus.GetComponent<Button>().interactable = true;
            Minus10.GetComponent<Button>().interactable = true;
            Zero.GetComponent<Button>().interactable = true;
            Polar.GetComponent<Button>().interactable = true;
        }
        }
    //Подпрограмма поворота стрелки
    IEnumerator RotateMeNow(float targetAngle, float rotationSpeed)
    {
        while (true)
        {
            float step = rotationSpeed * Time.deltaTime;
            if (currentAngle + step >= targetAngle)
            {
                // Докручиваем до нашего угла
                step = targetAngle - currentAngle;
                currentAngle += step;
                Arrow.transform.Rotate(Vector3.back, step);
                //UnityEngine.Debug.Log("Текущий угол: " + currentAngle);
                break;
            }
            else if (currentAngle!=targetAngle)
            {
                currentAngle += step;
                Arrow.transform.Rotate(Vector3.back, step);
                //UnityEngine.Debug.Log("Текущий угол: " + currentAngle);
            }
            yield return null;
        }
    }
    //Подпрограмма поворота стрелки в обратную сторону
    IEnumerator RotateMeNowCC(float targetAngle, float rotationSpeed)
    {
        while (true)
        {
            float step = rotationSpeed * Time.deltaTime;
            if (currentAngle + step <= targetAngle)
            {
                // Докручиваем до нашего угла
                step = targetAngle - currentAngle;
                currentAngle += step;
                Arrow.transform.Rotate(Vector3.back, step);
                //UnityEngine.Debug.Log("Текущий угол: " + currentAngle);
                break;
            }
            else if (currentAngle != targetAngle)
            {
                currentAngle += step;
                Arrow.transform.Rotate(Vector3.back, step);
                //UnityEngine.Debug.Log("Текущий угол: " + currentAngle);
            }
            yield return null;
        }
    }

    //Инициализация поворота
    void rttclockwise(float ang)
    {
        var targetAngle = ang;
        float rotationSpeed = 50f; // Скорость поворота
        StartCoroutine(RotateMeNow(targetAngle, rotationSpeed));
        Deg.GetComponent<Text>().text = ((Math.Round(ang, 3)).ToString()+"°");
    }
    //Инициализация поворота в обратную сторону
    void rttcounterclockwise(float ang)
    {
        var targetAngle = ang;
        float rotationSpeed = -50f; // Скорость поворота
        StartCoroutine(RotateMeNowCC(targetAngle, rotationSpeed));
        Deg.GetComponent<Text>().text = ((Math.Round(ang, 3)).ToString() + "°");
    }
    //Функция для кнопки "Вкл"
    void Enable()
    {
        if (isOn == false)
        {
            isOn = true;
            Init.GetComponentInChildren<Text>().text = "Выключить";
            StopAllCoroutines();
            if (mA != 0)
            {
                rttclockwise(mA * 0.195f);
            }
        }
        else if (isOn == true)
        {
            isOn = false;
            Init.GetComponentInChildren<Text>().text = "Включить";
            mA = 0;
            StopAllCoroutines();
            if (mA > currentValue)
            {
                currentValue = mA;
                cur.text = mA.ToString();
                rttclockwise(0); // тест
            }
            else
            {
                currentValue = mA;
                cur.text = mA.ToString();
                rttcounterclockwise(0);
            }
            //Временное решение, чтобы нужно было перезапустить после выбранного смещения для изменения тока
            bAngle = 0f;
            DS.value = 0;
            Shifter_Check_If_Changed = false;
        }
    }
    //Функция затемнения компаса
    void toggleBlackout(bool state) {
        if (state)
        {
            Blackout.transform.gameObject.SetActive(true);
            Deg.transform.gameObject.SetActive(true);
        }
        else
        {
            Blackout.transform.gameObject.SetActive(false);
            Deg.transform.gameObject.SetActive(false);
        }
    }

    void Back()
    {
        SceneManager.LoadScene("Menu", LoadSceneMode.Single);
    }
}
