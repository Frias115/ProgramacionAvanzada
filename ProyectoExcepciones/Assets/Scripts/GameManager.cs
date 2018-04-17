using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : Singleton<GameManager> {
    public Text _timerText, _resultText;
    public int Delivered, Returned;

    public int Pending {
        get {
            var pending = 0;
            foreach (var parcel in FindObjectsOfType<Parcel>()) {
                if (!parcel.Done) {
                    pending++;
                }
            }
            return pending;
        }
    }

	// Use this for initialization
	void Awake () {
        _timerText = transform.Find("Timer Text").GetComponent<Text>();
        _resultText = transform.Find("Result Text").GetComponent<Text>();
        Invoke("CheckOk", 70);
	}
	
	// Update is called once per frame
	void Update () {
        _timerText.text = Time.time.ToString("n1");
    }

    /// <summary>
    /// Verifica que la escena se haya ejecutado correctamente
    /// </summary>
    protected void CheckOk() {
        Time.timeScale = 0;

        _resultText.text = Pending == 0 ? "OK" : "Error, no se entregaron todos los paquetes";
        _resultText.text += "\nEntregados: " + Delivered;
        _resultText.text += "\nDevueltos: " + Returned;
        _resultText.text += "\nPendientes: " + Pending;
    }
}
