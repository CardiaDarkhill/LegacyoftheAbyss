using System;

// Token: 0x020005F8 RID: 1528
public interface ITrackTriggerObject
{
	// Token: 0x06003684 RID: 13956
	void OnTrackTriggerEntered(TrackTriggerObjects enteredRange);

	// Token: 0x06003685 RID: 13957
	void OnTrackTriggerExited(TrackTriggerObjects exitedRange);
}
