using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Dynamic;
using UnityEngine.UI;

namespace TH.Core {

public class BuildingInfoPanel : MonoBehaviour
{
    #region PublicVariables
	#endregion

	#region PrivateVariables
	private const string NEED_ELEMENT_SLOT_PANEL = "NeedElementShowPanel/SlotContents";
	private const string BUILDING_NAME_TEXT = "BuildingNameText";
	private const string BUILDING_DESCRIPTION_TEXT = "BuildingDescriptionPanel/BuildingDescriptionText";
	private const string BUILDING_ELEMENT_SLOT_PREFAB_PATH = "Prefabs/UI/BuildingSystem/BuildingElementSlot";
	private const string BUILD_BUTTON = "BuildButton";

	private ObjectGetter _needElementSlotPanel = 
		new ObjectGetter(TypeOfGetter.ChildByName, NEED_ELEMENT_SLOT_PANEL);
	private ComponentGetter<TextMeshProUGUI> _buildingNameText = 
		new ComponentGetter<TextMeshProUGUI>(TypeOfGetter.ChildByName, BUILDING_NAME_TEXT);

	private ComponentGetter<TextMeshProUGUI> _buildingDescriptionText = 
		new ComponentGetter<TextMeshProUGUI>(TypeOfGetter.ChildByName, BUILDING_DESCRIPTION_TEXT);

	private ComponentGetter<Button> _buildButton = 
		new ComponentGetter<Button>(TypeOfGetter.ChildByName, BUILD_BUTTON);

	private BuildingElementSlot[] _elementSlots;
	#endregion

	#region PublicMethod
	public void Init(int maxElementSlotNum) {
		_elementSlots = new BuildingElementSlot[maxElementSlotNum];

		_buildingNameText.Get(gameObject).text = "-";
		_buildingDescriptionText.Get(gameObject).text = "-";

		for (int i = 0; i < maxElementSlotNum; i++) {
			GameObject elementSlot = Instantiate(Resources.Load<GameObject>(BUILDING_ELEMENT_SLOT_PREFAB_PATH));
			elementSlot.transform.SetParent(_needElementSlotPanel.Get(gameObject).transform);
			elementSlot.transform.localScale = Vector3.one;

			_elementSlots[i] = elementSlot.GetComponent<BuildingElementSlot>();
			_elementSlots[i].DisableElement();
		}

		_buildButton.Get(gameObject).interactable = false;
	}

	public void SetBuildingInfo(BuildingData buildingData) {
		_buildingNameText.Get(gameObject).text = buildingData.buildingName;
		_buildingDescriptionText.Get(gameObject).text = buildingData.buildingDescription;

		for (int i = 0; i < buildingData.needElements.Length; i++) {
			_elementSlots[i].SetElement(
				WorldManager.Instance.GetItemData(buildingData.needElements[i].itemID),
				buildingData.needElements[i].number
			);
		}

		_buildButton.Get(gameObject).interactable = true;
	}
	#endregion
    
	#region PrivateMethod
	#endregion
}

}