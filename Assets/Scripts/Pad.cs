using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class Pad
{
	private static Pad instance_;

	// タッチ開始位置
	public Vector2 start_;
	// 前回タッチ位置
	public Vector2 prev_;
	// 最新タッチ位置
	public Vector2 latest_;
	// 移動量
	public Vector2 vec;
	// 総移動量
	public Vector2 totalvec;

	// プッシュON
	bool push_;

	// タッチ番号
	int touchId_;

	bool active_ = true;

	public bool touch1st = false;

	public bool Active {
		set { active_ = value; }
		get { return active_; }
	}

	// コンストラクタ
	public Pad()
	{
		touchId_ = -1;
	}

	public static Pad Instance {
		get {
			if (instance_ == null) {
				instance_ = new Pad();
			}
			return instance_;
		}
	}

	void OnDestroy()
	{
		if (this == instance_) {
			instance_ = null;
		}
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
		Stay,
		Ended
	}

	eTouchState touchState = eTouchState.Idle;

	float brake = 0.01f;

	public void Update()
	{
		push_ = false;

		if (!active_) {
			return;
		}
//		if (IsPointerOverGameObject()) {
//			return;
//		}
		if (EventSystem.current.currentSelectedGameObject != null) {
			return;
		}

		// タッチ処理
		foreach (var touch in Input.touches) {
			switch (touch.phase) {
			case TouchPhase.Began:
				if (touchId_ == -1) {
					// タッチ開始
					touchId_ = touch.fingerId;
					latest_ = touch.position;
					touchState = eTouchState.Began;
				}
				break;

			case TouchPhase.Stationary:
			case TouchPhase.Moved:
				if (touch.fingerId == touchId_) {
					// タッチ継続中
					latest_ = touch.position;
					touchState = eTouchState.Stay;
				}
				break;
			
			case TouchPhase.Canceled:
			case TouchPhase.Ended:
				if (touch.fingerId == touchId_) {
					// タッチ終了
					touchId_ = -1;
					latest_ = new Vector2(0, 0);
					touchState = eTouchState.Ended;
				}
				break;			
			}
		}

		// マウス処理
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
					touchState = eTouchState.Stay;
				}
				else if (Input.GetMouseButtonUp(0)) {
						// マウスリリース
						latest_ = new Vector2(0, 0);
						touchState = eTouchState.Ended;
					}

			// 右クリックやスペースキーでもPUSHを検出
			if (Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.Space)) {
				push_ = true;
			}
		}

		// 共通処理
		switch (touchState) {
		case eTouchState.Began:
			start_ = prev_ = latest_;
			push_ = true;
			touch1st = true;
			// 等速再生
			Time.timeScale = 1.0f;
			break;

		case eTouchState.Stay:
			vec = Camera.main.ScreenToWorldPoint(latest_) - Camera.main.ScreenToWorldPoint(prev_);

			totalvec = latest_ - start_;
			prev_ = latest_;
			break;

		case eTouchState.Ended:
			start_ = prev_ = vec = totalvec = latest_;
			touchId_ = -1;
			// スロー再生
			Time.timeScale = 0.99f;
			// アイドル状態へ
			touchState = eTouchState.Idle;
			break;			
		}

		// 減速
		if (Time.timeScale != 1.0f) {
			if (Time.timeScale > 0.0f) {
				float tmScale = Time.timeScale;

				tmScale -= brake;

				if (0.0f > tmScale) {
					tmScale = 0.0f;
				}
				Time.timeScale = tmScale;
				brake += 0.0002f * (60 * Time.deltaTime * 4);
			}
		}
		else {
			brake = 0.01f;
		}
	}

	//-------------------------------------------------------------
	// EventSystemのGameObjectにマウスオーバーしているか？
	//-------------------------------------------------------------
	public bool IsPointerOverGameObject()
	{ 
		EventSystem current = EventSystem.current; 
		if (current != null) { 
			if (current.IsPointerOverGameObject()) { 
				return true; 
			} 
			 
			foreach (Touch t in Input.touches) { 
				if (current.IsPointerOverGameObject(t.fingerId)) { 
					return true; 
				} 
			} 
		} 
		return false; 
	}
}
