using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TH.Core {

public class WorldQuest : Singleton<WorldQuest>
{
    #region PublicVariables
	public bool HasInitialized => _hasInitialized;
	#endregion

	#region PrivateVariables
	private bool _hasInitialized = false;
	
	private List<Quest> quests = new List<Quest>();
	#endregion

	#region PublicMethod
	public void RegisterQuest(Func<bool> questFunc, int priority, float duration) {
		Quest quest = new Quest(questFunc, priority, duration);
		quests.Add(quest);
	}
	#endregion
    
	#region PrivateMethod
	protected override void Init() {
		_hasInitialized = true;
	}
	#endregion
}

public class Quest {
	public Quest(Func<bool> questFunc, int priority, float duration) {
		targetFunc = questFunc;
		this.priority = priority;
		this.duration = duration;

		isCompleted = false;
		hasTaken = false;
		hasStarted = false;
		takeAI = null;
	}

	public float duration {get; private set;}
	public int priority {get; private set;}
	public Func<bool> targetFunc {get; private set;}
	public bool isCompleted {get; private set;}
	public bool hasTaken {get; private set;}
	public bool hasStarted {get; private set;}
	public SmartAI takeAI {get; private set;}

	public Func<bool> Take(SmartAI ai) {
		hasTaken = true;
		takeAI = ai;

		return DoQuest;
	}

	public bool DoQuest() {
		if (hasStarted == false) {
			hasStarted = true;
			return true;
		}

		if (targetFunc()) {
			isCompleted = true;
			return true;
		}

		return false;
	}
}

}