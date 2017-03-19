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
		Pressing,
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
					start_ = latest_ = prev_ = touch.position;
					push_ = true;

					Time.timeScale = 1.0f;
				}
				break;

			case TouchPhase.Stationary:
			case TouchPhase.Moved:
				if (touch.fingerId == touchId_) {
					latest_ = touch.position;
//                  vec = latest_ - prev_;
					vec = Camera.main.ScreenToWorldPoint(latest_) - Camera.main.ScreenToWorldPoint(prev_);

					totalvec = latest_ - start_;
					prev_ = latest_;
				}
				break;
			
			case TouchPhase.Canceled:
			case TouchPhase.Ended:
				if (touch.fingerId == touchId_) {
					start_ = latest_ = prev_ = vec = totalvec = new Vector2(0, 0);
					touchId_ = -1;

					Time.timeScale = 0.3f;
				}
				break;			
			}
		}
#if true
		if (touchId_ == -1) {
			// 左クリックを検出
			if (Input.GetMouseButtonDown(0)) {
				// マウスボタン押下
				start_ = latest_ = prev_ = Input.mousePosition;
				push_ = true;

				Time.timeScale = 1.0f;
			}
			else if (Input.GetMouseButton(0)) {
				// マウス押下中
				latest_ = Input.mousePosition;
//                vec = latest_ - prev_;
				vec = Camera.main.ScreenToWorldPoint(latest_) - Camera.main.ScreenToWorldPoint(prev_);

				totalvec = latest_ - start_;
				prev_ = latest_;
			}
			else if (Input.GetMouseButtonUp(0)) {
				start_ = latest_ = prev_ = vec = totalvec = new Vector2(0, 0);

				Time.timeScale = 0.3f;
			}

			// 右クリックやスペースキーでもPUSHを検出
			if (Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.Space)) {
				push_ = true;
			}
		}
#endif
	}
}
