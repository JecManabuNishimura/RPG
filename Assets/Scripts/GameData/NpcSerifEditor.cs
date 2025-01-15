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
			//初アクセス時にロードする
			if (_entity == null)
			{
				_entity = Resources.Load<NpcSerifEditor>("Serif/NpcSerif");

				//ロード出来なかった場合はエラーログを表示
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