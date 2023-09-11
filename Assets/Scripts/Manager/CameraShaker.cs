using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Sirenix.OdinInspector;

public class CameraShaker : MonoBehaviour
{
	#region PublicVariables
	/// <summary>
	/// datas�� ������ ������ �ش� enumŸ�� ������ ������� ��.
	/// </summary>
	public enum EShakingType
	{
		crash = 0,

	}
	#endregion

	#region PrivateVariables
	/// <summary>
	/// EShakingData�� ������ ������ �ش� list�� ������ ������� ��.
	/// </summary>
	[SerializeField] private List<CameraShakingData> datas = new List<CameraShakingData>();
	#endregion

	#region PublicMethod
	[Button]
	public void Shake(EShakingType type)
	{
		CameraShakingData data = datas[(int)type];
		transform.DOShakePosition(data.duration, data.strength, data.vibrato, data.randomness);
	}
	#endregion

	#region PrivateMethod
	#endregion
}
