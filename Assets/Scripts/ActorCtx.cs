﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// Actorコンテキスト
public class ActorCtx<Type> where Type : Actor
{
	int mySize = 0;

	GameObject myPrefab = null;
	List<Type> pool = null;

	/// Order in Layer
	int layerOrder = 0;

	/// ForEach関数に渡す関数の型
	public delegate void FuncT(Type t);

	/// コンストラクタ
	/// プレハブは必ず"Resources/Prefabs/"に配置すること
	public ActorCtx(string prefabName, int size = 0)
	{
		mySize = size;
		myPrefab = Resources.Load("Prefabs/" + prefabName) as GameObject;

		if (myPrefab == null) {
			Debug.LogError("Not found prefab. name=" + prefabName);
		}
		pool = new List<Type>();

		if (size > 0) {
			// サイズ指定があれば固定アロケーションとする
			for (int i = 0; i < size; i++) {
				GameObject g = GameObject.Instantiate(myPrefab, new Vector3(), Quaternion.identity) as GameObject;
				Type obj = g.GetComponent<Type>();
				if (obj == null) {
					Debug.LogError(prefabName + "にスクリプトが未設定です");
				}
				obj.DiscardCannotOverride();
				pool.Add(obj);
			}
		}
	}

	/// オブジェクトを再利用する
	Type RecycleT(Type obj, float x, float y, float direction, float speed)
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
		obj.Renderer.sortingOrder = layerOrder++;

		return obj;
	}

	/// インスタンスを取得する
	public Type Add(float x, float y, float direction = 0.0f, float speed = 0.0f)
	{
		foreach (Type obj in pool) {
			if (obj.Exists == false) {
				// 未使用のオブジェクトを見つけた
				return RecycleT(obj, x, y, direction, speed);
			}
		}

		if (mySize == 0) {
			// 自動で拡張
			GameObject g = GameObject.Instantiate(myPrefab, new Vector3(), Quaternion.identity) as GameObject;
			Type obj = g.GetComponent<Type>();
			pool.Add(obj);
			return RecycleT(obj, x, y, direction, speed);
		}
		return null;
	}

	/// 生存するインスタンスに対してラムダ式を実行する
	public void RunAll(FuncT func)
	{
		foreach (var obj in pool) {
			if (obj.Exists) {
				func(obj);
			}
		}
	}

	/// 生存しているインスタンスをすべて破棄する
	public void DiscardAll()
	{
		RunAll(t => t.Discard());
	}

	/// インスタンスの生存数を取得する
	public int Count()
	{
		int ret = 0;
		RunAll(t => ret++);

		return ret;
	}
}
