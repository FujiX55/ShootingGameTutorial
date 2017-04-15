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

	public Text message, outline1, outline2, outline3, outline4;

	bool ButtonActive { set; get; }

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
		// ActorMgrの参照を消す
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

		// ショットオブジェクトを32個確保しておく
		Shot.parent = new ActorMgr<Shot>("Shot", 32);

		// パーティクルオブジェクトを256個確保しておく
		Particle.parent = new ActorMgr<Particle>("Particle", 256);

		// 敵弾オブジェクトを256個確保しておく
		Bullet.parent = new ActorMgr<Bullet>("Bullet", 256);

		// 敵オブジェクトを64個確保しておく
		Enemy.parent = new ActorMgr<Enemy>("Enemy", 64);

		for (int i = 0; i < (int)eEnemyType.Count; i++) {
			Enemy.EnemyCount[i] = 0;
		}

		// プレイヤの参照を敵に登録する
		Enemy.target = GameObject.Find("Player").GetComponent<Player>();

		message.text = outline1.text = outline2.text = outline3.text = outline4.text = "";

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

		ButtonActive = false;
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

			// メイン状態へ遷移する
			state_ = eState.Main;
			break;
		   
		case eState.Main:
			if (Boss.bDestroyed) {
				// ボスを倒したのでゲームクリア
				// BGMを止める
				Sound.StopBgm();
				state_ = eState.GameClear;
			}
			else if (Enemy.target.Exists == false) {
				// プレイヤが死亡したのでゲームオーバー
				state_ = eState.GameOver;
			}
			break;

		case eState.GameClear:
			break;

		case eState.GameOver:
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
//				DrawLabelCenter("PAUSED");
				message.text = "PAUSED\n<size=40><color=#ff0000>タップで再開</color></size>";
				outline1.text = outline2.text = outline3.text = outline4.text = "PAUSED\n<size=40>タップで再開</size>";
//				MainUI.SetActive("Restart", true);
//				MainUI.SetActive("Title", true);
				MainUI.SetActive("Setting", true);
				ButtonActive = true;
			}
			else {
				if (ButtonActive) {
					MainUI.SetActive("Setting", false);
					MainUI.SetActive("Restart", false);
					MainUI.SetActive("Title", false);
					ButtonActive = false;
					message.text = outline1.text = outline2.text = outline3.text = outline4.text = "";
				}
			}
			break;
		case eState.GameClear:
//			MainUI.SetActive("Restart", true);
//			MainUI.SetActive("Title", true);
//			MainUI.SetActive("Setting", true);
//			MainUI.SetActive("Next", true);
//			StartCoroutine(GoNext());
			string msg = "STAGE CLEAR!";
			if (message.text != msg) {
				fade.FadeToNext();
			}
			ButtonActive = false;

			Time.timeScale = 1.0f;
//			DrawLabelCenter("GAME CLEAR!");
			message.text = msg;
			break;
		case eState.GameOver:
			MainUI.SetActive("Restart", true);
			MainUI.SetActive("Title", true);
			MainUI.SetActive("Setting", true);
			ButtonActive = true;

			Time.timeScale = 0.3f;
//			DrawLabelCenter("GAME OVER");
			message.text = "GAME OVER";
			break;
		}
	}

	IEnumerator GoNext()
	{
		yield return new WaitForSeconds(3.0f);

		// 次のシーンへ
		SceneManager.LoadScene("End");
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
