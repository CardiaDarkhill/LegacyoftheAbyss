using System;
using InControl;
using UnityEngine;

// Token: 0x020003D0 RID: 976
public class NativeInputModuleManager : MonoBehaviour
{
	// Token: 0x17000378 RID: 888
	// (get) Token: 0x06002163 RID: 8547 RVA: 0x0009A644 File Offset: 0x00098844
	public static bool IsUsedAtStart
	{
		get
		{
			return NativeInputModuleManager.isUsedAtStart;
		}
	}

	// Token: 0x17000379 RID: 889
	// (get) Token: 0x06002164 RID: 8548 RVA: 0x0009A64B File Offset: 0x0009884B
	// (set) Token: 0x06002165 RID: 8549 RVA: 0x0009A652 File Offset: 0x00098852
	public static bool IsUsed
	{
		get
		{
			return NativeInputModuleManager.isUsed;
		}
		set
		{
			NativeInputModuleManager.ChangeIsUsed(value);
		}
	}

	// Token: 0x1700037A RID: 890
	// (get) Token: 0x06002166 RID: 8550 RVA: 0x0009A65A File Offset: 0x0009885A
	public static bool IsRestartRequired
	{
		get
		{
			return NativeInputModuleManager.isUsedAtStart != NativeInputModuleManager.isUsed;
		}
	}

	// Token: 0x06002167 RID: 8551 RVA: 0x0009A66B File Offset: 0x0009886B
	private void Awake()
	{
		if (NativeInputModuleManager.instance != null)
		{
			Object.Destroy(this);
			return;
		}
		NativeInputModuleManager.instance = this;
	}

	// Token: 0x06002168 RID: 8552 RVA: 0x0009A687 File Offset: 0x00098887
	private void OnDestroy()
	{
		if (NativeInputModuleManager.instance == this)
		{
			NativeInputModuleManager.instance = null;
		}
	}

	// Token: 0x06002169 RID: 8553 RVA: 0x0009A69C File Offset: 0x0009889C
	protected void OnEnable()
	{
		if (NativeInputModuleManager.instance != this)
		{
			return;
		}
		NativeInputModuleManager.isUsedAtStart = ConfigManager.IsNativeInputEnabled;
		InControlManager component = base.GetComponent<InControlManager>();
		if (component == null)
		{
			Debug.LogError("Unable to find input manager.");
			return;
		}
		if (InputManager.IsSetup)
		{
			Debug.LogError("Too late to enable native input module.");
			return;
		}
		component.enableXInput = NativeInputModuleManager.isUsedAtStart;
		component.enableNativeInput = NativeInputModuleManager.isUsedAtStart;
		component.nativeInputEnableXInput = NativeInputModuleManager.isUsedAtStart;
		NativeInputModuleManager.isUsed = NativeInputModuleManager.IsUsedAtStart;
	}

	// Token: 0x0600216A RID: 8554 RVA: 0x0009A719 File Offset: 0x00098919
	private static void ChangeIsUsed(bool willUse)
	{
		if (NativeInputModuleManager.isUsed != willUse)
		{
			NativeInputModuleManager.isUsed = willUse;
		}
	}

	// Token: 0x04002043 RID: 8259
	private static NativeInputModuleManager instance;

	// Token: 0x04002044 RID: 8260
	private static bool isUsedAtStart;

	// Token: 0x04002045 RID: 8261
	private static bool isUsed;
}
