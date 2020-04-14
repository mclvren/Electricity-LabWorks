using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LR1 : MonoBehaviour
{
    [SerializeField] GameObject Arrow;
    [SerializeField] GameObject Deg;
    [SerializeField] GameObject Init;
    [SerializeField] GameObject Polar;
    [SerializeField] GameObject Menu;
    [SerializeField] GameObject Slider;
    [SerializeField] GameObject Current;

    Coroutine coroutine;
    private bool isOn = false;
    private float mA;
    private Text cur;
    private Slider sl;
    bool goRotatePlr = false;
    private float currentAngle = 0f;
    private float currentValue;

    void Start()
    {
        Init.GetComponent<Button>().onClick.AddListener(Enable);
        Deg.GetComponent<Text>().text = "0°";
        cur = Current.GetComponent<Text>();
        sl = Slider.GetComponent<Slider>();
        sl.onValueChanged.AddListener(delegate { ValueChanged(); });
        rttclockwise(0);
    }

    void ValueChanged()
    {
        mA = sl.value;
        if (mA > currentValue)
        {
            currentValue = mA;
            cur.text = mA.ToString();
            if (isOn) rttclockwise(mA / 10); // тест
        }
        else
        {
            cur.text = mA.ToString();
            //Debug.Log("обратно не робит");
            if (isOn) rttcounterclockwise(mA / 10);
        }

    }

    void Update()
    {
        
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
        float rotationSpeed = 3f; // Скорость поворота
        StartCoroutine(RotateMeNow(targetAngle, rotationSpeed));
        Deg.GetComponent<Text>().text = (ang.ToString()+"°");
    }

    void rttcounterclockwise(float ang)
    {
        var targetAngle = ang;
        float rotationSpeed = -3f; // Скорость поворота
        StartCoroutine(RotateMeNowCC(targetAngle, rotationSpeed));
        Deg.GetComponent<Text>().text = (ang.ToString() + "°");
    }

    void Enable()
    {
        if (isOn == false)
        {
            isOn = true;
            Init.GetComponentInChildren<Text>().text = "Выключить";
        }
        else if (isOn == true)
        {
            isOn = false;
            Init.GetComponentInChildren<Text>().text = "Включить";
        }
    }

}
