using UnityEngine;
using System.Collections;

/// <summary>
/// 敵弾
/// </summary>
public class Bullet : Token {

	/// 敵弾処理
	public static TokenMgr<Bullet> parent = null;

	/// 敵弾の取得
	public static Bullet Add(float x, float y, float direction, float speed)
	{
		return parent.Add(x, y, direction, speed);
	}

	/// 更新
	void Update()
	{
		if ( IsOutside() )
		{
			// 画面外に出たら消える
			Vanish();
		}
	}
}
