using System;
using System.Collections.Generic;
using HutongGames.PlayMaker;
using UnityEngine;

// Token: 0x020001A1 RID: 417
public static class PlayMakerValidator
{
	// Token: 0x06001035 RID: 4149 RVA: 0x0004E300 File Offset: 0x0004C500
	public static List<string> ValidatePlayMakerState(PlayMakerValidator.FixMode tryFixIssues = PlayMakerValidator.FixMode.All)
	{
		List<string> result = new List<string>();
		PlayMakerValidator.ValidateFsmExecutionStack(ref result, tryFixIssues.HasFlag(PlayMakerValidator.FixMode.FixFsmExecutionStack));
		if (tryFixIssues.HasFlag(PlayMakerValidator.FixMode.FixGlobalVariables))
		{
			PlayMakerValidator.FixGlobalVariables(null);
		}
		return result;
	}

	// Token: 0x06001036 RID: 4150 RVA: 0x0004E348 File Offset: 0x0004C548
	private static void ValidateFsmExecutionStack(ref List<string> errorMessages, bool tryFixIssues = true)
	{
		if (FsmExecutionStack.ExecutingFsm == null)
		{
			return;
		}
		errorMessages.Add("FsmExecutionStack is not empty! FsmExecutionStack should be empty when validating the PlayMaker state. Non empty stack may result in major memory leaks in the runtime. The next messages contain the names of leaked FSMs. Those are the names of the FSM, not the GameObject that triggered the execution.");
		while (FsmExecutionStack.ExecutingFsm != null)
		{
			Fsm executingFsm = FsmExecutionStack.ExecutingFsm;
			string name = executingFsm.Name;
			string text = (executingFsm.OwnerObject != null) ? executingFsm.OwnerObject.name : "(no object)";
			errorMessages.Add(string.Concat(new string[]
			{
				"Leaked FSM on the FsmExecutionStack. Name:",
				executingFsm.Name ?? "(no name)",
				", Owner: ",
				text,
				". ",
				tryFixIssues ? "(Issue automatically fixed)" : ""
			}));
			if (!tryFixIssues)
			{
				return;
			}
			executingFsm.Stop();
			FsmExecutionStack.PopFsm();
		}
	}

	// Token: 0x06001037 RID: 4151 RVA: 0x0004E40C File Offset: 0x0004C60C
	public static void FixGlobalVariables(List<NamedVariable> fixedVariables)
	{
		PlayMakerGlobals instance = PlayMakerGlobals.Instance;
		if (instance == null)
		{
			return;
		}
		FsmObject[] objectVariables = instance.Variables.ObjectVariables;
		if (objectVariables != null)
		{
			foreach (FsmObject fsmObject in objectVariables)
			{
				if (fsmObject != null && fsmObject.Value.IsLeakedManagedShellObject())
				{
					fsmObject.Value = null;
					if (fixedVariables != null)
					{
						fixedVariables.Add(fsmObject);
					}
				}
			}
		}
		FsmGameObject[] gameObjectVariables = instance.Variables.GameObjectVariables;
		if (gameObjectVariables != null)
		{
			foreach (FsmGameObject fsmGameObject in gameObjectVariables)
			{
				if (fsmGameObject != null && fsmGameObject.Value.IsLeakedManagedShellObject())
				{
					fsmGameObject.Value = null;
					if (fixedVariables != null)
					{
						fixedVariables.Add(fsmGameObject);
					}
				}
			}
		}
	}

	// Token: 0x06001038 RID: 4152 RVA: 0x0004E4C8 File Offset: 0x0004C6C8
	private static bool IsLeakedManagedShellObject(this Object obj)
	{
		return obj == null && obj != null;
	}

	// Token: 0x020014DF RID: 5343
	[Flags]
	public enum FixMode
	{
		// Token: 0x04008509 RID: 34057
		None = 0,
		// Token: 0x0400850A RID: 34058
		FixFsmExecutionStack = 1,
		// Token: 0x0400850B RID: 34059
		FixGlobalVariables = 2,
		// Token: 0x0400850C RID: 34060
		All = 3
	}
}
