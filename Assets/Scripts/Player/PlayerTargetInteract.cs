using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Player))]
public class PlayerTargetInteract : MonoBehaviour
{
	#region PublicVariables
	#endregion

	#region PrivateVariables
	[SerializeField] private GameObject _interactableUI;

	private IInteractable _target;

	[SerializeField] private float interactableDistance;
	#endregion

	#region PublicMethod
	public IInteractable GetTarget() => _target;
	public void CheckTarget()
	{
		Collider2D playerAround = CheckAroundPlayer();
		if (playerAround != null)
		{
			IInteractable target;
			playerAround.gameObject.TryGetComponent(out target);
			_target = target;
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
			HighlightInteractableObject();
		}
		else
		{
			_interactableUI.SetActive(false);
		}
	}
	#endregion

	#region PrivateMethod
	private Collider2D CheckAroundPlayer()
	{
		Collider2D hit = Physics2D.OverlapCircle(transform.position, interactableDistance, 1 << LayerMask.NameToLayer("Target"));
		return hit;
	}
	private void HighlightInteractableObject()
	{
		if(_target is IInteractable)
		{
			_interactableUI.SetActive(true);
			_interactableUI.transform.position = _target.GetPosition();
		}
		else
		{
			_interactableUI.SetActive(false);
		}
	}
	#endregion
}
