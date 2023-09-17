using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

namespace TH.Core {

public class UIBuilding : MonoBehaviour
{
    #region PublicVariables
	#endregion

	#region PrivateVariables
	private const int BUILDING_ELEMENT_SLOT_NUMBER = 8;
	private const string BUILDING_SELECT_BUTTON_PREFAB_PATH = "Prefabs/UI/BuildingSystem/BuildingSelectButton";
	private const string BUTTON_LIST_PANEL = "Scroll View/Viewport/Content";
	private const string EXIT_BUTTON = "ExitButton";

	private ComponentGetter<BuildingInfoPanel> _buildingInfoPanel = 
		new ComponentGetter<BuildingInfoPanel>(TypeOfGetter.ChildByName, "BuildingInfoPanel");
	private ObjectGetter _buttonListPanel = 
		new ObjectGetter(TypeOfGetter.ChildByName, BUTTON_LIST_PANEL);
	private ComponentGetter<Button> _exitButton = 
		new ComponentGetter<Button>(TypeOfGetter.ChildByName, EXIT_BUTTON);
	private ComponentGetter<Button> _buildButton =
		new ComponentGetter<Button>(TypeOfGetter.ChildByName, "BuildingInfoPanel/BuildButton");

	private BuildingData _selectedBuildingData;
	#endregion

	#region PublicMethod
	public void Init() {
		_buildingInfoPanel.Get(gameObject).Init(BUILDING_ELEMENT_SLOT_NUMBER);

		var buildingDataList = WorldManager.Instance.BuildingDataDict.ToList();
		for (int i = 0; i < WorldManager.Instance.BuildingDataDict.ToList().Count; i++) {
			GameObject buildingSelectButton = Instantiate(Resources.Load<GameObject>(BUILDING_SELECT_BUTTON_PREFAB_PATH));
			buildingSelectButton.transform.SetParent(_buttonListPanel.Get(gameObject).transform);
			buildingSelectButton.transform.localScale = Vector3.one;

			buildingSelectButton.GetComponentInChildren<TextMeshProUGUI>().text = buildingDataList[i].Value.buildingName;
			
			int idx = i;
			buildingSelectButton.GetComponent<Button>().onClick.AddListener(() => {
				_buildingInfoPanel.Get(gameObject).SetBuildingInfo(buildingDataList[idx].Value);
				_selectedBuildingData = buildingDataList[idx].Value;
			});
		}

		_exitButton.Get(gameObject).onClick.AddListener(() => {
			gameObject.SetActive(false);
		});

		_selectedBuildingData = null;
	}

	public void OnOpenPanel(Func<BuildingData, bool> onBuildingSelect) {
		_buildButton.Get(gameObject).onClick.RemoveAllListeners();
		_buildButton.Get(gameObject).onClick.AddListener(() => {
			onBuildingSelect(_selectedBuildingData);
			gameObject.SetActive(false);
		});
	}
	#endregion
    
	#region PrivateMethod
	#endregion
}

}