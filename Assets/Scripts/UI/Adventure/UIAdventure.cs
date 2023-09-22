using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

namespace TH.Core {

public class UIAdventure : MonoBehaviour
{
    #region PublicVariables
	#endregion

	#region PrivateVariables
	private const string ITEM_SELECT_BUTTON_PREFAB_PATH = "Prefabs/UI/BuildingSystem/BuildingSelectButton";
	private const string BUTTON_LIST_PANEL = "Scroll View/Viewport/Content";
	private const string SECTION_INFO_PANEL = "SectionInfoPanel";
	private const string ADVENTURE_BUTTON = SECTION_INFO_PANEL + "/" + "AdventureButton";
	private const string EXIT_BUTTON = "ExitButton";

	private ComponentGetter<SectionInfoPanel> _sectionInfoPanel = 
		new ComponentGetter<SectionInfoPanel>(TypeOfGetter.ChildByName, SECTION_INFO_PANEL);
	private ObjectGetter _buttonListPanel = 
		new ObjectGetter(TypeOfGetter.ChildByName, BUTTON_LIST_PANEL);
	private ComponentGetter<Button> _exitButton = 
		new ComponentGetter<Button>(TypeOfGetter.ChildByName, EXIT_BUTTON);
	private ComponentGetter<Button> _adventureButton =
		new ComponentGetter<Button>(TypeOfGetter.ChildByName, ADVENTURE_BUTTON);

	private WorldSetting.SectionSetting _selectedSectionSetting;
	private int _selectedIdx;
	private int _targetNumber = 1;
	#endregion

	#region PublicMethod
	public void Init(Func<int, bool> onSectionSelect) {
		_sectionInfoPanel.Get(gameObject).Init();

		var sectionList = WorldManager.Instance.GetSectionSettings();
		for (int i = 3; i < sectionList.Count; i++) {
			GameObject sectionSelectButton = Instantiate(Resources.Load<GameObject>(ITEM_SELECT_BUTTON_PREFAB_PATH));
			sectionSelectButton.transform.SetParent(_buttonListPanel.Get(gameObject).transform);
			sectionSelectButton.transform.localScale = Vector3.one;

			sectionSelectButton.GetComponentInChildren<TextMeshProUGUI>().text = sectionList[i].sectionName;
			
			int idx = i;
			sectionSelectButton.GetComponent<Button>().onClick.AddListener(() => {
				_targetNumber = 1;
				_sectionInfoPanel.Get(gameObject).SetAdventureInfo(sectionList[idx]);
				_selectedIdx = idx;
			});
		}

		_exitButton.Get(gameObject).onClick.AddListener(() => {
			Destroy(gameObject);
		});

		_adventureButton.Get(gameObject).onClick.AddListener(() => {
			
			Destroy(gameObject);
		});

		gameObject.SetActive(true);
	}
	#endregion
    
	#region PrivateMethod
	#endregion
}

}