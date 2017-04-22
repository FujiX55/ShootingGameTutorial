using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Fade : MonoBehaviour
{
	float alpha = 0.0f;

	// Use this for initialization
	void Start()
	{

	}
	
	// Update is called once per frame
	void Update()
	{
		if (GameMgr.State != GameMgr.eState.GameClear) {
			if (Time.timeScale != 1.0f) {
				if (alpha < 0.5f) {
					alpha += 0.01f * (60 * Time.deltaTime);
				}
				GetComponent<Image>().color = new Color(0.0f, 0.0f, 0.0f, alpha);
			}
			else {
				GetComponent<Image>().color = new Color(0.0f, 0.0f, 0.0f, 0.0f);
				alpha = 0.0f;
			}
		}
	}

	public void FadeToNext()
	{
		StartCoroutine(CoFadeToNext());
	}

	public IEnumerator CoFadeToNext()
	{
		yield return new WaitForSeconds(1.0f);

		Sound.PlaySe("yattane");

		yield return new WaitForSeconds(2.0f);

		Color color = new Color(1.0f, 1.0f, 1.0f, 0.0f);

		color.a = 0.0f;

		// フェードアウト
		while (color.a < 1) {
			color.a += Time.deltaTime;
			GetComponent<Image>().color = 
				new Color(color.r, color.g, color.b, color.a);
			yield return null;
		}

		// 次のシーンへ
		SceneManager.LoadScene("End");
	}
}
