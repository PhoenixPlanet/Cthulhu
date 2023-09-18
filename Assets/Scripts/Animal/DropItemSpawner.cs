using DG.Tweening;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using TH.Core;
using UnityEngine;

public class DropItemSpawner : MonoBehaviour
{
	#region PublicVariables
	#endregion

	#region PrivateVariables
	[SerializeField] private string _objectID; // ���� �Ŵ��� �̴ϼȶ����� �� �ϸ� ����� ���� �� ���� �ӽ÷� Serialize ���ѵ�
	#endregion

	#region PublicMethod
	[Button]
	public void Drop()
	{
		int rand = Random.Range(WorldManager.Instance.GetObjectData(_objectID).dropQuantityMin
			, WorldManager.Instance.GetObjectData(_objectID).dropQuantityMax);
		Drop(rand);
	}

	public void Drop(int quantity) {
		EffectManager.Instance.SpawnDropEffect(transform.position);
		for(int i = 0; i < quantity; ++i)
		{
			Instantiate(WorldManager.Instance.GetItemPrefab(_objectID), transform.position, Quaternion.identity);
		}
	}

	public void Drop(ItemData itemData, int quantity) {
		EffectManager.Instance.SpawnDropEffect(transform.position);
		for(int i = 0; i < quantity; ++i)
		{
			Instantiate(itemData.ItemPrefab, transform.position + Vector3.down * 2, Quaternion.identity);
		}
	}
	public void SetObjectID(string objectID) => _objectID = objectID;
	#endregion

	#region PrivateMethod
	#endregion
}
