#nullable enable

using UnityEngine;

namespace LegacyoftheAbyss.Shade
{
    internal sealed class ShadeCharmSavedItem : SavedItem
    {
        private const string NotificationKeyPrefix = "shade::charm_pickup::";

        private static Sprite? s_defaultSprite;

        private ShadeCharmId _charmId;

        internal ShadeCharmId CharmId => _charmId;

        internal static ShadeCharmSavedItem Create(ShadeCharmId charmId)
        {
            var item = CreateInstance<ShadeCharmSavedItem>();
            item.hideFlags = HideFlags.HideAndDontSave;
            item.Initialize(charmId);
            return item;
        }

        internal static Sprite ResolveCharmSprite(ShadeCharmId charmId, out Color tint)
        {
            if (TryGetDefinition(charmId, out var definition))
            {
                tint = definition.Icon != null ? Color.white : definition.FallbackTint;
                return definition.Icon ?? EnsureDefaultSprite();
            }

            tint = Color.white;
            return EnsureDefaultSprite();
        }

        public override bool IsUnique => true;

        public override bool CanGetMore()
        {
            try
            {
                return !ShadeRuntime.IsCharmCollected(_charmId);
            }
            catch
            {
                return true;
            }
        }

        public override void Get(bool showPopup = true)
        {
            bool collected = false;
            try
            {
                collected = ShadeRuntime.TryCollectCharm(_charmId);
            }
            catch
            {
            }

            if (collected)
            {
                TryQueueNotification();
            }
        }

        public override string GetPopupName()
        {
            if (TryGetDefinition(out var definition))
            {
                return definition.DisplayName;
            }

            return _charmId.ToString();
        }

        public override Sprite GetPopupIcon()
        {
            return ResolveCharmSprite(_charmId, out _);
        }

        private void Initialize(ShadeCharmId charmId)
        {
            _charmId = charmId;
            name = $"ShadeCharm_{charmId}_SavedItem";
        }

        private void TryQueueNotification()
        {
            if (!TryGetDefinition(out var definition))
            {
                return;
            }

            try
            {
                string message = definition.DisplayName;
                string key = $"{NotificationKeyPrefix}{_charmId}";
                ShadeRuntime.EnqueueNotification(key, message, ShadeUnlockNotificationType.Charm, icon: definition.Icon);
            }
            catch
            {
            }
        }

        private static bool TryGetDefinition(ShadeCharmId charmId, out ShadeCharmDefinition definition)
        {
            try
            {
                var inventory = ShadeRuntime.Charms;
                if (inventory != null)
                {
                    definition = inventory.GetDefinition(charmId);
                    return true;
                }
            }
            catch
            {
            }

            definition = null!;
            return false;
        }

        private bool TryGetDefinition(out ShadeCharmDefinition definition)
        {
            return TryGetDefinition(_charmId, out definition);
        }

        private static Sprite EnsureDefaultSprite()
        {
            if (s_defaultSprite != null)
            {
                return s_defaultSprite;
            }

            var tex = new Texture2D(16, 16, TextureFormat.ARGB32, false);
            for (int x = 0; x < tex.width; x++)
            {
                for (int y = 0; y < tex.height; y++)
                {
                    float dx = (x + 0.5f) / tex.width - 0.5f;
                    float dy = (y + 0.5f) / tex.height - 0.5f;
                    float dist = Mathf.Sqrt(dx * dx + dy * dy);
                    float alpha = dist <= 0.45f ? 1f : Mathf.Clamp01(0.55f - dist) / 0.1f;
                    tex.SetPixel(x, y, new Color(0.2f, 0.25f, 0.35f, alpha));
                }
            }

            tex.Apply();
            tex.filterMode = FilterMode.Point;
            tex.hideFlags = HideFlags.HideAndDontSave;

            s_defaultSprite = Sprite.Create(tex, new Rect(0f, 0f, tex.width, tex.height), new Vector2(0.5f, 0.5f), 32f);
            s_defaultSprite.hideFlags = HideFlags.HideAndDontSave;
            s_defaultSprite.name = "ShadeCharmDefaultPickup";
            return s_defaultSprite;
        }
    }
}

