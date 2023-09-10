using DG.Tweening;
using Pathfinding;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using TH.Core;
using UnityEngine;

[RequireComponent(typeof(Animal))]
public class AnimalMove : MonoBehaviour
{
	#region PublicVariables
	#endregion

	#region PrivateVariables
	[SerializeField] private string _objectID; // ���� �Ŵ��� �̴ϼȶ����� �� �ϸ� ����� ���� �� ���� �ӽ÷� Serialize ���ѵ�
	private Animator _animator;
	private AIPath _aiPath;
	private AIDestinationSetter _destination;

	[SerializeField] private int _idleMoveDistance;
	#endregion

	#region PublicMethod
	public void SetObjectID(string id) => _objectID = id;
	public void SetSpeed(int speed) => _aiPath.maxSpeed = speed;
	public void RandomMove()
	{
		_aiPath.maxSpeed = ((AnimalData)WorldManager.Instance.GetObjectData(_objectID)).speedIdle;
		_animator.SetBool("move", true);
		_destination.target = null;
		_aiPath.SetPath(RandomPath.Construct(transform.position, _idleMoveDistance));

	}
	public void ChasePlayer()
	{
		_aiPath.maxSpeed = ((AnimalData)WorldManager.Instance.GetObjectData(_objectID)).speedAttack;
		_animator.SetBool("move", true);
		_destination.target = GameManager.Instance.GetPlayer().transform;
	}
	#endregion

	#region PrivateMethod
	private void Awake()
	{
		TryGetComponent(out _aiPath);
		TryGetComponent(out _destination);
		transform.Find("Renderer").TryGetComponent(out _animator);
	}
	private void Update()
	{
		SetAnimation();
	}
	private void SetAnimation()
	{
		SetDirectionByDestination();
		StopAnimationWhenReachedDestination();
	}
	private void SetDirectionByDestination()
	{
		if(_aiPath.destination.x > transform.position.x)
		{
			_animator.transform.localScale = new Vector3(-1, 1, 1);
		}
		else
		{
			_animator.transform.localScale = new Vector3(1, 1, 1);
		}
	}
	private void StopAnimationWhenReachedDestination()
	{
		if (_aiPath.reachedDestination == true)
		{
			_animator.SetBool("move", false);
		}
	}
	#endregion
}
