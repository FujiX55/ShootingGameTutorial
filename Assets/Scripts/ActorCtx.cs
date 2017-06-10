using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// Actorコンテキスト
public class ActorCtx<Type> where Type : Actor
{
	int size_ = 0;

	GameObject prefab_ = null;
	List<Type> pool_ = null;

	/// Order in Layer
	int order_ = 0;

	/// ForEach関数に渡す関数の型
	public delegate void FuncT(Type t);

	/// コンストラクタ
	/// プレハブは必ず"Resources/Prefabs/"に配置すること
	public ActorCtx(string prefabName, int size = 0)
	{
		size_ = size;
		prefab_ = Resources.Load("Prefabs/" + prefabName) as GameObject;

		if (prefab_ == null) {
			Debug.LogError("Not found prefab. name=" + prefabName);
		}
		pool_ = new List<Type>();

		if (size > 0) {
			// サイズ指定があれば固定アロケーションとする
			for (int i = 0; i < size; i++) {
				GameObject g = GameObject.Instantiate(prefab_, new Vector3(), Quaternion.identity) as GameObject;
				Type obj = g.GetComponent<Type>();
				if (obj == null) {
					Debug.LogError(prefabName + "にスクリプトが未設定です");
				}
				obj.DiscardCannotOverride();
				pool_.Add(obj);
			}
		}
	}

	/// オブジェクトを再利用する
	Type Recycle_(Type obj, float x, float y, float direction, float speed)
	{
		// 復活
		obj.Activate();
		obj.SetPosition(x, y);

		// Rigidbody2Dがあるときのみ速度を設定する
		if (obj.RigidBody != null) {
			obj.SetVelocity(direction, speed);
		}
		obj.Angle = 0;

		// Order in Layerを設定してインクリメントする
		obj.Renderer.sortingOrder = order_++;

		return obj;
	}

	/// インスタンスを取得する
	public Type Add(float x, float y, float direction = 0.0f, float speed = 0.0f)
	{
		foreach (Type obj in pool_) {
			if (obj.Exists == false) {
				// 未使用のオブジェクトを見つけた
				return Recycle_(obj, x, y, direction, speed);
			}
		}

		if (size_ == 0) {
			// 自動で拡張
			GameObject g = GameObject.Instantiate(prefab_, new Vector3(), Quaternion.identity) as GameObject;
			Type obj = g.GetComponent<Type>();
			pool_.Add(obj);
			return Recycle_(obj, x, y, direction, speed);
		}
		return null;
	}

	/// 生存するインスタンスに対してラムダ式を実行する
	public void ForEachExist(FuncT func)
	{
		foreach (var obj in pool_) {
			if (obj.Exists) {
				func(obj);
			}
		}
	}

	/// 生存しているインスタンスをすべて破棄する
	public void Discard()
	{
		ForEachExist(t => t.Discard());
	}

	/// インスタンスの生存数を取得する
	public int Count()
	{
		int ret = 0;
		ForEachExist(t => ret++);

		return ret;
	}
}
