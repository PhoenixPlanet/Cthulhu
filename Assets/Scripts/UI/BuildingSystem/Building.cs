using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering;

namespace TH.Core {

[RequireComponent(typeof(DropItemSpawner))]
public class Building : MonoBehaviour, IInteractable
{
    #region PublicVariables
	#endregion

	#region PrivateVariables
	private const string CRAFTING_UI_PREFAB_PATH = "Prefabs/UI/CraftingSystem/CraftItemPanel";
	private const string UI_CANVAS_PATH = "UI Canvas";
	private const string PROGRESS_FILL_PATH = "ProgressBar/ProgressFill";

	private Inventory _inventory;

	private ObjectGetter _uiCanvas = new ObjectGetter(TypeOfGetter.GlobalByName, UI_CANVAS_PATH);
	private ComponentGetter<DropItemSpawner> _dropItemSpawner = new ComponentGetter<DropItemSpawner>(TypeOfGetter.This);
	private ComponentGetter<SpriteRenderer> _progressFillSpriteRenderer = new ComponentGetter<SpriteRenderer>(TypeOfGetter.ChildByName, PROGRESS_FILL_PATH);
	private BuildingData _buildingData;
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
		UICrafting uiCrafting = Instantiate(Resources.Load<UICrafting>(CRAFTING_UI_PREFAB_PATH), _uiCanvas.Get().transform);
		uiCrafting.Init(_buildingData, (itemRecipe, quantity) => {
			for (int i = 0; i < itemRecipe.needElements.Length; i++) {
				InventorySystem.Instance.GetInventory(
					GameManager.Instance
						.GetPlayer()
						.GetComponent<InventoryOwner>()
				).UseItem(itemRecipe.needElements[i].itemID, quantity * itemRecipe.needElements[i].number);
			}
			
			StartCoroutine(DropItemCoroutine(WorldManager.Instance.GetItemData(itemRecipe.outItemID), quantity, itemRecipe.time));
			return true;
		});
	}

	public void OnBuild(BuildingData buildingData) {
		transform.DOJump(transform.position, 1f, 1, 0.5f);
		transform.DOScale(1, 0.5f).SetEase(Ease.OutBack);

		_buildingData = buildingData;
		_progressFillSpriteRenderer.Get(gameObject).transform.parent.gameObject.SetActive(false);
	}
	#endregion

	#region PrivateMethod
	private Inventory GetInventory()
	{
		_inventory ??= InventorySystem.Instance.GetInventory(GameManager.Instance.GetPlayer().GetComponent<InventoryOwner>());
		return _inventory;
	}

	private IEnumerator DropItemCoroutine(ItemData item, int quantity, float time) {
		_progressFillSpriteRenderer.Get(gameObject).transform.parent.gameObject.SetActive(true);
		for (int i = 0; i < quantity; i++) {
			_progressFillSpriteRenderer.Get(gameObject).material.SetFloat("_ClipUvRight", 1);
			DOTween.To(() => _progressFillSpriteRenderer.Get(gameObject).material.GetFloat("_ClipUvRight"), 
				x => _progressFillSpriteRenderer.Get(gameObject).material.SetFloat("_ClipUvRight", x), 0, time);
			yield return new WaitForSeconds(time);
			_dropItemSpawner.Get(gameObject).Drop(item, 1);
		}
		_progressFillSpriteRenderer.Get(gameObject).transform.parent.gameObject.SetActive(false);
	}
	#endregion
}

}