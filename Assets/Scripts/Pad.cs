using UnityEngine;
using System.Collections;

public class Pad {

	Vector2 start_;
	Vector2 latest_;
	bool	push_;

	int		leverId_;
	float	pushStart_;
	float	longStart_;
	float	dblcStart_;

	// コンストラクタ
	public Pad()
	{
		leverId_ = -1;

		pushStart_ = 0;
		longStart_ = 0;
		dblcStart_ = 0;
	}

	/// 入力方向を取得する.
	public Vector2 GetVector() {
//        if ( pushStart_ == 0 )
//        {
//            return new Vector2(0,0);
//        }
		// 右・左
		// 上・下
		float x = Input.GetAxisRaw("Horizontal");
		float y = Input.GetAxisRaw("Vertical");

		x += latest_.x - start_.x;
		y += latest_.y - start_.y;

		start_ = latest_;

		// 移動する向きを求める
//		return new Vector2(x, y).normalized;
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

		float elapsePush = Time.time - pushStart_;

		// タッチを検出
		foreach (var touch in Input.touches) {

			switch ( touch.phase )
			{
			case TouchPhase.Began:
				if ( leverId_ == -1 )
				{
					leverId_ = touch.fingerId;
					start_ = touch.position;
					latest_ = start_;

					if ( longStart_ > 0.0f )
					{
						longStart_ = 0;
						dblcStart_ = Time.time;
//						push_ = true;
					}
					else
					{
						// 計測開始
						pushStart_ = Time.time;
						longStart_ = Time.time;
					}
				}
				else
				{
					push_ = true;
				}
				break;

			case TouchPhase.Stationary:
				if ( touch.fingerId == leverId_ )
				{
					if ( (dblcStart_ > 0.0f) && ( Time.time - dblcStart_ > 0.03f ) )
					{
//						if (( touch.position.x - start_.x < 5 ) && ( touch.position.y - start_.y < 5 ))
						{
							push_ = true;
							dblcStart_ = 0;
						}
					}
				}
				break;

			case TouchPhase.Moved:
				if ( touch.fingerId == leverId_ )
				{
					latest_ = touch.position;

					// キャンセルのためにリセットする
					pushStart_ = 0;
					longStart_ = Time.time;
					dblcStart_ = 0;
				}
				break;
			
			case TouchPhase.Canceled:
			case TouchPhase.Ended:
				if ( touch.fingerId == leverId_ )
				{
//					start_ = new Vector2(0,0).normalized;
                    start_ = new Vector2(0,0);
					latest_ = start_;
					leverId_ = -1;

					if ( elapsePush < 0.5f )
					{
						push_ = true;
					}
					pushStart_ = 0;
					longStart_ = Time.time;
					dblcStart_ = 0;
				}
				break;			
			}
		}

		if ( Time.time - longStart_ > 0.5f )
		{
			longStart_ = 0;
		}

		// 左クリックを検出
		if ( Input.GetMouseButtonDown(0) ) {
			// マウスボタン押下
			start_ = Input.mousePosition;
            latest_ = start_;

			pushStart_ = Time.time;
		}
		else if ( Input.GetMouseButton(0) ) {
			// マウス押下中
			latest_ = Input.mousePosition;
		}
		else if ( Input.GetMouseButtonUp(0) ) {
//			start_ = new Vector2(0,0).normalized;
            start_ = new Vector2(0,0);
			latest_ = start_;

			if ( elapsePush < 0.5f )
			{
				push_ = true;
			}
			pushStart_ = 0;
		}

		// 右クリックやスペースキーでもPUSHを検出
		if ( Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.Space) ) {
			push_ = true;
		}
	}
}
