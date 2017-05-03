using UnityEngine;
using System.Collections;

public class SoundMgr : MonoBehaviour
{

	/// <summary>
	/// サウンド管理
	/// </summary>
	void Start()
	{
		// サウンドをロード
		// "bgm01"をロード　キーは"bgm"とする
		Sound.LoadBgm("bgm", "bgm01");
	
		// "ジングル01"をロード　キーは"jingle"とする
		Sound.LoadBgm("jingle", "ジングル01");

		// "damage"をロード　キーは"damage"とする
		Sound.LoadSe("damage", "damage");
        
		// "destroy"をロード　キーは"destroy"とする
		Sound.LoadSe("destroy", "destroy");    

		// "0000_さとうささら_おしまい"をロード　キーは"oshimai"とする
		Sound.LoadSe("oshimai", "0000_さとうささら_おしまい");    

		// "0001_さとうささら_はい"をロード　キーは"hai"とする
		Sound.LoadSe("hai", "0001_さとうささら_はい");    

		// "0002_さとうささら_キャッ"をロード　キーは"kya"とする
		Sound.LoadSe("kya", "0002_さとうささら_キャッ");    

		// "0003_さとうささら_アンッ"をロード　キーは"ann"とする
		Sound.LoadSe("ann", "0003_さとうささら_アンッ");    

		// "0004_さとうささら_はじまりー"をロード　キーは"hajimari"とする
		Sound.LoadSe("hajimari", "0004_さとうささら_はじまりー");

		// "0005_さとうささら_ヤッタネ"をロード　キーは"yattane"とする
		Sound.LoadSe("yattane", "0005_さとうささら_ヤッタネ");
	}
}
