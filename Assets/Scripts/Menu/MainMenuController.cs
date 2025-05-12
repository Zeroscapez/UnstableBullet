using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{

    public GameObject mainmenu;
    public GameObject controls;
    // Start is called before the first frame update
    void Start()
    {
        controls.SetActive(false);
        mainmenu.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartLevel()
    {
        SceneManager.LoadScene("Level1TheSun"); 
    }

    public void Controls()
    {
        controls.SetActive(true);
        mainmenu.SetActive(false);
    }

    public void Back()
    {
        controls.SetActive(false);
        mainmenu.SetActive(true);
    }


}
