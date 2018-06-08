using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LeaderboardController : MonoBehaviour
{
    public GameObject SendDataFrame;
    public GameObject LeaderboardFrame;
    public GameObject LoadingFrame;
    protected InputField _inputField;
    protected OnlineGameManager ogm;
    public Text leaderboardIndex;
    public Text leaderboardNames;
    public Text leaderboardScores;
    public Text playerScore;
    void Awake()
    {
        ogm = FindObjectOfType<OnlineGameManager>();
    }

    public void SendScore()
    {
        _inputField = GetComponentInChildren<InputField>();
        //_inputField.text = PlayerPrefs.GetString("playerName");

        if (_inputField.text.Length > 0 && _inputField.text.Length <= 5)
        {
            //PlayerPrefs.SetString("playerName", _inputField.text);
            ogm.SendGameDataToServer();
            StartCoroutine("WaitForServer");
            SendDataFrame.SetActive(false);
            LoadingFrame.SetActive(true);
        }
    }

    IEnumerator WaitForServer()
    {
        yield return new WaitForSeconds(3.0f);
        for (int i = 0; i < ogm.highScores.Count; i++)
        {
            leaderboardIndex.text += i + 1 + ".\n";
            leaderboardNames.text += ogm.highScores[i][0] + "\n";
            leaderboardScores.text += ogm.highScores[i][1] + "\n";
        }
        playerScore.text = ogm.playerHighScoreIndex+1 + ". " + ogm.playerStats[0] + " " + ogm.playerStats[1];
        LoadingFrame.SetActive(false);
        LeaderboardFrame.SetActive(true);
    }

    public string GetPlayerName()
    {
        return _inputField.text;
    }
}