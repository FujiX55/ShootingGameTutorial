using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class TitleMgr : MonoBehaviour
{
	Pad pad;

	/// Press to startの表示フラグ
	bool bDrawPressStart = false;

	void Awake()
	{
		pad = new Pad();
	}

	IEnumerator Start()
	{
		while (true) {
			// 0.6秒で点滅する
			bDrawPressStart = !bDrawPressStart;

			yield return new WaitForSeconds(0.6f);
		}
	}

	void Update()
	{
		pad.Update();

		if (pad.IsPushed()) {
			// Spaceキーを押したらゲーム開始
			SceneManager.LoadScene("Main");
		}
	}

	void OnGUI()
	{
		if (bDrawPressStart) {
			// ゲーム開始メッセージの描画
			// フォントサイズ設定
			Util.SetFontSize(32);

			// 中央揃え
			Util.SetFontAlignment(TextAnchor.MiddleCenter);

			// フォントの位置
			float w = 128;  // 幅
			float h = 32;   // 高さ
			float px = Screen.width / 2 - w / 2;
			float py = Screen.height / 2 - h / 2;

			// 少し下にずらす
			py += 65;

			// フォント描画
			Util.GUILabel(px, py, w, h, "タップでゲーム開始");
		}
	}
}
