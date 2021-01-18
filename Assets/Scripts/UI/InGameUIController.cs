using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InGameUIController : MonoBehaviour
{
    [SerializeField] GameObject menu;
    [SerializeField] GameObject[] selects, optSelects, screens, optionTab;
    [SerializeField] Button[] quitBts;
    int currentScreen = 0, currentOptionTab = 0;
    bool configuring;
    void Start()
    {
        PauseGame(true);
    }

    void Update()
    {
        /*if (Input.GetKey("Cancel")) 
        {
            PauseGame(true);
        }
        if (configuring)
        {
            if (Input.GetKey("Cancel")) ChangeSelection(currentOptionTab);
        }*/
    }

    public void PauseGame(bool paused)
    {
        menu.SetActive(paused);
        Time.timeScale = paused? 0 : 1;
        if (paused) ChangeScreen(0);
    }

    public void ChangeSelection(int btID)
    {
        if (screens[screens.Length - 1].activeSelf) setQuitScreen(false);
        switch (currentScreen)
        {
            case 0:
                for (int i = 0; i < selects.Length; i++)
                    if (i != btID) selects[i].SetActive(false);
                selects[btID].SetActive(true);
                break;
            case 1:
                for (int i = 0; i < optSelects.Length; i++)
                    if (i != btID) optSelects[i].SetActive(false);
                optSelects[btID].SetActive(true);
                if (btID >= optionTab.Length) btID = optionTab.Length - 1;
                setOptionTab(btID);
                break;
        }
    }

    public void ChangeScreen(int screen)
    {
        currentScreen = screen;
        for (int i = 0; i < screens.Length - 1; i++)
            if (i != currentScreen) screens[i].SetActive(false);
        screens[currentScreen].SetActive(true);

        switch (currentScreen)
        {
            case 0:
                selects[0].GetComponentInParent<Button>().Select();
                break;
            case 1:
                optSelects[0].GetComponentInParent<Button>().Select();
                break;
        }
    }

    public void setQuitScreen(bool active)
    {
        screens[screens.Length - 1].SetActive(active);
        if (active) quitBts[0].Select();
        else selects[selects.Length-1].GetComponentInParent<Button>().Select();
    }

    public void setOptionTab(int id)
    {
        for (int i = 0; i < optionTab.Length; i++)
            if (i != id) optionTab[i].SetActive(false);
        optionTab[id].SetActive(true);
        currentOptionTab = id;

        switch (id)
        {
            case 0:

                break;
            case 1:
                optionTab[1].transform.GetChild(0).GetComponent<Slider>().value = QualitySettings.GetQualityLevel();
                break;
        }
        optionTab[id].transform.GetChild(0).GetComponent<Slider>().Select();
        configuring = true;
    }

    public void ChangeQuality()
    {
        QualitySettings.SetQualityLevel((int)optionTab[1].transform.GetChild(0).GetComponent<Slider>().value);
    }

    public void QuitToMenu()
    {
        Debug.Log("Go to Menu");
        //Application.Quit();
    }
}
