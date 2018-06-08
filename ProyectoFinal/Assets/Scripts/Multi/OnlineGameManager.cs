using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SimpleJSON;

public class OnlineGameManager : MonoBehaviour{
    public JSONArray highScores;
    public int playerHighScoreIndex;
    public JSONArray playerStats;
    public bool responseRecieved = false;
    public string HostIp;
    public int HostPort;

    protected LeaderboardController _leaderboard;
    protected HUDManager _HUD;
    protected int _playerId;

    protected TAPNet _client;
    protected int highscore = -1;

    // Use this for initialization
    protected virtual void Awake() {
        _HUD = FindObjectOfType<HUDManager>();
        _client = new TAPNet(HostIp, HostPort) {
            onResponseReceived = OnServerResponse
        };
    }

    /// <summary>
    /// Método a ejecutar cuando el servidor nos responda a una petición
    /// </summary>
    /// <param name="json">JSON de respuesta que llega desde el servidor.</param>
    public void OnServerResponse(JSONNode json) {
        UnityMainThreadDispatcher.Instance().Enqueue(ProcessJSON(json));
    }

    /// <summary>
    /// Lo utilizamos para procesar el JSON que nos llega desde el servidor.
    /// </summary>
    /// <param name="json">JSON que nos envía el servidor.</param>
    IEnumerator ProcessJSON(JSONNode json) {
        highScores = json["high_scores"].AsArray;
        playerHighScoreIndex = json["player_rank"].AsInt;
        playerStats = json["player_stats"].AsArray;
        responseRecieved = true;
        yield return null;
    }

    /// <summary>
    /// Se actualiza periódicamente para mantener el estado de la partida actualizado
    /// </summary>
    public void SendGameDataToServer() {
        _leaderboard = FindObjectOfType<LeaderboardController>();
        var updateData = new UpdateData {
            name = _leaderboard.GetComponent<LeaderboardController>().GetPlayerName(),
            score = _HUD.GetComponent<HUDManager>().GetScore()
        };
        _client.Send(updateData.ToJson());
    }

    void OnDestroy()
    {
        _client.Cleanup();
    }
}
