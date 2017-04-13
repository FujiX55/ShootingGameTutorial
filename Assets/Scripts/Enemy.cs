using UnityEngine;
using System.Collections;

public enum eEnemyType : long
{
	Boss,
	// ボス
	Bon,
	// 覆水盆
	Metsu,
	// 滅
	Fugu,
	// 河豚
	Pencil,
	// ペンシルロケット
	MrPeppy,
	// ミスターペッピー
	Count
	// 総数
}

/// <summary>
/// 敵
/// </summary>
public class Enemy : Actor
{
	static int[] enemyCount = new int[(int)eEnemyType.Count];

	static public int[] EnemyCount {
		get { return Enemy.enemyCount; }
	}

	/// スプライト
	public Sprite Spr0;
	public Sprite Spr1;
	public Sprite Spr2;
	public Sprite Spr3;
	public Sprite Spr4;
	public Sprite Spr5;

	/// 敵のID
	int id_ = (int)eEnemyType.Boss;

	/// HP
	int hp_ = 0;

	/// HPの取得
	public int Hp {
		get { return hp_; }
	}

	// 敵管理
	public static ActorMgr<Enemy> parent = null;

	public int phase_;

	// 敵の追加
	public static Enemy Add(int id, float x, float y, float direction, float speed)
	{
		if (4 <= Enemy.enemyCount[id]) {
			return null;
		}

		Enemy e = parent.Add(x, y, direction, speed);

		e.phase_ = 0;

		if (e == null) {
			return null;
		}

		Enemy.enemyCount[id]++;
		
		// 初期パラメータ設定
		e.SetParam(id);

		return e;
	}

	/// 狙い撃ちするターゲット
	public static Player target = null;

	/// 狙い撃ち角度を取得する
	public float GetAim()
	{
		float dx = target.X - X;
		float dy = target.Y - Y;

		return Mathf.Atan2(dy, dx) * Mathf.Rad2Deg;
	}

	/// ダメージを与える
	bool Damage(int v)
	{
		hp_ -= v;
        
		if (hp_ <= 0) {
			// HPがなくなったので死亡
			Vanish();
            
			// 倒した
			for (int i = 0; i < 4; i++) {
				Particle.Add(X, Y);
			}
            
			// 破壊SE再生
			Sound.PlaySe("destroy", 0);

			// ボスを倒したらザコ敵と敵弾を消す
			if ((eEnemyType)id_ == eEnemyType.Boss) {
				// 生存しているザコ敵を消す
				Enemy.parent.ForEachExist(e => e.Damage(9999));

				// 敵弾をすべて消す
				if (Bullet.parent != null) {
					Bullet.parent.Vanish();
				}
			}
			return true;
		}
        
		// まだ生きている
		return false;
	}

	/// 更新
	IEnumerator _Update1()
	{
		while (true) {
			// 2秒おきに弾を撃つ
			yield return new WaitForSeconds(2.0f);
			
			// 狙い撃ち角度を取得
//			float dir = GetAim();
			float dx = target.X - X - 0.5f;
			float dy = target.Y - Y;

			float dir = Mathf.Atan2(dy, dx) * Mathf.Rad2Deg;

			Bullet.Add(X, Y, dir, 3);

			// 画面外に出たら消える
			if (IsOutside()) {
				Vanish();
			}
		}
	}

	IEnumerator _Update2()
	{
		// 発射角度を回転しながら撃つ
		yield return new WaitForSeconds(2.0f);

		float dir = 0;

		while (true) {
			Bullet.Add(X, Y, dir, 2);

			Angle = dir;

			dir += 16;

			yield return new WaitForSeconds(0.1f);

			// 画面外に出たら消える
			if (IsOutside()) {
				Vanish();
			}
		}
	}

	IEnumerator _Update3()
	{
		// 3Way弾を撃つ
		while (true) {
			// 2秒おきに弾を撃つ
			yield return new WaitForSeconds(2.0f);
			
			// 画面外に出たら消える
			if (IsOutside()) {
				Vanish();
			}
			else {
				DoBullet(180 - 2, 2);
				DoBullet(180, 2);
				DoBullet(180 + 2, 2);
			}
		}
	}

	IEnumerator _Update4()
	{
		while (true) {
			// まっすぐ飛ぶ
			yield return new WaitForSeconds(0.05f);

			if (RigidBody.velocity.magnitude < 3.0f) {
				MulVelocity(1.05f);
			}

			// 画面外に出たら消える
			if (IsOutside()) {
				Vanish();
			}
			else {
				// 噴煙をパーティクルで表現
				Particle p = Particle.Add(X, Y);
				if (p) {
					p.SetVelocity(Random.Range(0, 359), 0.0f);  // 全方向に速度0.3で放射
					p.Visible = false;                          // 非表示で開始
					p.Delay = 5;                                // 可視状態までの遅延フレーム数
					//              p.SetColor(1.0f, 0.2f, 0.0f);
					p.RigidBody.gravityScale = -0.1f;
				}
			}
		}
	}

	IEnumerator _Update5()
	{
		// 1回の更新で旋回する角度
		const float ROT = 5.0f;

		// ホーミングする
		while (true) {
			// 0.02秒おきに更新する
			yield return new WaitForSeconds(0.02f);

			// 現在の角度
			float dir = Direction;

			// 狙い撃ち角度
			float aim = GetAim();

			// 角度差を求める
			float delta = Mathf.DeltaAngle(dir, aim);

			if (Mathf.Abs(delta) < ROT) {
				// 角度差が小さいので回転不要
			}
			else if (delta > 0) {
				// 左回り
				dir += ROT;
			}
			else {
				// 右回り
				dir -= ROT;
			}
			SetVelocity(dir, Speed);

			// 画像も回転させる
			Angle = dir;

			// 画面外に出たら消える
			if (IsOutside()) {
				Vanish();
			}
			else {
				// 噴煙をパーティクルで表現
				Particle p = Particle.Add(X, Y);
				if (p) {
					p.SetVelocity(Random.Range(0, 359), 0.3f);  // 全方向に速度0.3で放射
					p.Visible = false;                          // 非表示で開始
					p.Delay = 5;                                // 可視状態までの遅延フレーム数
					p.SetColor(1.0f, 0.2f, 0.0f);
				}
			}
		}
	}

	/// 衝突判定
	void OnTriggerEnter2D(Collider2D other)
	{
		// Layer名を取得する
		string name = LayerMask.LayerToName(other.gameObject.layer);

		if (name == "Shot") {
			// ショットであれば当たりとする
			Shot s = other.GetComponent<Shot>();

			// ショットを消す
			s.Vanish();

			// ダメージ処理
			Damage(1);
		}
	}

	/// 弾を発射する
	void DoBullet(float direction, float speed)
	{
		Bullet.Add(X, Y, direction, speed);
	}

	/// IDからパラメータを設定
	public void SetParam(int id)
	{
		if (id_ != (int)eEnemyType.Boss) {
			// 前回のコルーチンを終了する
			StopCoroutine("_Update" + id_);
		}

		if (id != (int)eEnemyType.Boss) {
			// コルーチンを新しく開始する
			StartCoroutine("_Update" + id);
		}

		// IDを設定
		id_ = id;

		//              0,  1,  2,  3,  4,  5
		// HPテーブル
#if UNITY_EDITOR
		int[] hps = { 100, 30, 30, 30, 30, 10 };
#else
		int[] hps = { 500, 30, 30, 30, 30, 10 };
#endif

		// スプライトテーブル
		Sprite[] sprs = { Spr0, Spr1, Spr2, Spr3, Spr4, Spr5 };

		// HPを設定
		hp_ = hps[id];

		// スプライトを設定
		SetSprite(sprs[id]);

		// サイズ変更
		Scale = 0.5f;
	}

	/// 固定フレームで更新
	void FixedUpdate()
	{
		if ((eEnemyType)id_ <= eEnemyType.Fugu && (eEnemyType)id_ != eEnemyType.Boss) {
			switch (phase_) {
			case 0:
				// 通常の敵だけ移動速度を減衰する
				MulVelocity(0.93f);
				if (0.01 > RigidBody.velocity.magnitude) {
					phase_ = 1;
				}
				break;
			case 1:
				SetVelocity(180, 0.1f);
				phase_ = 2;
				break;
			default:
				MulVelocity(1.005f);
				break;
			}
		}
	}

	/// 更新
	void Update()
	{
		if ((eEnemyType)id_ == eEnemyType.Pencil) {
			// ペンシルロケットのみ
			Vector2 min = GetWorldMin();
			Vector2 max = GetWorldMax();

			if (Y < min.y || max.y < Y) {
				// 上下ではみ出したら跳ね返るようにする
//				ClampScreen();

				// 移動速度を反転
//				VY *= -1;
			}
			if (X < min.x || max.x < X) {
				// 左右ではみ出したら消滅する
				Vanish();
			}

			// 移動方向を向くようにする
			Angle = Direction;
		}
	}

	public override void Vanish()
	{
		if (0 < Enemy.enemyCount[id_]) {
			Enemy.enemyCount[id_]--;
		}
		
		VanishCannotOverride();
	}
}
