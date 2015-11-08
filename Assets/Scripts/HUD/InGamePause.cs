using UnityEngine;
using System.Collections;

public class InGamePause : MonoBehaviour {
	
	public Canvas pauseCanvas;
	bool paused = false;
	
	public GameObject crosshair;
	
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown ("escape")) {
			if(paused == true){
				pauseCanvas.enabled = false;
				crosshair.SetActive(true);
				Cursor.visible = false;
				Cursor.lockState = CursorLockMode.Locked;
				paused = false;
			} else {
				pauseCanvas.enabled = true;
				crosshair.SetActive(false);
				Cursor.visible = true;
				Cursor.lockState = CursorLockMode.None;
				paused = true;
			}
		}
	}
	
	public void Resume () {
		pauseCanvas.enabled = false;
		Cursor.visible = false;
		Cursor.lockState = CursorLockMode.Locked;
		paused = false;
	}
	
	public void Quit () {
		Application.LoadLevel ("MainMenu");
	}
}
