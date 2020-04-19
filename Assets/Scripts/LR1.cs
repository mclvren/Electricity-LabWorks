using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;

public class LR1 : MonoBehaviour
{
    [SerializeField] GameObject Arrow;
    [SerializeField] GameObject Deg;
    [SerializeField] GameObject Init;
    [SerializeField] GameObject Polar;
    [SerializeField] GameObject Menu;
    [SerializeField] GameObject Current;
    [SerializeField] GameObject Plus;
    [SerializeField] GameObject Minus;
    [SerializeField] GameObject Zero;
    [SerializeField] GameObject Blackout;

    Coroutine coroutine;
    private bool isOn = false;
    private float mA = 0;
    private Text cur;
    bool goRotatePlr = false;
    private float currentAngle = 0f;
    private float currentValue;
    private bool inverseState = false;

    void Start()
    {
        Init.GetComponent<Button>().onClick.AddListener(Enable);
        Plus.GetComponent<Button>().onClick.AddListener(PlusX);
        Minus.GetComponent<Button>().onClick.AddListener(MinusX);
        Zero.GetComponent<Button>().onClick.AddListener(ZeroX);
        Polar.GetComponent<Button>().onClick.AddListener(Inverse);
        Deg.GetComponent<Text>().text = "0°";
        cur = Current.GetComponent<Text>();
        rttclockwise(0);
    }

    void PlusX()
    {
        mA += 15;
        currentValue = mA;
        cur.text = mA.ToString();
        StopAllCoroutines();
        if (isOn) rttclockwise(mA * 0.195f);
    }

    void MinusX()
    {
        if (currentValue > 0)
        {
            mA -= 15;
            currentValue = mA;
            cur.text = mA.ToString();
            StopAllCoroutines();
            if (isOn) rttcounterclockwise(mA * 0.195f);
        }
    }

    void ZeroX()
    {
        mA = 0;
        StopAllCoroutines();
        if (mA > currentValue)
        {
            currentValue = mA;
            cur.text = mA.ToString();
            if (isOn) rttclockwise(0); // тест
        }
        else
        {
            currentValue = mA;
            cur.text = mA.ToString();
            //Debug.Log("обратно не робит");
            if (isOn) rttcounterclockwise(0);
        }
    }

    void Inverse()
    {
        float k;
        mA *= -1;
        inverseState = !inverseState;
        StopAllCoroutines();
        if (inverseState) k = 0.2046f;
        else k = 0.195f;
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

    void Update()
    {
        if (isOn&&(mA != 0))
        {
            toggleBlackout(true);
        }
        else
        {
            toggleBlackout(false);
        }
    }

    IEnumerator RotateMeNow(float targetAngle, float rotationSpeed)
    {
        while (true)
        {
            float step = rotationSpeed * Time.deltaTime;
            if (currentAngle + step >= targetAngle)
            {
                // Докручиваем до нашего угла
                step = targetAngle - currentAngle;
                Arrow.transform.Rotate(Vector3.back, step);
                break;
            }
            else if (currentAngle!=targetAngle)
            {
                currentAngle += step;
                Arrow.transform.Rotate(Vector3.back, step);
            }
            yield return null;
        }
    }

    IEnumerator RotateMeNowCC(float targetAngle, float rotationSpeed)
    {
        while (true)
        {
            float step = rotationSpeed * Time.deltaTime;
            if (currentAngle + step <= targetAngle)
            {
                // Докручиваем до нашего угла
                step = targetAngle - currentAngle;
                Arrow.transform.Rotate(Vector3.back, step);
                break;
            }
            else if (currentAngle != targetAngle)
            {
                currentAngle += step;
                Arrow.transform.Rotate(Vector3.back, step);
            }
            yield return null;
        }
    }


    void rttclockwise(float ang)
    {
        var targetAngle = ang;
        float rotationSpeed = 50f; // Скорость поворота
        StartCoroutine(RotateMeNow(targetAngle, rotationSpeed));
        Deg.GetComponent<Text>().text = (ang.ToString()+"°");
    }

    void rttcounterclockwise(float ang)
    {
        var targetAngle = ang;
        float rotationSpeed = -50f; // Скорость поворота
        StartCoroutine(RotateMeNowCC(targetAngle, rotationSpeed));
        Deg.GetComponent<Text>().text = (ang.ToString() + "°");
    }

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
        }
    }

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
}
