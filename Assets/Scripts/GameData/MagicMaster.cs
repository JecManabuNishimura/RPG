using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MagicMaster", menuName = "Master/MagicMaster")]
public class MagicMaster : ScriptableObject
{
    private static MagicMaster _entity;

    public static MagicMaster Entity
    {
        get
        {
            //���A�N�Z�X���Ƀ��[�h����
            if (_entity == null)
            {
                _entity = Resources.Load<MagicMaster>("Master/MagicMaster");
                //���[�h�o���Ȃ������ꍇ�̓G���[���O��\��
                if (_entity == null)
                {
                    Debug.LogError(nameof(MagicMaster) + " not found");
                }
            }
            return _entity;
        }
    }

    public List<MagicData> magicList = new();
    
    public MagicData GetMagicData(string name)
    {
        return magicList.Find(m => m.magicName == name);
    }
}
