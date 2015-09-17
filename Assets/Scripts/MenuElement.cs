using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MenuElement : MonoBehaviour {

	public Canvas menuCanvas;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void onClick(Canvas clickedCanvas) {
		menuCanvas.enabled = false;
		clickedCanvas.enabled = true;
        Debug.LogWarning("Switch");
	}

	public void changeResolution(Button button) {
		if (button.name.Equals("800x600Button")) {
			if (Application.isEditor) {
				Debug.Log ("800x600");
			}
			Screen.SetResolution (800, 600, true);
		}
		//...
	}

	public void dropMenu(Canvas dropCanvas) {
		if (dropCanvas.isActiveAndEnabled) {
			dropCanvas.enabled = false;
		} else {
			dropCanvas.gameObject.SetActive(true);
			dropCanvas.enabled = true;
		}
	}

	public void back(Canvas clickedCanvas) {
		clickedCanvas.enabled = false;
		menuCanvas.enabled = true;
	}

	public void settingsBack(Canvas clickedCanvas) {
		GameObject.Find ("ResolutionCanvas").SetActive(false);
		back (clickedCanvas);
	}

	public void exit() {
		if (Application.isEditor) {
			Debug.Log ("EXIT");
		}
		Application.Quit ();
	}
}
