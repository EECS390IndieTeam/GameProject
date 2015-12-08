using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Indicators : MonoBehaviour {

	private Flag friendlyFlag;
	private Flag enemyFlag;
	private FlagCapturePoint friendlyBase;
	private FlagCapturePoint enemyBase;

	public GameObject friendlyFlagIndicator;
	public GameObject enemyFlagIndicator;
	public GameObject friendlyBaseIndicator;
	public GameObject enemyBaseIndicator;

	private AbstractPlayer player;
	public Camera camera;
	public Canvas canvas;
	private CaptureTheFlagMode ctf;
	private RectTransform crt;

	// Use this for initialization
	void Start () {
		player = (AbstractPlayer)GameManager.instance.CurrentPlayer;
		ctf = (CaptureTheFlagMode)GameManager.instance.GameMode;
		crt = canvas.GetComponent<RectTransform>();
		friendlyBase = ctf.GetCapPointForTeam(player.Team);
		enemyBase = ctf.GetCapPointForTeam(player.Team==1?2:1);
		friendlyFlag = ctf.GetFlagForTeam(player.Team);
		enemyFlag = ctf.GetFlagForTeam(player.Team==1?2:1);
		friendlyBaseIndicator.GetComponent<Image>().color = Teams.Colors[player.Team];
		friendlyFlagIndicator.GetComponent<Image>().color = Teams.Colors[player.Team];
		enemyBaseIndicator.GetComponent<Image>().color = Teams.Colors[player.Team==1?2:1];
		enemyFlagIndicator.GetComponent<Image>().color = Teams.Colors[player.Team==1?2:1];
	}
	
	// Update is called once per frame
	void LateUpdate () {
		if (!friendlyBase) {
			friendlyBase = ctf.GetCapPointForTeam(player.Team);
		} else {
			updateIndicator(friendlyBaseIndicator, friendlyBase.transform);
		}
		if (!enemyBase) {
			enemyBase = ctf.GetCapPointForTeam(player.Team==1?2:1);
		} else {
			updateIndicator(enemyBaseIndicator, enemyBase.transform);
		}
		if (!friendlyFlag) {
			friendlyFlag = ctf.GetFlagForTeam(player.Team);
		} else {
			if (!friendlyBase || !friendlyBase.FlagAtBase) {
				updateIndicator(friendlyFlagIndicator, friendlyFlag.transform);
			} else {
				friendlyFlagIndicator.SetActive(false);
			}
		}
		if (!enemyFlag) {
			enemyFlag = ctf.GetFlagForTeam(player.Team==1?2:1);
		} else {
			if (!enemyBase || !enemyBase.FlagAtBase) {
				updateIndicator(enemyFlagIndicator, enemyFlag.transform);
			} else {
				enemyFlagIndicator.SetActive(false);
			}
		}
	}

	private void updateIndicator(GameObject indicator, Transform source) {
		RectTransform rt = (RectTransform)indicator.transform;
		Vector3 point = camera.WorldToViewportPoint(source.position);
		Vector2 screenposition = new Vector2((point.x*crt.sizeDelta.x)-(crt.sizeDelta.x*0.5f), ((point.y*crt.sizeDelta.y)-(crt.sizeDelta.y*0.5f)));
		if (point.z > 0f) {
			indicator.SetActive(true);
			screenposition.x = Mathf.Sign(screenposition.x) * Mathf.Min (400f, Mathf.Abs(screenposition.x));
			screenposition.y = Mathf.Sign(screenposition.y) * Mathf.Min (210f, Mathf.Abs(screenposition.y));
			rt.anchoredPosition = screenposition;
		} else {
			indicator.SetActive(false);
		}
	}
}
