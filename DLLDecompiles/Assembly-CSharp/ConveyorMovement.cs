using System;
using TeamCherry.SharedUtils;
using UnityEngine;

// Token: 0x020004C2 RID: 1218
public class ConveyorMovement : MonoBehaviour
{
	// Token: 0x06002BF4 RID: 11252 RVA: 0x000C0A6C File Offset: 0x000BEC6C
	public void OnEnable()
	{
		ComponentSingleton<ConveyorMovementCallbackHooks>.Instance.OnLateUpdate += this.OnLateUpdate;
		this.onConveyor = false;
	}

	// Token: 0x06002BF5 RID: 11253 RVA: 0x000C0A8B File Offset: 0x000BEC8B
	private void OnDisable()
	{
		ComponentSingleton<ConveyorMovementCallbackHooks>.Instance.OnLateUpdate -= this.OnLateUpdate;
	}

	// Token: 0x06002BF6 RID: 11254 RVA: 0x000C0AA3 File Offset: 0x000BECA3
	public void StartConveyorMove(float c_xSpeed, float c_ySpeed)
	{
		this.onConveyor = true;
		this.xSpeed = c_xSpeed;
		this.ySpeed = c_ySpeed;
	}

	// Token: 0x06002BF7 RID: 11255 RVA: 0x000C0ABA File Offset: 0x000BECBA
	public void StopConveyorMove()
	{
		this.onConveyor = false;
	}

	// Token: 0x06002BF8 RID: 11256 RVA: 0x000C0AC4 File Offset: 0x000BECC4
	private void OnLateUpdate()
	{
		if (this.onConveyor && this.xSpeed != 0f)
		{
			base.transform.position = new Vector3(base.transform.position.x + this.xSpeed * Time.deltaTime, base.transform.position.y, base.transform.position.z);
		}
	}

	// Token: 0x04002D54 RID: 11604
	private float xSpeed;

	// Token: 0x04002D55 RID: 11605
	private float ySpeed;

	// Token: 0x04002D56 RID: 11606
	public bool onConveyor;
}
