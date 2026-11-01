using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{

    public GameObject mainmenu;
    public GameObject controls;
    [SerializeField] private SettingsMenuControl settingsMenu;
    [SerializeField] private GameObject quitButton; // Desktop only; Application.Quit does nothing in a browser
    // Start is called before the first frame update
    void Start()
    {
        controls.SetActive(false);
        if (settingsMenu != null) settingsMenu.gameObject.SetActive(false);
        if (quitButton != null && Application.platform == RuntimePlatform.WebGLPlayer) quitButton.SetActive(false);
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

    public void Settings()
    {
        mainmenu.SetActive(false);
        settingsMenu.Open(() => mainmenu.SetActive(true));
    }

    public void Quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void Back()
    {
        controls.SetActive(false);
        mainmenu.SetActive(true);
    }


}
