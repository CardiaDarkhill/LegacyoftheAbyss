using System;
using TMProOld;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

// Token: 0x02000474 RID: 1140
public class VibrationTesterButton : MonoBehaviour
{
	// Token: 0x060028B5 RID: 10421 RVA: 0x000B306A File Offset: 0x000B126A
	private void Start()
	{
		this.button.onClick.AddListener(new UnityAction(this.OnClick));
	}

	// Token: 0x060028B6 RID: 10422 RVA: 0x000B3088 File Offset: 0x000B1288
	private void OnClick()
	{
		this.Toggle();
	}

	// Token: 0x060028B7 RID: 10423 RVA: 0x000B3090 File Offset: 0x000B1290
	public void SetUp(CameraShakeProfile profile, CameraManagerReference reference)
	{
		this.isPlaying = false;
		this.profile = profile;
		this.reference = reference;
		this.UpdateText();
	}

	// Token: 0x060028B8 RID: 10424 RVA: 0x000B30B0 File Offset: 0x000B12B0
	private void UpdateText()
	{
		string text = this.profile.name + " - " + (this.isPlaying ? "<color=\"green\">Is Playing</color=\"green\">" : "<color=\"red\">Not Playing</color=\"red\">");
		this.text.text = text;
	}

	// Token: 0x060028B9 RID: 10425 RVA: 0x000B30F3 File Offset: 0x000B12F3
	public void Play()
	{
		this.isPlaying = true;
		this.reference.DoShake(this.profile, this, true, true, true);
		this.UpdateText();
	}

	// Token: 0x060028BA RID: 10426 RVA: 0x000B3117 File Offset: 0x000B1317
	public void Stop()
	{
		this.isPlaying = false;
		this.reference.CancelShake(this.profile);
		this.UpdateText();
	}

	// Token: 0x060028BB RID: 10427 RVA: 0x000B3137 File Offset: 0x000B1337
	public void Toggle()
	{
		if (!this.isPlaying)
		{
			this.Play();
			return;
		}
		this.Stop();
	}

	// Token: 0x040024BA RID: 9402
	[SerializeField]
	private Button button;

	// Token: 0x040024BB RID: 9403
	[SerializeField]
	private TextMeshProUGUI text;

	// Token: 0x040024BC RID: 9404
	public CameraShakeProfile profile;

	// Token: 0x040024BD RID: 9405
	public CameraManagerReference reference;

	// Token: 0x040024BE RID: 9406
	public bool isPlaying;
}
