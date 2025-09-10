using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000236 RID: 566
public class GrabPassBatcher : MonoBehaviour
{
	// Token: 0x060014BB RID: 5307 RVA: 0x0005D528 File Offset: 0x0005B728
	private void Awake()
	{
		if (GrabPassBatcher._activeBatches == null)
		{
			GrabPassBatcher._activeBatches = new Dictionary<Material, GrabPassBatcher.Batch>();
		}
		GrabPassBatcher.Batch batch;
		if (!GrabPassBatcher._activeBatches.TryGetValue(this.grabMaterial, out batch))
		{
			batch = (GrabPassBatcher._activeBatches[this.grabMaterial] = new GrabPassBatcher.Batch());
			GameObject gameObject = GameObject.CreatePrimitive(PrimitiveType.Quad);
			gameObject.name = "GrabPassBatcher";
			gameObject.GetComponent<MeshRenderer>().material = this.grabMaterial;
			Transform transform = gameObject.transform;
			transform.SetParentReset(GameCameras.instance.mainCamera.transform);
			transform.SetPositionZ(this.targetZ);
			batch.BatchRenderer = gameObject;
		}
		batch.ActiveList.Add(this);
	}

	// Token: 0x060014BC RID: 5308 RVA: 0x0005D5D0 File Offset: 0x0005B7D0
	private void OnDestroy()
	{
		GrabPassBatcher.Batch batch;
		if (!GrabPassBatcher._activeBatches.TryGetValue(this.grabMaterial, out batch))
		{
			return;
		}
		batch.ActiveList.Remove(this);
		if (batch.ActiveList.Count != 0)
		{
			return;
		}
		GrabPassBatcher._activeBatches.Remove(this.grabMaterial);
		Object.Destroy(batch.BatchRenderer);
	}

	// Token: 0x0400131E RID: 4894
	[SerializeField]
	private Material grabMaterial;

	// Token: 0x0400131F RID: 4895
	[SerializeField]
	private float targetZ;

	// Token: 0x04001320 RID: 4896
	private static Dictionary<Material, GrabPassBatcher.Batch> _activeBatches;

	// Token: 0x02001546 RID: 5446
	private class Batch
	{
		// Token: 0x06008643 RID: 34371 RVA: 0x002725B9 File Offset: 0x002707B9
		public Batch()
		{
			this.ActiveList = new HashSet<GrabPassBatcher>();
		}

		// Token: 0x04008692 RID: 34450
		public readonly HashSet<GrabPassBatcher> ActiveList;

		// Token: 0x04008693 RID: 34451
		public GameObject BatchRenderer;
	}
}
