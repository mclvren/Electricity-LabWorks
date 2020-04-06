using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RotateGO : MonoBehaviour
{
    [SerializeField] GameObject go;
    [SerializeField] GameObject DegText;
    [SerializeField] GameObject InitButton;

    Coroutine coroutine;
    private bool isClicked = false;

    void Start()
    {
		InitButton.GetComponent<Button>().onClick.AddListener(TaskOnClick);
        DegText.GetComponent<Text>().text = "0°";
    }

    void TaskOnClick()
    {
        if ((coroutine == null) && (isClicked == false))
            {
                coroutine = StartCoroutine(c_Rotate(90.0f, 5.0f));
                DegText.GetComponent<Text>().text = "90°";
                isClicked = true;
            InitButton.GetComponentInChildren<Text>().text = "Выключить";
        }
    }

    IEnumerator c_Rotate(float angle, float intensity)
    {
        var me = go.transform;
        var to = me.rotation * Quaternion.Euler(0.0f, 0.0f, angle);

        while (true)
        {
            me.rotation = Quaternion.Lerp(me.rotation, to, intensity * Time.deltaTime);

            if (Quaternion.Angle(me.rotation, to) < 0.01f)
            {
                coroutine = null;
                me.rotation = to;
                yield break;
            }

            yield return null;
        }
    }
}
