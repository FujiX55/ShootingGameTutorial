using UnityEngine;
using System.Collections;

public class Pad {

	public Vector2 start_;	// タッチ開始位置
	public Vector2 latest_; // 最新タッチ位置
	public Vector2 vec;		// 移動量

	bool	push_;			// プッシュON
	int		touchId_;		// タッチ番号

	// コンストラクタ
	public Pad()
	{
		touchId_ = -1;
	}

	/// 入力方向を取得する.
	public Vector2 GetVector() {
		// 右・左
		// 上・下
		float x = Input.GetAxisRaw("Horizontal");	// GamePad入力
		float y = Input.GetAxisRaw("Vertical");		// 	〃

		x += vec.x;									// タッチ入力を加算
		y += vec.y;									// 	〃

		// 移動する向きを求める
		return new Vector2(x, y);
	}

	/// PUSHを検出する
	public bool IsPushed() {
		return push_;
	}

	/// 戻るボタンを検出する
	public bool IsEscape() {
		return Input.GetKeyDown( KeyCode.Escape );
	}

	public void Update()
	{
		push_ = false;

		// タッチを検出
		foreach (var touch in Input.touches) {
			switch ( touch.phase )
			{
			case TouchPhase.Began:
				if ( touchId_ == -1 ) {
					touchId_ = touch.fingerId;
//					latest_ = start_ = Camera.main.ScreenToWorldPoint(touch.position);
//					latest_ = start_ = Camera.main.WorldToScreenPoint(touch.position);
					latest_ = start_ = touch.position;
					push_ = true;
				}
				break;

			case TouchPhase.Stationary:
			case TouchPhase.Moved:
				if ( touch.fingerId == touchId_ ) {
//					latest_ = Camera.main.ScreenToWorldPoint(touch.position);
//					latest_ = Camera.main.WorldToScreenPoint(touch.position);
					latest_ = touch.position;
					vec = latest_ - start_;
					start_ = latest_;
				}
				break;
			
			case TouchPhase.Canceled:
			case TouchPhase.Ended:
				if ( touch.fingerId == touchId_ ) {
					latest_ = start_ = vec = new Vector2(0,0);
					touchId_ = -1;
				}
				break;			
			}
		}
#if true
        if ( touchId_ == -1 ) {
            // 左クリックを検出
            if ( Input.GetMouseButtonDown(0) ) {
                // マウスボタン押下
                latest_ = start_ = Input.mousePosition;
                push_ = true;
            }
            else if ( Input.GetMouseButton(0) ) {
                // マウス押下中
                latest_ = Input.mousePosition;
                vec = latest_ - start_;
                start_ = latest_;
            }
            else if ( Input.GetMouseButtonUp(0) ) {
                latest_ = start_ = vec = new Vector2(0,0);
            }

            // 右クリックやスペースキーでもPUSHを検出
            if ( Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.Space) ) {
                push_ = true;
            }
        }
#endif
	}
}
