#nullable enable

using UnityEngine;
using LegacyoftheAbyss.Shade;

/// <summary>
/// Moving the cursor up out of the charm grid and into the Equipped row, which is how Hollow Knight
/// lets a charm be taken off without hunting for it among everything owned.
/// <para>
/// A second view onto the same selection rather than a selection of its own. Focusing a slot points
/// <c>selectedIndex</c> at that charm's grid entry, so the detail panel, the notch meter, the "seen"
/// marking and Submit all go on working unchanged - what the focus decides is only where the
/// highlight is drawn and what the arrow keys mean.
/// </para>
/// </summary>
internal sealed partial class ShadeInventoryPane
{
    /// <summary>Which equipped slot the cursor is on, or -1 while it is down in the grid.</summary>
    private int equippedFocusIndex = -1;

    /// <summary>Where in the grid the cursor came from, so Down puts it back where it was.</summary>
    private int gridIndexBeforeEquippedFocus = -1;

    internal bool EquippedRowFocused => equippedFocusIndex >= 0;

    /// <summary>How many slots of the equipped row are actually holding a charm.</summary>
    private int EquippedSlotCount()
    {
        int count = 0;
        for (int i = 0; i < equippedDisplayIds.Count; i++)
        {
            if (equippedDisplayIds[i].HasValue)
            {
                count++;
            }
        }

        return count;
    }

    /// <summary>
    /// Moves the cursor into the equipped row, or reports that there is nothing there to move into.
    /// Called when Up is pressed on the grid's top row, where it previously did nothing.
    /// </summary>
    private bool TryFocusEquippedRow()
    {
        if (EquippedRowFocused || EquippedSlotCount() == 0)
        {
            return false;
        }

        gridIndexBeforeEquippedFocus = selectedIndex;
        FocusEquippedSlot(0);
        return true;
    }

    /// <summary>Puts the cursor back in the grid, on the entry it left.</summary>
    private void LeaveEquippedRow()
    {
        if (!EquippedRowFocused)
        {
            return;
        }

        int restore = gridIndexBeforeEquippedFocus;
        equippedFocusIndex = -1;
        gridIndexBeforeEquippedFocus = -1;

        SelectIndex(restore >= 0 ? restore : selectedIndex);
    }

    /// <summary>Left and right along the row. Clamped rather than wrapping, as the grid is.</summary>
    private void MoveWithinEquippedRow(int direction)
    {
        int count = EquippedSlotCount();
        if (count == 0)
        {
            LeaveEquippedRow();
            return;
        }

        int target = Mathf.Clamp(equippedFocusIndex + direction, 0, count - 1);
        if (target != equippedFocusIndex)
        {
            FocusEquippedSlot(target);
        }
    }

    /// <summary>
    /// Focuses one slot and points the grid selection at the same charm, which is what makes every
    /// other part of the pane - the description, the notch meter, Submit - answer for it.
    /// </summary>
    private void FocusEquippedSlot(int slot)
    {
        int count = EquippedSlotCount();
        if (count == 0)
        {
            LeaveEquippedRow();
            return;
        }

        equippedFocusIndex = Mathf.Clamp(slot, 0, count - 1);

        var id = equippedDisplayIds.Count > equippedFocusIndex ? equippedDisplayIds[equippedFocusIndex] : null;
        int gridIndex = id.HasValue ? GridIndexForCharm(id.Value) : -1;

        // SelectIndex draws the highlight, and reads EquippedRowFocused to know it belongs on the
        // row rather than on the grid entry it is selecting.
        SelectIndex(gridIndex >= 0 ? gridIndex : selectedIndex);
    }

    private int GridIndexForCharm(ShadeCharmId id)
    {
        for (int i = 0; i < entries.Count; i++)
        {
            if (entries[i].Id == id)
            {
                return i;
            }
        }

        return -1;
    }

    /// <summary>The rect of one equipped slot, or null when that slot is not drawing anything.</summary>
    private RectTransform? EquippedSlotRect(int slot)
    {
        if (slot < 0 || slot >= equippedIcons.Count)
        {
            return null;
        }

        var icon = equippedIcons[slot];
        return icon != null && icon.gameObject.activeInHierarchy ? icon.rectTransform : null;
    }

    /// <summary>
    /// Draws the cursor on the focused slot. Returns false when the row cannot hold the cursor after
    /// all - the slot has gone, which is what unequipping the last charm in the row looks like - so
    /// the caller can fall back to the grid.
    /// </summary>
    private bool TryPositionEquippedHighlight(RectTransform highlightRect)
    {
        var slotRect = EquippedSlotRect(equippedFocusIndex);
        if (slotRect == null)
        {
            return false;
        }

        highlightRect.gameObject.SetActive(true);
        PositionHighlight(highlightRect, slotRect);
        return true;
    }

    /// <summary>
    /// Re-seats the cursor after the row itself has changed, which it does on every equip and
    /// unequip. Taking a charm off shortens the row, so the slot the cursor was on may no longer
    /// exist; it steps back to the last one, and out of the row entirely once it is empty.
    /// </summary>
    private void RefreshEquippedFocusAfterChange()
    {
        if (!EquippedRowFocused)
        {
            return;
        }

        int count = EquippedSlotCount();
        if (count == 0)
        {
            LeaveEquippedRow();
            return;
        }

        FocusEquippedSlot(Mathf.Min(equippedFocusIndex, count - 1));
    }

    /// <summary>Drops the cursor back to the grid. Called when the pane is opened or closed.</summary>
    private void ResetEquippedFocus()
    {
        equippedFocusIndex = -1;
        gridIndexBeforeEquippedFocus = -1;
    }
}
