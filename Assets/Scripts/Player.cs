using UnityEngine;
using System.Collections;

public class Player : Actor
{
	public GameMgr gameMgr_;
	public Vector2 latest_;

	// 画像
	public Sprite Spr0;
	public Sprite Spr1;
	public Sprite Spr2;

	/// アニメーションタイマ
	int _tAnim = 0;

	// 近接センサ
	ProximitySensor sensor;

	/// 開始
	void Start()
	{
		// 画面からはみ出ないようにする
		var w = this.Renderer.bounds.size.x / 2;
		var h = this.Renderer.bounds.size.y / 2;

		w -= w / 2;
		h -= h / 2;

		//	SetSize (w, h);
		SetSize(0, 0);

		sensor = gameObject.transform.GetChild(0).GetComponent<ProximitySensor>();
	}

	/// 固定フレームで更新
	void FixedUpdate()
	{
		bool bShoot = false;
		bool panic = false;
		_tAnim++;
		
		if (_tAnim % 48 < 24) {
			// 0～23フレーム
			if (0 < sensor.Alert) {
				// 近接センサ反応中は「Spr2」を表示
				this.Renderer.sprite = Spr2;
				panic = true;
			}
			else {
				// 通常は「Spr0」を表示
				this.Renderer.sprite = Spr0;
			}
			// 0～23フレームでは必ずショットを撃つ
			bShoot = true;
		}
		else {
			// 24～48フレーム
			if (0 < sensor.Alert) {
				// 近接センサ反応中は「Spr2」を表示
				this.Renderer.sprite = Spr2;
				
				// 近接センサ反応中はパニックショットを発射する
//				bShoot = true;
				panic = true;
			}
			else {
				// 通常は「Spr1」を表示
				this.Renderer.sprite = Spr1;
			}
		}
		// ショットを発射
		if (bShoot) {
			// X座標をランダムでずらす
			float px = X + Random.Range(0, this.Renderer.bounds.size.x / 2);

			// 発射角度を    ±3する
			float angle = 3.0f;
#if false
			if (panic) {
				// パニックショット
				angle = 80.0f;
			}
			else
			{
#if false
				// 位置に応じて角度を変化させる(-1.5より左ではレンジが開く)
				angle *= (1.0f + (-5 * (X + 1.5f)));
				if (angle < 3.0f) {
					angle = 3.0f;
				}
				else if (angle > 60.0f){
					angle = 60.0f;
				}
#endif
			}
#endif			

			float dir = Random.Range(-angle, angle);

			Shot.Add(px, Y, dir, 10);
		}
		// パニックショット
//		else
		if (panic && (_tAnim % 2 == 0)) {
			// X座標をランダムでずらす
			float px = X + Random.Range(0, this.Renderer.bounds.size.x / 2);

			float angle = 80.0f;

			float dir = Random.Range(-angle, angle);

			Shot.Add(px, Y, dir, 10);
		}

		// 噴煙をパーティクルで表現
		Particle p = Particle.Add(X - 0.2f, Y - 0.5f);
		if (p) {
			p.SetVelocity(Random.Range(225, 270), 2.5f);  // 下後方に速度2.5で放射
			p.Renderer.sortingLayerName = "Default";
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

		//	Angle = v.x * -300;

		ClampScreen();
	}

	/// 衝突判定
	void OnTriggerEnter2D(Collider2D other)
	{
		if (GameMgr.GetDbgUndead()) {
			return;
		}
#if true
		string name = LayerMask.LayerToName(other.gameObject.layer);

		switch (name) {
		case "Enemy":
		case "Bullet":
			// 振動してみたり
			if (PlayerPrefs.GetInt("Vibrate") == 1) {
				Vibration.Vibrate(100);
			}
			// ゲームオーバー
			Discard();

			// パーティクル生成
			for (int i = 0; i < 8; i++) {
				Particle.Add(X, Y);
			}

            // やられSE再生
//			Sound.PlaySe("ann", 0);
			Sound.PlaySe("damage");

            // BGMを止める
//			Sound.StopBgm();
			SoundMgr.StopBgm();
			break;
		}
#endif
	}

	/// 座標の描画
	void OnGUI()
	{
		string text;

		// テキストを黒にする
		Util.SetFontColor(Color.black);

		// テキストを大きめにする
		Util.SetFontSize(24);

		// 中央揃えにする
		Util.SetFontAlignment(TextAnchor.MiddleCenter);

		text = string.Format("player.x={0}", X);
		Util.GUILabel(0, Screen.height / 20 * 5, 120, 30, text);

		text = string.Format("player.y={0}", Y);
		Util.GUILabel(0, Screen.height / 20 * 6, 120, 30, text);
	}
}
