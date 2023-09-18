using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TH.Core {

public class EmptyGround : MonoBehaviour, IInteractable
{
    #region PublicVariables
	#endregion

	#region PrivateVariables
	private const string BUILDING_PREFAB_PATH = "Prefabs/Building/";

	private Inventory _inventory;

	private ObjectGetter _buildingGround = new ObjectGetter(TypeOfGetter.Parent);
	#endregion

	#region PublicMethod
	public Vector2 GetPosition() => transform.position;
	/// <summary>
	/// index가 -1일 때는 호출하면 안 됨. 즉, 반드시 무언가를 캐릭터가 들고 있어야(선택해야) 함.
	/// 막아줄까 말까 고민하다가 버그 터질 때에는 터지는 게 차라리 나을 것 같아서 안 막아 줌.
	/// </summary>
	/// <param name="inventoryIndex"></param>
	public void Interact(int inventoryIndex)
	{
		WorldManager.Instance.OpenBuildingUI(BuildTarget);
	}
	#endregion

	#region PrivateMethod
	private Inventory GetInventory()
	{
		_inventory ??= InventorySystem.Instance.GetInventory(GameManager.Instance.GetPlayer().GetComponent<InventoryOwner>());
		return _inventory;
	}

	private bool BuildTarget(BuildingData buildingData) {
		foreach (var e in buildingData.needElements) {
			if (GetInventory().HasItem(e.itemID, e.number) == false) {
				return false;
			}
		}

		foreach (var e in buildingData.needElements) {
			GetInventory().UseItem(e.itemID, e.number);
		}

		Building building = Instantiate(Resources.Load<Building>(BUILDING_PREFAB_PATH + buildingData.buildingID), _buildingGround.Get(gameObject).transform);
		building.OnBuild(buildingData);

		gameObject.SetActive(false);

		return true;
	}
	#endregion
}

}