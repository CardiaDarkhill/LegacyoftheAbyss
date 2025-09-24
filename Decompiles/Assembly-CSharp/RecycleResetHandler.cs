using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000538 RID: 1336
[DisallowMultipleComponent]
public class RecycleResetHandler : MonoBehaviour
{
	// Token: 0x06003001 RID: 12289 RVA: 0x000D3AA8 File Offset: 0x000D1CA8
	public void OnPreRecycle()
	{
		if (this.resetActions != null)
		{
			foreach (Action action in this.resetActions)
			{
				action();
			}
			this.resetActions.Clear();
		}
		if (this.resetSelfActions != null)
		{
			foreach (Action<GameObject> action2 in this.resetSelfActions)
			{
				action2(base.gameObject);
			}
			this.resetSelfActions.Clear();
		}
	}

	// Token: 0x06003002 RID: 12290 RVA: 0x000D3B64 File Offset: 0x000D1D64
	private void OnDisable()
	{
		this.OnPreRecycle();
	}

	// Token: 0x06003003 RID: 12291 RVA: 0x000D3B6C File Offset: 0x000D1D6C
	public static void Add(GameObject target, Action resetAction)
	{
		if (resetAction == null)
		{
			return;
		}
		target.AddComponentIfNotPresent<RecycleResetHandler>().AddTempAction(resetAction);
	}

	// Token: 0x06003004 RID: 12292 RVA: 0x000D3B7E File Offset: 0x000D1D7E
	public void AddTempAction(Action resetAction)
	{
		if (resetAction == null)
		{
			return;
		}
		if (this.resetActions == null)
		{
			this.resetActions = new List<Action>();
		}
		this.resetActions.Add(resetAction);
	}

	// Token: 0x06003005 RID: 12293 RVA: 0x000D3BA3 File Offset: 0x000D1DA3
	public static void Add(GameObject target, Action<GameObject> resetAction)
	{
		if (resetAction == null)
		{
			return;
		}
		target.AddComponentIfNotPresent<RecycleResetHandler>().AddTempAction(resetAction);
	}

	// Token: 0x06003006 RID: 12294 RVA: 0x000D3BB5 File Offset: 0x000D1DB5
	public void AddTempAction(Action<GameObject> resetAction)
	{
		if (resetAction == null)
		{
			return;
		}
		if (this.resetSelfActions == null)
		{
			this.resetSelfActions = new List<Action<GameObject>>();
		}
		this.resetSelfActions.Add(resetAction);
	}

	// Token: 0x040032EA RID: 13034
	private List<Action> resetActions;

	// Token: 0x040032EB RID: 13035
	private List<Action<GameObject>> resetSelfActions;
}
