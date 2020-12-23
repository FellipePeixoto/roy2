using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class ResetScene : MonoBehaviour
{
    bool restartComing;

    private void Update()
    {
        if (restartComing)
            return;

        foreach(Gamepad pad in Gamepad.all)
        {
            if (pad.startButton.wasPressedThisFrame && !restartComing)
            {
                restartComing = true;
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
        }
    }
}
