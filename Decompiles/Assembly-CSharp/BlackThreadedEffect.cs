using System;
using UnityEngine;

// Token: 0x02000218 RID: 536
public abstract class BlackThreadedEffect : MonoBehaviour, IInitialisable, AutoRecycleSelf.IRecycleResponder
{
	// Token: 0x060013D2 RID: 5074 RVA: 0x0005A2A7 File Offset: 0x000584A7
	private void Awake()
	{
		this.OnAwake();
	}

	// Token: 0x060013D3 RID: 5075 RVA: 0x0005A2B0 File Offset: 0x000584B0
	private void Start()
	{
		this.OnStart();
	}

	// Token: 0x060013D4 RID: 5076 RVA: 0x0005A2B9 File Offset: 0x000584B9
	public virtual bool OnAwake()
	{
		if (this.hasAwaken)
		{
			return false;
		}
		this.hasAwaken = true;
		this.EnsureInitialized();
		return true;
	}

	// Token: 0x060013D5 RID: 5077 RVA: 0x0005A2D3 File Offset: 0x000584D3
	public virtual bool OnStart()
	{
		this.OnAwake();
		if (this.hasStarted)
		{
			return false;
		}
		this.hasStarted = true;
		return true;
	}

	// Token: 0x060013D6 RID: 5078 RVA: 0x0005A2EE File Offset: 0x000584EE
	protected virtual void OnValidate()
	{
	}

	// Token: 0x060013D7 RID: 5079 RVA: 0x0005A2F0 File Offset: 0x000584F0
	[ContextMenu("Test Black Thread Effect")]
	private void TestBlackThreadEffect()
	{
		this.SetBlackThreadAmount(1f);
	}

	// Token: 0x060013D8 RID: 5080 RVA: 0x0005A2FD File Offset: 0x000584FD
	public void SetBlackThreadAmount(float amount)
	{
		this.EnsureInitialized();
		this.DoSetBlackThreadAmount(amount);
		this.SetBlackThreadedKeyword(true);
	}

	// Token: 0x060013D9 RID: 5081 RVA: 0x0005A313 File Offset: 0x00058513
	public virtual void OnRecycled()
	{
		this.EnsureInitialized();
		this.SetBlackThreadAmount(1f);
		this.SetBlackThreadedKeyword(false);
	}

	// Token: 0x060013DA RID: 5082 RVA: 0x0005A32D File Offset: 0x0005852D
	private void EnsureInitialized()
	{
		if (!this.initialized)
		{
			this.block = new MaterialPropertyBlock();
			this.Initialize();
			this.initialized = true;
		}
	}

	// Token: 0x060013DB RID: 5083 RVA: 0x0005A34F File Offset: 0x0005854F
	public void SetBlackThreadedKeyword(bool enabled)
	{
		this.EnsureInitialized();
		if (enabled == this.enabledKeyword)
		{
			return;
		}
		this.enabledKeyword = enabled;
		if (enabled)
		{
			this.EnableKeyword();
			return;
		}
		this.DisableKeyword();
	}

	// Token: 0x060013DC RID: 5084
	protected abstract void Initialize();

	// Token: 0x060013DD RID: 5085
	protected abstract void DoSetBlackThreadAmount(float amount);

	// Token: 0x060013DE RID: 5086
	protected abstract void EnableKeyword();

	// Token: 0x060013DF RID: 5087
	protected abstract void DisableKeyword();

	// Token: 0x060013E2 RID: 5090 RVA: 0x0005A391 File Offset: 0x00058591
	GameObject IInitialisable.get_gameObject()
	{
		return base.gameObject;
	}

	// Token: 0x04001235 RID: 4661
	protected static readonly int BLACK_THREAD_AMOUNT = Shader.PropertyToID("_BlackThreadAmount");

	// Token: 0x04001236 RID: 4662
	private bool initialized;

	// Token: 0x04001237 RID: 4663
	private bool enabledKeyword;

	// Token: 0x04001238 RID: 4664
	protected MaterialPropertyBlock block;

	// Token: 0x04001239 RID: 4665
	private bool hasAwaken;

	// Token: 0x0400123A RID: 4666
	private bool hasStarted;
}
