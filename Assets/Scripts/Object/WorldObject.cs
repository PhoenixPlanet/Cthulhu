using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TH.Core;
using Sirenix.OdinInspector;

[RequireComponent(typeof(DropItemSpawner))]
public class WorldObject : MonoBehaviour, IHittable
{
	#region PublicVariables
	public string ObjectID => _objectID;
	public int HP => _hp;
	public int MaxHP => WorldManager.Instance.GetObjectData(_objectID).hpMax;
	public bool HasHit => _hasHit;
	#endregion

	#region PrivateVariables
	[SerializeField] protected string _objectID;
	private bool _hasInitialized = false;
	private Vector2Int _areaPos;
	private Action<string, Vector2Int> _onObjectDestroyed;

	private SpriteRenderer _sr;
	protected DropItemSpawner _drop;
	protected int _hp;
	protected int _maxhp;
	[SerializeField] private Vector2 hittablePointA = new Vector2(-0.5f, 0.5f);
	[SerializeField] private Vector2 hittablePointB = new Vector2(0.5f, -0.5f);

	private bool _hasHit;
	#endregion

	#region PublicMethod
	public virtual void Init(string id, Vector2Int areaPos, Action<string, Vector2Int> onObjectDestroyed)
	{
		_hasInitialized = true;
		_objectID = id;
		_areaPos = areaPos;
		_onObjectDestroyed = onObjectDestroyed;
		_maxhp = WorldManager.Instance.GetObjectData(_objectID).hpMax;
		_hp = _maxhp;
	}
	public Vector2 GetPosition() => transform.position;
	public Vector2 GetHittableUIPositionA() => (Vector2)transform.position + hittablePointA;
	public Vector2 GetHittableUIPositionB() => (Vector2)transform.position + hittablePointB;
	public virtual void Hit(int damage)
	{
		_hasHit = true;

		_sr.material.EnableKeyword("HITEFFECT_ON");
		Invoke(nameof(DisableHitEffect), 0.13f);
		_sr.transform.DOShakePosition(0.13f, 0.4f);
		_hp = Mathf.Clamp(_hp - damage, 0, WorldManager.Instance.GetObjectData(_objectID).hpMax);
		if (_hp == 0)
		{
			Die();
		}
	}

	public virtual void Heal(int heal)
	{
        _hp = Mathf.Clamp(_hp + heal, 0, WorldManager.Instance.GetObjectData(_objectID).hpMax);
    }

	public virtual void Die()
	{
		CameraManager.Instance.Shake(CameraShaker.EShakingType.crash);
		_drop.Drop();
		Destroy(gameObject);
	}

	public virtual WorldObjectAward GetAwardInfo() {
		if (_objectID == "Berry") {
			return new WorldObjectAward(WorldObjectAward.AwardType.HEALTH, 1);
		} else if (_objectID == "Beet") {
			return new WorldObjectAward(WorldObjectAward.AwardType.SANITY, 1);
		}

		return new WorldObjectAward(WorldObjectAward.AwardType.NONE, 0);
	}
	#endregion

	#region PrivateMethod
	protected virtual void Awake()
	{
		TryGetComponent(out _drop);
		transform.Find("Renderer").TryGetComponent(out _sr);
	}
	private void OnDestroy()
	{
		_onObjectDestroyed(_objectID, _areaPos);
	}
	private void DisableHitEffect()
	{
		_sr.material.DisableKeyword("HITEFFECT_ON");
	}
	#endregion

	public class WorldObjectAward {
		public enum AwardType {
			NONE,
			HEALTH,
			SANITY
		}

		public AwardType type {get; private set;}
		public int amount {get; private set;}

		public WorldObjectAward(AwardType type, int amount) {
			this.type = type;
			this.amount = amount;
		}
	}
}
