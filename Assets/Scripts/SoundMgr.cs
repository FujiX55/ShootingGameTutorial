using UnityEngine;
using System.Collections;

public class SoundMgr : MonoBehaviour {

    /// <summary>
    /// サウンド管理
    /// </summary>
    void Start () 
    {
        // サウンドをロード
        // "bgm01"をロード　キーは"bgm"とする
        Sound.LoadBgm("bgm", "bgm01");
	
        // "damage"をロード　キーは"damage"とする
        Sound.LoadSe("damage", "damage");
        
        // "destroy"をロード　キーは"destroy"とする
        Sound.LoadSe("destroy", "destroy");    
    }
}
