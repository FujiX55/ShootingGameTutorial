using UnityEngine;
using System.Collections;

/// キャラクター基底クラス.
/// SpriteRendererが必要.
[RequireComponent(typeof(SpriteRenderer))]
public class Actor : MonoBehaviour
{
	/// 生存フラグ.
	bool exists_ = true;

	public bool Exists {
		get { return exists_; }
		set { exists_ = value; }
	}

	/// レンダラー.
	SpriteRenderer renderer_ = null;

	public SpriteRenderer Renderer {
		get { return renderer_ ?? (renderer_ = gameObject.GetComponent<SpriteRenderer>()); }
	}

	/// 座標(X).
	public float X {
		set {
			Vector3 pos = transform.position;
			pos.x = value;
			transform.position = pos;
		}
		get { return transform.position.x; }
	}

	/// 座標(Y).
	public float Y {
		set {
			Vector3 pos = transform.position;
			pos.y = value;
			transform.position = pos;
		}
		get { return transform.position.y; }
	}

	/// 座標を設定する.
	public void SetPosition(float x, float y)
	{
		Vector3 pos = transform.position;
		pos.Set(x, y, 0);
		transform.position = pos;
	}

	/// スケール値を設定.
	public void SetScale(float x, float y)
	{
		Vector3 scale = transform.localScale;
		scale.Set(x, y, (x + y) / 2);
		transform.localScale = scale;
	}

	/// スケール値(X/Y).
	public float Scale {
		get {
			Vector3 scale = transform.localScale;
			return (scale.x + scale.y) / 2.0f;
		}
		set {
			Vector3 scale = transform.localScale;
			scale.x = value;
			scale.y = value;
			transform.localScale = scale;
		}
	}

	/// 剛体.
	Rigidbody2D rigidbody2D_ = null;

	public Rigidbody2D RigidBody {
		get { return rigidbody2D_ ?? (rigidbody2D_ = gameObject.GetComponent<Rigidbody2D>()); }
	}

	/// 移動量を設定.
	public void SetVelocity(float direction, float speed)
	{
		Vector2 v;
		v.x = Util.CosEx(direction) * speed;
		v.y = Util.SinEx(direction) * speed;
		RigidBody.velocity = v;
	}

	/// 方向.
	public float Direction {
		get {
			Vector2 v = RigidBody.velocity;
			return Mathf.Atan2(v.y, v.x) * Mathf.Rad2Deg;
		}
	}

	/// 速度.
	public float Speed {
		get {
			Vector2 v = RigidBody.velocity;
			return Mathf.Sqrt(v.x * v.x + v.y * v.y);
		}
	}

	/// 回転角度.
	public float Angle {
		set { transform.eulerAngles = new Vector3(0, 0, value); }
		get { return transform.eulerAngles.z; }
	}

	/// 色設定.
	public void SetColor(float r, float g, float b)
	{
		var c = Renderer.color;
		c.r = r;
		c.g = g;
		c.b = b;
		Renderer.color = c;
	}

	//  /// アルファ値を設定.
	//  public void SetAlpha (float a)
	//  {
	//    var c = Renderer.color;
	//    c.a = a;
	//    Renderer.color = c;
	//  }
	//
	//  /// アルファ値を取得.
	//  public float GetAlpha ()
	//  {
	//    var c = Renderer.color;
	//    return c.a;
	//  }
	//
	//  /// アルファ値.
	//  public float Alpha {
	//    set { SetAlpha (value); }
	//    get { return GetAlpha (); }
	//  }

	/// サイズを設定.(画像サイズとは異なる)
	float width_ = 0.0f;
	float height_ = 0.0f;

	public void SetSize(float width, float height)
	{
		width_ = width;
		height_ = height;
	}

	/// 画面内に収めるようにする.
	public void ClampScreen()
	{
		Vector2 min = GetWorldMin();
		Vector2 max = GetWorldMax();
		Vector2 pos = transform.position;
		// 画面内に収まるように制限をかける.
		pos.x = Mathf.Clamp(pos.x, min.x, max.x);
		pos.y = Mathf.Clamp(pos.y, min.y, max.y);

		// プレイヤーの座標を反映.
		transform.position = pos;
	}

	/// 画面外に出たかどうか.
	public bool IsOutside()
	{
		Vector2 min = GetWorldMin();
		Vector2 max = GetWorldMax();
		Vector2 pos = transform.position;
		if (pos.x < min.x || pos.y < min.y) {
			return true;
		}
		if (pos.x > max.x || pos.y > max.y) {
			return true;
		}
		return false;
	}

	/// 画面の左下のワールド座標を取得する.
	public Vector2 GetWorldMin(bool noMergin = false)
	{
		Vector2 min = Camera.main.ViewportToWorldPoint(Vector2.zero);
		if (noMergin) {
			// そのまま返す.
			return min;
		}

		// 自身のサイズを考慮する.
		min.x += width_;
		min.y += height_;
		return min;
	}

	/// 画面右上のワールド座標を取得する.
	public Vector2 GetWorldMax(bool noMergin = false)
	{
		Vector2 max = Camera.main.ViewportToWorldPoint(Vector2.one);
		if (noMergin) {
			// そのまま返す.
			return max;
		}

		// 自身のサイズを考慮する.
		max.x -= width_;
		max.y -= height_;
		return max;
	}

	/// アクティブにする.
	public virtual void Activate()
	{
		gameObject.SetActive(true);
		Exists = true;
		this.Renderer.enabled = true;
	}

	/// 消滅する（オーバーライド可能）
	/// ただし base.Discard()を呼ばないと消滅しなくなることに注意
	public virtual void Discard()
	{
		DiscardCannotOverride();
	}

	/// 消滅する（オーバーライド禁止）
	public void DiscardCannotOverride()
	{
		gameObject.SetActive(false);
		Exists = false;
	}
}
