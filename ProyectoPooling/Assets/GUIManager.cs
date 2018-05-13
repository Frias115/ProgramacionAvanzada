using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GUIManager : MonoBehaviour {
	public void ClickedButton() {
		SceneManager.LoadScene(0);
	}
}
