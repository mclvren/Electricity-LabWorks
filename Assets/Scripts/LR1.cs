using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

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
    //Прибавление тока
    void PlusX()
    {
        mA += 15;
        currentValue = mA;
        cur.text = mA.ToString();
        StopAllCoroutines();
        if (isOn) rttclockwise(mA * Random.Range(0.1935f, 0.1965f));
    }

    void Plus10X()
    {
        mA += 10;
        currentValue = mA;
        cur.text = mA.ToString();
        StopAllCoroutines();
        if (isOn) rttclockwise(mA * Random.Range(0.1935f, 0.1965f));
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
                if (isOn) rttcounterclockwise(mA * Random.Range(0.1935f, 0.1965f));
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
                if (isOn) rttcounterclockwise(mA * Random.Range(0.1935f, 0.1965f));
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
        if (inverseState) k = Random.Range(0.2026f, 0.2066f);
        else k = Random.Range(0.1935f, 0.1965f);
        if (mA > currentValue)
        {
            currentValue = mA;
            cur.text = mA.ToString();
            if (isOn) rttclockwise(mA * k);
        }
        else if (mA < currentValue)
        {
            currentValue = mA;
            cur.text = mA.ToString();
            if (isOn) rttcounterclockwise(mA * k);
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
        if (bAngle != null) tAngle = bAngle;
        else tAngle = currentAngle;
        //UnityEngine.Debug.Log(val);
        switch (val)
        {
            case 0:
                tAngle = bAngle;
                break;
            case 1:
                tAngle += Random.Range(1.90f, 2.10f);
                break;
            case 2:
                tAngle -= Random.Range(0.90f, 1.10f);
                break;
            case 3:
                tAngle -= Random.Range(3.90f, 4.10f);
                break;
            case 4:
                tAngle -= Random.Range(7.90f, 8.10f);
                break;
            case 5:
                tAngle -= Random.Range(15.90f, 16.10f);
                break;
            default:
                UnityEngine.Debug.Log("Смещение задано не верно");
                break;
        }
        ShifterH(tAngle);
        if (!Shifter_Check_If_Changed) bAngle = currentAngle;
        Shifter_Check_If_Changed = true;
        UnityEngine.Debug.Log("bAngle = "+bAngle);
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
        }
        else
        {
            Plus.GetComponent<Button>().interactable = true;
            Plus10.GetComponent<Button>().interactable = true;
            Minus.GetComponent<Button>().interactable = true;
            Minus10.GetComponent<Button>().interactable = true;
            Zero.GetComponent<Button>().interactable = true;
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
                UnityEngine.Debug.Log("Текущий угол: " + currentAngle);
                break;
            }
            else if (currentAngle!=targetAngle)
            {
                currentAngle += step;
                Arrow.transform.Rotate(Vector3.back, step);
                UnityEngine.Debug.Log("Текущий угол: " + currentAngle);
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
                UnityEngine.Debug.Log("Текущий угол: " + currentAngle);
                break;
            }
            else if (currentAngle != targetAngle)
            {
                currentAngle += step;
                Arrow.transform.Rotate(Vector3.back, step);
                UnityEngine.Debug.Log("Текущий угол: " + currentAngle);
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
        Deg.GetComponent<Text>().text = (ang.ToString()+"°");
    }
    //Инициализация поворота в обратную сторону
    void rttcounterclockwise(float ang)
    {
        var targetAngle = ang;
        float rotationSpeed = -50f; // Скорость поворота
        StartCoroutine(RotateMeNowCC(targetAngle, rotationSpeed));
        Deg.GetComponent<Text>().text = (ang.ToString() + "°");
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
        SceneManager.LoadScene("SampleScene", LoadSceneMode.Single);
    }
}
