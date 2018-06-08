using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LeaderboardController : MonoBehaviour
{
    public GameObject SendDataFrame;
    public GameObject LeaderboardFrame;
    protected InputField _inputField;
    protected OnlineGameManager ogm;

    void Awake()
    {
        ogm = FindObjectOfType<OnlineGameManager>();
    }

    public void SendScore()
    {
        Debug.Log("El boton ha funcionado");
        _inputField = GetComponentInChildren<InputField>();
        //_inputField.text = PlayerPrefs.GetString("playerName");

        if (_inputField.text.Length > 0 && _inputField.text.Length <= 5)
        {
            Debug.Log("entrooo ");
            //PlayerPrefs.SetString("playerName", _inputField.text);
            ogm.SendGameDataToServer();
            Debug.Log("highscore: " + ogm.highScores);

            SendDataFrame.SetActive(false);
            LeaderboardFrame.SetActive(true);

        }
    }

    public string GetPlayerName()
    {
        return _inputField.text;
    }
}