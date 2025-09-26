#nullable enable

using UnityEngine;

namespace LegacyoftheAbyss.Shade
{
    internal sealed class ShadeNotchSavedItem : SavedItem
    {
        private const string NotificationKeyPrefix = "shade::notch_pickup::";
        private const string PopupName = "Charm Notch";
        private const int MaxCapacity = 20;

        private static Sprite? s_defaultSprite;

        private int _targetCapacity;
        private int _increment;

        internal int TargetCapacity => _targetCapacity;

        internal static ShadeNotchSavedItem Create(int targetCapacity, int increment)
        {
            var item = CreateInstance<ShadeNotchSavedItem>();
            item.hideFlags = HideFlags.HideAndDontSave;
            item.Initialize(targetCapacity, increment);
            return item;
        }

        internal static Sprite ResolveSprite(out Color tint)
        {
            tint = new Color(0.85f, 0.95f, 1f, 1f);
            return EnsureDefaultSprite();
        }

        public override bool IsUnique => true;

        public override bool CanGetMore()
        {
            try
            {
                return ShadeRuntime.GetNotchCapacity() < _targetCapacity;
            }
            catch
            {
                return true;
            }
        }

        public override void Get(bool showPopup = true)
        {
            int current = 0;
            try
            {
                current = ShadeRuntime.GetNotchCapacity();
            }
            catch
            {
            }

            if (current >= _targetCapacity)
            {
                return;
            }

            int increment = Mathf.Max(1, _increment);
            int desired = Mathf.Clamp(Mathf.Max(_targetCapacity, current + increment), 0, MaxCapacity);

            bool changed = false;
            try
            {
                changed = ShadeRuntime.SetNotchCapacity(desired);
            }
            catch
            {
            }

            if (changed)
            {
                TryQueueNotification(desired);
            }
        }

        public override string GetPopupName() => PopupName;

        public override Sprite GetPopupIcon() => EnsureDefaultSprite();

        private void Initialize(int targetCapacity, int increment)
        {
            _targetCapacity = Mathf.Clamp(targetCapacity, 0, MaxCapacity);
            _increment = Mathf.Max(1, increment);
            name = $"ShadeNotch_Target{_targetCapacity}_SavedItem";
        }

        private void TryQueueNotification(int newCapacity)
        {
            try
            {
                string key = $"{NotificationKeyPrefix}{_targetCapacity}";
                string message = $"Shade unlocked a new charm notch ({newCapacity} capacity).";
                ShadeRuntime.EnqueueNotification(key, message, ShadeUnlockNotificationType.Ability, icon: EnsureDefaultSprite());
            }
            catch
            {
            }
        }

        private static Sprite EnsureDefaultSprite()
        {
            if (s_defaultSprite != null)
            {
                return s_defaultSprite;
            }

            var tex = new Texture2D(20, 20, TextureFormat.ARGB32, false)
            {
                hideFlags = HideFlags.HideAndDontSave,
                filterMode = FilterMode.Point
            };

            Color inner = new Color(0.35f, 0.6f, 1f, 1f);
            Color border = new Color(0.12f, 0.24f, 0.48f, 1f);

            for (int x = 0; x < tex.width; x++)
            {
                for (int y = 0; y < tex.height; y++)
                {
                    float dx = (x + 0.5f) / tex.width - 0.5f;
                    float dy = (y + 0.5f) / tex.height - 0.5f;
                    float dist = Mathf.Sqrt(dx * dx + dy * dy);
                    Color pixel = dist <= 0.35f
                        ? inner
                        : dist <= 0.45f
                            ? border
                            : new Color(0f, 0f, 0f, 0f);
                    tex.SetPixel(x, y, pixel);
                }
            }

            tex.Apply();

            s_defaultSprite = Sprite.Create(tex, new Rect(0f, 0f, tex.width, tex.height), new Vector2(0.5f, 0.5f), 32f);
            s_defaultSprite.hideFlags = HideFlags.HideAndDontSave;
            s_defaultSprite.name = "ShadeNotchPickup";
            return s_defaultSprite;
        }
    }
}
