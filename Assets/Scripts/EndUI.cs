﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class EndUI : MonoBehaviour
{
	bool pressed = false;
	static Canvas canvas;
	static GameObject panel;

	Pad pad;

	// Use this for initialization
	void Start()
	{
		Time.timeScale = 1.0f;

		canvas = GetComponent<Canvas>();
		panel = GameObject.Find("Panel");
		pad = Pad.Instance;
		pad.Active = false;

		SetActive("Baloon", false);

		StartCoroutine(FadeIn());	
	}

	public static void SetActive(string name, bool b)
	{
		foreach (Transform child in canvas.transform) {
			// 子の要素をたどる
			if (child.name == name) {
				// 指定した名前と一致
				// 表示フラグを設定
				child.gameObject.SetActive(b);
				// おしまい
				return;
			}
		}
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
	}

	// タイトルへ戻る
	public void TransitTitle()
	{
		StartCoroutine(CoFadeToTitle());
	}

	IEnumerator FadeIn()
	{
		float alpha = 1.0f;

		// フェード処理
		while (alpha > 0.0f) {
			alpha -= Time.deltaTime;
			panel.GetComponent<Image>().color = new Color(1.0f, 1.0f, 1.0f, alpha);
			yield return null;
		}
		SetActive("Baloon", true);
		if (!pressed) {
			Sound.PlaySe("oshimai", 0);
		}
	}

	// フェイドしつつタイトルへ
	IEnumerator CoFadeToTitle()
	{
		SetActive("GoTitle", false);
		pressed = true;

		Sound.PlaySe("hai", 0);

		float alpha = 0.0f;

		// フェード処理
		while (alpha < 1.0f) {
			alpha += Time.deltaTime;
			panel.GetComponent<Image>().color = new Color(1.0f, 1.0f, 1.0f, alpha);
			yield return null;
		}

		// タイトルへ戻る
		SceneManager.LoadScene("Title");
	}
}
