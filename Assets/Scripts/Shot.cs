using UnityEngine;
using System.Collections;

public class Shot : Actor
{
	/// 親オブジェクト
	public static ActorCtx<Shot> parent = null;

	/// インスタンスの取得
	public static Shot Add(float x, float y, float direction, float speed)
	{
		Shot shot = parent.Add(x, y, direction, speed);

		if (shot != null) {
			shot.Angle = Random.Range(0, 359);
		}

		return shot;
	}

	/// 更新
	void Update()
	{
		if (IsOutside()) {
			// 画面外に出たので消す
			//this.DestroyObj();
			this.Discard();
		}
	}

	/// 消滅
	public override void Discard()
	{
		// パーティクル生成
		Particle p = Particle.Add(X, Y);

		if (p != null) {
			// 青色にする
			p.SetColor(0.1f, 0.1f, 1);

			// 速度を少し遅くする
			p.MulVelocity(0.7f);
		}
		base.Discard();
	}
}
