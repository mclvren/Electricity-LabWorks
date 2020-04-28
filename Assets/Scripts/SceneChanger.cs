using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneChanger : MonoBehaviour
{
    [SerializeField] GameObject Lab21;
    [SerializeField] GameObject Lab22;
    [SerializeField] GameObject Lab25;
    [SerializeField] GameObject Lab29;
    [SerializeField] GameObject Lab211;



    // Start is called before the first frame update
    void Start()
    {
        Lab25.GetComponent<Button>().onClick.AddListener(Scene25);
    }

    void Scene25()
    {
        SceneManager.LoadScene("Laba25", LoadSceneMode.Single);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
