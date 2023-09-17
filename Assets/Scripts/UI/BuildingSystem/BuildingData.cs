using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace TH.Core {

[Serializable]
public class BuildingData
{
    #region PublicVariables
	public string buildingID;
	public string buildingName;
	public string buildingDescription;
	public BuildingElement[] needElements;
	#endregion

	#region PrivateVariables
	#endregion

	#region PublicMethod
	public BuildingData(BuildingData buildingData) {
		buildingID = buildingData.buildingID;
		buildingName = buildingData.buildingName;
		buildingDescription = buildingData.buildingDescription;
		needElements = new BuildingElement[buildingData.needElements.Length];
		for (int i = 0; i < buildingData.needElements.Length; i++) {
			needElements[i] = new BuildingElement(buildingData.needElements[i]);
		}
	}
	#endregion
    
	#region PrivateMethod
	#endregion

	[Serializable]
	public class BuildingElement {
		public string itemID;
		public int number;

		public BuildingElement(string itemID, int number) {
			this.itemID = itemID;
			this.number = number;
		}

		public BuildingElement(BuildingElement buildingElement) {
			itemID = buildingElement.itemID;
			number = buildingElement.number;
		}
	}
}

}