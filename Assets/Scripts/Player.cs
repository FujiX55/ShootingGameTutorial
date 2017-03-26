using UnityEngine;
using System.Collections;

public class Player : Actor
{
	public GameMgr gameMgr_;
	public Vector2 latest_;

	// 待機画像1
	public Sprite Spr0;
	// 待機画像2
	public Sprite Spr1;
	// 待機画像3
	public Sprite Spr2;

	/// 移動速度
	//  public float MoveSpeed = 5.0f;
	public float MoveSpeed = 0.3f;

	/// アニメーションタイマ
	int _tAnim = 0;

	ProximitySensor sensor;

	/// 開始
	void Start()
	{
		// 画面からはみ出ないようにする
		var w = SpriteWidth / 2;
		var h = SpriteHeight / 2;

		w -= w / 2;
		h -= h / 2;

		//	SetSize (w, h);
		SetSize(0, 0);

		sensor = gameObject.transform.GetChild(0).GetComponent<ProximitySensor>();
	}

	/// 固定フレームで更新
	void FixedUpdate()
	{
		_tAnim++;
		if (_tAnim % 48 < 24) {
			if (0 < sensor.Alert) {
				// 0～23フレームは「Spr2」
				SetSprite(Spr2);
			}
			else {
				// 0～23フレームは「Spr0」
				SetSprite(Spr0);
			}

			// X座標をランダムでずらす
			float px = X + Random.Range(0, SpriteWidth / 2);

			// 発射角度を    ±3する
			float dir = Random.Range(-3.0f, 3.0f);

			Shot.Add(px, Y, dir, 10);
		}
		else {
			if (0 < sensor.Alert) {
				// 24～48フレームは「Spr2」
				SetSprite(Spr2);
			}
			else {
				// 24～48フレームは「Spr1」
				SetSprite(Spr1);
			}
		}
		// 噴煙をパーティクルで表現
		Particle p = Particle.Add(X - 0.2f, Y - 0.5f);
		if (p) {
			p.SetVelocity(Random.Range(225, 270), 2.5f);  // 下後方に速度2.5で放射
			p.SortingLayer = "Default";
			p.SetColor(0.2f, 1.0f, 0.2f);
		}
	}

	/// 更新
	void Update()
	{
		// 移動量
		Vector2 v = gameMgr_.pad.GetVector();
		latest_ = v;

		// 移動して画面外に出ないようにする
//        ClampScreenAndMove (v * Time.deltaTime);
		X += v.x;
		Y += v.y;
		ClampScreen();
	}

	/// 衝突判定
	void OnTriggerEnter2D(Collider2D other)
	{
#if true
		string name = LayerMask.LayerToName(other.gameObject.layer);

		switch (name) {
//		case "Dummy":
//			if (SystemInfo.supportsVibration) {
//				Handheld.Vibrate();	// パーミッション設定のためのダミー呼び出し
//			}
//			break;
		case "Enemy":
		case "Bullet":
			// 振動してみたり
//			if (SystemInfo.supportsVibration) {
//				Handheld.Vibrate();
//			}
			if (GameMgr.Vibrate) {
				Vibration.Vibrate(100);
			}
			// ゲームオーバー
			Vanish();

			// パーティクル生成
			for (int i = 0; i < 8; i++) {
				Particle.Add(X, Y);
			}

            // やられSE再生
			Sound.PlaySe("damage");

            // BGMを止める
			Sound.StopBgm();
			break;
		}
#endif
	}
}
