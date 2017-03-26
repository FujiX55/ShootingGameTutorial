using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class Setting : MonoBehaviour
{
	Canvas canvas;
	Toggle vibrate;
	Toggle vibnear;

	void Start()
	{
		canvas = GetComponent<Canvas>();

		foreach (Transform child in canvas.transform) {
			if (child.name == "Vibrate") {
				vibrate = child.gameObject.GetComponent<Toggle>();
			}
			else if (child.name == "VibNear") {
				vibnear = child.gameObject.GetComponent<Toggle>();
			}
		}
		vibrate.isOn = GameMgr.GetVibrate();
		vibnear.isOn = GameMgr.GetVibNear();
	}

	void Update()
	{
		//		Debug.Log(vibrate.isOn);
	}

	public void ChangeVibrate()
	{
		GameMgr.SetVibrate(vibrate.isOn);
	}

	public void ChangeVibNear()
	{
		GameMgr.SetVibNear(vibnear.isOn);
	}

	public void ExitScene()
	{
		// タイトルへ戻る
		SceneManager.LoadScene("Title");
	}
}
