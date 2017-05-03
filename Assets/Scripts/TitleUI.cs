using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class TitleUI : MonoBehaviour
{
	Pad pad;

	bool pressed = false;
	bool start = false;

	// Use this for initialization
	void Start()
	{
		Time.timeScale = 1.0f;

		// BGMを止める
		Sound.StopBgm();


		pad = Pad.Instance;
		pad.Active = false;
	}
	
	// Update is called once per frame
	void Update()
	{
		pad.Update();

		// 戻るボタンで終了
		if (pad.IsEscape()) {
			//			if (SystemInfo.supportsVibration) {
			//				VibrateScript.Destruct();
			//			}
			Application.Quit();
			return;
		}

		if (start) {
			SceneManager.LoadScene("Main");
			start = false;
		}
	}

	public void TransitSetting()
	{
		// 設定画面へ
		SceneManager.LoadScene("Setting", LoadSceneMode.Additive);
	}

	public void StartGame()
	{
		if (pressed) {
			return;
		}
		pressed = true;

		Sound.PlaySe("hajimari");

		StartCoroutine(CoStartGame());
	}

	public IEnumerator CoStartGame()
	{
		yield return new WaitForSeconds(0.8f);

		// ゲーム開始
		start = true;
	}
}
