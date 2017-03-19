using UnityEngine;
using System.Collections;

public class Pad
{
	public Vector2 start_;
	// タッチ開始位置
	public Vector2 prev_;
	// 前回タッチ位置
	public Vector2 latest_;
	// 最新タッチ位置
	public Vector2 vec;
	// 移動量
	public Vector2 totalvec;
	// 総移動量

	bool	push_;

	// プッシュON
	int touchId_;
	// タッチ番号

	// コンストラクタ
	public Pad()
	{
		touchId_ = -1;
	}

	/// 入力方向を取得する.
	public Vector2 GetVector()
	{
		// 右・左
		// 上・下
		float x = Input.GetAxisRaw("Horizontal");	// GamePad入力
		float y = Input.GetAxisRaw("Vertical");		// 	〃

		x += vec.x;									// タッチ入力を加算
		y += vec.y;									// 	〃

		// 移動する向きを求める
		return new Vector2(x, y);
	}

	/// タッチからの総移動量を取得する.
	public Vector2 GetTotalVector()
	{
		return totalvec;
	}

	/// タッチの現在位置を取得する.
	public Vector2 GetPosition()
	{
		return latest_;
	}

	/// PUSHを検出する
	public bool IsPushed()
	{
		return push_;
	}

	/// 戻るボタンを検出する
	public bool IsEscape()
	{
		return Input.GetKeyDown(KeyCode.Escape);
	}

	enum eTouchState
	{
		Idle,
		Began,
		Moving,
		Ended
	}

	eTouchState touchState = eTouchState.Idle;

	public void Update()
	{
		push_ = false;

		// タッチを検出
		foreach (var touch in Input.touches) {
			switch (touch.phase) {
			case TouchPhase.Began:
				if (touchId_ == -1) {
					touchId_ = touch.fingerId;
					latest_ = touch.position;
					touchState = eTouchState.Began;
				}
				break;

			case TouchPhase.Stationary:
			case TouchPhase.Moved:
				if (touch.fingerId == touchId_) {
					latest_ = touch.position;
					touchState = eTouchState.Moving;
				}
				break;
			
			case TouchPhase.Canceled:
			case TouchPhase.Ended:
				if (touch.fingerId == touchId_) {
					touchId_ = -1;
					latest_ = new Vector2(0, 0);
					touchState = eTouchState.Ended;
				}
				break;			
			}
		}
#if true
		if (touchId_ == -1) {
			// 左クリックを検出
			if (Input.GetMouseButtonDown(0)) {
				// マウスボタン押下
				latest_ = Input.mousePosition;
				touchState = eTouchState.Began;
			}
			else if (Input.GetMouseButton(0)) {
				// マウス押下中
				latest_ = Input.mousePosition;
				touchState = eTouchState.Moving;
			}
			else if (Input.GetMouseButtonUp(0)) {
				latest_ = new Vector2(0, 0);
				touchState = eTouchState.Ended;
			}

			// 右クリックやスペースキーでもPUSHを検出
			if (Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.Space)) {
				push_ = true;
			}
		}
#endif
		switch (touchState) {
		case eTouchState.Began:
			start_ = prev_ = latest_;
			push_ = true;

			Time.timeScale = 1.0f;
			break;

		case eTouchState.Moving:
			vec = Camera.main.ScreenToWorldPoint(latest_) - Camera.main.ScreenToWorldPoint(prev_);

			totalvec = latest_ - start_;
			prev_ = latest_;
			break;

		case eTouchState.Ended:
			start_ = prev_ = vec = totalvec = latest_;
			touchId_ = -1;

			Time.timeScale = 0.3f;
			touchState = eTouchState.Idle;
			break;			
		}
	}
}
