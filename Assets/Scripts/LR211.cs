using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LR211 : MonoBehaviour
{
    [SerializeField] GameObject Eds;
    [SerializeField] GameObject Menu;
    [SerializeField] GameObject SButton;
    [SerializeField] GameObject Locker;

    //Массивы с объектами
    public GameObject[] Resistors;
    public GameObject[] Resistance;
    public GameObject[] Init;
    public GameObject[] Button;
    public GameObject[] Canvas;
    public GameObject[] RB;
    //Переменные
    private bool[] isOn = { false, false };
    //Напряжения источников, В
    private float[] source = new float[] { 9f, 12f }; // ЭДС1 - ЭДС2
    //Внутреннее сопротивление источников
    private float[] rin = new float[] { 0.5f, 0.1f }; // ЭДС1 - ЭДС2
    //Номиналы резисторов, Ом
    private float[] res = new float[] { 0.5f, 1f, 2.2f, 47f, 100f, 2700f, 4700f }; // R1 - R7
    //Вспомогательные переменные
    private Dropdown EDS;
    private float currentSource;
    private float currentResistor;
    private float Rs;
    public Text[] Current;
    public Text[] VoltageIn;
    public Text[] VoltageOut;

    //Возврат в мен.
    void Back()
    {
        SceneManager.LoadScene("Menu", LoadSceneMode.Single);
    }
    //Обработчик кнопки включить источник
    void Enable(int N)
    {
        //Схема 2.6
        if (N == 1)
        {
            if (isOn[0] == false)
            {
                isOn[0] = true;
                Init[0].GetComponentInChildren<Text>().text = "Выключить";
                VoltageIn[0].text = currentSource.ToString();
                getOutput(currentSource, currentResistor, Rs);
            }
            else if (isOn[0] == true)
            {
                isOn[0] = false;
                VoltageIn[0].text = "0";
                VoltageOut[0].text = "0";
                Current[0].text = "0";
                Init[0].GetComponentInChildren<Text>().text = "Включить";
            }
        }
        //Схема 2.5
        else if (N == 2)
        {
            if (isOn[1] == false)
            {
                isOn[1] = true;
                Init[1].GetComponentInChildren<Text>().text = "Выключить";
                VoltageIn[1].text = source[0].ToString();
                VoltageIn[2].text = source[1].ToString();
                //
                var R1 = float.Parse(Resistance[1].GetComponent<Text>().text);
                var R2 = float.Parse(Resistance[2].GetComponent<Text>().text);
                var R3 = float.Parse(Resistance[3].GetComponent<Text>().text);
                //getOutput2(M, source[0], source[1], R1, R2, R3, rin[0], rin[1]);
            }
            else if (isOn[1] == true)
            {
                Locker.SetActive(true);
                isOn[1] = false;
                VoltageIn[1].text = "0";
                VoltageIn[2].text = "0";
                VoltageOut[1].text = "0";
                Current[1].text = "0";
                Init[1].GetComponentInChildren<Text>().text = "Включить";
            }
        }
    }
    //Функуция выбора источника для первой схемы
    void xEDS(Dropdown DSX)
    {
        var val = DSX.value;
        currentSource = source[val];
        Rs = rin[val];
        VoltageIn[0].text = currentSource.ToString();
        getOutput(currentSource, currentResistor, Rs);
        UnityEngine.Debug.Log("Источник №" + val + " = " + currentSource + " В");
    }
    //Функция отображения номинала резистора по его номеру
    void xRES(Dropdown DSX, int N)
    {
        var val = DSX.value;
        if (N == 0)
        {
            currentResistor = res[val];
            Resistance[0].GetComponent<Text>().text = currentResistor.ToString();
            getOutput(currentSource, currentResistor, Rs);
        }
        else
        {
            Resistance[N].GetComponent<Text>().text = res[val].ToString();
        }
        UnityEngine.Debug.Log("R" + N + " №" + (val + 1) + " = " + res[val] + " Ом");
    }
    //Подсчет выходных значений
    void getOutput(float U, float R, float r)
    {
        float I = U / (R + r);
        Current[0].text = I.ToString();
        VoltageOut[0].text = (I * R).ToString();
    }
    //Подсчет выходных значений для схемы 2.5 (без учета внутреннего сопротивления источников, т.к. в задании эго не учитывали)
    void getOutput2(int M, float U1, float U2, float R1, float R2, float R3, float r1, float r2)
    {
        float I = 0;
        switch (M)
        {
            case 0:
                I = (U1 * (R2 + R3) + U2 * R3) / (R3 * (R1 + R2) + R1 * R2);
                break;
            case 1:
                I = (U1 * R3 + U2 * (R1 + R3)) / (R3 * (R1 + R2) + R1 * R2);
                break;
            case 2:
                I = (U1 * R2 - U2 * R1) / (R3 * (R1 + R2) + R1 * R2);
                break;
            default:
                UnityEngine.Debug.Log("Ошибка при ветви измерений");
                break;
        }
        Current[1].text = I.ToString();
        VoltageOut[1].text = (I * float.Parse(Resistance[M].GetComponent<Text>().text)).ToString();
    }
    //Выбор интерфейсы для определенной схемы
    void switchCanvas(int n)
    {
        switch (n)
        {
            case 0:
                Canvas[1].SetActive(false);
                Canvas[2].SetActive(false);
                Canvas[0].SetActive(true);
                SButton.SetActive(false);
                break;
            case 1:
                Canvas[0].SetActive(false);
                Canvas[1].SetActive(true);
                SButton.SetActive(true);
                break;
            case 2:
                Canvas[0].SetActive(false);
                Canvas[2].SetActive(true);
                SButton.SetActive(true);
                break;
            default:
                UnityEngine.Debug.Log("Ошибка при выборе схемы");
                break;
        }
    }

    //Перед первым кадром
    async void Start()
    {
        //События для кнопок
        Menu.GetComponent<Button>().onClick.AddListener(Back);
        Init[0].GetComponent<Button>().onClick.AddListener(delegate { Enable(1); });
        Init[1].GetComponent<Button>().onClick.AddListener(delegate { Enable(2); });
        Button[0].GetComponent<Button>().onClick.AddListener(delegate { switchCanvas(1); });
        Button[1].GetComponent<Button>().onClick.AddListener(delegate { switchCanvas(2); });
        SButton.GetComponent<Button>().onClick.AddListener(delegate { switchCanvas(0); });
        setRBEvents();
        //Дропдаун меню
        EDS = Eds.GetComponent<Dropdown>();
        //События для дропдаун меню
        EDS.onValueChanged.AddListener(delegate
        {
            xEDS(EDS);
        });
        setREvents();
        //Значения по умолчанию
        currentSource = source[0];
        Rs = rin[0];
        currentResistor = res[0];
        Resistance[0].GetComponent<Text>().text = currentResistor.ToString();
    }
    //Назначаем события для кнопок выбора резисторов
    void setREvents()
    {
        int count = 0;
        Dropdown prevD = null;
        foreach (GameObject element in this.Resistors)
        {
            count++;
            // Фикс неправильного назначения на один и тот же дропдаун
            var dropdown = element.GetComponent<Dropdown>();
            var N = count - 1;
            //
            if (dropdown != prevD)
            {
                UnityEngine.Debug.Log("EventListener for R" + N + " is active");
                dropdown.onValueChanged.AddListener(delegate
                {
                    Locker.SetActive(true);
                    xRES(dropdown, N);
                });
            }
            prevD = dropdown;
        }
    }
    //Назначаем события для кнопок выбора ветви измерений
    void setRBEvents()
    {
        int count = 0;
        foreach (GameObject element in this.RB)
        {
            count++;
            // Фикс неправильного назначения на один и тот же дропдаун
            var button = element.GetComponent<Button>();
            var N = count - 1;
            //
            UnityEngine.Debug.Log("EventListener for RB" + N + " is active");
            button.onClick.AddListener(delegate
            {
                Locker.SetActive(false);
                var R1 = float.Parse(Resistance[1].GetComponent<Text>().text);
                var R2 = float.Parse(Resistance[2].GetComponent<Text>().text);
                var R3 = float.Parse(Resistance[3].GetComponent<Text>().text);
                getOutput2(N, source[0], source[1], R1, R2, R3, rin[0], rin[1]);
            });
        }
    }
    // Update is called once per frame
    void Update()
    {

    }
}
