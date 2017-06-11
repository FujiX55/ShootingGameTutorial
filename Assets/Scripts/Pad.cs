using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class Pad
{
	private static Pad instance;

	// タッチ開始位置
	public Vector2 touchStart;
	// 前回タッチ位置
	public Vector2 touchPrev;
	// 最新タッチ位置
	public Vector2 touchLatest;
	// 移動量
	public Vector2 vec;
	// 総移動量
	public Vector2 vecTotal;

	// プッシュON
	bool push;

	// タッチ番号
	int idTouch;

	bool isActive = true;

	public bool isTouch1st = false;

	public bool Active {
		set { isActive = value; }
		get { return isActive; }
	}

	// コンストラクタ
	public Pad()
	{
		idTouch = -1;
	}

	public static Pad Instance {
		get {
			if (instance == null) {
				instance = new Pad();
			}
			return instance;
		}
	}

	void OnDestroy()
	{
		if (this == instance) {
			instance = null;
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
		return vecTotal;
	}

	/// タッチの現在位置を取得する.
	public Vector2 GetPosition()
	{
		return touchLatest;
	}

	/// PUSHを検出する
	public bool IsPushed()
	{
		return push;
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
		push = false;

		if (!isActive) {
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
				if (idTouch == -1) {
					// タッチ開始
					idTouch = touch.fingerId;
					touchLatest = touch.position;
					touchState = eTouchState.Began;
				}
				break;

			case TouchPhase.Stationary:
			case TouchPhase.Moved:
				if (touch.fingerId == idTouch) {
					// タッチ継続中
					touchLatest = touch.position;
					touchState = eTouchState.Stay;
				}
				break;
			
			case TouchPhase.Canceled:
			case TouchPhase.Ended:
				if (touch.fingerId == idTouch) {
					// タッチ終了
					idTouch = -1;
					touchLatest = new Vector2(0, 0);
					touchState = eTouchState.Ended;
				}
				break;			
			}
		}

		// マウス処理
		if (idTouch == -1) {
			// 左クリックを検出
			if (Input.GetMouseButtonDown(0)) {
				// マウスボタン押下
				touchLatest = Input.mousePosition;
				touchState = eTouchState.Began;
			}
			else if (Input.GetMouseButton(0)) {
					// マウス押下中
					touchLatest = Input.mousePosition;
					touchState = eTouchState.Stay;
				}
				else if (Input.GetMouseButtonUp(0)) {
						// マウスリリース
						touchLatest = new Vector2(0, 0);
						touchState = eTouchState.Ended;
					}

			// 右クリックやスペースキーでもPUSHを検出
			if (Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.Space)) {
				push = true;
			}
		}

		// 共通処理
		switch (touchState) {
		case eTouchState.Began:
			touchStart = touchPrev = touchLatest;
			push = true;
			isTouch1st = true;
			// 等速再生
			Time.timeScale = 1.0f;
			break;

		case eTouchState.Stay:
			vec = Camera.main.ScreenToWorldPoint(touchLatest) - Camera.main.ScreenToWorldPoint(touchPrev);

			vecTotal = touchLatest - touchStart;
			touchPrev = touchLatest;
			break;

		case eTouchState.Ended:
			touchStart = touchPrev = vec = vecTotal = touchLatest;
			idTouch = -1;
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
