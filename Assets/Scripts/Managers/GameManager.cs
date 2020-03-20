using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    static GameManager _instance;

    public static GameManager Instance { get => _instance; }

    [SerializeField] FloatVarSO _loadProgress;
    [SerializeField] IntVarSO _playersPaired;

    [SerializeField] UnityEvent onAllPlayersPaired;

    private void Awake()
    {
        if (_instance == null && _instance != this)
        {
            _instance = this;
            _playersPaired.Value = 0;
            _loadProgress.Value = 0;
            DontDestroyOnLoad(_instance);
            return;
        }

        Destroy(this);
    }

    public void CheckPlayersPaired()
    {
        if (_playersPaired.Value == 1)
        {
            if (GameObject.FindGameObjectWithTag("LoadScreen"))
            {
                GameObject.FindGameObjectWithTag("LoadScreen").GetComponent<Canvas>().enabled = true;
            }

            onAllPlayersPaired.Invoke();
        }
    }

    public void LoadLevelByScene(Object scene)
    {
        StartCoroutine(LoadSceneInBackground(scene.name));
    }

    public void LoadALevel(string name)
    {
        StartCoroutine(LoadSceneInBackground(name));
    }

    IEnumerator LoadSceneInBackground(string name)
    {
        AsyncOperation asynncProgress = SceneManager.LoadSceneAsync(name);

        while (!asynncProgress.isDone)
        {
            _loadProgress.Value = asynncProgress.progress / .9f;
            yield return null;
        }
    }
}
