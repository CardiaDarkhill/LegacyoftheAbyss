#nullable enable

using LegacyoftheAbyss.Shade;
using UnityEngine;

/// <summary>
/// Lightweight helper that raises a shade unlock notification when a pickup is collected.
/// Producers can configure the message, category, and suppression key directly in the inspector.
/// </summary>
[AddComponentMenu("Legacy of the Abyss/Shade Unlock Pickup")]
public sealed class ShadeUnlockPickup : MonoBehaviour
{
    [Tooltip("Unique key used to suppress duplicate notifications. If blank the pickup's message is used.")]
    public string notificationKey = string.Empty;

    [Tooltip("Message shown when this pickup is collected.")]
    [TextArea]
    public string message = "Shade unlocked a new ability!";

    [Tooltip("Notification category used for popup styling.")]
    public ShadeRuntime.ShadeUnlockNotificationType notificationType = ShadeRuntime.ShadeUnlockNotificationType.Ability;

    [Tooltip("Optional popup duration override. Zero falls back to the configured default.")]
    public float durationSeconds = 0f;

    [Tooltip("Destroy this pickup object once the notification has been dispatched.")]
    public bool destroyOnPickup = true;

    [Tooltip("If true, only colliders tagged as Player will trigger the pickup notification.")]
    public bool requirePlayerTag = true;

    private bool consumed;

    private void Reset()
    {
        if (string.IsNullOrWhiteSpace(notificationKey) && !string.IsNullOrWhiteSpace(gameObject?.name))
        {
            notificationKey = $"pickup::{gameObject.name}";
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        TryHandleCollider(other);
    }

    private void OnTriggerEnter(Collider other)
    {
        TryHandleCollider(other);
    }

    private void TryHandleCollider(Component? component)
    {
        if (consumed || component == null)
        {
            return;
        }

        if (requirePlayerTag && !IsPlayerComponent(component))
        {
            return;
        }

        TriggerNotification();
    }

    private static bool IsPlayerComponent(Component component)
    {
        try
        {
            var go = component.gameObject;
            if (go == null)
            {
                return false;
            }

            if (go.CompareTag("Player"))
            {
                return true;
            }

            var root = go.transform?.root;
            if (root != null && root.CompareTag("Player"))
            {
                return true;
            }
        }
        catch
        {
        }

        return false;
    }

    /// <summary>
    /// Dispatches the configured notification immediately. Useful for scripted pickups.
    /// </summary>
    public void TriggerNotification(string? overrideMessage = null)
    {
        if (consumed)
        {
            return;
        }

        consumed = true;

        string resolvedMessage = !string.IsNullOrWhiteSpace(overrideMessage) ? overrideMessage : message;
        if (!string.IsNullOrWhiteSpace(resolvedMessage))
        {
            string key = !string.IsNullOrWhiteSpace(notificationKey) ? notificationKey : resolvedMessage;
            float duration = durationSeconds > 0f ? durationSeconds : ShadeRuntime.ShadeUnlockNotification.DefaultDuration;
            ShadeRuntime.EnqueueNotification(key, resolvedMessage, notificationType, duration);
        }

        if (destroyOnPickup)
        {
            try
            {
                Destroy(gameObject);
            }
            catch
            {
            }
        }
    }

    /// <summary>
    /// Allows re-arming the pickup in editor/testing scenarios.
    /// </summary>
    public void ResetPickup()
    {
        consumed = false;
    }
}

