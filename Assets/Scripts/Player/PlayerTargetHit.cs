using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Player))]
public class PlayerTargetHit : MonoBehaviour
{
	#region PublicVariables
	#endregion

	#region PrivateVariables
	[SerializeField] private HitTarget _hittableUI;

	private IHittable _target;

	[SerializeField] private float interactableDistance;
	#endregion

	#region PublicMethod
	public IHittable GetTarget() => _target;
	public void CheckTarget()
	{
		RaycastHit2D mouseHit = CheckMouseRay();
		if (mouseHit.collider != null)
		{
			IHittable target;
			mouseHit.collider.gameObject.TryGetComponent(out target);
			if (target != null && Vector2.Distance(transform.position, mouseHit.transform.position) < interactableDistance)
			{
				_target = target;
			}
			else
			{
				_target = null;
			}
		}
		else
		{
			_target = null;
		}
	}
	public void HighlightTarget()
	{
		if(_target != null)
		{
			HighlightAttackableObject();
		}
		else
		{
			_hittableUI.Deactivate();
		}
	}
	#endregion

	#region PrivateMethod
	private RaycastHit2D CheckMouseRay()
	{
		RaycastHit2D hit = Physics2D.Raycast(Utils.MousePosition, Vector2.zero
			, float.MaxValue, 1 << LayerMask.NameToLayer("Target"));
		return hit;
	}
	
	private void HighlightAttackableObject()
	{
		if (_target is IHittable)
		{
			IHittable targetHit = _target as IHittable;
			_hittableUI.Activate(targetHit.GetHittableUIPositionA(), targetHit.GetHittableUIPositionB());
			_hittableUI.transform.position = _target.GetPosition();
		}
		else
		{
			_hittableUI.Deactivate();
		}
	}
	#endregion
}
