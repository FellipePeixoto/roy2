using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] GameObject[] screens, optionTab;
    [SerializeField] GameObject delSlotScreen, btQuitCredits;
    [SerializeField] Button[] quitBts;
    int currentScreen = 0, selectedSlot=0, currentOptionTab = 0;
    [SerializeField] GameObject[] mainSelects, optSelects, slotSelects;
    bool configuring;

    void Start()
    {
        ChangeScreen(0);        
    }

    public void ChangeSelection(int btID)
    {
        if (screens[screens.Length-1].activeSelf) setQuitScreen(false);
        switch (currentScreen)
        {
            case 0:
                for (int i = 0; i < mainSelects.Length; i++)
                    if (i != btID) mainSelects[i].SetActive(false);
                mainSelects[btID].SetActive(true);
                break;
            case 1:
                for (int i = 0; i < slotSelects.Length; i++)
                    if (i != btID) slotSelects[i].SetActive(false);
                slotSelects[btID].SetActive(true);
                selectedSlot = btID;
                break;
            case 3:
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
        for (int i = 0; i < screens.Length-1; i++)
            if (i != currentScreen) screens[i].SetActive(false);
        screens[currentScreen].SetActive(true);

        switch (currentScreen)
        {
            case 0:
                mainSelects[0].GetComponentInParent<Button>().Select();
                break;
            case 1:
                slotSelects[0].GetComponentInParent<Button>().Select();
                break;
            case 3:
                optSelects[0].GetComponentInParent<Button>().Select();
                break;
            case 4:
                btQuitCredits.GetComponent<Button>().Select();
                break;
        }
    }

    void Update()
    {
        /*if (currentScreen == 1)
        {
            if (Input.GetKey("Cancel")) ChangeScreen(0);
        }
        if (configuring)
        {
            if (Input.GetKey("Cancel")) ChangeSelection(currentOptionTab);
        }*/
    }

    public void setQuitScreen(bool active)
    {
        screens[screens.Length-1].SetActive(active);
        if (active) quitBts[0].Select();
        else mainSelects[screens.Length - 1].GetComponentInParent<Button>().Select();
    }

    public void DeleteScreen(bool call)
    {
        //if(selectedSlot == saveExistente)
        delSlotScreen.SetActive(call);
        if (call) delSlotScreen.transform.GetChild(1).GetComponent<Button>().Select();
        else slotSelects[slotSelects.Length - 1].GetComponentInParent<Button>().Select();
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

    public void QuitGame()
    {
        Debug.Log("Quitting Application");
        Application.Quit();
    }

    public void ChangeToScene(int buildIndex)
    {
        SceneSingleton.LoadScene(buildIndex);
    }
    public void ChangeToScene(string sceneName)
    {
        SceneSingleton.LoadScene(sceneName);
    }
}
