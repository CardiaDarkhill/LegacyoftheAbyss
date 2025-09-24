using System;
using System.Linq;
using TeamCherry.Splines;
using UnityEngine;

// Token: 0x02000491 RID: 1169
[ExecuteInEditMode]
public class AutoGenerateHangingRope : MonoBehaviour
{
	// Token: 0x14000083 RID: 131
	// (add) Token: 0x06002A2A RID: 10794 RVA: 0x000B6BB4 File Offset: 0x000B4DB4
	// (remove) Token: 0x06002A2B RID: 10795 RVA: 0x000B6BEC File Offset: 0x000B4DEC
	public event Action Generated;

	// Token: 0x170004FE RID: 1278
	// (get) Token: 0x06002A2C RID: 10796 RVA: 0x000B6C21 File Offset: 0x000B4E21
	// (set) Token: 0x06002A2D RID: 10797 RVA: 0x000B6C29 File Offset: 0x000B4E29
	public bool HasGenerated { get; private set; }

	// Token: 0x06002A2E RID: 10798 RVA: 0x000B6C32 File Offset: 0x000B4E32
	private void OnValidate()
	{
		if (this.subdivisions < 2)
		{
			this.subdivisions = 2;
		}
	}

	// Token: 0x06002A2F RID: 10799 RVA: 0x000B6C44 File Offset: 0x000B4E44
	private void Awake()
	{
		if (!Application.isPlaying)
		{
			return;
		}
		this.OnValidate();
		Transform transform = this.pinnedControlPoint.transform;
		Transform transform2 = this.hangingControlPoint.transform;
		Vector3 localPosition = transform.localPosition;
		Vector3 localPosition2 = transform2.localPosition;
		float num = Vector3.Distance(localPosition, localPosition2);
		if (num < 1.5f)
		{
			return;
		}
		this.UpdateTextureTiling(this.template, this.pinnedControlPoint.transform, this.hangingControlPoint.transform);
		float width = this.template.Width;
		SplineBase.TextureTilingMethods textureTilingMethod = this.template.TextureTilingMethod;
		float textureOffset = this.template.TextureOffset;
		bool flipTextureU = this.template.FlipTextureU;
		bool flipTextureV = this.template.FlipTextureV;
		float fpsLimit = this.template.FpsLimit;
		bool preventCulling = this.template.PreventCulling;
		SplineBase.TangentSources tangentSource = this.template.TangentSource;
		SplineBase.UpdateConditions updateCondition = this.template.UpdateCondition;
		bool reverseDirection = this.template.ReverseDirection;
		Color vertexColor = this.template.VertexColor;
		float num2 = this.template.TextureTiling;
		if (textureTilingMethod != SplineBase.TextureTilingMethods.Relative)
		{
			num2 *= 0.5f;
		}
		PinTransformToSpline[] array = (from pin in base.GetComponentsInChildren<PinTransformToSpline>(true)
		where pin.Spline == this.template
		select pin).ToArray<PinTransformToSpline>();
		GameObject gameObject = this.template.gameObject;
		Object.DestroyImmediate(this.template, false);
		Rigidbody2D rigidbody2D;
		HingeJoint2D hingeJoint2D;
		if (this.hangingControlPoint.bodyType == RigidbodyType2D.Static && this.pinnedControlPoint.bodyType == RigidbodyType2D.Static)
		{
			rigidbody2D = null;
			hingeJoint2D = null;
			Object.Destroy(this.hangingControlPoint);
			Object.Destroy(this.pinnedControlPoint);
		}
		else
		{
			rigidbody2D = this.hangingControlPoint;
			hingeJoint2D = this.pinnedControlPoint.GetComponent<HingeJoint2D>();
		}
		int num3 = Mathf.Max(4, Mathf.RoundToInt(num / this.controlPointDistance));
		Transform[] array2 = new Transform[num3];
		array2[0] = transform;
		array2[num3 - 1] = transform2;
		for (int i = num3 - 2; i >= 0; i--)
		{
			HingeJoint2D hingeJoint2D2;
			if (i != 0)
			{
				GameObject gameObject2;
				if (this.midPointTemplate)
				{
					gameObject2 = Object.Instantiate<GameObject>(this.midPointTemplate);
					gameObject2.name = string.Format("{0} (Control Point {1})", this.midPointTemplate.name, i);
				}
				else
				{
					gameObject2 = new GameObject("Control Point " + i.ToString());
				}
				array2[i] = gameObject2.transform;
				if (rigidbody2D)
				{
					AutoGenerateHangingRope.AddCopiedBody(gameObject2, rigidbody2D);
				}
				hingeJoint2D2 = (hingeJoint2D ? AutoGenerateHangingRope.AddCopiedHinge(gameObject2, hingeJoint2D) : null);
				gameObject2.transform.SetParent(transform.parent);
				float t = (float)i / (float)(num3 - 1);
				Vector3 localPosition3 = Vector3.Lerp(localPosition, localPosition2, t);
				gameObject2.transform.localPosition = localPosition3;
			}
			else
			{
				hingeJoint2D2 = array2[i].GetComponent<HingeJoint2D>();
			}
			if (!(hingeJoint2D2 == null))
			{
				Rigidbody2D component = array2[i + 1].GetComponent<Rigidbody2D>();
				hingeJoint2D2.connectedBody = component;
				hingeJoint2D2.autoConfigureConnectedAnchor = false;
				hingeJoint2D2.connectedAnchor = hingeJoint2D2.transform.position - component.transform.position;
			}
		}
		if (this.midPointTemplate)
		{
			this.midPointTemplate.gameObject.SetActive(false);
		}
		HermiteSpline hermiteSpline = gameObject.AddComponent<HermiteSpline>();
		hermiteSpline.Width = width;
		hermiteSpline.TextureTiling = num2;
		hermiteSpline.TextureTilingMethod = textureTilingMethod;
		hermiteSpline.TextureOffset = textureOffset;
		hermiteSpline.FlipTextureU = flipTextureU;
		hermiteSpline.FlipTextureV = flipTextureV;
		hermiteSpline.FpsLimit = fpsLimit;
		hermiteSpline.PreventCulling = preventCulling;
		hermiteSpline.TangentSource = tangentSource;
		hermiteSpline.UpdateCondition = updateCondition;
		hermiteSpline.ReverseDirection = reverseDirection;
		hermiteSpline.Subdivisions = this.subdivisions;
		hermiteSpline.ControlPoints = array2.ToList<Transform>();
		hermiteSpline.NormaliseDistances = false;
		hermiteSpline.VertexColor = vertexColor;
		hermiteSpline.UpdateSpline(true);
		PinTransformToSpline[] array3 = array;
		for (int j = 0; j < array3.Length; j++)
		{
			array3[j].Spline = hermiteSpline;
		}
		this.HasGenerated = true;
		if (this.Generated != null)
		{
			this.Generated();
		}
		Object.Destroy(this);
	}

	// Token: 0x06002A30 RID: 10800 RVA: 0x000B705C File Offset: 0x000B525C
	private void LateUpdate()
	{
		if (!Application.isPlaying && this.template && this.pinnedControlPoint && this.hangingControlPoint)
		{
			this.UpdateTextureTiling(this.template, this.pinnedControlPoint.transform, this.hangingControlPoint.transform);
		}
	}

	// Token: 0x06002A31 RID: 10801 RVA: 0x000B70BC File Offset: 0x000B52BC
	private static void AddCopiedBody(GameObject obj, Rigidbody2D sourceBody)
	{
		if (obj.GetComponent<Rigidbody2D>())
		{
			return;
		}
		Rigidbody2D rigidbody2D = obj.AddComponent<Rigidbody2D>();
		rigidbody2D.bodyType = sourceBody.bodyType;
		rigidbody2D.mass = sourceBody.mass;
		rigidbody2D.linearDamping = sourceBody.linearDamping;
		rigidbody2D.angularDamping = sourceBody.angularDamping;
		rigidbody2D.gravityScale = sourceBody.gravityScale;
		rigidbody2D.sleepMode = sourceBody.sleepMode;
	}

	// Token: 0x06002A32 RID: 10802 RVA: 0x000B7124 File Offset: 0x000B5324
	private static HingeJoint2D AddCopiedHinge(GameObject obj, HingeJoint2D sourceHinge)
	{
		HingeJoint2D component = obj.GetComponent<HingeJoint2D>();
		if (component)
		{
			return component;
		}
		HingeJoint2D hingeJoint2D = obj.AddComponent<HingeJoint2D>();
		hingeJoint2D.autoConfigureConnectedAnchor = true;
		hingeJoint2D.useLimits = sourceHinge.useLimits;
		hingeJoint2D.limits = sourceHinge.limits;
		return hingeJoint2D;
	}

	// Token: 0x06002A33 RID: 10803 RVA: 0x000B7168 File Offset: 0x000B5368
	private void UpdateTextureTiling(SplineBase spline, Transform startPoint, Transform endPoint)
	{
		if (this.scaleColliderToMatch)
		{
			Vector3 position = this.scaleColliderToMatch.transform.position;
			Vector2 offset = this.scaleColliderToMatch.offset;
			Vector2 size = this.scaleColliderToMatch.size;
			Vector3 position2 = startPoint.position;
			Vector3 position3 = endPoint.position;
			this.scaleColliderToMatch.size = new Vector2(size.x, Mathf.Abs(position2.y - position3.y));
			float y = (position2.y + position3.y) / 2f - position.y;
			this.scaleColliderToMatch.offset = new Vector2(offset.x, y);
		}
		if (this.autoTextureTiling == 0f)
		{
			return;
		}
		float num = Vector3.Distance(startPoint.localPosition, endPoint.localPosition);
		spline.TextureTiling = this.autoTextureTiling * (num / (float)this.subdivisions / this.controlPointDistance);
		if (spline.TextureTilingMethod != SplineBase.TextureTilingMethods.Explicit)
		{
			spline.TextureTilingMethod = SplineBase.TextureTilingMethods.Explicit;
		}
	}

	// Token: 0x04002A9E RID: 10910
	[SerializeField]
	private LinearSpline template;

	// Token: 0x04002A9F RID: 10911
	[SerializeField]
	private Rigidbody2D pinnedControlPoint;

	// Token: 0x04002AA0 RID: 10912
	[SerializeField]
	private Rigidbody2D hangingControlPoint;

	// Token: 0x04002AA1 RID: 10913
	[Space]
	[SerializeField]
	private float controlPointDistance = 2f;

	// Token: 0x04002AA2 RID: 10914
	[SerializeField]
	private int subdivisions;

	// Token: 0x04002AA3 RID: 10915
	[SerializeField]
	private float autoTextureTiling;

	// Token: 0x04002AA4 RID: 10916
	[Header("Optional")]
	[SerializeField]
	private GameObject midPointTemplate;

	// Token: 0x04002AA5 RID: 10917
	[SerializeField]
	private BoxCollider2D scaleColliderToMatch;
}
