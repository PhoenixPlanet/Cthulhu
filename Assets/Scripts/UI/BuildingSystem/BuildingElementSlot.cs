using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace TH.Core {

public class BuildingElementSlot : MonoBehaviour
{
    #region PublicVariables
	#endregion

	#region PrivateVariables
	private ComponentGetter<Image> _panelImage = 
		new ComponentGetter<Image>(TypeOfGetter.This);
	private ComponentGetter<Image> _elementImage = 
		new ComponentGetter<Image>(TypeOfGetter.ChildByName, "ItemImage");
	private ComponentGetter<TextMeshProUGUI> _elementNumberText = 
		new ComponentGetter<TextMeshProUGUI>(TypeOfGetter.ChildByName, "numberText");

	private bool _hasSet = false;
	private ItemData _targetItemData;
	private int _targetNumber;
	#endregion

	#region PublicMethod
	public void SetElement(ItemData itemData, int number) {
		_panelImage.Get(gameObject).color = Color.white;
		_elementImage.Get(gameObject).sprite = itemData.ItemSprite;
		_elementImage.Get(gameObject).color = Color.white;
		_elementNumberText.Get(gameObject).text = number.ToString();

		_hasSet = true;
		_targetItemData = itemData;
		_targetNumber = number;
	}

	public void DisableElement() {
		_panelImage.Get(gameObject).color = Color.gray;
		_elementImage.Get(gameObject).sprite = null;
		_elementImage.Get(gameObject).color = Color.clear;
		_elementNumberText.Get(gameObject).text = "";

		_hasSet = false;
	}
	#endregion
    
	#region PrivateMethod
	private void Update() {
		if (_hasSet) {
			if (InventorySystem.Instance.GetInventory(GameManager.Instance.GetPlayer().GetComponent<InventoryOwner>())
				.HasItem(_targetItemData.ItemID, _targetNumber)) {
				_elementNumberText.Get(gameObject).color = Color.white;
			} else {
				_elementNumberText.Get(gameObject).color = Color.red;
			}
		}
	}
	#endregion
}

}