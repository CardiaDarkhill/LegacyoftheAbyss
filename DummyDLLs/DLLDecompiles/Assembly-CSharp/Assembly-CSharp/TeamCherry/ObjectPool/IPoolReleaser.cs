using System;

namespace TeamCherry.ObjectPool
{
	// Token: 0x020008C3 RID: 2243
	public interface IPoolReleaser<T>
	{
		// Token: 0x06004D9A RID: 19866
		void Release(T element);
	}
}
