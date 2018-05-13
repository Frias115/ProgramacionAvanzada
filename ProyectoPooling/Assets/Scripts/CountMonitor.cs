using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CountMonitor : MonoBehaviour {

	public string prefab;
	private Text text;

	// Use this for initialization
	void Awake () {
		text = GetComponent<Text> ();
	}
	
	// Update is called once per frame
	void Update () {
		if (prefab != ""){
			text.text = prefab + ": " + PoolManager.Instance.GetNumeroPrefabs(prefab).ToString();
		}
	}
}
