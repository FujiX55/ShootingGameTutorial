using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class Setting : MonoBehaviour
{
	Canvas canvas;
	Toggle vibrate;
	Toggle vibnear;
	Toggle parallax;
	Toggle parainv;

	public Pad pad;

	void Start()
	{
		pad = new Pad();

		canvas = GetComponent<Canvas>();

		foreach (Transform child in canvas.transform) {
			switch (child.name) {
			case "Vibrate":
				vibrate = child.gameObject.GetComponent<Toggle>();
				break;
			case "VibNear":
				vibnear = child.gameObject.GetComponent<Toggle>();
				break;
			case "Parallax":
				parallax = child.gameObject.GetComponent<Toggle>();
				break;
			case "ParaInv":
				parainv = child.gameObject.GetComponent<Toggle>();
				break;
			}
		}
		vibrate.isOn = GameMgr.GetVibrate();
		vibnear.isOn = GameMgr.GetVibNear();
		parallax.isOn = Background.GetParallax();
		parainv.isOn = Background.GetParaInv();
	}

	void Update()
	{
		// 戻るボタンで終了
		if (pad.IsEscape()) {
			Application.Quit();
			return;
		}
	}

	public void ChangeVibrate()
	{
		GameMgr.SetVibrate(vibrate.isOn);
	}

	public void ChangeVibNear()
	{
		GameMgr.SetVibNear(vibnear.isOn);
	}

	public void ChangeParallax()
	{
		Background.SetParallax(parallax.isOn);
	}

	public void ChangeParaInv()
	{
		Background.SetParaInv(parainv.isOn);
	}

	public void ExitScene()
	{
		// タイトルへ戻る
		SceneManager.LoadScene("Title");
	}
}
