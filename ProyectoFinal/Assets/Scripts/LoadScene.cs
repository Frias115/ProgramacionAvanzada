using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class LoadScene : MonoBehaviour
{
    public string SceneName = "Menu";

    void Update()
    {
        if (Input.anyKeyDown)
        {
            SceneManager.LoadScene(SceneName);
            Time.timeScale = 1f;
        }
    }
}
