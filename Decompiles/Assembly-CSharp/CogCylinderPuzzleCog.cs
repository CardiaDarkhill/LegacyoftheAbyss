using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020004BD RID: 1213
public class CogCylinderPuzzleCog : MonoBehaviour
{
	// Token: 0x14000089 RID: 137
	// (add) Token: 0x06002BC6 RID: 11206 RVA: 0x000BFD6C File Offset: 0x000BDF6C
	// (remove) Token: 0x06002BC7 RID: 11207 RVA: 0x000BFDA4 File Offset: 0x000BDFA4
	public event Action RotateFinished;

	// Token: 0x17000515 RID: 1301
	// (get) Token: 0x06002BC8 RID: 11208 RVA: 0x000BFDD9 File Offset: 0x000BDFD9
	public bool IsInTargetPos
	{
		get
		{
			return this.animateRoutine == null && this.currentSectionX.IsWithinTolerance(0.1f, (float)this.targetSectionX);
		}
	}

	// Token: 0x06002BC9 RID: 11209 RVA: 0x000BFDFC File Offset: 0x000BDFFC
	private void OnValidate()
	{
		if (Application.isPlaying)
		{
			return;
		}
		if (this.fleurScroller)
		{
			this.SetInitialBlock();
		}
	}

	// Token: 0x06002BCA RID: 11210 RVA: 0x000BFE1C File Offset: 0x000BE01C
	private void Awake()
	{
		this.SetInitialBlock();
		Material sharedMaterial = this.wiggleScroller.sharedMaterial;
		this.wiggleSt = sharedMaterial.GetVector(CogCylinderPuzzleCog._texStProp);
		this.wiggleBlock = new MaterialPropertyBlock();
		this.wiggleScroller.GetPropertyBlock(this.wiggleBlock);
		Mesh mesh = this.fleurScroller.GetComponent<MeshFilter>().mesh;
		Color[] array = new Color[mesh.vertexCount];
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = this.fluerScrollerVertexColor;
		}
		mesh.colors = array;
	}

	// Token: 0x06002BCB RID: 11211 RVA: 0x000BFEA7 File Offset: 0x000BE0A7
	private void SetInitialBlock()
	{
		if (this.fleurBlock == null)
		{
			this.fleurBlock = new MaterialPropertyBlock();
		}
		this.fleurBlock.Clear();
		this.fleurScroller.GetPropertyBlock(this.fleurBlock);
		this.UpdateScrollerPos();
	}

	// Token: 0x06002BCC RID: 11212 RVA: 0x000BFEE0 File Offset: 0x000BE0E0
	private void UpdateScrollerPos()
	{
		float x = 1f / this.sectionCountX;
		float num = 1f / this.sectionCountY;
		float z = this.currentSectionX / this.sectionCountX;
		float w = this.currentSectionY / this.sectionCountY * -1f - num;
		this.fleurBlock.SetVector(CogCylinderPuzzleCog._texStProp, new Vector4(x, num, z, w));
		this.fleurScroller.SetPropertyBlock(this.fleurBlock);
	}

	// Token: 0x06002BCD RID: 11213 RVA: 0x000BFF55 File Offset: 0x000BE155
	private IEnumerator AnimateScrollerPos()
	{
		float lastSectionX = this.currentSectionX;
		Vector2 lastWiggleOffset = new Vector2(this.wiggleSt.z, this.wiggleSt.w);
		Vector2 nextWiggleOffset = lastWiggleOffset + this.wiggleOffset;
		bool hasImpacted = false;
		this.OnRotateStart.Invoke();
		float elapsed = 0f;
		while (elapsed < this.hitSpinDuration)
		{
			float t = elapsed / this.hitSpinDuration;
			float t2 = this.hitSpinCurve.Evaluate(t);
			this.currentSectionX = Mathf.LerpUnclamped(lastSectionX, this.nextSectionX, t2);
			this.UpdateScrollerPos();
			float t3 = this.wiggleCurve.Evaluate(t);
			Vector2 vector = Vector2.LerpUnclamped(lastWiggleOffset, nextWiggleOffset, t3);
			this.wiggleSt.z = vector.x;
			this.wiggleSt.w = vector.y;
			this.wiggleBlock.SetVector(CogCylinderPuzzleCog._texStProp, this.wiggleSt);
			this.wiggleScroller.SetPropertyBlock(this.wiggleBlock);
			yield return null;
			elapsed += Time.deltaTime;
			if (!hasImpacted && t >= this.impactPoint)
			{
				hasImpacted = true;
				this.OnRotateImpact.Invoke();
			}
		}
		this.currentSectionX = this.nextSectionX % this.sectionCountX;
		this.UpdateScrollerPos();
		this.animateRoutine = null;
		if (this.RotateFinished != null)
		{
			this.RotateFinished();
		}
		yield break;
	}

	// Token: 0x06002BCE RID: 11214 RVA: 0x000BFF64 File Offset: 0x000BE164
	public void HitMove()
	{
		if (this.animateRoutine == null)
		{
			this.nextSectionX = this.currentSectionX + 1f;
		}
		else
		{
			base.StopCoroutine(this.animateRoutine);
			this.nextSectionX += 1f;
		}
		this.animateRoutine = base.StartCoroutine(this.AnimateScrollerPos());
	}

	// Token: 0x06002BCF RID: 11215 RVA: 0x000BFFBD File Offset: 0x000BE1BD
	public void SetComplete()
	{
		if (this.leverFsm && this.leverFsm.isActiveAndEnabled)
		{
			this.leverFsm.SendEvent("RETRACT");
		}
		this.currentSectionX = (float)this.targetSectionX;
		this.UpdateScrollerPos();
	}

	// Token: 0x04002D13 RID: 11539
	[Header("Structure")]
	[SerializeField]
	private MeshRenderer wiggleScroller;

	// Token: 0x04002D14 RID: 11540
	[SerializeField]
	private Vector2 wiggleOffset;

	// Token: 0x04002D15 RID: 11541
	[SerializeField]
	private AnimationCurve wiggleCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

	// Token: 0x04002D16 RID: 11542
	[Space]
	[SerializeField]
	private MeshRenderer fleurScroller;

	// Token: 0x04002D17 RID: 11543
	[SerializeField]
	private Color fluerScrollerVertexColor;

	// Token: 0x04002D18 RID: 11544
	[SerializeField]
	private float hitSpinDuration;

	// Token: 0x04002D19 RID: 11545
	[SerializeField]
	private AnimationCurve hitSpinCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

	// Token: 0x04002D1A RID: 11546
	[SerializeField]
	[Range(0f, 1f)]
	private float impactPoint;

	// Token: 0x04002D1B RID: 11547
	[SerializeField]
	private PlayMakerFSM leverFsm;

	// Token: 0x04002D1C RID: 11548
	[Space]
	public UnityEvent OnRotateStart;

	// Token: 0x04002D1D RID: 11549
	public UnityEvent OnRotateImpact;

	// Token: 0x04002D1E RID: 11550
	[Space]
	[SerializeField]
	private float sectionCountX;

	// Token: 0x04002D1F RID: 11551
	[SerializeField]
	private float sectionCountY;

	// Token: 0x04002D20 RID: 11552
	[SerializeField]
	private float currentSectionY;

	// Token: 0x04002D21 RID: 11553
	[Header("Target Config")]
	[SerializeField]
	private int targetSectionX;

	// Token: 0x04002D22 RID: 11554
	[Header("Active Gameplay")]
	[SerializeField]
	private float currentSectionX;

	// Token: 0x04002D23 RID: 11555
	private MaterialPropertyBlock fleurBlock;

	// Token: 0x04002D24 RID: 11556
	private MaterialPropertyBlock wiggleBlock;

	// Token: 0x04002D25 RID: 11557
	private Vector4 wiggleSt;

	// Token: 0x04002D26 RID: 11558
	private Coroutine animateRoutine;

	// Token: 0x04002D27 RID: 11559
	private float nextSectionX;

	// Token: 0x04002D28 RID: 11560
	private static readonly int _texStProp = Shader.PropertyToID("_MainTex_ST");
}
