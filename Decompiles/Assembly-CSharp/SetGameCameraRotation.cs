using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000171 RID: 369
public class SetGameCameraRotation : MonoBehaviour
{
	// Token: 0x06000BBF RID: 3007 RVA: 0x00035803 File Offset: 0x00033A03
	private void OnEnable()
	{
		this.previousRotation = this.rotation;
		SetGameCameraRotation._activeRotations.Add(this);
		SetGameCameraRotation.UpdateRotation();
	}

	// Token: 0x06000BC0 RID: 3008 RVA: 0x00035821 File Offset: 0x00033A21
	private void OnDisable()
	{
		SetGameCameraRotation._activeRotations.Remove(this);
		SetGameCameraRotation.UpdateRotation();
	}

	// Token: 0x06000BC1 RID: 3009 RVA: 0x00035834 File Offset: 0x00033A34
	private void LateUpdate()
	{
		if (Math.Abs(this.rotation - this.previousRotation) <= Mathf.Epsilon)
		{
			return;
		}
		this.previousRotation = this.rotation;
		SetGameCameraRotation.UpdateRotation();
	}

	// Token: 0x06000BC2 RID: 3010 RVA: 0x00035864 File Offset: 0x00033A64
	private static void UpdateRotation()
	{
		GameCameras instance = GameCameras.instance;
		if (!instance)
		{
			return;
		}
		Transform transform = instance.mainCamera.transform;
		if (SetGameCameraRotation._activeRotations.Count == 0)
		{
			transform.SetLocalRotation2D(0f);
			return;
		}
		List<SetGameCameraRotation> activeRotations = SetGameCameraRotation._activeRotations;
		SetGameCameraRotation setGameCameraRotation = activeRotations[activeRotations.Count - 1];
		transform.SetLocalRotation2D(setGameCameraRotation.rotation);
	}

	// Token: 0x04000B52 RID: 2898
	[SerializeField]
	private float rotation;

	// Token: 0x04000B53 RID: 2899
	private float previousRotation;

	// Token: 0x04000B54 RID: 2900
	private static readonly List<SetGameCameraRotation> _activeRotations = new List<SetGameCameraRotation>();
}
