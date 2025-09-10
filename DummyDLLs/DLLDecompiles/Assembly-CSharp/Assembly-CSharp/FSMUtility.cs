using System;
using System.Collections.Generic;
using HutongGames.PlayMaker;
using UnityEngine;

// Token: 0x02000763 RID: 1891
public static class FSMUtility
{
	// Token: 0x06004353 RID: 17235 RVA: 0x00128704 File Offset: 0x00126904
	private static List<PlayMakerFSM> ObtainFsmList()
	{
		if (FSMUtility.fsmListPool.Count > 0)
		{
			List<PlayMakerFSM> result = FSMUtility.fsmListPool[FSMUtility.fsmListPool.Count - 1];
			FSMUtility.fsmListPool.RemoveAt(FSMUtility.fsmListPool.Count - 1);
			return result;
		}
		return new List<PlayMakerFSM>();
	}

	// Token: 0x06004354 RID: 17236 RVA: 0x00128750 File Offset: 0x00126950
	private static void ReleaseFsmList(List<PlayMakerFSM> fsmList)
	{
		fsmList.Clear();
		if (FSMUtility.fsmListPool.Count < 20)
		{
			FSMUtility.fsmListPool.Add(fsmList);
		}
	}

	// Token: 0x06004355 RID: 17237 RVA: 0x00128774 File Offset: 0x00126974
	public static bool ContainsFSM(GameObject go, string fsmName)
	{
		if (go == null)
		{
			return false;
		}
		List<PlayMakerFSM> list = FSMUtility.ObtainFsmList();
		go.GetComponents<PlayMakerFSM>(list);
		bool result = false;
		for (int i = 0; i < list.Count; i++)
		{
			if (list[i].FsmName == fsmName)
			{
				result = true;
				break;
			}
		}
		FSMUtility.ReleaseFsmList(list);
		return result;
	}

	// Token: 0x06004356 RID: 17238 RVA: 0x001287CC File Offset: 0x001269CC
	public static PlayMakerFSM LocateFSM(GameObject go, string fsmName)
	{
		if (go == null)
		{
			return null;
		}
		List<PlayMakerFSM> list = FSMUtility.ObtainFsmList();
		go.GetComponents<PlayMakerFSM>(list);
		PlayMakerFSM result = null;
		for (int i = 0; i < list.Count; i++)
		{
			PlayMakerFSM playMakerFSM = list[i];
			if (playMakerFSM.FsmName == fsmName)
			{
				result = playMakerFSM;
				break;
			}
		}
		FSMUtility.ReleaseFsmList(list);
		return result;
	}

	// Token: 0x06004357 RID: 17239 RVA: 0x00128825 File Offset: 0x00126A25
	public static PlayMakerFSM LocateMyFSM(this GameObject go, string fsmName)
	{
		return FSMUtility.LocateFSM(go, fsmName);
	}

	// Token: 0x06004358 RID: 17240 RVA: 0x0012882E File Offset: 0x00126A2E
	public static PlayMakerFSM GetFSM(GameObject go)
	{
		return go.GetComponent<PlayMakerFSM>();
	}

	// Token: 0x06004359 RID: 17241 RVA: 0x00128836 File Offset: 0x00126A36
	public static void SendEventToGameObject(GameObject go, string eventName, bool isRecursive = false)
	{
		if (go != null)
		{
			FSMUtility.SendEventToGameObject(go, FsmEvent.FindEvent(eventName), isRecursive);
		}
	}

	// Token: 0x0600435A RID: 17242 RVA: 0x00128850 File Offset: 0x00126A50
	public static void SendEventToGameObject(GameObject go, FsmEvent ev, bool isRecursive = false)
	{
		if (go != null)
		{
			List<PlayMakerFSM> list = FSMUtility.ObtainFsmList();
			go.GetComponents<PlayMakerFSM>(list);
			for (int i = 0; i < list.Count; i++)
			{
				list[i].Fsm.Event(ev);
			}
			FSMUtility.ReleaseFsmList(list);
			if (isRecursive)
			{
				Transform transform = go.transform;
				for (int j = 0; j < transform.childCount; j++)
				{
					FSMUtility.SendEventToGameObject(transform.GetChild(j).gameObject, ev, isRecursive);
				}
			}
		}
	}

	// Token: 0x0600435B RID: 17243 RVA: 0x001288CA File Offset: 0x00126ACA
	public static void SendEventUpwards(GameObject go, string eventName)
	{
		if (go != null)
		{
			FSMUtility.SendEventUpwards(go, FsmEvent.FindEvent(eventName));
		}
	}

	// Token: 0x0600435C RID: 17244 RVA: 0x001288E4 File Offset: 0x00126AE4
	public static void SendEventUpwards(GameObject go, FsmEvent ev)
	{
		if (go != null)
		{
			List<PlayMakerFSM> list = FSMUtility.ObtainFsmList();
			go.GetComponents<PlayMakerFSM>(list);
			foreach (PlayMakerFSM playMakerFSM in list)
			{
				playMakerFSM.Fsm.Event(ev);
			}
			FSMUtility.ReleaseFsmList(list);
			if (go.transform.parent)
			{
				FSMUtility.SendEventUpwards(go.transform.parent.gameObject, ev);
			}
		}
	}

	// Token: 0x0600435D RID: 17245 RVA: 0x0012897C File Offset: 0x00126B7C
	public static GameObject GetSafe(this FsmOwnerDefault ownerDefault, FsmStateAction stateAction)
	{
		if (ownerDefault.OwnerOption == OwnerDefaultOption.UseOwner)
		{
			return stateAction.Owner;
		}
		return ownerDefault.GameObject.Value;
	}

	// Token: 0x0600435E RID: 17246 RVA: 0x00128998 File Offset: 0x00126B98
	public static T GetSafe<T>(this FsmOwnerDefault ownerDefault, FsmStateAction stateAction) where T : Component
	{
		GameObject safe = ownerDefault.GetSafe(stateAction);
		if (safe != null)
		{
			return safe.GetComponent<T>();
		}
		return default(T);
	}

	// Token: 0x0600435F RID: 17247 RVA: 0x001289C8 File Offset: 0x00126BC8
	public static T GetSafe<T>(this FsmGameObject ownerDefault) where T : Component
	{
		GameObject value = ownerDefault.Value;
		if (value != null)
		{
			return value.GetComponent<T>();
		}
		return default(T);
	}

	// Token: 0x06004360 RID: 17248 RVA: 0x001289F5 File Offset: 0x00126BF5
	public static bool GetBool(PlayMakerFSM fsm, string variableName)
	{
		return fsm.FsmVariables.FindFsmBool(variableName).Value;
	}

	// Token: 0x06004361 RID: 17249 RVA: 0x00128A08 File Offset: 0x00126C08
	public static int GetInt(PlayMakerFSM fsm, string variableName)
	{
		return fsm.FsmVariables.FindFsmInt(variableName).Value;
	}

	// Token: 0x06004362 RID: 17250 RVA: 0x00128A1B File Offset: 0x00126C1B
	public static float GetFloat(PlayMakerFSM fsm, string variableName)
	{
		return fsm.FsmVariables.FindFsmFloat(variableName).Value;
	}

	// Token: 0x06004363 RID: 17251 RVA: 0x00128A2E File Offset: 0x00126C2E
	public static string GetString(PlayMakerFSM fsm, string variableName)
	{
		return fsm.FsmVariables.FindFsmString(variableName).Value;
	}

	// Token: 0x06004364 RID: 17252 RVA: 0x00128A41 File Offset: 0x00126C41
	public static Vector3 GetVector3(PlayMakerFSM fsm, string variableName)
	{
		return fsm.FsmVariables.FindFsmVector3(variableName).Value;
	}

	// Token: 0x06004365 RID: 17253 RVA: 0x00128A54 File Offset: 0x00126C54
	public static void SetBool(PlayMakerFSM fsm, string variableName, bool value)
	{
		fsm.FsmVariables.GetFsmBool(variableName).Value = value;
	}

	// Token: 0x06004366 RID: 17254 RVA: 0x00128A68 File Offset: 0x00126C68
	public static void SetBoolOnGameObjectFSMs(GameObject go, string variableName, bool value)
	{
		if (go == null)
		{
			return;
		}
		List<PlayMakerFSM> list = FSMUtility.ObtainFsmList();
		go.GetComponents<PlayMakerFSM>(list);
		foreach (PlayMakerFSM fsm in list)
		{
			FSMUtility.SetBool(fsm, variableName, value);
		}
		FSMUtility.ReleaseFsmList(list);
	}

	// Token: 0x06004367 RID: 17255 RVA: 0x00128AD4 File Offset: 0x00126CD4
	public static void SetInt(PlayMakerFSM fsm, string variableName, int value)
	{
		fsm.FsmVariables.GetFsmInt(variableName).Value = value;
	}

	// Token: 0x06004368 RID: 17256 RVA: 0x00128AE8 File Offset: 0x00126CE8
	public static void SetFloat(PlayMakerFSM fsm, string variableName, float value)
	{
		fsm.FsmVariables.GetFsmFloat(variableName).Value = value;
	}

	// Token: 0x06004369 RID: 17257 RVA: 0x00128AFC File Offset: 0x00126CFC
	public static void SetString(PlayMakerFSM fsm, string variableName, string value)
	{
		fsm.FsmVariables.GetFsmString(variableName).Value = value;
	}

	// Token: 0x0600436A RID: 17258 RVA: 0x00128B10 File Offset: 0x00126D10
	public static PlayMakerFSM FindFSMWithPersistentBool(PlayMakerFSM[] fsmArray)
	{
		for (int i = 0; i < fsmArray.Length; i++)
		{
			if (fsmArray[i].FsmVariables.FindFsmBool("Activated") != null)
			{
				return fsmArray[i];
			}
		}
		return null;
	}

	// Token: 0x0600436B RID: 17259 RVA: 0x00128B44 File Offset: 0x00126D44
	public static PlayMakerFSM FindFSMWithPersistentInt(PlayMakerFSM[] fsmArray)
	{
		for (int i = 0; i < fsmArray.Length; i++)
		{
			if (fsmArray[i].FsmVariables.FindFsmInt("Value") != null)
			{
				return fsmArray[i];
			}
		}
		return null;
	}

	// Token: 0x0600436C RID: 17260 RVA: 0x00128B78 File Offset: 0x00126D78
	public static void SendEventToGlobalGameObject(string gameObjectName, string eventName)
	{
		FsmGameObject fsmGameObject = FsmVariables.GlobalVariables.GetFsmGameObject(gameObjectName);
		if (fsmGameObject != null && fsmGameObject.Value != null)
		{
			FSMUtility.SendEventToGameObject(fsmGameObject.Value, eventName, false);
			return;
		}
		Debug.LogError(string.Format("Could not send {0} to {1}, GameObject could not be found in global variables!", eventName, gameObjectName));
	}

	// Token: 0x04004510 RID: 17680
	private const int FsmListPoolSizeMax = 20;

	// Token: 0x04004511 RID: 17681
	private static List<List<PlayMakerFSM>> fsmListPool = new List<List<PlayMakerFSM>>();

	// Token: 0x02001A34 RID: 6708
	public abstract class CheckFsmStateAction : FsmStateAction
	{
		// Token: 0x17001104 RID: 4356
		// (get) Token: 0x06009636 RID: 38454
		public abstract bool IsTrue { get; }

		// Token: 0x06009637 RID: 38455 RVA: 0x002A7CDA File Offset: 0x002A5EDA
		public override void Reset()
		{
			this.trueEvent = null;
			this.falseEvent = null;
		}

		// Token: 0x06009638 RID: 38456 RVA: 0x002A7CEC File Offset: 0x002A5EEC
		public override void OnEnter()
		{
			bool isTrue = this.IsTrue;
			this.storeValue.Value = isTrue;
			if (isTrue)
			{
				base.Fsm.Event(this.trueEvent);
			}
			else
			{
				base.Fsm.Event(this.falseEvent);
			}
			base.Finish();
		}

		// Token: 0x04009917 RID: 39191
		public FsmEvent trueEvent;

		// Token: 0x04009918 RID: 39192
		public FsmEvent falseEvent;

		// Token: 0x04009919 RID: 39193
		[UIHint(UIHint.Variable)]
		public FsmBool storeValue;
	}

	// Token: 0x02001A35 RID: 6709
	public abstract class CheckFsmStateEveryFrameAction : FsmStateAction
	{
		// Token: 0x17001105 RID: 4357
		// (get) Token: 0x0600963A RID: 38458
		public abstract bool IsTrue { get; }

		// Token: 0x0600963B RID: 38459 RVA: 0x002A7D41 File Offset: 0x002A5F41
		public override void Reset()
		{
			this.TrueEvent = null;
			this.FalseEvent = null;
			this.StoreValue = null;
			this.EveryFrame = false;
		}

		// Token: 0x0600963C RID: 38460 RVA: 0x002A7D5F File Offset: 0x002A5F5F
		public override void OnEnter()
		{
			this.DoAction();
			if (!this.EveryFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x0600963D RID: 38461 RVA: 0x002A7D75 File Offset: 0x002A5F75
		public override void OnUpdate()
		{
			this.DoAction();
		}

		// Token: 0x0600963E RID: 38462 RVA: 0x002A7D80 File Offset: 0x002A5F80
		private void DoAction()
		{
			bool isTrue = this.IsTrue;
			this.StoreValue.Value = isTrue;
			if (isTrue)
			{
				base.Fsm.Event(this.TrueEvent);
				return;
			}
			base.Fsm.Event(this.FalseEvent);
		}

		// Token: 0x0400991A RID: 39194
		public FsmEvent TrueEvent;

		// Token: 0x0400991B RID: 39195
		public FsmEvent FalseEvent;

		// Token: 0x0400991C RID: 39196
		[UIHint(UIHint.Variable)]
		public FsmBool StoreValue;

		// Token: 0x0400991D RID: 39197
		public bool EveryFrame;
	}

	// Token: 0x02001A36 RID: 6710
	public abstract class GetIntFsmStateAction : FsmStateAction
	{
		// Token: 0x17001106 RID: 4358
		// (get) Token: 0x06009640 RID: 38464
		public abstract int IntValue { get; }

		// Token: 0x06009641 RID: 38465 RVA: 0x002A7DCE File Offset: 0x002A5FCE
		public override void Reset()
		{
			this.storeValue = null;
		}

		// Token: 0x06009642 RID: 38466 RVA: 0x002A7DD7 File Offset: 0x002A5FD7
		public override void OnEnter()
		{
			if (!this.storeValue.IsNone)
			{
				this.storeValue.Value = this.IntValue;
			}
			base.Finish();
		}

		// Token: 0x0400991E RID: 39198
		[UIHint(UIHint.Variable)]
		public FsmInt storeValue;
	}

	// Token: 0x02001A37 RID: 6711
	public abstract class SetBoolFsmStateAction : FsmStateAction
	{
		// Token: 0x17001107 RID: 4359
		// (set) Token: 0x06009644 RID: 38468
		public abstract bool BoolValue { set; }

		// Token: 0x06009645 RID: 38469 RVA: 0x002A7E05 File Offset: 0x002A6005
		public override void Reset()
		{
			this.setValue = null;
		}

		// Token: 0x06009646 RID: 38470 RVA: 0x002A7E0E File Offset: 0x002A600E
		public override void OnEnter()
		{
			if (!this.setValue.IsNone)
			{
				this.BoolValue = this.setValue.Value;
			}
			base.Finish();
		}

		// Token: 0x0400991F RID: 39199
		public FsmBool setValue;
	}

	// Token: 0x02001A38 RID: 6712
	public abstract class GetComponentFsmStateAction<T> : FsmStateAction where T : Component
	{
		// Token: 0x17001108 RID: 4360
		// (get) Token: 0x06009648 RID: 38472 RVA: 0x002A7E3C File Offset: 0x002A603C
		protected virtual bool AutoFinish
		{
			get
			{
				return true;
			}
		}

		// Token: 0x17001109 RID: 4361
		// (get) Token: 0x06009649 RID: 38473 RVA: 0x002A7E3F File Offset: 0x002A603F
		protected virtual bool LogMissingComponent
		{
			get
			{
				return false;
			}
		}

		// Token: 0x0600964A RID: 38474 RVA: 0x002A7E42 File Offset: 0x002A6042
		public override void Reset()
		{
			this.Target = null;
		}

		// Token: 0x0600964B RID: 38475 RVA: 0x002A7E4C File Offset: 0x002A604C
		public override void OnEnter()
		{
			GameObject safe = this.Target.GetSafe(this);
			if (safe)
			{
				T component = safe.GetComponent<T>();
				if (component != null)
				{
					this.DoAction(component);
				}
				else
				{
					this.DoActionNoComponent(safe);
				}
			}
			if (this.AutoFinish)
			{
				base.Finish();
			}
		}

		// Token: 0x0600964C RID: 38476
		protected abstract void DoAction(T component);

		// Token: 0x0600964D RID: 38477 RVA: 0x002A7EA1 File Offset: 0x002A60A1
		protected virtual void DoActionNoComponent(GameObject target)
		{
		}

		// Token: 0x04009920 RID: 39200
		public FsmOwnerDefault Target;
	}
}
