using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class EndUI : MonoBehaviour
{
	public GameObject sprite;

	// Use this for initialization
	void Start()
	{
		StartCoroutine(FadeIn());	
	}
	
	// Update is called once per frame
	void Update()
	{
	
	}

	public void TransitTitle()
	{
		StartCoroutine(FadeToTitle());
	}

	IEnumerator FadeIn()
	{
		Color spriteColor = sprite.GetComponent<SpriteRenderer>().color;

		spriteColor.a = 0.0f;

		// フェードアウト
		while (spriteColor.a < 1) {
			spriteColor.a += Time.deltaTime;
			sprite.GetComponent<SpriteRenderer>().color = 
				new Color(spriteColor.r, spriteColor.g, spriteColor.b, spriteColor.a);
			yield return null;
		}
	}

	IEnumerator FadeToTitle()
	{
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
