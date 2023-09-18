using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Dynamic;
using UnityEngine.UI;

namespace TH.Core {

public class ItemCraftInfoPanel : MonoBehaviour
{
    #region PublicVariables
	#endregion

	#region PrivateVariables
	private const string NEED_ELEMENT_SLOT_PANEL = "NeedElementShowPanel/SlotContents";
	private const string ITEM_NAME_TEXT = "ItemNameText";
	private const string ITEM_DESCRIPTION_TEXT = "ItemDescriptionPanel/ItemDescriptionText";
	private const string ITEM_ELEMENT_SLOT_PREFAB_PATH = "Prefabs/UI/BuildingSystem/BuildingElementSlot";
	private const string CRAFT_BUTTON = "CraftButton";

	private ObjectGetter _needElementSlotPanel = 
		new ObjectGetter(TypeOfGetter.ChildByName, NEED_ELEMENT_SLOT_PANEL);
	private ComponentGetter<TextMeshProUGUI> _itemNameText = 
		new ComponentGetter<TextMeshProUGUI>(TypeOfGetter.ChildByName, ITEM_NAME_TEXT);

	private ComponentGetter<TextMeshProUGUI> _itemDescriptionText = 
		new ComponentGetter<TextMeshProUGUI>(TypeOfGetter.ChildByName, ITEM_DESCRIPTION_TEXT);

	private ComponentGetter<Button> _craftButton = 
		new ComponentGetter<Button>(TypeOfGetter.ChildByName, CRAFT_BUTTON);

	private BuildingElementSlot[] _elementSlots;
	#endregion

	#region PublicMethod
	public void Init(int maxElementSlotNum) {
		_elementSlots = new BuildingElementSlot[maxElementSlotNum];

		_itemNameText.Get(gameObject).text = "-";
		_itemDescriptionText.Get(gameObject).text = "-";

		for (int i = 0; i < maxElementSlotNum; i++) {
			GameObject elementSlot = Instantiate(Resources.Load<GameObject>(ITEM_ELEMENT_SLOT_PREFAB_PATH));
			elementSlot.transform.SetParent(_needElementSlotPanel.Get(gameObject).transform);
			elementSlot.transform.localScale = Vector3.one;

			_elementSlots[i] = elementSlot.GetComponent<BuildingElementSlot>();
			_elementSlots[i].DisableElement();
		}

		_craftButton.Get(gameObject).interactable = false;
	}

	public void SetCraftingInfo(BuildingData.ItemRecipe itemRecipe, int multiple = 1) {
		_itemNameText.Get(gameObject).text = WorldManager.Instance.GetItemData(itemRecipe.outItemID).ItemName;
		_itemDescriptionText.Get(gameObject).text = WorldManager.Instance.GetItemData(itemRecipe.outItemID).ItemDescription;

		for (int i = 0; i < itemRecipe.needElements.Length; i++) {
			_elementSlots[i].SetElement(
				WorldManager.Instance.GetItemData(itemRecipe.needElements[i].itemID),
				itemRecipe.needElements[i].number * multiple
			);
		}

		_craftButton.Get(gameObject).interactable = true;
	}
	#endregion
    
	#region PrivateMethod
	#endregion
}

}