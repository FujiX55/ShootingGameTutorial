using UnityEngine;
using System.Collections;

public class Player : Token {
    public GameMgr gameMgr_;
    public Vector2 latest_;

	public Sprite Spr0;	// 待機画像1
	public Sprite Spr1;	// 待機画像2

	/// 移動速度
    //  public float MoveSpeed = 5.0f;
    public float MoveSpeed = 0.3f;

	/// アニメーションタイマ
	int _tAnim = 0;

	/// 開始
	void Start()
	{
		// 画面からはみ出ないようにする
		var w = SpriteWidth / 2;
		var h = SpriteHeight / 2;

        w -= w / 2;
        h -= h / 2;

	//	SetSize (w, h);
        SetSize (0, 0);

        Angle = -5;
	}

	/// 固定フレームで更新
	void FixedUpdate()
	{
		_tAnim++;
		if (_tAnim % 48 < 24) {
			// 0～23フレームは「Spr0」
			SetSprite (Spr0);
            {
                // X座標をランダムでずらす
                float px = X + Random.Range(0, SpriteWidth / 2);

                // 発射角度を    ±3する
                float dir = Random.Range(-3.0f, 3.0f);

                Shot.Add(px, Y, dir, 10);
            }
		} else {
			// 24～48フレームは「Spr1」
			SetSprite(Spr1);
		}
	}

	/// 更新
	void Update()
	{
		// 移動速度
		Vector2 v = gameMgr_.pad.GetVector();
//		float speed = MoveSpeed * Time.deltaTime;

		// 移動して画面外に出ないようにする
//      ClampScreenAndMove (v * speed);
        ClampScreenAndMove (v * Time.deltaTime);

	    Vector2 pos = transform.position;
		latest_ = Camera.main.WorldToScreenPoint(pos);
	}

	/// 衝突判定
	void OnTriggerEnter2D(Collider2D other)
	{
		string name = LayerMask.LayerToName (other.gameObject.layer);
#if true
		switch (name)
		{
		case "Enemy":
		case "Bullet":
			// ゲームオーバー
			Vanish();

			// パーティクル生成
			for (int i = 0; i < 8; i++)
			{
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
