using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// ゲーム管理
/// </summary>
public class GameMgr : MonoBehaviour
{
	public Fade fade;

	// タッチ開始位置
	public Vector2 touchStart;
	// 最新タッチ位置
	public Vector2 touchLatest;

	/// 状態
	public enum eState
	{
		// 初期化
		Init,
		// メイン
		Main,
		// ゲームクリア
		GameClear,
		// つぎへ
		GoNext,
		// ゲームオーバー
		GameOver
	}

	/// 開始時は初期化状態にする
	static eState state = eState.Init;

	static public eState State {
		get { return state; }
	}

	// 設定取得
	//	public static bool Vibrate { set; get; }

	//	public static bool VibNear { set; get; }

	public Pad pad;

	public Text message, outline1, outline2, outline3, outline4, subtext;

	bool Paused { set; get; }

	float timeBgmPlay;

	/// 破壊
	void OnDestroy()
	{
		// ActorCtxの参照を消す
		Shot.parent = null;
		Enemy.parent = null;
		Bullet.parent = null;
		Particle.parent = null;
		Enemy.target = null;
	}

	/// 開始
	void Awake()
	{
		// パッド入力を取得
		pad = Pad.Instance;
		pad.Active = true;

		// ショットオブジェクトを32個確保しておく
		Shot.parent = new ActorCtx<Shot>("Shot", 32);

		// パーティクルオブジェクトを256個確保しておく
		Particle.parent = new ActorCtx<Particle>("Particle", 256);

		// 敵弾オブジェクトを256個確保しておく
		Bullet.parent = new ActorCtx<Bullet>("Bullet", 256);

		// 敵オブジェクトを64個確保しておく
		Enemy.parent = new ActorCtx<Enemy>("Enemy", 64);

		for (int i = 0; i < (int)eEnemyType.Count; i++) {
			Enemy.EnemyCount[i] = 0;
		}

		// プレイヤの参照を敵に登録する
		Enemy.target = GameObject.Find("Player").GetComponent<Player>();

		message.text 
			= outline1.text 
			= outline2.text 
			= outline3.text 
			= outline4.text 
			= subtext.text 
			= "";

		Time.timeScale = 1.0f;
	}

	void OnLevelWasLoaded(int level)
	{
		if (state != eState.Main) {
//			Destroy(gameObject);
			state = eState.Init;
		}
	}

	void Start()
	{
//		DontDestroyOnLoad(gameObject);

		MainUI.SetActive("Setting", false);
		MainUI.SetActive("Restart", false);
		MainUI.SetActive("Title", false);
		MainUI.SetActive("Next", false);

		Paused = false;
		pad.touch1st = false;
	}

	/// 更新
	void Update()
	{
		pad.Update();

		touchStart = pad.touchStart;	// タッチ開始位置
		touchLatest = pad.touchLatest;	// 最新タッチ位置

		// 戻るボタンで終了
		if (pad.IsEscape()) {
			Application.Quit();
			return;
		}

		switch (state) {
		case eState.Init:
			// BGM再生開始
//			Sound.PlayBgm("bgm");
			SoundMgr.PlayBgm("intro", "loop");

			pad.touch1st = false;

			// メイン状態へ遷移する
			state = eState.Main;
			break;
		   
		case eState.Main:
			if (Boss.bDestroyed) {
				// ボスを倒したのでゲームクリア
				// BGMを止める
				SoundMgr.StopBgm();
				// Jingle再生開始
				SoundMgr.PlayBgm("jingle", false);
				state = eState.GameClear;
			}
			else if (Enemy.target.Exists == false) {
					// プレイヤが死亡したのでゲームオーバー
					state = eState.GameOver;
				}
			break;

		default:
			break;
		}
	}

	/// ラベルを画面中央に表示
	void DrawLabelCenter(string message)
	{
		// フォントサイズ設定
		Util.SetFontSize(32);

		// 中央揃え
		Util.SetFontAlignment(TextAnchor.MiddleCenter);

		// フォントの位置
		float w = 128;	// 幅
		float h = 32;	// 高さ
		float px = Screen.width / 2 - w / 2;
		float py = Screen.height / 2 - h / 2;

		// フォント描画
		Util.GUILabel(px, py, w, h, message);
	}

	//
	void OnGUI()
	{
		switch (state) {
		case eState.Main:
			if (Time.timeScale == 0.0f) {
				message.text = "PAUSED\n<size=40><color=#ff0000>タップで再開</color></size>";
				outline1.text = outline2.text = outline3.text = outline4.text = "PAUSED\n<size=40>タップで再開</size>";
//				MainUI.SetActive("Restart", true);
//				MainUI.SetActive("Title", true);
				MainUI.SetActive("Setting", true);
				Paused = true;
			}
			else {
				if (Paused) {
					MainUI.SetActive("Setting", false);
					MainUI.SetActive("Restart", false);
					MainUI.SetActive("Title", false);
					Paused = false;
					message.text = outline1.text = outline2.text = outline3.text = outline4.text = "";
				}
			}
			break;
		case eState.GameClear:
			string msg = "STAGE CLEAR!";
			if (message.text != msg) {
				StartCoroutine(CoFadeToNext());
			}
			Paused = false;

			Time.timeScale = 1.0f;
			message.text = msg;
//			state = eState.GoNext;
			break;
		case eState.GoNext:
			break;
		case eState.GameOver:
			MainUI.SetActive("Restart", true);
			MainUI.SetActive("Title", true);
			MainUI.SetActive("Setting", true);
			Paused = true;

			Time.timeScale = 0.3f;
			message.text = "GAME OVER";
			break;
		}
	}

	public IEnumerator CoFadeToNext()
	{
		yield return new WaitForSeconds(1.8f);

		subtext.text = "CONGRATULATION";

		yield return new WaitForSeconds(3.2f);

		fade.FadeToNext();

		yield return new WaitForSeconds(0.5f);

		Sound.PlaySe("yattane");
	}

	//
	public static void SetVibrate(bool b)
	{
		PlayerPrefs.SetInt("Vibrate", b ? 1 : 0);
		PlayerPrefs.Save();
	}

	public static bool GetVibrate()
	{
		// ※セーブデータがない場合のデフォルト値=false
		return (PlayerPrefs.GetInt("Vibrate", 0) == 1);
	}

	//
	public static void SetVibNear(bool b)
	{
		PlayerPrefs.SetInt("VibNear", b ? 1 : 0);
		PlayerPrefs.Save();
	}

	public static bool GetVibNear()
	{
		// ※セーブデータがない場合のデフォルト値=false
		return (PlayerPrefs.GetInt("VibNear", 0) == 1);
	}

	//
	public static void SetDbgUndead(bool b)
	{
		PlayerPrefs.SetInt("DbgUndead", b ? 1 : 0);
		PlayerPrefs.Save();
	}

	public static bool GetDbgUndead()
	{
		// ※セーブデータがない場合のデフォルト値=false
		return (PlayerPrefs.GetInt("DbgUndead", 0) == 1);
	}
}
