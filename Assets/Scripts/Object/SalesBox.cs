using System.Collections;
using System.Collections.Generic;
using TH.Core;
using UnityEngine;

public class SalesBox : MonoBehaviour, IInteractable
{
	#region PublicVariables
	#endregion

	#region PrivateVariables
	private Inventory _inventory;
	#endregion

	#region PublicMethod
	public void Sell(int inventoryIndex)
	{
		
	}
	public Vector2 GetPosition() => transform.position;
	/// <summary>
	/// index�� -1�� ���� ȣ���ϸ� �� ��. ��, �ݵ�� ���𰡸� ĳ���Ͱ� ��� �־��(�����ؾ�) ��.
	/// �����ٱ� ���� ����ϴٰ� ���� ���� ������ ������ �� ���� ���� �� ���Ƽ� �� ���� ��.
	/// </summary>
	/// <param name="inventoryIndex"></param>
	public void Interact(int inventoryIndex)
	{
		InventoryItem item = GetInventory().GetItem(inventoryIndex);
		Debug.Log(item.TargetItem.Gold + " / " + item.StackedNumber);
		GameManager.Instance.AddGold(item.TargetItem.Gold * item.StackedNumber);
		GetInventory().DeleteItem(inventoryIndex);
	}
	#endregion

	#region PrivateMethod
	private Inventory GetInventory()
	{
		_inventory ??= InventorySystem.Instance.GetInventory(GameManager.Instance.GetPlayer().GetComponent<InventoryOwner>());
		return _inventory;
	}
	#endregion
}
