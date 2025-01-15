using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectController : MonoBehaviour
{
    void Start()
    {

        StartCoroutine(AnimPlay());
    }

    private IEnumerator AnimPlay()
    {
        var animator = gameObject.GetComponent<Animator>();
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);
        BattleManager.Instance.IsEndMotion = true;
        Destroy(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
