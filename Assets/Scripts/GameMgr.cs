using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// ゲーム管理
/// </summary>
public class GameMgr : MonoBehaviour
{
	private static GameMgr instance_;

	public Fade fade;

	// タッチ開始位置
	public Vector2 start_;
	// 最新タッチ位置
	public Vector2 latest_;

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
	static eState state_ = eState.Init;

	static public eState State {
		get { return state_; }
	}

	// 設定取得
	//	public static bool Vibrate { set; get; }

	//	public static bool VibNear { set; get; }

	public Pad pad;

	public Text message, outline1, outline2, outline3, outline4, subtext;

	bool Paused { set; get; }

	//	public Texture2D blackTexture;

	static GameMgr Instance {
		get {
			if (instance_ == null) {
				GameObject obj = new GameObject("GameMgr");
				instance_ = obj.AddComponent<GameMgr>();
			}
			return instance_;
		}
	}

	/// 破壊
	void OnDestroy()
	{
		// ActorCtxの参照を消す
		Shot.parent = null;
		Enemy.parent = null;
		Bullet.parent = null;
		Particle.parent = null;
		Enemy.target = null;

		if (this == instance_) {
			instance_ = null;
		}
	}

	/// 開始
	void Awake()
	{
//		if (this != instance_) {
//			return;
//		}

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
		if (state_ != eState.Main) {
//			Destroy(gameObject);
			state_ = eState.Init;
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

		start_ = pad.start_;	// タッチ開始位置
		latest_ = pad.latest_;	// 最新タッチ位置

		// 戻るボタンで終了
		if (pad.IsEscape()) {
			Application.Quit();
			return;
		}

		switch (state_) {
		case eState.Init:
			// BGM再生開始
			Sound.PlayBgm("bgm");

			pad.touch1st = false;

			// メイン状態へ遷移する
			state_ = eState.Main;
			break;
		   
		case eState.Main:
			if (Boss.bDestroyed) {
				// ボスを倒したのでゲームクリア
				// BGMを止める
				Sound.StopBgm();
				// Jingle再生開始
				Sound.PlayBgm("jingle", false);
				state_ = eState.GameClear;
			}
			else if (Enemy.target.Exists == false) {
					// プレイヤが死亡したのでゲームオーバー
					state_ = eState.GameOver;
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
		switch (state_) {
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
//			state_ = eState.GoNext;
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
	public static void SetVibrate(bool state)
	{
		PlayerPrefs.SetInt("Vibrate", state ? 1 : 0);
		PlayerPrefs.Save();
	}

	public static bool GetVibrate()
	{
		// ※セーブデータがない場合のデフォルト値=false
		return (PlayerPrefs.GetInt("Vibrate", 0) == 1);
	}

	//
	public static void SetVibNear(bool state)
	{
		PlayerPrefs.SetInt("VibNear", state ? 1 : 0);
		PlayerPrefs.Save();
	}

	public static bool GetVibNear()
	{
		// ※セーブデータがない場合のデフォルト値=false
		return (PlayerPrefs.GetInt("VibNear", 0) == 1);
	}

	//
	public static void SetDbgUndead(bool state)
	{
		PlayerPrefs.SetInt("DbgUndead", state ? 1 : 0);
		PlayerPrefs.Save();
	}

	public static bool GetDbgUndead()
	{
		// ※セーブデータがない場合のデフォルト値=false
		return (PlayerPrefs.GetInt("DbgUndead", 0) == 1);
	}
}
