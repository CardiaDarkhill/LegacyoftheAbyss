#nullable enable

using System.IO;
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
            tint = Color.white;
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
                ShadeRuntime.EnqueueNotification(key, PopupName, ShadeUnlockNotificationType.Ability, icon: EnsureDefaultSprite());
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

            s_defaultSprite = TryLoadAssetSprite() ?? CreateFallbackSprite();
            return s_defaultSprite;
        }

        private static Sprite? TryLoadAssetSprite()
        {
            try
            {
                if (!ModPaths.TryGetAssetPath(out var path, "CharmIcons", "Charm_Notch.png") || !File.Exists(path))
                {
                    return null;
                }

                var bytes = File.ReadAllBytes(path);
                if (bytes == null || bytes.Length == 0)
                {
                    return null;
                }

                var tex = new Texture2D(2, 2, TextureFormat.ARGB32, false);
                if (!ImageConversion.LoadImage(tex, bytes, false))
                {
                    Object.Destroy(tex);
                    return null;
                }

                tex.filterMode = FilterMode.Point;
                tex.wrapMode = TextureWrapMode.Clamp;
                tex.hideFlags = HideFlags.HideAndDontSave;
                tex.name = Path.GetFileName(path);

                var sprite = Sprite.Create(tex, new Rect(0f, 0f, tex.width, tex.height), new Vector2(0.5f, 0.5f), 64f);
                sprite.hideFlags = HideFlags.HideAndDontSave;
                sprite.name = Path.GetFileNameWithoutExtension(path);
                return sprite;
            }
            catch
            {
                return null;
            }
        }

        private static Sprite CreateFallbackSprite()
        {
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

            var sprite = Sprite.Create(tex, new Rect(0f, 0f, tex.width, tex.height), new Vector2(0.5f, 0.5f), 32f);
            sprite.hideFlags = HideFlags.HideAndDontSave;
            sprite.name = "ShadeNotchPickup";
            return sprite;
        }
    }
}
