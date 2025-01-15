using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NpcSerif", menuName = "NpcSerifEditor")]
public class NpcSerifEditor : ScriptableObject
{
	private static NpcSerifEditor _entity;

	public static NpcSerifEditor Entity
	{
		get
		{
			//���A�N�Z�X���Ƀ��[�h����
			if (_entity == null)
			{
				_entity = Resources.Load<NpcSerifEditor>("Serif/NpcSerif");

				//���[�h�o���Ȃ������ꍇ�̓G���[���O��\��
				if (_entity == null)
				{
					Debug.LogError(nameof(NpcSerifEditor) + " not found");
				}
			}

			return _entity;
		}
	}

	public List<SerifData> serifData = new();

}
[Serializable]
public class SerifData
{
	public int id;
	[TextArea]
	public string serif;
}