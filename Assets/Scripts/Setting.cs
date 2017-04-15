using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class Setting : MonoBehaviour
{
	bool initialized = false;

	Canvas canvas;
	Toggle vibrate;
	Toggle vibnear;
	Toggle parallax;
	Toggle parainv;
	Toggle dbg_undead;

	public Pad pad;

	void Awake()
	{
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
			case "DbgUndead":
				dbg_undead = child.gameObject.GetComponent<Toggle>();
				break;
			}
		}
		vibrate.isOn = GameMgr.GetVibrate();
		vibnear.isOn = GameMgr.GetVibNear();
		parallax.isOn = Background.GetParallax();
		parainv.isOn = Background.GetParaInv();	
		dbg_undead.isOn = GameMgr.GetDbgUndead();

		initialized = true;
	}

	void Start()
	{
		pad = Pad.Instance;

		pad.Active = false;
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

		if (initialized && vibrate.isOn) {
			Vibration.Vibrate(100);
		}
	}

	public void ChangeVibNear()
	{
		GameMgr.SetVibNear(vibnear.isOn);

		if (initialized && vibnear.isOn) {
			Vibration.Vibrate(50);
		}
	}

	public void ChangeParallax()
	{
		Background.SetParallax(parallax.isOn);
	}

	public void ChangeParaInv()
	{
		Background.SetParaInv(parainv.isOn);
	}

	public void ChangeDbgUndead()
	{
		GameMgr.SetDbgUndead(dbg_undead.isOn);
	}

	// シーン終了
	public void ExitScene()
	{
		pad.Active = true;

		if (GameMgr.eState.Init == GameMgr.State) {
			// タイトルへ戻る
			SceneManager.LoadScene("Title");
		}
		else {
			// 前のシーンに戻る
			SceneManager.UnloadScene("Setting");
			Resources.UnloadUnusedAssets();
		}
	}
}
