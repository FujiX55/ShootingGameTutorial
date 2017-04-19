using UnityEngine;
using System.Collections;

/// <summary>
/// ボス
/// </summary>
public class Boss : Enemy
{
	/// ボスを倒したフラグ
	public static bool bDestroyed = false;

	/// コルーチンを開始したかどうか
	bool _bStart = false;

	/// 開始
	void Start()
	{
		// パラメータを設定
		SetParam((int)eEnemyType.Boss);

		// ボスを倒したフラグ初期化
		bDestroyed = false;
	}

	/// 消滅
	public override void Discard()
	{
		// ボスを倒した
		bDestroyed = true;

		base.Discard();
	}

	/// 更新
	void Update()
	{
		if (_bStart == false) {
			// 敵生成開始
			StartCoroutine("CoGenerateEnemy");

			_bStart = true;
		}
	}

	/// 敵生成
	IEnumerator CoGenerateEnemy()
	{
		while (true) {
			AddEnemy(1, 135, 5);
			AddEnemy(1, 225, 5);
			yield return new WaitForSeconds(3);
			BuildPencil();
			yield return new WaitForSeconds(2);

			AddEnemy(2, 90, 5);
			AddEnemy(2, 270, 5);
			BuildPeppy();
			yield return new WaitForSeconds(3);

			AddEnemy(3, 60, 5);
			AddEnemy(3, -60, 5);
			yield return new WaitForSeconds(3);
			BuildPencil();
			yield return new WaitForSeconds(2);
		}
	}

	/// 敵の生成
	Enemy AddEnemy(int id, float direction, float speed)
	{
		return Enemy.Add(id, X, Y, direction, speed);
	}

	/// ペンシルを3方向に発射
	void BuildPencil()
	{
		// プレイヤと±30度にペンシルを発射
		float aim = GetAim();

		AddEnemy(4, aim, 1);
		AddEnemy(4, aim - 30, 1);
		AddEnemy(4, aim + 30, 1);
	}

	/// Mr.Peppyを発射
	void BuildPeppy()
	{
		AddEnemy(5, 30, 3);
		AddEnemy(5, -30, 3);
	}

	/// HPの描画
	void OnGUI()
	{
		// テキストを黒にする
		Util.SetFontColor(Color.black);

		// テキストを大きめにする
		Util.SetFontSize(24);

		// 中央揃えにする
		Util.SetFontAlignment(TextAnchor.MiddleCenter);

		// テキスト描画
		string text = string.Format("{0,3}", Hp);

		Vector2 pos = Camera.main.WorldToScreenPoint(transform.position);
		Util.GUILabel(pos.x - 60, pos.y, 120, 30, text);
	}
}
