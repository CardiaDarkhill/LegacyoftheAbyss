using System;
using System.Globalization;
using System.Text;
using InControl;
using LegacyoftheAbyss.Shade.Ai;
using UnityEngine;

public enum ShadeAction
{
    MoveLeft,
    MoveRight,
    MoveUp,
    MoveDown,
    Fire,
    Nail,
    NailUp,
    NailDown,
    Teleport,
    Focus,
    Sprint,
    /// <summary>
    /// Opens the targeting reticle that tells an AI-driven Shade where to stand. Bound on Hornet's
    /// side of the controls rather than the Shade's, because in AI mode nobody is holding the
    /// Shade's own inputs - see LegacyHelper.ShadeController.AiCommand.cs.
    /// </summary>
    CommandShade,
    /// <summary>
    /// Swaps the companion between the Knight and the Shade on the spot, so a player can pick a
    /// body for the room they are in rather than for the session. Kept out of the debug block
    /// below deliberately: it is an ordinary control and takes part in device reservation.
    /// </summary>
    SwapCharacter,
    // Developer-only utility actions. Only ever surfaced in the Controls menu when
    // ModConfig.Instance.debugKeysEnabled is on (see BuildControlsMenu), and only ever
    // read when the same flag is on (see SimpleHUD.HandleDebugKeys), so an ordinary
    // player never sees or triggers these regardless of what they're bound to.
    DebugDamageShade,
    DebugHealShade,
    DebugSoulIncrease,
    DebugSoulDecrease,
    DebugSoulReset
}

public enum ShadeBindingOptionType
{
    None,
    Key,
    Controller
}

[Serializable]
public struct ShadeBindingOption
{
    public ShadeBindingOptionType type;
    public KeyCode key;
    public InputControlType control;
    public int controllerDevice;

    public static ShadeBindingOption None() => new()
    {
        type = ShadeBindingOptionType.None,
        key = KeyCode.None,
        control = InputControlType.None,
        controllerDevice = -1
    };

    public static ShadeBindingOption FromKey(KeyCode keyCode) => new()
    {
        type = ShadeBindingOptionType.Key,
        key = keyCode,
        control = InputControlType.None,
        controllerDevice = -1
    };

    public static ShadeBindingOption FromControl(InputControlType controlType, int controllerIndex = -1) => new()
    {
        type = ShadeBindingOptionType.Controller,
        key = KeyCode.None,
        control = controlType,
        controllerDevice = controllerIndex
    };
}

[Serializable]
public class ShadeBinding
{
    public ShadeBindingOption primary = ShadeBindingOption.None();
    public ShadeBindingOption secondary = ShadeBindingOption.None();

    public ShadeBinding()
    {
    }

    public ShadeBinding(ShadeBindingOption first, ShadeBindingOption second)
    {
        primary = first;
        secondary = second;
    }

    public ShadeBinding Clone() => new ShadeBinding(primary, secondary);
}

[Serializable]
public class ShadeInputConfig
{
    public int controllerDeviceIndex = 1;
    public float controllerDeadzone = 0.25f;

    public ShadeBinding moveLeft = new();
    public ShadeBinding moveRight = new();
    public ShadeBinding moveUp = new();
    public ShadeBinding moveDown = new();
    public ShadeBinding fire = new();
    public ShadeBinding nail = new();
    public ShadeBinding nailUp = new();
    public ShadeBinding nailDown = new();
    public ShadeBinding teleport = new();
    public ShadeBinding focus = new();
    public ShadeBinding sprint = new();
    public ShadeBinding commandShade = new();
    public ShadeBinding swapCharacter = new();
    public ShadeBinding debugDamageShade = new();
    public ShadeBinding debugHealShade = new();
    public ShadeBinding debugSoulIncrease = new();
    public ShadeBinding debugSoulDecrease = new();
    public ShadeBinding debugSoulReset = new();

    public ShadeInputConfig()
    {
        ResetToDefaults();
    }

    public static ShadeInputConfig CreateDefault() => new ShadeInputConfig();

    public void ResetToDefaults()
    {
        controllerDeviceIndex = 1;
        NormaliseDeadzone();

        ApplyKeyboardBindings();

        // Middle mouse and the left stick of the *first* pad: this is Hornet's control, not the
        // Shade player's, so it is pinned to device 0 rather than following controllerDeviceIndex.
        commandShade = new ShadeBinding(
            ShadeBindingOption.FromKey(KeyCode.Mouse2),
            ShadeBindingOption.FromControl(InputControlType.LeftStickButton, 0));

        // Right stick click, which Silksong itself leaves free, alongside the keyboard key
        // ApplyKeyboardBindings just gave the same action. Both are only a starting point:
        // DropCollidingDefaults takes either of them back off a config where the player has already
        // spent that control on something else.
        swapCharacter.secondary = ShadeBindingOption.FromControl(InputControlType.RightStickButton);

        // Matches the defaults these carried as hardcoded, unrebindable KeyCode constants
        // in SimpleHUD before they moved into the normal binding system -- unbound except
        // for soul reset, so existing behaviour doesn't change until someone rebinds them.
        // The row of keys to the right of the letters, in the order they sit on the board, so
        // the whole debug set is reachable without looking: [ and ] hurt and heal, - and = take
        // and give soul, \ empties it.
        debugDamageShade = new ShadeBinding(ShadeBindingOption.FromKey(KeyCode.LeftBracket), ShadeBindingOption.None());
        debugHealShade = new ShadeBinding(ShadeBindingOption.FromKey(KeyCode.RightBracket), ShadeBindingOption.None());
        debugSoulIncrease = new ShadeBinding(ShadeBindingOption.FromKey(KeyCode.Equals), ShadeBindingOption.None());
        debugSoulDecrease = new ShadeBinding(ShadeBindingOption.FromKey(KeyCode.Minus), ShadeBindingOption.None());
        debugSoulReset = new ShadeBinding(ShadeBindingOption.FromKey(KeyCode.Backslash), ShadeBindingOption.None());
    }

    /// <summary>
    /// The companion's whole layout on a pad. Applied when it moves onto one from the keyboard,
    /// where there is nothing to carry across: the two device kinds share no controls, so a move
    /// between them has to start from a layout rather than re-point the one that is there.
    /// </summary>
    public void ApplyControllerLayout(int deviceIndex)
    {
        controllerDeviceIndex = deviceIndex;
        NormaliseDeadzone();

        moveLeft = new ShadeBinding(ShadeBindingOption.FromControl(InputControlType.LeftStickLeft), ShadeBindingOption.None());
        moveRight = new ShadeBinding(ShadeBindingOption.FromControl(InputControlType.LeftStickRight), ShadeBindingOption.None());
        moveUp = new ShadeBinding(ShadeBindingOption.FromControl(InputControlType.LeftStickUp), ShadeBindingOption.None());
        moveDown = new ShadeBinding(ShadeBindingOption.FromControl(InputControlType.LeftStickDown), ShadeBindingOption.None());
        fire = new ShadeBinding(ShadeBindingOption.FromControl(InputControlType.RightBumper), ShadeBindingOption.None());
        nail = new ShadeBinding(ShadeBindingOption.FromControl(InputControlType.Action3), ShadeBindingOption.None());
        nailUp = new ShadeBinding(ShadeBindingOption.FromControl(InputControlType.Action4), ShadeBindingOption.None());
        nailDown = new ShadeBinding(ShadeBindingOption.FromControl(InputControlType.Action1), ShadeBindingOption.None());
        teleport = new ShadeBinding(ShadeBindingOption.FromControl(InputControlType.LeftStickButton), ShadeBindingOption.None());
        focus = new ShadeBinding(ShadeBindingOption.FromControl(InputControlType.Action2), ShadeBindingOption.None());
        sprint = new ShadeBinding(ShadeBindingOption.FromControl(InputControlType.RightTrigger), ShadeBindingOption.None());
        swapCharacter = new ShadeBinding(ShadeBindingOption.FromControl(InputControlType.RightStickButton), ShadeBindingOption.None());
    }

    /// <summary>Keeps the deadzone usable when the stored value is zero or out of range.</summary>
    private void NormaliseDeadzone()
    {
        controllerDeadzone = Mathf.Clamp(controllerDeadzone <= 0f ? 0.25f : controllerDeadzone, 0.01f, 1f);
    }

    /// <summary>
    /// The companion's whole layout on the keyboard. Applied when it moves onto one from a pad, and
    /// the keyboard half of <see cref="ResetToDefaults"/>, so there is one keyboard layout rather
    /// than two that can drift.
    /// </summary>
    public void ApplyKeyboardLayout()
    {
        controllerDeviceIndex = -1;
        NormaliseDeadzone();

        ApplyKeyboardBindings();
        commandShade = new ShadeBinding(ShadeBindingOption.FromKey(KeyCode.Mouse2), ShadeBindingOption.None());
    }

    /// <summary>
    /// Hollow Knight's own keyboard defaults, row for row, so a player arriving from that game
    /// already knows the companion: arrows to move, Z jump, X attack, A focus, F quick cast, C dash.
    /// <para>
    /// The two actions Hollow Knight has no row for take the keys its rows we do not implement
    /// leave free under the same resting hand - <see cref="ShadeAction.Teleport"/> on Dream Nail's
    /// D, <see cref="ShadeAction.NailUp"/> on Super Dash's S - and the character swap sits on V,
    /// beside the dash key.
    /// </para>
    /// <para>
    /// This is deliberately the side of the board Hornet's own keyboard controls use, which the
    /// keypad layout it replaces was written to avoid. The companion only holds the keyboard while
    /// Hornet is on a pad, and that preset clears her keyboard bindings outright
    /// (<c>HornetInput.ApplyControllerDefaults</c>), so there is nothing left on these keys to
    /// collide with.
    /// </para>
    /// </summary>
    private void ApplyKeyboardBindings()
    {
        moveLeft = new ShadeBinding(ShadeBindingOption.FromKey(KeyCode.LeftArrow), ShadeBindingOption.None());
        moveRight = new ShadeBinding(ShadeBindingOption.FromKey(KeyCode.RightArrow), ShadeBindingOption.None());
        moveUp = new ShadeBinding(ShadeBindingOption.FromKey(KeyCode.UpArrow), ShadeBindingOption.None());
        moveDown = new ShadeBinding(ShadeBindingOption.FromKey(KeyCode.DownArrow), ShadeBindingOption.None());
        nail = new ShadeBinding(ShadeBindingOption.FromKey(KeyCode.X), ShadeBindingOption.None());
        // Jump. The Knight has no down-slash of its own - it aims with the movement keys - so the
        // two share this slot; see ShadeController.KnightJumpAction.
        nailDown = new ShadeBinding(ShadeBindingOption.FromKey(KeyCode.Z), ShadeBindingOption.None());
        nailUp = new ShadeBinding(ShadeBindingOption.FromKey(KeyCode.S), ShadeBindingOption.None());
        focus = new ShadeBinding(ShadeBindingOption.FromKey(KeyCode.A), ShadeBindingOption.None());
        fire = new ShadeBinding(ShadeBindingOption.FromKey(KeyCode.F), ShadeBindingOption.None());
        sprint = new ShadeBinding(ShadeBindingOption.FromKey(KeyCode.C), ShadeBindingOption.None());
        teleport = new ShadeBinding(ShadeBindingOption.FromKey(KeyCode.D), ShadeBindingOption.None());
        swapCharacter = new ShadeBinding(ShadeBindingOption.FromKey(KeyCode.V), ShadeBindingOption.None());
    }

    private static bool BindingUsesController(ShadeBinding binding)
    {
        if (binding == null)
            return false;
        return binding.primary.type == ShadeBindingOptionType.Controller || binding.secondary.type == ShadeBindingOptionType.Controller;
    }

    private static bool OptionUsesControllerIndex(ShadeBindingOption option, int fallbackIndex, int targetIndex)
    {
        if (option.type != ShadeBindingOptionType.Controller)
            return false;
        int actualIndex = option.controllerDevice >= 0 ? option.controllerDevice : fallbackIndex;
        return actualIndex == targetIndex;
    }

    private static bool BindingUsesControllerIndex(ShadeBinding binding, int fallbackIndex, int targetIndex)
    {
        if (binding == null)
            return false;
        return OptionUsesControllerIndex(binding.primary, fallbackIndex, targetIndex) || OptionUsesControllerIndex(binding.secondary, fallbackIndex, targetIndex);
    }

    /// <summary>Whether any of the Shade's controls, the command binding included, is on a pad.</summary>
    public bool UsesControllerBindings() => AnyControllerBinding(index: null, includeCommandShade: true);

    /// <summary>
    /// Whether a controller is <em>the Shade player's</em>, and so has to be kept away from Hornet.
    /// <para>
    /// Not the same question as <see cref="IsControllerIndexInUse"/>, and the difference is the
    /// whole point of this method. <see cref="commandShade"/> is bound to the first pad on purpose -
    /// ordering the Shade about is Hornet's player's control, not the Shade player's - so a literal
    /// "is any shade binding on this pad" reads Hornet's own controller as the Shade's. With exactly
    /// two pads that made the Shade look like it had claimed both, which tripped the guard that
    /// stops Hornet being left with no controller at all, and neither pad ended up reserved: Hornet
    /// answered to both, and the Shade player's stick drove both characters at once.
    /// </para>
    /// </summary>
    public bool ReservesControllerIndex(int index) => AnyControllerBinding(index, includeCommandShade: false);

    /// <summary>Whether any pad at all is the Shade player's. See <see cref="ReservesControllerIndex"/>.</summary>
    public bool ReservesAnyController() => AnyControllerBinding(index: null, includeCommandShade: false);

    /// <summary>
    /// Whether any shade binding at all sits on this controller, the command binding included.
    /// For reserving a pad away from Hornet use <see cref="ReservesControllerIndex"/> instead.
    /// </summary>
    public bool IsControllerIndexInUse(int index) => AnyControllerBinding(index, includeCommandShade: true);

    /// <summary>
    /// The one roll-call the four questions above are asked against.
    /// <para>
    /// Each of them used to carry its own hand-written list of every action, which is four places to
    /// remember when one is added and four chances to differ by accident - and they already did, in
    /// exactly the way that matters: only one of them left <see cref="commandShade"/> out.
    /// </para>
    /// <para>
    /// A null <paramref name="index"/> asks "any pad at all"; a value asks about that one, counting
    /// a binding that names no device of its own as being on whichever pad the config points at.
    /// </para>
    /// <para>
    /// The debug actions are outside all of this, as they were in every one of the lists this
    /// replaces. They are keyboard by default and only read at all while Debug Keys is switched on,
    /// so one bound to a pad is not grounds for taking that pad away from Hornet.
    /// </para>
    /// </summary>
    private bool AnyControllerBinding(int? index, bool includeCommandShade)
    {
        if (index.HasValue && index.Value < 0)
        {
            return false;
        }

        int fallbackIndex = Mathf.Max(-1, controllerDeviceIndex);

        foreach (var action in AllActions)
        {
            if (IsDebugAction(action))
            {
                continue;
            }

            if (!includeCommandShade && action == ShadeAction.CommandShade)
            {
                continue;
            }

            var binding = GetBinding(action);
            if (binding == null)
            {
                continue;
            }

            bool matches = index.HasValue
                ? BindingUsesControllerIndex(binding, fallbackIndex, index.Value)
                : BindingUsesController(binding);

            if (matches)
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>The developer-only actions, which are never part of a device reservation.</summary>
    private static bool IsDebugAction(ShadeAction action) => action >= ShadeAction.DebugDamageShade;

    /// <summary>Every action a binding can be held for, so a sweep cannot miss one.</summary>
    internal static readonly ShadeAction[] AllActions =
        (ShadeAction[])Enum.GetValues(typeof(ShadeAction));

    /// <summary>
    /// The debug keys' defaults, for a config saved before they had any.
    /// </summary>
    private static readonly (ShadeAction Action, KeyCode Key)[] DebugKeyDefaults =
    {
        (ShadeAction.DebugDamageShade, KeyCode.LeftBracket),
        (ShadeAction.DebugHealShade, KeyCode.RightBracket),
        (ShadeAction.DebugSoulIncrease, KeyCode.Equals),
        (ShadeAction.DebugSoulDecrease, KeyCode.Minus),
        (ShadeAction.DebugSoulReset, KeyCode.Backslash)
    };

    /// <summary>
    /// Gives the debug keys their defaults on a config that predates them, and takes nothing away.
    /// <para>
    /// Two rules, both of them about not surprising anyone who has already set their controls up.
    /// A debug action that is already bound to something is left exactly as it is - the default is
    /// a starting point, not a preference. And a default whose key is being used for anything else
    /// is dropped rather than applied, because the player who bound <c>-</c> to the Shade's nail
    /// would otherwise find it quietly draining their soul the day they switched the debug keys on.
    /// </para>
    /// <para>
    /// Returns how many were filled in, for the load-time log line.
    /// </para>
    /// </summary>
    public int ApplyMissingDebugDefaults()
    {
        int filled = 0;

        foreach (var (action, key) in DebugKeyDefaults)
        {
            var binding = GetBinding(action);
            if (binding == null || IsBound(binding) || IsKeyInUse(key))
            {
                continue;
            }

            binding.primary = ShadeBindingOption.FromKey(key);
            filled++;
        }

        return filled;
    }

    private static bool IsBound(ShadeBinding binding)
        => binding.primary.type != ShadeBindingOptionType.None
            || binding.secondary.type != ShadeBindingOptionType.None;

    /// <summary>Whether any action - debug or not - already answers to this key.</summary>
    private bool IsKeyInUse(KeyCode key)
        => IsOptionInUse(ShadeBindingOption.FromKey(key), except: null);

    /// <summary>
    /// Whether any action other than <paramref name="except"/> already answers to this control.
    /// </summary>
    private bool IsOptionInUse(ShadeBindingOption option, ShadeAction? except)
    {
        foreach (var action in AllActions)
        {
            if (except.HasValue && action == except.Value)
            {
                continue;
            }

            var binding = GetBinding(action);
            if (binding == null)
            {
                continue;
            }

            if (OptionsMatch(binding.primary, option) || OptionsMatch(binding.secondary, option))
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Whether two options name the same physical control. A controller option carries the device
    /// it was captured on, and -1 means "whichever pad is the companion's", so an option that names
    /// a device matches an option that does not.
    /// </summary>
    private static bool OptionsMatch(ShadeBindingOption a, ShadeBindingOption b)
    {
        if (a.type != b.type)
        {
            return false;
        }

        return a.type switch
        {
            ShadeBindingOptionType.Key => a.key == b.key,
            ShadeBindingOptionType.Controller => a.control == b.control
                && (a.controllerDevice < 0 || b.controllerDevice < 0 || a.controllerDevice == b.controllerDevice),
            _ => false
        };
    }

    /// <summary>
    /// Actions added after this mod started shipping. Their defaults arrive on every config,
    /// including one saved before the action existed, because an absent field keeps whatever
    /// <see cref="ResetToDefaults"/> gave it - so nobody ever chose them.
    /// </summary>
    private static readonly ShadeAction[] YieldingDefaults = { ShadeAction.SwapCharacter };

    /// <summary>
    /// Takes a default back off any control the player has already spent on something else, and
    /// reports how many it dropped.
    /// <para>
    /// The same rule the debug keys follow, from the other side. There a default was withheld
    /// because it would have collided; here it has already been handed out by the constructor, so
    /// the collision has to be undone instead. Either way an existing binding wins and the new
    /// action is left plainly unbound for the player to place themselves.
    /// </para>
    /// </summary>
    public int DropCollidingDefaults()
    {
        var pristine = CreateDefault();
        int dropped = 0;

        foreach (var action in YieldingDefaults)
        {
            var binding = GetBinding(action);
            var fallback = pristine.GetBinding(action);
            if (binding == null || fallback == null)
            {
                continue;
            }

            if (ShouldDrop(binding.primary, fallback.primary, action))
            {
                binding.primary = ShadeBindingOption.None();
                dropped++;
            }

            if (ShouldDrop(binding.secondary, fallback.secondary, action))
            {
                binding.secondary = ShadeBindingOption.None();
                dropped++;
            }
        }

        return dropped;
    }

    /// <summary>
    /// A binding is only dropped while it is still sitting on the value nobody chose. Once it has
    /// been rebound - even onto a control something else uses - it is the player's own doing and is
    /// left alone.
    /// </summary>
    private bool ShouldDrop(ShadeBindingOption current, ShadeBindingOption fallback, ShadeAction action)
        => current.type != ShadeBindingOptionType.None
            && OptionsMatch(current, fallback)
            && IsOptionInUse(current, except: action);

    /// <summary>
    /// Records which pad each player is holding.
    /// <para>
    /// Setting <see cref="controllerDeviceIndex"/> is not enough on its own. A binding captured on
    /// a pad remembers that pad in its own <c>controllerDevice</c>, and that takes precedence over
    /// the config-level index - so a companion moved to a different controller would go on
    /// answering to the old one for every control the player had ever rebound, which is the
    /// assignment appearing not to work at all.
    /// </para>
    /// <para>
    /// <see cref="ShadeAction.CommandShade"/> goes the other way, onto Hornet's pad: it is the
    /// button her player presses to send the companion somewhere, which is why it is the one action
    /// excluded from the reservation. Every controller option is left holding an explicit device
    /// afterwards - once both players have said which pad is theirs, there is nothing left for
    /// "whichever one the config says" to usefully mean.
    /// </para>
    /// <para>Returns how many bindings moved, for the log line.</para>
    /// </summary>
    public int ApplyControllerAssignment(int hornetIndex, int companionIndex)
    {
        controllerDeviceIndex = companionIndex;

        int moved = 0;
        foreach (var action in AllActions)
        {
            var binding = GetBinding(action);
            if (binding == null)
            {
                continue;
            }

            if (action == ShadeAction.CommandShade && hornetIndex < 0)
            {
                // Hornet is on the keyboard, so there is no pad for her control to move to.
                // Pointing it at "whichever the config says" would put it on the companion's.
                moved += ClearControllerOption(ref binding.primary);
                moved += ClearControllerOption(ref binding.secondary);
                continue;
            }

            int wanted = action == ShadeAction.CommandShade ? hornetIndex : companionIndex;
            moved += RepointControllerOption(ref binding.primary, wanted);
            moved += RepointControllerOption(ref binding.secondary, wanted);
        }

        return moved;
    }

    private static int RepointControllerOption(ref ShadeBindingOption option, int deviceIndex)
    {
        if (option.type != ShadeBindingOptionType.Controller || option.controllerDevice == deviceIndex)
        {
            return 0;
        }

        option.controllerDevice = deviceIndex;
        return 1;
    }

    private static int ClearControllerOption(ref ShadeBindingOption option)
    {
        if (option.type != ShadeBindingOptionType.Controller)
        {
            return 0;
        }

        option = ShadeBindingOption.None();
        return 1;
    }

    /// <summary>
    /// Clears any binding that was captured as a device-agnostic joystick key, and reports how many
    /// it cleared.
    /// <para>
    /// Rebinding on a pad used to store <c>KeyCode.JoystickButtonN</c>, which fires on every
    /// attached controller regardless of the device the binding names. Those are now ignored when
    /// read, but an ignored binding is an invisible one - the Controls screen would still show the
    /// button while nothing happened. Clearing them instead leaves the row plainly unbound, and
    /// rebinding it now records the device properly.
    /// </para>
    /// </summary>
    public int ClearControllerKeyBindings()
    {
        int cleared = 0;

        foreach (var action in AllActions)
        {
            var binding = GetBinding(action);
            if (binding == null)
                continue;

            if (binding.primary.type == ShadeBindingOptionType.Key && ShadeInput.IsControllerKeyCode(binding.primary.key))
            {
                binding.primary = ShadeBindingOption.None();
                cleared++;
            }

            if (binding.secondary.type == ShadeBindingOptionType.Key && ShadeInput.IsControllerKeyCode(binding.secondary.key))
            {
                binding.secondary = ShadeBindingOption.None();
                cleared++;
            }
        }

        return cleared;
    }

    public ShadeBinding GetBinding(ShadeAction action) => action switch
    {
        ShadeAction.MoveLeft => moveLeft,
        ShadeAction.MoveRight => moveRight,
        ShadeAction.MoveUp => moveUp,
        ShadeAction.MoveDown => moveDown,
        ShadeAction.Fire => fire,
        ShadeAction.Nail => nail,
        ShadeAction.NailUp => nailUp,
        ShadeAction.NailDown => nailDown,
        ShadeAction.Teleport => teleport,
        ShadeAction.Focus => focus,
        ShadeAction.Sprint => sprint,
        ShadeAction.CommandShade => commandShade,
        ShadeAction.SwapCharacter => swapCharacter,
        ShadeAction.DebugDamageShade => debugDamageShade,
        ShadeAction.DebugHealShade => debugHealShade,
        ShadeAction.DebugSoulIncrease => debugSoulIncrease,
        ShadeAction.DebugSoulDecrease => debugSoulDecrease,
        ShadeAction.DebugSoulReset => debugSoulReset,
        _ => moveLeft
    };

    public void SetBinding(ShadeAction action, ShadeBinding binding)
    {
        switch (action)
        {
            case ShadeAction.MoveLeft:
                moveLeft = binding;
                break;
            case ShadeAction.MoveRight:
                moveRight = binding;
                break;
            case ShadeAction.MoveUp:
                moveUp = binding;
                break;
            case ShadeAction.MoveDown:
                moveDown = binding;
                break;
            case ShadeAction.Fire:
                fire = binding;
                break;
            case ShadeAction.Nail:
                nail = binding;
                break;
            case ShadeAction.NailUp:
                nailUp = binding;
                break;
            case ShadeAction.NailDown:
                nailDown = binding;
                break;
            case ShadeAction.Teleport:
                teleport = binding;
                break;
            case ShadeAction.Focus:
                focus = binding;
                break;
            case ShadeAction.Sprint:
                sprint = binding;
                break;
            case ShadeAction.CommandShade:
                commandShade = binding;
                break;
            case ShadeAction.SwapCharacter:
                swapCharacter = binding;
                break;
            case ShadeAction.DebugDamageShade:
                debugDamageShade = binding;
                break;
            case ShadeAction.DebugHealShade:
                debugHealShade = binding;
                break;
            case ShadeAction.DebugSoulIncrease:
                debugSoulIncrease = binding;
                break;
            case ShadeAction.DebugSoulDecrease:
                debugSoulDecrease = binding;
                break;
            case ShadeAction.DebugSoulReset:
                debugSoulReset = binding;
                break;
        }
    }

    public void SetBindingOption(ShadeAction action, bool secondary, ShadeBindingOption option)
    {
        var binding = GetBinding(action);
        if (binding == null)
        {
            binding = new ShadeBinding();
            SetBinding(action, binding);
        }
        if (secondary)
            binding.secondary = option;
        else
            binding.primary = option;
    }

    private static ShadeBinding CloneBinding(ShadeBinding binding)
    {
        return binding != null ? binding.Clone() : new ShadeBinding();
    }

    public ShadeInputConfig Clone()
    {
        var clone = new ShadeInputConfig();
        clone.controllerDeviceIndex = controllerDeviceIndex;
        clone.controllerDeadzone = controllerDeadzone;
        clone.moveLeft = CloneBinding(moveLeft);
        clone.moveRight = CloneBinding(moveRight);
        clone.moveUp = CloneBinding(moveUp);
        clone.moveDown = CloneBinding(moveDown);
        clone.fire = CloneBinding(fire);
        clone.nail = CloneBinding(nail);
        clone.nailUp = CloneBinding(nailUp);
        clone.nailDown = CloneBinding(nailDown);
        clone.teleport = CloneBinding(teleport);
        clone.focus = CloneBinding(focus);
        clone.sprint = CloneBinding(sprint);
        clone.commandShade = CloneBinding(commandShade);
        clone.debugDamageShade = CloneBinding(debugDamageShade);
        clone.debugHealShade = CloneBinding(debugHealShade);
        clone.debugSoulIncrease = CloneBinding(debugSoulIncrease);
        clone.debugSoulDecrease = CloneBinding(debugSoulDecrease);
        clone.debugSoulReset = CloneBinding(debugSoulReset);
        return clone;
    }

}

public static class ShadeInput
{
    private static readonly KeyCode[] AllKeyCodes = Enum.GetValues(typeof(KeyCode)) as KeyCode[] ?? Array.Empty<KeyCode>();

    /// <summary>
    /// Whether a <see cref="KeyCode"/> is a controller button rather than a key.
    /// <para>
    /// These have no place in a Shade binding, and the reason is the whole point of this method.
    /// <c>KeyCode.JoystickButton0</c> and its siblings are Unity's <em>device-agnostic</em> pad
    /// buttons: <c>Input.GetKey(JoystickButton1)</c> is true when button 1 is down on <em>any</em>
    /// attached pad. A Shade control captured as one of those fires when Hornet's player presses the
    /// same button, whatever device the binding claims to be on - the key path never consults a
    /// device at all. The numbered <c>Joystick1Button0</c> forms are per-device, but they are keyed
    /// to Unity's joystick numbering rather than to InControl's device list, which is what the rest
    /// of this file reserves and reads by, so they are no use here either.
    /// </para>
    /// <para>
    /// Controller presses belong on the controller path, which records which device they came from.
    /// See <see cref="TryCaptureKey"/> and <see cref="TryCaptureControl"/>.
    /// </para>
    /// </summary>
    internal static bool IsControllerKeyCode(KeyCode code)
    {
        return code >= KeyCode.JoystickButton0;
    }
    private static readonly InputControlType[] CaptureControls =
    {
        InputControlType.Action1,
        InputControlType.Action2,
        InputControlType.Action3,
        InputControlType.Action4,
        InputControlType.Action5,
        InputControlType.Action6,
        InputControlType.LeftTrigger,
        InputControlType.RightTrigger,
        InputControlType.LeftBumper,
        InputControlType.RightBumper,
        InputControlType.LeftStickButton,
        InputControlType.RightStickButton,
        InputControlType.LeftStickUp,
        InputControlType.LeftStickDown,
        InputControlType.LeftStickLeft,
        InputControlType.LeftStickRight,
        InputControlType.DPadUp,
        InputControlType.DPadDown,
        InputControlType.DPadLeft,
        InputControlType.DPadRight,
        InputControlType.Start,
        InputControlType.Back,
        InputControlType.Select,
        InputControlType.Options,
        InputControlType.Command
    };

    private static ShadeInputConfig ConfigInstance
    {
        get
        {
            var cfg = ModConfig.Instance;
            if (cfg.shadeInput == null)
            {
                cfg.shadeInput = ShadeInputConfig.CreateDefault();
            }
            return cfg.shadeInput;
        }
    }

    public static ShadeInputConfig Config => ConfigInstance;

    public static ShadeBindingOption GetBindingOption(ShadeAction action, bool secondary)
    {
        var binding = ConfigInstance.GetBinding(action);
        if (binding == null)
            return ShadeBindingOption.None();
        return secondary ? binding.secondary : binding.primary;
    }

    public static void SetBindingOption(ShadeAction action, bool secondary, ShadeBindingOption option)
    {
        if (option.type != ShadeBindingOptionType.Controller)
        {
            option.controllerDevice = -1;
        }
        ConfigInstance.SetBindingOption(action, secondary, option);
    }

    public static float GetActionValue(ShadeAction action) => GetActionValue(action, null);

    // The three reads below are the only place the Shade AI exists as far as the rest of the mod is
    // concerned: it publishes a frame of synthesised input and every handler that polls an action
    // gets it without knowing. A caller that names a requiredType is asking about physical hardware
    // (the Hornet-input bridge in LegacyHelper.Patches.cs, and the bindings UI), so those read past
    // it. See LegacyoftheAbyss.Shade.Ai.ShadeAiInput.
    internal static float GetActionValue(ShadeAction action, ShadeBindingOptionType? requiredType)
    {
        if (requiredType == null)
        {
            if (ShadeAiInput.TryGetValue(action, out float driven))
                return driven;
            if (ShadeAiInput.Suppressed(action))
                return 0f;
        }

        return GetActionValueRaw(action, requiredType);
    }

    internal static float GetActionValueRaw(ShadeAction action, ShadeBindingOptionType? requiredType = null)
    {
        var binding = ConfigInstance.GetBinding(action);
        if (binding == null)
            return 0f;
        return Mathf.Max(GetOptionValue(binding.primary, requiredType), GetOptionValue(binding.secondary, requiredType));
    }

    public static bool IsActionHeld(ShadeAction action) => IsActionHeld(action, null);

    internal static bool IsActionHeld(ShadeAction action, ShadeBindingOptionType? requiredType)
    {
        if (requiredType == null)
        {
            if (ShadeAiInput.TryGetHeld(action, out bool driven))
                return driven;
            if (ShadeAiInput.Suppressed(action))
                return false;
        }

        return IsActionHeldRaw(action, requiredType);
    }

    internal static bool IsActionHeldRaw(ShadeAction action, ShadeBindingOptionType? requiredType = null)
    {
        var binding = ConfigInstance.GetBinding(action);
        if (binding == null)
            return false;
        return IsOptionHeld(binding.primary, requiredType) || IsOptionHeld(binding.secondary, requiredType);
    }

    public static bool WasActionPressed(ShadeAction action) => WasActionPressed(action, null);

    internal static bool WasActionPressed(ShadeAction action, ShadeBindingOptionType? requiredType)
    {
        if (requiredType == null)
        {
            if (ShadeAiInput.TryGetPressed(action, out bool driven))
                return driven;
            if (ShadeAiInput.Suppressed(action))
                return false;
        }

        return WasActionPressedRaw(action, requiredType);
    }

    internal static bool WasActionPressedRaw(ShadeAction action, ShadeBindingOptionType? requiredType = null)
    {
        var binding = ConfigInstance.GetBinding(action);
        if (binding == null)
            return false;
        return WasOptionPressed(binding.primary, requiredType) || WasOptionPressed(binding.secondary, requiredType);
    }

    /// <summary>
    /// A stick on whichever device an action's controller binding points at, or zero when it has
    /// none. Used to steer the Shade command reticle with the same pad that opened it, rather than
    /// guessing which device the player is holding.
    /// </summary>
    /// <param name="rightStick">
    /// True for the right stick. The reticle uses it because the left stick is Hornet's movement and
    /// aiming must not cost the player the ability to walk.
    /// </param>
    public static Vector2 GetActionStick(ShadeAction action, bool rightStick)
    {
        try
        {
            var binding = ConfigInstance.GetBinding(action);
            if (binding == null)
                return Vector2.zero;

            Vector2 primary = ReadStick(binding.primary, rightStick);
            return primary.sqrMagnitude > 0f ? primary : ReadStick(binding.secondary, rightStick);
        }
        catch
        {
            return Vector2.zero;
        }
    }

    private static Vector2 ReadStick(ShadeBindingOption option, bool rightStick)
    {
        if (option.type != ShadeBindingOptionType.Controller)
            return Vector2.zero;

        var device = GetDeviceForOption(option);
        if (device == null || device == InputDevice.Null)
            return Vector2.zero;

        var stick = rightStick
            ? new Vector2(device.RightStickX.Value, device.RightStickY.Value)
            : new Vector2(device.LeftStickX.Value, device.LeftStickY.Value);
        float deadzone = Mathf.Clamp(ConfigInstance.controllerDeadzone, 0.01f, 1f);
        return stick.sqrMagnitude >= deadzone * deadzone ? stick : Vector2.zero;
    }

    public static string DescribeBindingOption(ShadeBindingOption option)
    {
        return option.type switch
        {
            ShadeBindingOptionType.Key => DescribeKey(option.key),
            ShadeBindingOptionType.Controller => DescribeControl(option.control, GetEffectiveControllerIndex(option)),
            _ => "Unbound"
        };
    }

    public static void EnsureControllerIndex(int index)
    {
        ConfigInstance.controllerDeviceIndex = index;
    }

    /// <summary>
    /// Whether this binding is to be ignored this frame. Every Shade input read funnels through
    /// here, which makes it the one place that can keep the Shade from acting on a bug report being
    /// typed into the overlay - blocking at the binding level rather than per-action means a new
    /// action is covered for free.
    /// </summary>
    private static bool ShouldSuppressOption()
        => LegacyoftheAbyss.Diagnostics.BugReportSystem.IsCapturingText;

    private static bool IsOptionHeld(ShadeBindingOption option, ShadeBindingOptionType? requiredType = null)
    {
        if (requiredType.HasValue && option.type != requiredType.Value)
            return false;
        if (ShouldSuppressOption())
            return false;

        return option.type switch
        {
            ShadeBindingOptionType.Key => option.key != KeyCode.None && Input.GetKey(option.key),
            ShadeBindingOptionType.Controller => GetControl(option, out var control) && control.IsPressed,
            _ => false
        };
    }

    private static bool WasOptionPressed(ShadeBindingOption option, ShadeBindingOptionType? requiredType = null)
    {
        if (requiredType.HasValue && option.type != requiredType.Value)
            return false;
        if (ShouldSuppressOption())
            return false;

        return option.type switch
        {
            ShadeBindingOptionType.Key => option.key != KeyCode.None && Input.GetKeyDown(option.key),
            ShadeBindingOptionType.Controller => GetControl(option, out var control) && control.WasPressed,
            _ => false
        };
    }

    private static float GetOptionValue(ShadeBindingOption option, ShadeBindingOptionType? requiredType)
    {
        if (requiredType.HasValue && option.type != requiredType.Value)
            return 0f;
        if (ShouldSuppressOption())
            return 0f;

        return option.type switch
        {
            // The joystick guard matters for configs written before capture was fixed: such a
            // binding reads every attached pad, so it is ignored rather than left cross-firing.
            ShadeBindingOptionType.Key => option.key != KeyCode.None && !IsControllerKeyCode(option.key) && Input.GetKey(option.key) ? 1f : 0f,
            ShadeBindingOptionType.Controller => GetControl(option, out var control) ? Mathf.Clamp01(Mathf.Abs(control.Value)) : 0f,
            _ => 0f
        };
    }

    private static bool GetControl(ShadeBindingOption option, out InputControl control)
    {
        control = InputControl.Null;
        if (option.type != ShadeBindingOptionType.Controller)
            return false;
        try
        {
            var device = GetDeviceForOption(option);
            if (device == null || device == InputDevice.Null)
                return false;
            control = device.GetControl(option.control);
            return control != null && control != InputControl.Null;
        }
        catch
        {
            control = InputControl.Null;
            return false;
        }
    }

    /// <summary>
    /// The one device a Shade controller binding is allowed to read, or null.
    /// <para>
    /// Null rather than a stand-in, deliberately. This used to fall back to
    /// <c>InputManager.ActiveDevice</c> when the binding named no device and to the last attached
    /// pad when it named one that is not plugged in - both of which hand the Shade whichever
    /// controller the other player happens to be holding. A Shade control whose device is absent
    /// should do nothing, not something on someone else's pad.
    /// </para>
    /// </summary>
    private static InputDevice? GetDeviceForOption(ShadeBindingOption option)
    {
        int index = GetEffectiveControllerIndex(option);
        if (index < 0)
            return null;

        var devices = InputManager.Devices;
        if (devices == null || index >= devices.Count)
            return null;

        var selected = devices[index];
        return selected ?? InputDevice.Null;
    }

    private static int GetEffectiveControllerIndex(ShadeBindingOption option)
    {
        if (option.controllerDevice >= 0)
            return option.controllerDevice;
        return Mathf.Max(-1, ConfigInstance.controllerDeviceIndex);
    }

    private static string DescribeKey(KeyCode key)
    {
        if (key == KeyCode.None)
            return "Unbound";

        // Unity numbers the mouse from zero, so the enum name renders as "Mouse 0" - which is
        // nobody's name for the left button. The first three get what players call them; the rest
        // keep a number, shifted up one so it agrees with how mice are labelled.
        switch (key)
        {
            case KeyCode.Mouse0:
                return "LMB";
            case KeyCode.Mouse1:
                return "RMB";
            case KeyCode.Mouse2:
                return "MMB";
            case KeyCode.Mouse3:
            case KeyCode.Mouse4:
            case KeyCode.Mouse5:
            case KeyCode.Mouse6:
                return "Mouse " + ((int)key - (int)KeyCode.Mouse0 + 1).ToString(CultureInfo.InvariantCulture);
        }

        return FormatEnumName(key.ToString());
    }

    private static string DescribeControl(InputControlType control, int deviceIndex)
    {
        string controlName = FormatEnumName(control.ToString());
        if (deviceIndex < 0)
            return controlName;
        return $"Controller {deviceIndex + 1} {controlName}";
    }

    private static string FormatEnumName(string raw)
    {
        if (string.IsNullOrEmpty(raw))
            return raw;
        var sb = new StringBuilder(raw.Length * 2);
        char previous = '\0';
        foreach (char c in raw)
        {
            if (c == '_')
            {
                if (sb.Length > 0 && sb[sb.Length - 1] != ' ')
                    sb.Append(' ');
                previous = c;
                continue;
            }
            if (char.IsUpper(c) && previous != '\0' && !char.IsUpper(previous) && previous != ' ')
                sb.Append(' ');
            else if (char.IsDigit(c) && previous != '\0' && !char.IsDigit(previous) && previous != ' ')
                sb.Append(' ');
            sb.Append(c);
            previous = c;
        }
        return sb.ToString();
    }

    public static bool TryCaptureKey(out KeyCode key)
    {
        foreach (var code in AllKeyCodes)
        {
            if (code == KeyCode.None)
                continue;

            // Skipped so the press falls through to TryCaptureControl, which records the device it
            // came from. This check ran first and matched everything, so every rebind made on a pad
            // was stored as a device-agnostic joystick key and fired on both players' controllers.
            if (IsControllerKeyCode(code))
                continue;

            if (Input.GetKeyDown(code))
            {
                key = code;
                return true;
            }
        }
        key = KeyCode.None;
        return false;
    }

    public static bool TryCaptureControl(out InputControlType controlType, out int deviceIndex)
    {
        controlType = InputControlType.None;
        deviceIndex = -1;
        var devices = InputManager.Devices;
        if (devices == null || devices.Count == 0)
            return false;
        for (int i = 0; i < devices.Count; i++)
        {
            var device = devices[i];
            foreach (var controlCandidate in CaptureControls)
            {
                var control = device.GetControl(controlCandidate);
                if (control == null || control == InputControl.Null)
                    continue;
                if (control.WasPressed)
                {
                    controlType = controlCandidate;
                    deviceIndex = i;
                    return true;
                }
            }
        }
        return false;
    }
}
