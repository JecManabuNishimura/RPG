using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyData :CharacterState
{
    //public CharacterState param { get; protected set; }
    public List<AttackPattern> AttackPatterns;
    [SerializeField] private TextMeshProUGUI nameText;
    private Material mat;
    private static readonly int DissolveAmount = Shader.PropertyToID("_DissolveAmount");
    
    private SpriteRenderer sr;
    [SerializeField] private Sprite IdleSprite,DamageSprite,AttackSprite;

    public int gold { get; protected set; }
    protected void Initialize()
    {
        sr = GetComponent<SpriteRenderer>();
        mat = sr.material;
        nameText.text = CharaName;
    }
    public void Select(bool select)
    {
        nameText.enabled = select;
    }

    public override IEnumerator PerformAction()
    {
        // １フレーム遅らせないと、メッセージが早くなってしまう
		yield return new WaitForEndOfFrame();
		if (DethFlag)
        {
            yield break;
        }
        to.Clear();
        // 攻撃対象選択
        int id =  SelectAttackPattern();
        List<PlayerCharacter> playerTo = new();
        switch (AttackMaster.Entity.AttackDatas[id].scope)
        {
            case AttackScope.One:
                playerTo.Add(PlayerDataRepository.Instance.RandomPlayer);        
                break;
            case AttackScope.All:
                playerTo = PlayerDataRepository.Instance.playersState;
                MessageManager.Instance.StartDialogMessage(CharaName+ "はぜんたいこうげき");
                yield return new WaitUntil(() => MessageManager.Instance.IsEndMessage);
                break;
        }
        ActionCommand = "こうげき";
        
        // 攻撃モーション
        sr.sprite = AttackSprite;
        Instantiate(EffectMaster.Entity.effectData[AttackMaster.Entity.AttackDatas[AttackPatterns[0].id].effectId].effect,transform.position,Quaternion.identity);
        yield return new WaitUntil(() => BattleManager.Instance.IsEndMotion);
        BattleManager.Instance.IsEndMotion = false;
        sr.sprite = IdleSprite;
        foreach (var t in playerTo)
        {
            int damage = (parameter.Atk / 2) - (t.TotalParam.Def / 4);
            if (parameter.Luc > Random.Range(0, 255))
            {
                t.Damage(damage * 2,true);                            
            }
            else
            {
                t.Damage(damage,false);
            }
			yield return new WaitUntil(() => MessageManager.Instance.IsEndMessage);
            
			//yield return new WaitForSeconds(1);
        }
        yield break;
    }

    public override void Damage(int damage,bool criticalFlag)
    {
        if (DethFlag)
        {
            MessageManager.Instance.StartDialogMessage("ミス");
            return;
        }

        StartCoroutine(DamageImageChange());
        int random = Random.Range(0, 255);

        int calcDamage = Mathf.Clamp(damage, 0, damage);
        
        base.Damage(calcDamage,criticalFlag);
        MessageManager.Instance.StartDialogMessage(CharaName + "に" +
                                                   (criticalFlag ? "クリティカル":"") +
                                                   calcDamage.ToString().ConvertToFullWidth() + "のダメージ");
        Debug.Log(CharaName + "に" +  calcDamage + "のダメージ");
        
        if (parameter.Hp <= 0)
        {
            // 取得経験値
            BattleManager.Instance.GetExp += parameter.Exp;
            BattleManager.Instance.GetGold += gold;

            DethFlag = true;
            
            // 死亡処理
            //StartCoroutine(DethAction());
        }
    }

    private IEnumerator DamageImageChange()
    {
        sr.sprite = DamageSprite;
        yield return new WaitForSeconds(0.3f);
        sr.sprite = parameter.Hp > 0 ? IdleSprite : null;
    }

    public IEnumerator DeathAction()
    {
        float value = 0.0f;
        while (true)
        {
            mat.SetFloat(DissolveAmount,value);
            value += 0.005f;
            yield return new WaitForEndOfFrame();

            if (value >= 1.0f) break;
        }

        DethFlag = true;
    }

    public int SelectAttackPattern()
    {
        int totalWight = AttackPatterns.Sum(data => data.weight);
        int random = Random.Range(1, totalWight + 1);

        foreach (var ap in AttackPatterns)
        {
            if (random <= ap.weight)
            {
                return ap.id;
            }
        }

        return -1;
    }
}
