using System;
using UnityEngine;

// Token: 0x02000254 RID: 596
public class ParticleCulling : MonoBehaviour
{
	// Token: 0x0600158D RID: 5517 RVA: 0x0006173B File Offset: 0x0005F93B
	private void Awake()
	{
		this.systems = base.GetComponentsInChildren<ParticleSystem>();
	}

	// Token: 0x0600158E RID: 5518 RVA: 0x00061749 File Offset: 0x0005F949
	private void Start()
	{
		this.camera = GameCameras.instance.mainCamera;
		this.cameraTrans = this.camera.transform;
	}

	// Token: 0x0600158F RID: 5519 RVA: 0x0006176C File Offset: 0x0005F96C
	private void LateUpdate()
	{
		float num = this.camera.aspect / 1.7777778f;
		Vector2 vector = new Vector2(14.6f * num + 5f, 13.3f);
		Vector2 vector2 = this.cameraTrans.position - base.transform.position;
		bool flag = false;
		if (vector.x > 0f && Mathf.Abs(vector2.x) > vector.x)
		{
			flag = true;
		}
		else if (vector.y > 0f && Mathf.Abs(vector2.y) > vector.y)
		{
			flag = true;
		}
		if (flag == this.wasCulled)
		{
			return;
		}
		this.wasCulled = flag;
		ParticleSystem[] array = this.systems;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].emission.enabled = !flag;
		}
	}

	// Token: 0x04001430 RID: 5168
	private const float CAM_PADDING_X = 5f;

	// Token: 0x04001431 RID: 5169
	private const float CAM_PADDING_Y = 5f;

	// Token: 0x04001432 RID: 5170
	private ParticleSystem[] systems;

	// Token: 0x04001433 RID: 5171
	private Camera camera;

	// Token: 0x04001434 RID: 5172
	private Transform cameraTrans;

	// Token: 0x04001435 RID: 5173
	private bool wasCulled;
}
