using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

namespace TH.Core {

public class UICrafting : MonoBehaviour
{
    #region PublicVariables
	#endregion

	#region PrivateVariables
	private const int CRAFTING_ELEMENT_SLOT_NUMBER = 8;
	private const string ITEM_SELECT_BUTTON_PREFAB_PATH = "Prefabs/UI/BuildingSystem/BuildingSelectButton";
	private const string BUTTON_LIST_PANEL = "Scroll View/Viewport/Content";
	private const string ITEM_CRAFT_INFO_PANEL = "ItemCraftInfoPanel";
	private const string CRAFT_BUTTON = ITEM_CRAFT_INFO_PANEL + "/" + "CraftButton";
	private const string INCREASE_BUTTON = ITEM_CRAFT_INFO_PANEL + "/" + "NumberSelect/IncreaseButton";
	private const string DECREASE_BUTTON = ITEM_CRAFT_INFO_PANEL + "/" + "NumberSelect/DecreaseButton";
	private const string TARGET_NUMBER_TEXT = ITEM_CRAFT_INFO_PANEL + "/" + "NumberSelect/TargetNumberText";
	private const string EXIT_BUTTON = "ExitButton";

	private ComponentGetter<ItemCraftInfoPanel> _craftingInfoPanel = 
		new ComponentGetter<ItemCraftInfoPanel>(TypeOfGetter.ChildByName, ITEM_CRAFT_INFO_PANEL);
	private ObjectGetter _buttonListPanel = 
		new ObjectGetter(TypeOfGetter.ChildByName, BUTTON_LIST_PANEL);
	private ComponentGetter<Button> _exitButton = 
		new ComponentGetter<Button>(TypeOfGetter.ChildByName, EXIT_BUTTON);
	private ComponentGetter<Button> _craftButton =
		new ComponentGetter<Button>(TypeOfGetter.ChildByName, CRAFT_BUTTON);
	private ComponentGetter<Button> _increaseButton =
		new ComponentGetter<Button>(TypeOfGetter.ChildByName, INCREASE_BUTTON);
	private ComponentGetter<Button> _decreaseButton =
		new ComponentGetter<Button>(TypeOfGetter.ChildByName, DECREASE_BUTTON);
	private ComponentGetter<TextMeshProUGUI> _targetNumberText =
		new ComponentGetter<TextMeshProUGUI>(TypeOfGetter.ChildByName, TARGET_NUMBER_TEXT);

	private BuildingData.ItemRecipe _selectedItemRecipe;
	private int _targetNumber = 1;
	#endregion

	#region PublicMethod
	public void Init(BuildingData buildingData, Func<BuildingData.ItemRecipe, int, bool> onItemSelect) {
		_craftingInfoPanel.Get(gameObject).Init(CRAFTING_ELEMENT_SLOT_NUMBER);

		var craftingList = buildingData.itemRecipes;
		for (int i = 0; i < craftingList.Length; i++) {
			GameObject itemSelectButton = Instantiate(Resources.Load<GameObject>(ITEM_SELECT_BUTTON_PREFAB_PATH));
			itemSelectButton.transform.SetParent(_buttonListPanel.Get(gameObject).transform);
			itemSelectButton.transform.localScale = Vector3.one;

			itemSelectButton.GetComponentInChildren<TextMeshProUGUI>().text = craftingList[i].outItemID;
			
			int idx = i;
			itemSelectButton.GetComponent<Button>().onClick.AddListener(() => {
				_targetNumber = 1;
				_craftingInfoPanel.Get(gameObject).SetCraftingInfo(craftingList[idx], _targetNumber);
				_selectedItemRecipe = craftingList[idx];

				_increaseButton.Get(gameObject).interactable = true;
				_decreaseButton.Get(gameObject).interactable = true;
			});
		}

		_exitButton.Get(gameObject).onClick.AddListener(() => {
			Destroy(gameObject);
		});

		_increaseButton.Get(gameObject).onClick.AddListener(() => {
			IncreaseNumber();
		});
		_increaseButton.Get(gameObject).interactable = false;

		_decreaseButton.Get(gameObject).onClick.AddListener(() => {
			DecreaseNumber();
		});
		_decreaseButton.Get(gameObject).interactable = false;

		_craftButton.Get(gameObject).onClick.AddListener(() => {
			foreach (var i in _selectedItemRecipe.needElements) {
				if (InventorySystem.Instance.GetInventory(
					GameManager.Instance.GetPlayer().GetComponent<InventoryOwner>()).HasItem(i.itemID, i.number * _targetNumber
				) == false) {
					return;
				}
			}

			onItemSelect(_selectedItemRecipe, _targetNumber);
			Destroy(gameObject);
		});

		_targetNumberText.Get(gameObject).text = _targetNumber.ToString();

		_selectedItemRecipe = null;

		gameObject.SetActive(true);
	}
	#endregion
    
	#region PrivateMethod
	private void IncreaseNumber() {
		Debug.Log("asdf");
		if (_targetNumber < 9) {
			_targetNumber++;
			_targetNumberText.Get(gameObject).text = _targetNumber.ToString();
			_craftingInfoPanel.Get(gameObject).SetCraftingInfo(_selectedItemRecipe, _targetNumber);
		}
	}

	private void DecreaseNumber() {
		Debug.Log("fdsa");
		if (_targetNumber > 1) {
			_targetNumber--;
			_targetNumberText.Get(gameObject).text = _targetNumber.ToString();
			_craftingInfoPanel.Get(gameObject).SetCraftingInfo(_selectedItemRecipe, _targetNumber);
		}
	}
	#endregion
}

}