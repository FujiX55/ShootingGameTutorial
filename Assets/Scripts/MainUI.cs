using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class MainUI : MonoBehaviour
{
	static Canvas canvas;

	// Use this for initialization
	void Start()
	{
		canvas = GetComponent<Canvas>();
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

	public void TransitSetting()
	{
		// 設定画面へ
		SceneManager.LoadScene("Setting");
	}

	public void TransitTitle()
	{
		// タイトルへ戻る
		SceneManager.LoadScene("Title");
	}

	public void Restart()
	{
		// ゲームに戻る
		SceneManager.LoadScene("Main");
	}
}
