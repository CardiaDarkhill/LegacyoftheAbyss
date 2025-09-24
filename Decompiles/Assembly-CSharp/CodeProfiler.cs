using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020005D1 RID: 1489
public class CodeProfiler : MonoBehaviour
{
	// Token: 0x060034E6 RID: 13542 RVA: 0x000EABF2 File Offset: 0x000E8DF2
	private void Awake()
	{
		this.startTime = Time.time;
		this.displayText = "\n\nTaking initial readings...";
	}

	// Token: 0x060034E7 RID: 13543 RVA: 0x000EAC0A File Offset: 0x000E8E0A
	private void OnGUI()
	{
		GUI.Box(this.displayRect, "Code Profiler");
		GUI.Label(this.displayRect, this.displayText);
	}

	// Token: 0x060034E8 RID: 13544 RVA: 0x000EAC2D File Offset: 0x000E8E2D
	public static void Begin(string id)
	{
		if (!CodeProfiler.recordings.ContainsKey(id))
		{
			CodeProfiler.recordings[id] = new ProfilerRecording(id);
		}
		CodeProfiler.recordings[id].Start();
	}

	// Token: 0x060034E9 RID: 13545 RVA: 0x000EAC5D File Offset: 0x000E8E5D
	public static void End(string id)
	{
		CodeProfiler.recordings[id].Stop();
	}

	// Token: 0x060034EA RID: 13546 RVA: 0x000EAC70 File Offset: 0x000E8E70
	private void Update()
	{
		this.numFrames++;
		if (Time.time > this.nextOutputTime)
		{
			int totalWidth = 10;
			this.displayText = "\n\n";
			float num = (Time.time - this.startTime) * 1000f;
			float num2 = num / (float)this.numFrames;
			float num3 = 1000f / (num / (float)this.numFrames);
			this.displayText += "Avg frame time: ";
			this.displayText = this.displayText + num2.ToString("0.#") + "ms, ";
			this.displayText = this.displayText + num3.ToString("0.#") + " fps \n";
			this.displayText += "Total".PadRight(totalWidth);
			this.displayText += "MS/frame".PadRight(totalWidth);
			this.displayText += "Calls/fra".PadRight(totalWidth);
			this.displayText += "MS/call".PadRight(totalWidth);
			this.displayText += "Label";
			this.displayText += "\n";
			foreach (KeyValuePair<string, ProfilerRecording> keyValuePair in CodeProfiler.recordings)
			{
				ProfilerRecording value = keyValuePair.Value;
				float num4 = value.Seconds * 1000f;
				float num5 = num4 * 100f / num;
				float num6 = num4 / (float)this.numFrames;
				float num7 = num4 / (float)value.Count;
				float num8 = (float)value.Count / (float)this.numFrames;
				this.displayText += (num5.ToString("0.000") + "%").PadRight(totalWidth);
				this.displayText += (num6.ToString("0.000") + "ms").PadRight(totalWidth);
				this.displayText += num8.ToString("0.000").PadRight(totalWidth);
				this.displayText += (num7.ToString("0.0000") + "ms").PadRight(totalWidth);
				this.displayText += value.id;
				this.displayText += "\n";
				value.Reset();
			}
			Debug.Log(this.displayText);
			this.numFrames = 0;
			this.startTime = Time.time;
			this.nextOutputTime = Time.time + 5f;
		}
	}

	// Token: 0x04003858 RID: 14424
	private float startTime;

	// Token: 0x04003859 RID: 14425
	private float nextOutputTime = 5f;

	// Token: 0x0400385A RID: 14426
	private int numFrames;

	// Token: 0x0400385B RID: 14427
	private static Dictionary<string, ProfilerRecording> recordings = new Dictionary<string, ProfilerRecording>();

	// Token: 0x0400385C RID: 14428
	private string displayText;

	// Token: 0x0400385D RID: 14429
	private Rect displayRect = new Rect(10f, 10f, 460f, 300f);
}
