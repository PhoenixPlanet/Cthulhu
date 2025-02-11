using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Linq;
using System;

using UnityEngine;

namespace TH.Core {

public class WorldManager : Singleton<WorldManager>
{
	#region PublicVariables
	public readonly Dictionary<int, int> AREA_TIER_COST = new Dictionary<int, int>() {
		{0, 0 },
		{1, 30},
		{2, 60},
		{3, 100},
		{4, 150},
		{5, 200},
		{6, 300},
		{7, 500}
	};

	public IReadOnlyDictionary<string, BuildingData> BuildingDataDict => _buildingDataDict;
	#endregion

	#region PrivateVariables
	[SerializeField] private AstarPath _aStar;
	[SerializeField] private int _areaPadding = 2;

	[SerializeField] private List<ObjectDataWrapper> _originalObjectDataList;
	[SerializeField] private List<AnimalDataWrapper> _originalAnimalDataList;
	[SerializeField] private List<ItemDataWrapper> _originalItemDataList;
	[SerializeField] private List<BuildingDataWrapper> _originalBuildingDataList;
	[SerializeField] private WorldSettingWrapper _originalWorldSetting;

	[SerializeField] private UIBuilding _uiBuilding;

	[ShowInInspector] private Dictionary<string, ObjectData> _objectDataDict;
	[ShowInInspector] private Dictionary<string, ItemData> _itemDataDict;
	[ShowInInspector] private Dictionary<string, BuildingData> _buildingDataDict;
	private WorldSetting _worldSetting;

	[ShowInInspector] private Dictionary<int, List<Area>> _areaDict;
	[ShowInInspector] private List<List<Area>> _areaList;
	#endregion

	#region PublicMethod
	public WorldSetting.SectionSetting GetSectionSetting(int section) {
		return _worldSetting.sectionSettings[section];
	}

	public ObjectData GetObjectData(string objectId)
	{
		return _objectDataDict[objectId];
	}

	public GameObject GetItemPrefab(string objectId)
	{
		return _objectDataDict[objectId].dropItem;
	}

	public ItemData GetItemData(string itemId) 
	{
		return _itemDataDict[itemId];	
	}

	public BuildingData GetBuildingData(string buildingId) 
	{
		return _buildingDataDict[buildingId];
	}

	public int GetAreaSize() {
		return _worldSetting.areaSize;
	}

	public int GetOpenedAreaCount(int section) {
		return _areaDict[section].FindAll(a => a.HasOpened).Count;
	}

	public IReadOnlyList<WorldSetting.SectionSetting> GetSectionSettings() {
		return _worldSetting.sectionSettings;
	}

	[Button]
	public void OpenArea(int section, int areaIdx) {
		_areaDict[section][areaIdx].Open();
	}

	public void MultiplyMaxStackableNumber(int multiplier) {
		foreach (var item in _itemDataDict) {
			item.Value.MaxStackableNumber *= multiplier;
		}
	}

	public void MultiplyMaxStackableNumber(string itemId, int multiplier) {
		if (_itemDataDict[itemId].IsStackable == false) {
			SetMaxStackableNumber(itemId, multiplier);
			return;
		}
		_itemDataDict[itemId].MaxStackableNumber *= multiplier;
	}

	public void SetMaxStackableNumber(string itemId, int maxStackableNumber) {
		if (maxStackableNumber == 1) {
			_itemDataDict[itemId].IsStackable = false;
		} else {
			_itemDataDict[itemId].IsStackable = true;
		}
		_itemDataDict[itemId].MaxStackableNumber = maxStackableNumber;
	}

	public void SetBerrySpawnGold(bool spawnGold) {
		if (spawnGold == true) {
			foreach (var data in _objectDataDict) {
				if (data.Value.objectID == "BerryBush") {
					data.Value.dropItem = _objectDataDict["Gold"].dropItem;
				}
			}
		} else {
			foreach (var data in _objectDataDict) {
				if (data.Value.objectID == "BerryBush") {
					data.Value.dropItem = _originalObjectDataList.Find(o => o.objectData.objectID == "BerryBush").objectData.dropItem;
				}
			}
		}
	}

	public void SetCopperSpawnSilver(bool spawnSilver) {
		if (spawnSilver == true) {
			foreach (var data in _objectDataDict) {
				if (data.Value.objectID == "Copper") {
					data.Value.dropItem = _objectDataDict["Silver"].dropItem;
				}
			}
		} else {
			foreach (var data in _objectDataDict) {
				if (data.Value.objectID == "Copper") {
					data.Value.dropItem = _originalObjectDataList.Find(o => o.objectData.objectID == "Copper").objectData.dropItem;
				}
			}
		}
	}

	public void MakeOreSpawnFater(float multiplier) {
		foreach (var s in _worldSetting.sectionSettings) {
			foreach (var data in s.spawnMineSettings) {
				data.spawnCycleMin *= multiplier;
				data.spawnCycleMax *= multiplier;
			}
		}
	}

	public void MakeBerryDropMore(int multiplier) {
		foreach (var data in _objectDataDict) {
			if (data.Value.objectID == "BerryBush") {
				data.Value.dropQuantityMax *= multiplier;
				data.Value.dropQuantityMin *= multiplier;
			}
		}
	}

	public void MakeOreDropMore(int multiplier) {
		foreach (var data in _objectDataDict) {
			if ((data.Value is AnimalData) == false && data.Value.objectID != "BerryBush") {
				data.Value.dropQuantityMax *= multiplier;
				data.Value.dropQuantityMin *= multiplier;
			}
		}
	}

	public void MakeAnimalDropMore(int multiplier) {
		foreach (var data in _objectDataDict) {
			if (data.Value is AnimalData) {
				data.Value.dropQuantityMax *= multiplier;
				data.Value.dropQuantityMin *= multiplier;
			}
		}
	}

	public void OpenBuildingUI(Func<BuildingData, bool> onBuildingSelected) {
		_uiBuilding.gameObject.SetActive(true);
		_uiBuilding.OnOpenPanel(onBuildingSelected);
	}

	public Area GetAreaByUnitPos(Vector2Int unitPos) {
		return _areaList[unitPos.x][unitPos.y];
	}
	public void Rescan()
	{
		_aStar.Scan();
	}
	#endregion
    
	#region PrivateMethod
	protected override void Init()
	{
		GenerateWorld();
	}

	private void GenerateWorld() {
		LoadInitialSettings();
		GenerateTiles();
		Rescan();

		InitUI();

		_areaDict[0][0].Open();
	}

	private void InitUI() {
		_uiBuilding.Init();
	}

	private void LoadInitialSettings() {
		List<ObjectData> objectDataList = new List<ObjectData>();
		foreach (var data in _originalObjectDataList) {
			objectDataList.Add(new ObjectData(data.objectData));
		}
		foreach (var data in _originalAnimalDataList)
		{
			objectDataList.Add(new AnimalData(data.objectData));
		}
		_objectDataDict = objectDataList.ToDictionary(o => o.objectID);
		
		List<ItemData> itemDataList = new List<ItemData>();
		foreach (var data in _originalItemDataList) {
			itemDataList.Add(new ItemData(data.itemData));
		}
		_itemDataDict = itemDataList.ToDictionary(i => i.ItemID);

		List<BuildingData> buildingDataList = new List<BuildingData>();
		foreach (var data in _originalBuildingDataList) {
			buildingDataList.Add(new BuildingData(data.buildingData));
		}
		_buildingDataDict = buildingDataList.ToDictionary(b => b.buildingID);

		_worldSetting = new WorldSetting(_originalWorldSetting.worldSetting);
	}

	private void GenerateTiles() {
		_areaDict = new Dictionary<int, List<Area>>();
		_areaList = new List<List<Area>>();
		
		int wholeWorldUnitSize = _worldSetting.sectionSettings.Length + 1;
		for (int i = 0; i < (_worldSetting.sectionSettings.Length + 1); i++) {
			List<Area> areaList = new List<Area>();
			for (int j = 0; j < (_worldSetting.sectionSettings.Length + 1); j++) {
				areaList.Add(null);
			}
			_areaList.Add(areaList);
		}
		
		List<Area> area0List = new List<Area>();
		Area area0 = Instantiate(_worldSetting.sectionSettings[0].sectionPrefab).GetComponent<Area>();
		area0.Init(0, 0, new Vector2Int(wholeWorldUnitSize /2, wholeWorldUnitSize / 2), GetSectionSetting(0));
		area0List.Add(area0);
		_areaDict.Add(0, area0List);
		_areaList[wholeWorldUnitSize / 2][wholeWorldUnitSize / 2] = area0;

		int gap = _worldSetting.areaSize + _areaPadding;
		int areaIdx;
		int areaUnitPosX;
		int areaUnitPosY;
		for (int i = 1; i < _worldSetting.sectionSettings.Length; i++) {
			List<Area> areaList = new List<Area>();
			int leftUpperMostX = -(gap * ((i + 1) / 2));
			int leftUpperMostY = gap * ((i + 1) / 2);
			areaIdx = 0;
			for (int j = 0; j < 4; j++) {
				if (i % 2 == 0) {
					Area area = Instantiate(_worldSetting.sectionSettings[i].sectionPrefab).GetComponent<Area>();
					int x = leftUpperMostX + (j % 2 == 0 ? 0 : (gap * i));
					int y = leftUpperMostY + (j / 2 == 0 ? 0 : -(gap * i));
					area.transform.position = new Vector3(x, y, 0);

					areaUnitPosX = x / gap + (wholeWorldUnitSize / 2);
					areaUnitPosY = y / gap + (wholeWorldUnitSize / 2);
					_areaList[areaUnitPosX][areaUnitPosY] = area;

					area.Init(i, areaIdx, new Vector2Int(areaUnitPosX, areaUnitPosY), GetSectionSetting(i));
					areaList.Add(area);

					areaIdx++;
				} else {
					int startX;
					int startY;
					if (j == 0) {
						startX = leftUpperMostX + gap;
						startY = leftUpperMostY;
					} else if (j == 1) {
						startX = leftUpperMostX + gap;
						startY = leftUpperMostY - (gap * (i + 1));
					} else if (j == 2) {
						startX = leftUpperMostX;
						startY = leftUpperMostY - gap;
					} else {
						startX = leftUpperMostX + (gap * (i + 1));
						startY = leftUpperMostY - gap;
					}

					for (int k = 0; k < i; k++) {
						Area area = Instantiate(_worldSetting.sectionSettings[i].sectionPrefab).GetComponent<Area>();
						int x = startX + (j / 2 == 0 ? gap * k : 0);
						int y = startY + (j / 2 == 0 ? 0 : -gap * k);
						area.transform.position = new Vector3(x, y, 0);
						
						areaUnitPosX = x / gap + (wholeWorldUnitSize / 2);
						areaUnitPosY = y / gap + (wholeWorldUnitSize / 2);
						_areaList[areaUnitPosX][areaUnitPosY] = area;

						area.Init(i, areaIdx, new Vector2Int(areaUnitPosX, areaUnitPosY), GetSectionSetting(i));
						areaList.Add(area);

						areaIdx++;
					}
				}	
			}
			if (i == 7) {
				for (int j = 0; j < 4; j++) {
					Area area = Instantiate(_worldSetting.sectionSettings[i].sectionPrefab).GetComponent<Area>();
					int x = leftUpperMostX + (j % 2 == 0 ? 0 : (gap * (i+1)));
					int y = leftUpperMostY + (j / 2 == 0 ? 0 : -(gap * (i+1)));
					area.transform.position = new Vector3(x, y, 0);

					areaUnitPosX = x / gap + (wholeWorldUnitSize / 2);
					areaUnitPosY = y / gap + (wholeWorldUnitSize / 2);
					_areaList[areaUnitPosX][areaUnitPosY] = area;

					area.Init(i, areaIdx, new Vector2Int(areaUnitPosX, areaUnitPosY), GetSectionSetting(i));
					areaList.Add(area);

					areaIdx++;
				} 
			}
			_areaDict.Add(i, areaList);
		}
	}
	#endregion
}

}