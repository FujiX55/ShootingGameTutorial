using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class EndUI : MonoBehaviour
{
	static Canvas canvas;
	static GameObject panel;

	// Use this for initialization
	void Start()
	{
		canvas = GetComponent<Canvas>();
		panel = GameObject.Find("Panel");

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
	}

	// フェイドしつつタイトルへ
	IEnumerator CoFadeToTitle()
	{
		SetActive("GoTitle", false);

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
