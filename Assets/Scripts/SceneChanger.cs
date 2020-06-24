using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneChanger : MonoBehaviour
{
    [SerializeField] GameObject Lab22;
    [SerializeField] GameObject Lab25;
    [SerializeField] GameObject Lab29;
    [SerializeField] GameObject Lab211;



    // Start is called before the first frame update
    void Start()
    {
        Lab22.GetComponent<Button>().onClick.AddListener(delegate { SceneX("Laba22"); });
        Lab29.GetComponent<Button>().onClick.AddListener(delegate { SceneX("Laba29"); });
        Lab25.GetComponent<Button>().onClick.AddListener(delegate { SceneX("Laba25"); });
        Lab211.GetComponent<Button>().onClick.AddListener(delegate { SceneX("Laba211"); });
    }

    void SceneX(string x)
    {
        SceneManager.LoadScene(x, LoadSceneMode.Single);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
