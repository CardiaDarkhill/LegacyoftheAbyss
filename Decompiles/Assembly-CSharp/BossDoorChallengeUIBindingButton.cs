using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Token: 0x020003A8 RID: 936
public class BossDoorChallengeUIBindingButton : MonoBehaviour, ISubmitHandler, IEventSystemHandler, ICancelHandler, IPointerClickHandler
{
	// Token: 0x14000060 RID: 96
	// (add) Token: 0x06001F81 RID: 8065 RVA: 0x0008FF88 File Offset: 0x0008E188
	// (remove) Token: 0x06001F82 RID: 8066 RVA: 0x0008FFC0 File Offset: 0x0008E1C0
	public event BossDoorChallengeUIBindingButton.OnSelectionEvent OnButtonSelected;

	// Token: 0x14000061 RID: 97
	// (add) Token: 0x06001F83 RID: 8067 RVA: 0x0008FFF8 File Offset: 0x0008E1F8
	// (remove) Token: 0x06001F84 RID: 8068 RVA: 0x00090030 File Offset: 0x0008E230
	public event BossDoorChallengeUIBindingButton.OnCancelEvent OnButtonCancelled;

	// Token: 0x1700032E RID: 814
	// (get) Token: 0x06001F85 RID: 8069 RVA: 0x00090065 File Offset: 0x0008E265
	public bool Selected
	{
		get
		{
			return this.selected;
		}
	}

	// Token: 0x06001F86 RID: 8070 RVA: 0x0009006D File Offset: 0x0008E26D
	private void Awake()
	{
		if (this.iconImage)
		{
			this.defaultSprite = this.iconImage.sprite;
		}
	}

	// Token: 0x06001F87 RID: 8071 RVA: 0x00090090 File Offset: 0x0008E290
	public void Reset()
	{
		this.selected = false;
		if (this.chainAnimator)
		{
			this.chainAnimator.Play("Unbind", 0, 1f);
		}
		if (this.iconImage)
		{
			this.iconImage.sprite = this.defaultSprite;
			this.iconImage.SetNativeSize();
		}
		base.StartCoroutine(this.SetAnimSizeDelayed("Unbind", 1f));
		if (this.bindAllEffect)
		{
			this.bindAllEffect.SetActive(false);
		}
	}

	// Token: 0x06001F88 RID: 8072 RVA: 0x00090120 File Offset: 0x0008E320
	private void OnDisable()
	{
		if (this.isCounted)
		{
			BossDoorChallengeUIBindingButton.currentAmount--;
			this.isCounted = false;
		}
	}

	// Token: 0x06001F89 RID: 8073 RVA: 0x00090140 File Offset: 0x0008E340
	public void OnSubmit(BaseEventData eventData)
	{
		this.selected = !this.selected;
		if (this.iconImage)
		{
			this.iconImage.sprite = (this.selected ? this.selectedSprite : this.defaultSprite);
			this.iconImage.SetNativeSize();
		}
		if (this.iconAnimator)
		{
			this.iconAnimator.Play("Select");
		}
		if (this.chainAnimator)
		{
			this.chainAnimator.Play(this.selected ? "Bind" : "Unbind");
		}
		if (this.selected && !this.isCounted)
		{
			this.isCounted = true;
			BossDoorChallengeUIBindingButton.currentAmount++;
		}
		else if (!this.selected && this.isCounted)
		{
			BossDoorChallengeUIBindingButton.currentAmount--;
			this.isCounted = false;
		}
		AudioEvent audioEvent = this.selected ? this.selectedSound : this.deselectedSound;
		float num = Mathf.Lerp(this.pitchShiftMin, this.pitchShiftMax, (float)BossDoorChallengeUIBindingButton.currentAmount / (float)this.maxAmount);
		audioEvent.PitchMin += num;
		audioEvent.PitchMax += num;
		audioEvent.SpawnAndPlayOneShot(this.audioSourcePrefab, base.transform.position, null);
		GameCameras.instance.cameraShakeFSM.SendEvent("EnemyKillShake");
		if (this.OnButtonSelected != null)
		{
			this.OnButtonSelected();
		}
	}

	// Token: 0x06001F8A RID: 8074 RVA: 0x000902B8 File Offset: 0x0008E4B8
	public void SetAllSelected(bool value)
	{
		if (this.iconImage)
		{
			if (value)
			{
				this.iconImage.sprite = (this.allSelectedSprite ? this.allSelectedSprite : this.selectedSprite);
			}
			else
			{
				this.iconImage.sprite = (this.selected ? this.selectedSprite : this.defaultSprite);
			}
			this.iconImage.SetNativeSize();
		}
		if (this.iconAnimator)
		{
			this.iconAnimator.Play("Select");
		}
		base.StartCoroutine(this.SetAnimSizeDelayed(value ? "Bind All" : (this.selected ? "Bind" : "Unbind"), (float)((!value && this.selected) ? 1 : 0)));
		if (this.bindAllEffect)
		{
			this.bindAllEffect.SetActive(value);
		}
	}

	// Token: 0x06001F8B RID: 8075 RVA: 0x0009039A File Offset: 0x0008E59A
	private IEnumerator SetAnimSizeDelayed(string anim, float normalizedTime)
	{
		if (this.chainAnimator)
		{
			this.chainAnimator.Play(anim, 0, normalizedTime);
		}
		float scale = this.chainAnimator.transform.localScale.x;
		this.chainAnimator.transform.SetScaleX(0f);
		yield return null;
		Image component = this.chainAnimator.GetComponent<Image>();
		if (component)
		{
			component.SetNativeSize();
		}
		this.chainAnimator.transform.SetScaleX(scale);
		yield break;
	}

	// Token: 0x06001F8C RID: 8076 RVA: 0x000903B7 File Offset: 0x0008E5B7
	public void OnCancel(BaseEventData eventData)
	{
		if (this.OnButtonCancelled != null)
		{
			this.OnButtonCancelled();
		}
	}

	// Token: 0x06001F8D RID: 8077 RVA: 0x000903CC File Offset: 0x0008E5CC
	public void OnPointerClick(PointerEventData eventData)
	{
		this.OnSubmit(eventData);
	}

	// Token: 0x04001E8F RID: 7823
	public Image iconImage;

	// Token: 0x04001E90 RID: 7824
	public Animator iconAnimator;

	// Token: 0x04001E91 RID: 7825
	private Sprite defaultSprite;

	// Token: 0x04001E92 RID: 7826
	public Sprite selectedSprite;

	// Token: 0x04001E93 RID: 7827
	public Sprite allSelectedSprite;

	// Token: 0x04001E94 RID: 7828
	public Animator chainAnimator;

	// Token: 0x04001E95 RID: 7829
	public GameObject bindAllEffect;

	// Token: 0x04001E96 RID: 7830
	public AudioSource audioSourcePrefab;

	// Token: 0x04001E97 RID: 7831
	public AudioEvent selectedSound;

	// Token: 0x04001E98 RID: 7832
	public AudioEvent deselectedSound;

	// Token: 0x04001E99 RID: 7833
	public float pitchShiftMin;

	// Token: 0x04001E9A RID: 7834
	public float pitchShiftMax = 0.5f;

	// Token: 0x04001E9B RID: 7835
	public int maxAmount = 4;

	// Token: 0x04001E9C RID: 7836
	private static int currentAmount;

	// Token: 0x04001E9D RID: 7837
	private bool isCounted;

	// Token: 0x04001E9E RID: 7838
	private bool selected;

	// Token: 0x0200165E RID: 5726
	// (Invoke) Token: 0x060089E2 RID: 35298
	public delegate void OnSelectionEvent();

	// Token: 0x0200165F RID: 5727
	// (Invoke) Token: 0x060089E6 RID: 35302
	public delegate void OnCancelEvent();
}
