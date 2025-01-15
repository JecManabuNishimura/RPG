using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StrengthMenuData : MonoBehaviour
{
    public new TextMeshProUGUI name;
    public TextMeshProUGUI job;
    public TextMeshProUGUI gender;
    public TextMeshProUGUI hp;
    public TextMeshProUGUI mp;
    public TextMeshProUGUI level;
    public TextMeshProUGUI hand1;
    public TextMeshProUGUI body;
    public TextMeshProUGUI hand2;
    public TextMeshProUGUI head;
    
    public TextMeshProUGUI maxHp;
    public TextMeshProUGUI maxMp;
    public TextMeshProUGUI str;         // ちから
    public TextMeshProUGUI res;         // みのまもり 
    public TextMeshProUGUI atkMgc;      // 魔法攻撃力
    public TextMeshProUGUI defMgc;      // 魔法防御力
    public TextMeshProUGUI atk;         // 攻撃力 
    public TextMeshProUGUI def;         // 防御力
    public TextMeshProUGUI agi;         // 素早さ
    public TextMeshProUGUI lucky;       // 運
    
    void Awake()
    {
        name.text = "みせってい";
        job.text = "みせってい";
        gender.text = "せいべつ：みせってい";
        hp.text = "ＨＰ：みせってい";
        mp.text = "ＭＰ：みせってい";
        level.text = "ＬＶ：みせってい";
        hand1.text = "みせってい";
        body.text = "みせってい";
        hand2.text = "みせってい";
        head.text = "みせってい";
        maxHp.text = "さいだいＨＰ：みせってい";
        maxMp.text = "さいだいＭＰ：みせってい";
        str.text = "ちから：みせってい";
        res.text = "みのまもり：みせってい";
        atkMgc.text = "こうげきまりょく：みせってい";
        defMgc.text = "ぼうぎょまりょく：みせってい";
        atk.text = "こうげきりょく：みせってい";
        agi.text = "ぼうぎょりょく：みせってい";
        def.text = "すばやさ：みせってい";
        lucky.text = "うん：みせってい";
    }
}
