using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class EndUI : MonoBehaviour
{
	static Canvas canvas;

	public GameObject sprite;

	// Use this for initialization
	void Start()
	{
		canvas = GetComponent<Canvas>();

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
		Color spriteColor = sprite.GetComponent<SpriteRenderer>().color;

		spriteColor.a = 0.0f;

		// フェード処理
		while (spriteColor.a < 1) {
			spriteColor.a += Time.deltaTime;
			sprite.GetComponent<SpriteRenderer>().color = 
				new Color(spriteColor.r, spriteColor.g, spriteColor.b, spriteColor.a);
			yield return null;
		}
	}

	// フェイドしつつタイトルへ
	IEnumerator CoFadeToTitle()
	{
		SetActive("GoTitle", false);

		Color spriteColor = sprite.GetComponent<SpriteRenderer>().color;

		spriteColor.a = 1.0f;

		// フェードアウト
		while (spriteColor.a > 0) {
			spriteColor.a -= Time.deltaTime;
			sprite.GetComponent<SpriteRenderer>().color = 
				new Color(spriteColor.r, spriteColor.g, spriteColor.b, spriteColor.a);
			yield return null;
		}
		// タイトルへ戻る
		SceneManager.LoadScene("Title");
	}
}
