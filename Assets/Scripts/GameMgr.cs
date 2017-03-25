using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

/// <summary>
/// ゲーム管理
/// </summary>
public class GameMgr : MonoBehaviour
{
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
	eState state_ = eState.Init;

	public eState State {
		get { return state_; }
	}

	public Pad pad;

	//	public Texture2D blackTexture;

	/// 開始
	void Awake()
	{
		pad = new Pad();

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

		Time.timeScale = 1.0f;

//		if (SystemInfo.supportsVibration) {
//			VibrateScript.Initialize();
//		}
	}

	/// 更新
	void Update()
	{
		pad.Update();
		start_ = pad.start_;	// タッチ開始位置
		latest_ = pad.latest_;	// 最新タッチ位置

		// 戻るボタンで終了
		if (pad.IsEscape()) {
//			if (SystemInfo.supportsVibration) {
//				VibrateScript.Destruct();
//			}
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
			if (pad.IsPushed()) {
				// タイトルへ戻る
				SceneManager.LoadScene("Title");
			}
			break;

		case eState.GameOver:
			if (pad.IsPushed()) {
				// ゲームをやり直す
				SceneManager.LoadScene("Main");
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
				DrawLabelCenter("PAUSED");
			}
			break;
		case eState.GameClear:
			Time.timeScale = 1.0f;
			DrawLabelCenter("GAME CLEAR!");
			break;
		case eState.GameOver:
			Time.timeScale = 0.3f;
			DrawLabelCenter("GAME OVER");
			break;
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
	}
}
