using System;
using System.Diagnostics;

namespace TeamCherry.GameCore
{
	// Token: 0x020008B6 RID: 2230
	public sealed class DeltaTimeCalculator
	{
		// Token: 0x06004D41 RID: 19777 RVA: 0x0016B232 File Offset: 0x00169432
		public void Start()
		{
			this._stopwatch.Start();
			this._lastElapsedMilliseconds = this._stopwatch.ElapsedMilliseconds;
		}

		// Token: 0x06004D42 RID: 19778 RVA: 0x0016B250 File Offset: 0x00169450
		public long GetDeltaTimeMillis()
		{
			long elapsedMilliseconds = this._stopwatch.ElapsedMilliseconds;
			long result = elapsedMilliseconds - this._lastElapsedMilliseconds;
			this._lastElapsedMilliseconds = elapsedMilliseconds;
			return result;
		}

		// Token: 0x06004D43 RID: 19779 RVA: 0x0016B278 File Offset: 0x00169478
		public float GetDeltaTimeSeconds()
		{
			return (float)this.GetDeltaTimeMillis() / 1000f;
		}

		// Token: 0x06004D44 RID: 19780 RVA: 0x0016B287 File Offset: 0x00169487
		public void Reset()
		{
			this._stopwatch.Reset();
			this._lastElapsedMilliseconds = 0L;
		}

		// Token: 0x04004E1F RID: 19999
		private Stopwatch _stopwatch = new Stopwatch();

		// Token: 0x04004E20 RID: 20000
		private long _lastElapsedMilliseconds;
	}
}
