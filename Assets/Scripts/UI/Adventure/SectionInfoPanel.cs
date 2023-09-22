using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Dynamic;
using UnityEngine.UI;

namespace TH.Core {

public class SectionInfoPanel : MonoBehaviour
{
    #region PublicVariables
	#endregion

	#region PrivateVariables
	private const string SECTION_NAME_TEXT = "SectionNameText";
	private const string SECTION_DESCRIPTION_TEXT = "SectionDescriptionPanel/SectionDescriptionText";
	private const string SECTION_REWARD_PANEL = "SectionRewardPanel";
	private const string SECTION_REWARD_TEXT = SECTION_REWARD_PANEL + "/Viewport/Content/SectionRewardText";
	private const string ADVENTURE_BUTTON = "AdventureButton";

	private ComponentGetter<TextMeshProUGUI> _sectionNameText = 
		new ComponentGetter<TextMeshProUGUI>(TypeOfGetter.ChildByName, SECTION_NAME_TEXT);

	private ComponentGetter<TextMeshProUGUI> _sectionDescriptionText = 
		new ComponentGetter<TextMeshProUGUI>(TypeOfGetter.ChildByName, SECTION_DESCRIPTION_TEXT);

	private ComponentGetter<TextMeshProUGUI> _sectionRewardText = 
		new ComponentGetter<TextMeshProUGUI>(TypeOfGetter.ChildByName, SECTION_REWARD_TEXT);

	private ComponentGetter<Button> _adventureButton = 
		new ComponentGetter<Button>(TypeOfGetter.ChildByName, ADVENTURE_BUTTON);
	#endregion

	#region PublicMethod
	public void Init() {
		_sectionNameText.Get(gameObject).text = "-";
		_sectionDescriptionText.Get(gameObject).text = "-";
		_sectionRewardText.Get(gameObject).text = "-";

		_adventureButton.Get(gameObject).interactable = false;
	}

	public void SetAdventureInfo(WorldSetting.SectionSetting sectionSetting) {
		_sectionNameText.Get(gameObject).text = WorldManager.Instance.GetItemData(sectionSetting.sectionName).ItemName;
		_sectionDescriptionText.Get(gameObject).text = WorldManager.Instance.GetItemData(sectionSetting.sectionDescription).ItemDescription;

		string rewardText = "";
		for (int i = 0; i < sectionSetting.spawnBerrySettings.Count; i++) {
			rewardText += WorldManager.Instance.GetObjectData(sectionSetting.spawnBerrySettings[i].objectID).objectName + "\n";
		}

		for (int i = 0; i < sectionSetting.spawnMineSettings.Count; i++) {
			rewardText += WorldManager.Instance.GetObjectData(sectionSetting.spawnMineSettings[i].objectID).objectName + "\n";
		}

		for (int i = 0; i < sectionSetting.spawnAnimalSettings.Count; i++) {
			rewardText += WorldManager.Instance.GetObjectData(sectionSetting.spawnAnimalSettings[i].objectID).objectName + "\n";
		}

		_adventureButton.Get(gameObject).interactable = true;
	}
	#endregion
    
	#region PrivateMethod
	#endregion
}

}