using System;
using UnityEngine;

namespace TeamCherry.PS5
{
	// Token: 0x020008A3 RID: 2211
	[CreateAssetMenu(menuName = "Platform/PS5/Vibration Asset")]
	public sealed class PS5VibrationData : ScriptableObject
	{
		// Token: 0x1700090C RID: 2316
		// (get) Token: 0x06004C89 RID: 19593 RVA: 0x0016838C File Offset: 0x0016658C
		public AudioClip VibrationClip
		{
			get
			{
				return this.vibrationClip;
			}
		}

		// Token: 0x1700090D RID: 2317
		// (get) Token: 0x06004C8A RID: 19594 RVA: 0x00168394 File Offset: 0x00166594
		public AudioClip ClipSource
		{
			get
			{
				return this.audioClipSource.Asset;
			}
		}

		// Token: 0x1700090E RID: 2318
		// (get) Token: 0x06004C8B RID: 19595 RVA: 0x001683A1 File Offset: 0x001665A1
		public bool PreventOverride
		{
			get
			{
				return this.preventOverride;
			}
		}

		// Token: 0x06004C8C RID: 19596 RVA: 0x001683A9 File Offset: 0x001665A9
		public static implicit operator AudioClip(PS5VibrationData vibrationData)
		{
			if (vibrationData.VibrationClip)
			{
				return vibrationData.VibrationClip;
			}
			return vibrationData.ClipSource;
		}

		// Token: 0x04004DB9 RID: 19897
		[SerializeField]
		private AudioClip vibrationClip;

		// Token: 0x04004DBA RID: 19898
		[SerializeField]
		private AssetLinker<AudioClip> audioClipSource = new AssetLinker<AudioClip>();

		// Token: 0x04004DBB RID: 19899
		[SerializeField]
		private bool preventOverride;
	}
}
