using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SettingsDropdowns : MonoBehaviour {
	
	public Dropdown resolutionDropdown;
	public Dropdown qualityDropdown;
	
	// Use this for initialization
	void Start () {
		Resolution[] resolutions = Screen.resolutions;
		resolutionDropdown.options.Clear ();
		setDropdownLabelText(resolutionDropdown, "Resolution");
		for (int i = 0; i < resolutions.Length; i++) {
			resolutionDropdown.options.Add (new Dropdown.OptionData(resolutions[i].width + "x" + resolutions[i].height));
			resolutionDropdown.onValueChanged.AddListener((entry) => {
				Screen.SetResolution(resolutions[entry].width, resolutions[entry].height, true);
				setDropdownLabelText(resolutionDropdown, "Resolution");
			});
		}
		string[] names = QualitySettings.names;;
		qualityDropdown.options.Clear ();
		setDropdownLabelText(qualityDropdown, "Quality");
		for (int i = 0; i < names.Length; i++) {
			qualityDropdown.options.Add(new Dropdown.OptionData(names[i]));
			qualityDropdown.onValueChanged.AddListener((entry) => {
				QualitySettings.SetQualityLevel(i, true);
				setDropdownLabelText(qualityDropdown, "Quality");
			});
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	
	public void setDropdownLabelText(Dropdown dd, string text) {
		for (int i = 0; i < dd.transform.childCount; i++) {
			if (dd.transform.GetChild(i).name.Equals("Label")) {
				dd.transform.GetChild(i).GetComponent<Text>().text = text;
			}
		}
	}
}
