using System;
using System.Collections;
using TeamCherry.Localization;
using TeamCherry.NestedFadeGroup;
using TMProOld;
using UnityEngine;

// Token: 0x02000484 RID: 1156
public class PromptMarker : MonoBehaviour
{
	// Token: 0x170004F8 RID: 1272
	// (get) Token: 0x060029C1 RID: 10689 RVA: 0x000B59C7 File Offset: 0x000B3BC7
	// (set) Token: 0x060029C2 RID: 10690 RVA: 0x000B59CF File Offset: 0x000B3BCF
	public bool IsVisible { get; private set; }

	// Token: 0x060029C3 RID: 10691 RVA: 0x000B59D8 File Offset: 0x000B3BD8
	private void Awake()
	{
		this.anim = base.GetComponent<tk2dSpriteAnimator>();
	}

	// Token: 0x060029C4 RID: 10692 RVA: 0x000B59E6 File Offset: 0x000B3BE6
	private void Start()
	{
		GameManager.instance.UnloadingLevel += this.RecycleOnLevelLoad;
		CameraRenderHooks.CameraPreCull += this.UpdatePosition;
	}

	// Token: 0x060029C5 RID: 10693 RVA: 0x000B5A0F File Offset: 0x000B3C0F
	private void OnDestroy()
	{
		if (GameManager.UnsafeInstance)
		{
			GameManager.UnsafeInstance.UnloadingLevel -= this.RecycleOnLevelLoad;
		}
		CameraRenderHooks.CameraPreCull -= this.UpdatePosition;
	}

	// Token: 0x060029C6 RID: 10694 RVA: 0x000B5A44 File Offset: 0x000B3C44
	private void RecycleOnLevelLoad()
	{
		if (base.gameObject.activeSelf)
		{
			base.gameObject.Recycle();
		}
	}

	// Token: 0x060029C7 RID: 10695 RVA: 0x000B5A60 File Offset: 0x000B3C60
	private void OnEnable()
	{
		this.anim.Play("Blank");
		this.group.AlphaSelf = 0f;
		GameCameras instance = GameCameras.instance;
		this.mainCam = instance.mainCamera;
		this.hudCam = instance.hudCamera;
		this.UpdatePosition();
	}

	// Token: 0x060029C8 RID: 10696 RVA: 0x000B5AB1 File Offset: 0x000B3CB1
	private void OnDisable()
	{
		this.followTransform = null;
	}

	// Token: 0x060029C9 RID: 10697 RVA: 0x000B5ABA File Offset: 0x000B3CBA
	private void UpdatePosition(CameraRenderHooks.CameraSource cameraSource)
	{
		if (!base.isActiveAndEnabled)
		{
			return;
		}
		if (cameraSource == CameraRenderHooks.CameraSource.HudCamera)
		{
			this.UpdatePosition();
		}
	}

	// Token: 0x060029CA RID: 10698 RVA: 0x000B5AD0 File Offset: 0x000B3CD0
	private void UpdatePosition()
	{
		if (this.IsVisible && (!this.owner || !this.owner.activeInHierarchy))
		{
			this.Hide();
		}
		if (!this.followTransform)
		{
			return;
		}
		Vector3 position = this.followTransform.position + this.followOffset;
		Vector3 position2 = this.mainCam.WorldToViewportPoint(position);
		Vector3 position3 = this.hudCam.ViewportToWorldPoint(position2);
		base.transform.position = position3;
	}

	// Token: 0x060029CB RID: 10699 RVA: 0x000B5B50 File Offset: 0x000B3D50
	public void SetLabel(string labelName)
	{
		if (!this.label)
		{
			return;
		}
		this.label.text = new LocalisedString("Prompts", labelName.ToUpper());
	}

	// Token: 0x060029CC RID: 10700 RVA: 0x000B5B80 File Offset: 0x000B3D80
	public void Show()
	{
		this.anim.Play("Up");
		base.transform.SetPositionZ(0f);
		this.group.FadeTo(1f, 0.4f, null, true, null);
		this.IsVisible = true;
	}

	// Token: 0x060029CD RID: 10701 RVA: 0x000B5BD0 File Offset: 0x000B3DD0
	public void Hide()
	{
		this.owner = null;
		this.IsVisible = false;
		if (base.gameObject.activeInHierarchy)
		{
			this.anim.Play("Down");
			this.group.FadeTo(0f, 0.233f, null, true, null);
			base.StartCoroutine(this.RecycleDelayed(0.233f));
			return;
		}
		base.gameObject.Recycle();
	}

	// Token: 0x060029CE RID: 10702 RVA: 0x000B5C3F File Offset: 0x000B3E3F
	private IEnumerator RecycleDelayed(float delay)
	{
		yield return new WaitForSecondsRealtime(delay);
		base.gameObject.Recycle();
		yield break;
	}

	// Token: 0x060029CF RID: 10703 RVA: 0x000B5C55 File Offset: 0x000B3E55
	public void SetOwner(GameObject obj)
	{
		this.owner = obj;
	}

	// Token: 0x060029D0 RID: 10704 RVA: 0x000B5C5E File Offset: 0x000B3E5E
	public void SetFollowing(Transform t, Vector3 offset)
	{
		this.followTransform = t;
		this.followOffset = offset;
	}

	// Token: 0x04002A47 RID: 10823
	[SerializeField]
	private TMP_Text label;

	// Token: 0x04002A48 RID: 10824
	[SerializeField]
	private NestedFadeGroup group;

	// Token: 0x04002A49 RID: 10825
	private tk2dSpriteAnimator anim;

	// Token: 0x04002A4A RID: 10826
	private GameObject owner;

	// Token: 0x04002A4B RID: 10827
	private Camera mainCam;

	// Token: 0x04002A4C RID: 10828
	private Camera hudCam;

	// Token: 0x04002A4D RID: 10829
	private Transform followTransform;

	// Token: 0x04002A4E RID: 10830
	private Vector3 followOffset;
}
