using System;
using System.Collections.Generic;
using LegacyoftheAbyss.Shade.Ai;
using UnityEngine;
using Xunit;

/// <summary>
/// The Shade AI: target selection, the approach, which slash reaches, when SOUL is worth spending,
/// staying out of what would hurt, and healing.
/// <para>
/// The brain is the whole of the decision and touches no Unity object, which is why it is a separate
/// class. The driver around it (<c>LegacyHelper.ShadeController.Ai.cs</c>) still needs a play
/// session: it wants a live scene, a <c>HealthManager</c> and a Shade.
/// </para>
/// </summary>
public class ShadeAiBrainTests
{
    private static readonly ShadeAiTuning Tuning = ShadeAiTuning.Default;

    private static ShadeAiTarget Basic(int id, float x, float y, float radius = 0.5f)
        => new ShadeAiTarget(id, new Vector2(x, y), radius, false, true);

    private static ShadeAiTarget Boss(int id, float x, float y, float radius = 0.5f)
        => new ShadeAiTarget(id, new Vector2(x, y), radius, true, true);

    /// <summary>An enemy behind terrain. Present, alive, and not a target.</summary>
    private static ShadeAiTarget Hidden(int id, float x, float y, float radius = 0.5f)
        => new ShadeAiTarget(id, new Vector2(x, y), radius, false, false);


    private static ShadeAiThreat Threat(float x, float y, float radius)
        => new ShadeAiThreat(new Vector2(x, y), radius);

    private static ShadeAiSnapshot Snapshot(
        IReadOnlyList<ShadeAiTarget> targets,
        Vector2 shade = default,
        Vector2 hornet = default,
        float time = 0f,
        int soul = 99,
        int soulReserve = 0,
        bool projectile = false,
        bool shriek = false,
        bool descendingDark = false,
        bool nailReady = true,
        bool spellsReady = true,
        bool canTakeDamage = false,
        IReadOnlyList<ShadeAiThreat> threats = null,
        float selfHealth = 1f,
        float hornetHealth = 1f,
        bool canFocusHeal = false,
        float moveSpeed = 10f,
        float nailInterval = 0.6f,
        int hornetFacing = 1,
        bool hornetAirborne = false,
        Vector2? command = null,
        bool isFocusing = false)
    {
        return new ShadeAiSnapshot
        {
            Time = time,
            ShadePosition = shade,
            HornetPosition = hornet,
            Facing = 1,
            HornetFacing = hornetFacing,
            HornetAirborne = hornetAirborne,
            HasCommand = command.HasValue,
            CommandPoint = command ?? Vector2.zero,
            SoftLeashRadius = 12f,
            MoveSpeed = moveSpeed,
            NailInterval = nailInterval,
            Soul = soul,
            SoulReserve = soulReserve,
            ProjectileSoulCost = 11,
            ShriekSoulCost = 33,
            QuakeSoulCost = 33,
            FocusSoulCost = 33,
            ProjectileUnlocked = projectile,
            ShriekUnlocked = shriek,
            DescendingDarkUnlocked = descendingDark,
            NailReady = nailReady,
            FireReady = spellsReady,
            ShriekReady = spellsReady,
            QuakeReady = spellsReady,
            CanFocusHeal = canFocusHeal,
            IsFocusing = isFocusing,
            FocusHealRange = 6f,
            SelfHealthFraction = selfHealth,
            HornetHealthFraction = hornetHealth,
            CanTakeDamage = canTakeDamage,
            Tuning = Tuning,
            Targets = targets,
            Threats = threats ?? Array.Empty<ShadeAiThreat>()
        };
    }

    // --- Target selection ---------------------------------------------------------------

    [Fact]
    public void EscortsHornetWithNothingToFight()
    {
        var plan = new ShadeAiBrain().Decide(Snapshot(Array.Empty<ShadeAiTarget>()));

        Assert.False(plan.HasTarget);
        Assert.Equal(ShadeAiAction.None, plan.Action);
        Assert.Equal(ShadeAiReason.NoTargets, plan.Reason);
        Assert.NotEqual(Vector2.zero, plan.Move);
    }

    // --- Navigation ---------------------------------------------------------------------

    /// <summary>
    /// The rotation the obstacle fan is built on. The steering itself needs Physics2D and a real
    /// level, so this is the only part of the navigator a test host can reach - which is worth
    /// saying plainly rather than leaving the coverage looking better than it is.
    /// </summary>
    [Theory]
    [InlineData(0f, 1f, 0f)]
    [InlineData(90f, 0f, 1f)]
    [InlineData(180f, -1f, 0f)]
    [InlineData(-90f, 0f, -1f)]
    public void RotatesAHeadingByDegrees(float degrees, float expectedX, float expectedY)
    {
        var rotated = ShadeAiNavigator.Rotate(Vector2.right, degrees);

        Assert.Equal(expectedX, rotated.x, 3);
        Assert.Equal(expectedY, rotated.y, 3);
    }

    [Fact]
    public void RotationPreservesLength()
    {
        var rotated = ShadeAiNavigator.Rotate(new Vector2(3f, 4f), 37f);

        Assert.Equal(5f, rotated.magnitude, 3);
    }

    // --- Player orders ------------------------------------------------------------------

    /// <summary>
    /// An order outranks the Shade's own idea of where to be. With nothing in reach it simply walks
    /// to the spot, rather than escorting Hornet or chasing.
    /// </summary>
    [Fact]
    public void GoesWhereItIsToldInsteadOfEscorting()
    {
        var plan = new ShadeAiBrain().Decide(Snapshot(
            Array.Empty<ShadeAiTarget>(),
            shade: new Vector2(-6f, 0f),
            command: new Vector2(4f, 0f)));

        Assert.Equal(ShadeAiReason.Commanded, plan.Reason);
        Assert.Equal(new Vector2(4f, 0f), plan.DesiredPosition);
        Assert.True(plan.Move.x > 0f, "should be heading for the ordered spot");
    }

    /// <summary>
    /// Told to hold a spot with an enemy already on top of it. A Shade that refused to swing at what
    /// walked into it would be worse than useless, so it fights from where it stands.
    /// </summary>
    [Fact]
    public void FightsWhatComesToItWhileHoldingPosition()
    {
        var plan = new ShadeAiBrain().Decide(Snapshot(
            new[] { Basic(1, 1.9f, 0f) },
            command: Vector2.zero));

        Assert.Equal(ShadeAiReason.Commanded, plan.Reason);
        Assert.Equal(ShadeAiAction.SlashSide, plan.Action);
        Assert.Equal(1, plan.TargetId);
    }

    /// <summary>The other half of that: it will not leave the spot to go and get one.</summary>
    [Fact]
    public void WillNotChaseWhileHoldingPosition()
    {
        var plan = new ShadeAiBrain().Decide(Snapshot(
            new[] { Basic(1, 9f, 0f) },
            command: Vector2.zero));

        Assert.Equal(ShadeAiReason.Commanded, plan.Reason);
        Assert.Equal(ShadeAiAction.None, plan.Action);
        Assert.Equal(Vector2.zero, plan.DesiredPosition);
        Assert.Equal(Vector2.zero, plan.Move);
    }

    /// <summary>
    /// An order cannot send the Shade past its leash - Hornet would simply drag it back, and the
    /// order would read as ignored.
    /// </summary>
    [Fact]
    public void ClampsAnOrderToTheLeash()
    {
        var plan = new ShadeAiBrain().Decide(Snapshot(
            Array.Empty<ShadeAiTarget>(),
            command: new Vector2(80f, 0f)));

        Assert.Equal(ShadeAiReason.Commanded, plan.Reason);
        Assert.True(plan.DesiredPosition.magnitude < 80f, "should have been pulled inside the leash");
    }

    /// <summary>Hazards still move it: the order says where, not that it should stand in spikes.</summary>
    [Fact]
    public void StepsAnOrderedSpotOutOfAHazard()
    {
        var plan = new ShadeAiBrain().Decide(Snapshot(
            Array.Empty<ShadeAiTarget>(),
            canTakeDamage: true,
            threats: new[] { Threat(4f, 0f, 2f) },
            command: new Vector2(4f, 0f)));

        Assert.Equal(ShadeAiReason.Commanded, plan.Reason);
        Assert.True(Vector2.Distance(plan.DesiredPosition, new Vector2(4f, 0f)) >= 2f,
            "the ordered spot was inside the hazard and should have been pushed clear");
    }

    /// <summary>Getting out of an attack still comes first - an order is not worth dying to keep.</summary>
    [Fact]
    public void StillEvadesWhileUnderOrders()
    {
        var plan = new ShadeAiBrain().Decide(Snapshot(
            Array.Empty<ShadeAiTarget>(),
            canTakeDamage: true,
            threats: new[] { Threat(0f, 0f, 3f) },
            command: Vector2.zero));

        Assert.Equal(ShadeAiReason.Evading, plan.Reason);
    }

    // --- Escorting ----------------------------------------------------------------------

    /// <summary>
    /// On the ground the Shade waits behind and above her: out of the way of whatever she is walking
    /// into, and clear of her own nail arc.
    /// </summary>
    [Theory]
    [InlineData(1)]
    [InlineData(-1)]
    public void WaitsBehindAndAboveHornetOnTheGround(int hornetFacing)
    {
        var plan = new ShadeAiBrain().Decide(Snapshot(
            Array.Empty<ShadeAiTarget>(),
            hornetFacing: hornetFacing,
            hornetAirborne: false));

        Assert.True(plan.DesiredPosition.y > 0f, "should be above Hornet");
        Assert.True(plan.DesiredPosition.x * hornetFacing < 0f, "should be behind Hornet");
        Assert.Equal(hornetFacing, plan.FaceX);
    }

    /// <summary>
    /// Airborne it inverts to ahead and below, which is the half that earns its keep: a Shade under
    /// the far side of a jump is a platform, and a gap Hornet would not clear becomes a pogo.
    /// </summary>
    [Theory]
    [InlineData(1)]
    [InlineData(-1)]
    public void DropsAheadAndBelowHornetInTheAir(int hornetFacing)
    {
        var plan = new ShadeAiBrain().Decide(Snapshot(
            Array.Empty<ShadeAiTarget>(),
            hornetFacing: hornetFacing,
            hornetAirborne: true));

        Assert.True(plan.DesiredPosition.y < 0f, "should be below Hornet");
        Assert.True(plan.DesiredPosition.x * hornetFacing > 0f, "should be ahead of Hornet");
    }

    /// <summary>
    /// Spikes where it wanted to wait. The horizontal mirror is tried first, because that keeps the
    /// height it was after - and height is the half that matters for a pogo.
    /// </summary>
    [Fact]
    public void MirrorsTheEscortCornerAwayFromAHazard()
    {
        var preferred = ShadeAiBrain.ComputeEscortPoint(
            Snapshot(Array.Empty<ShadeAiTarget>(), hornetFacing: 1),
            -1,
            1);

        var plan = new ShadeAiBrain().Decide(Snapshot(
            Array.Empty<ShadeAiTarget>(),
            hornetFacing: 1,
            canTakeDamage: true,
            threats: new[] { Threat(preferred.x, preferred.y, 1f) }));

        Assert.True(plan.DesiredPosition.x > 0f, "should have swapped to the other side");
        Assert.True(plan.DesiredPosition.y > 0f, "should have kept its height");
    }

    /// <summary>Nothing can hurt an invincible Shade, so the preferred corner stands.</summary>
    [Fact]
    public void KeepsTheEscortCornerWhileInvincible()
    {
        var preferred = ShadeAiBrain.ComputeEscortPoint(
            Snapshot(Array.Empty<ShadeAiTarget>(), hornetFacing: 1),
            -1,
            1);

        var plan = new ShadeAiBrain().Decide(Snapshot(
            Array.Empty<ShadeAiTarget>(),
            hornetFacing: 1,
            canTakeDamage: false,
            threats: new[] { Threat(preferred.x, preferred.y, 1f) }));

        Assert.Equal(preferred, plan.DesiredPosition);
    }

    /// <summary>
    /// Escorting is what the Shade does whenever it has nothing to fight, including when the only
    /// enemies around are ones it cannot reach or cannot see. The reason still says which.
    /// </summary>
    [Fact]
    public void EscortsWhenTheOnlyEnemiesAreUnreachable()
    {
        var plan = new ShadeAiBrain().Decide(Snapshot(new[] { Hidden(1, 3f, 0f) }));

        Assert.Equal(ShadeAiReason.NoLineOfSight, plan.Reason);
        Assert.NotEqual(Vector2.zero, plan.Move);
    }

    /// <summary>
    /// The Shade is on a leash. Committing to an enemy Hornet cannot be dragged near means the
    /// Shade spends the fight being pulled backwards mid-swing, so the brain has to decline the
    /// target outright - and say that is why, because "did nothing" and "could not reach" look
    /// identical from outside.
    /// </summary>
    [Fact]
    public void RefusesTargetsOutsideTheLeash()
    {
        var plan = new ShadeAiBrain().Decide(Snapshot(new[] { Basic(1, 30f, 0f) }));

        Assert.False(plan.HasTarget);
        Assert.Equal(ShadeAiReason.OutOfLeash, plan.Reason);
    }

    /// <summary>
    /// Regression for "Shade with AI active is just attacking the wall over and over". An enemy
    /// behind terrain was the nearest thing in range, so the Shade walked up to the wall and swung
    /// at it until the player noticed.
    /// </summary>
    [Fact]
    public void WillNotTargetAnEnemyItCannotSee()
    {
        var plan = new ShadeAiBrain().Decide(Snapshot(new[] { Hidden(1, 3f, 0f) }));

        Assert.False(plan.HasTarget);
        Assert.Equal(ShadeAiAction.None, plan.Action);
        Assert.Equal(ShadeAiReason.NoLineOfSight, plan.Reason);
    }

    [Fact]
    public void TakesAVisibleEnemyOverANearerHiddenOne()
    {
        var plan = new ShadeAiBrain().Decide(Snapshot(new[] { Hidden(1, 2f, 0f), Basic(2, 6f, 0f) }));

        Assert.True(plan.HasTarget);
        Assert.Equal(2, plan.TargetId);
    }

    [Fact]
    public void TakesTheNearestEnemy()
    {
        var plan = new ShadeAiBrain().Decide(Snapshot(new[] { Basic(1, 9f, 0f), Basic(2, 4f, 0f) }));

        Assert.True(plan.HasTarget);
        Assert.Equal(2, plan.TargetId);
    }

    /// <summary>A boss is worth walking past a nearer trash mob for, but only up to a point.</summary>
    [Fact]
    public void PrefersABossOverASlightlyNearerBasicEnemy()
    {
        var plan = new ShadeAiBrain().Decide(Snapshot(new[] { Basic(1, 5f, 0f), Boss(2, 7f, 0f) }));

        Assert.Equal(2, plan.TargetId);
    }

    [Fact]
    public void KeepsItsTargetUntilTheRetargetIntervalElapses()
    {
        var brain = new ShadeAiBrain();
        var near = Basic(1, 5f, 0f);
        var far = Basic(2, 9f, 0f);

        Assert.Equal(1, brain.Decide(Snapshot(new[] { near, far }, time: 0f)).TargetId);

        // The committed target runs away; the other is now clearly closer. Inside the interval the
        // decision still stands, so the Shade commits to a swing instead of turning around.
        var moved = new[] { Basic(1, 12f, 0f), far };
        Assert.Equal(1, brain.Decide(Snapshot(moved, time: 0.1f)).TargetId);

        // Past it, the difference is outside the dead band and the switch happens.
        Assert.Equal(2, brain.Decide(Snapshot(moved, time: 0.7f)).TargetId);
    }

    [Theory]
    [InlineData(10f, 7f, true)]
    [InlineData(10f, 8f, false)]
    [InlineData(10f, 10f, false)]
    [InlineData(10f, 12f, false)]
    public void OnlySwitchesTargetOnAClearImprovement(float committed, float candidate, bool expected)
    {
        Assert.Equal(expected, ShadeAiBrain.PreferNewTarget(committed, candidate, 0.25f));
    }

    // --- Approach -----------------------------------------------------------------------

    [Fact]
    public void ClosesOnADistantEnemyAndSprintsToDoIt()
    {
        var plan = new ShadeAiBrain().Decide(Snapshot(new[] { Basic(1, 10f, 0f) }));

        Assert.Equal(ShadeAiReason.Approaching, plan.Reason);
        Assert.Equal(ShadeAiAction.None, plan.Action);
        Assert.True(plan.Move.x > 0f, "should be steering toward the enemy");
        Assert.True(plan.Sprint);
        Assert.Equal(1, plan.FaceX);
    }

    /// <summary>
    /// The strike point sits on whichever side the Shade is already on. Approaching from the far
    /// side would mean walking through the enemy to reach it.
    /// </summary>
    [Fact]
    public void StrikesFromTheSideItIsAlreadyOn()
    {
        var target = Basic(1, 0f, 0f);

        var fromLeft = ShadeAiBrain.ComputeStrikePoint(Snapshot(new[] { target }, shade: new Vector2(-8f, 0f)), target);
        var fromRight = ShadeAiBrain.ComputeStrikePoint(Snapshot(new[] { target }, shade: new Vector2(8f, 0f)), target);

        Assert.True(fromLeft.x < 0f);
        Assert.True(fromRight.x > 0f);
    }

    [Fact]
    public void DoesNotSteerOnceItHasArrived()
    {
        Assert.Equal(Vector2.zero, ShadeAiBrain.SteerTo(new Vector2(0.2f, 0f), 0.2f, Tuning.ArriveDeadzone));
        Assert.NotEqual(Vector2.zero, ShadeAiBrain.SteerTo(new Vector2(4f, 0f), 4f, Tuning.ArriveDeadzone));
    }

    [Fact]
    public void KeepsTheStrikePointInsideTheLeash()
    {
        var clamped = ShadeAiBrain.ClampToLeash(new Vector2(40f, 0f), Vector2.zero, 10f);

        Assert.Equal(10f, clamped.magnitude, 3);
    }

    // --- Slashes ------------------------------------------------------------------------

    [Fact]
    public void SlashesSidewaysAtAnEnemyBesideIt()
    {
        var plan = new ShadeAiBrain().Decide(Snapshot(new[] { Basic(1, 2.5f, 0f) }));

        Assert.Equal(ShadeAiAction.SlashSide, plan.Action);
        Assert.Equal(ShadeAiReason.InRange, plan.Reason);
    }

    [Fact]
    public void SlashesUpwardAtAnEnemyOverhead()
    {
        var plan = new ShadeAiBrain().Decide(Snapshot(new[] { Basic(1, 0f, 2.5f) }));

        Assert.Equal(ShadeAiAction.SlashUp, plan.Action);
    }

    [Fact]
    public void SlashesDownwardAtAnEnemyBelow()
    {
        var plan = new ShadeAiBrain().Decide(Snapshot(new[] { Basic(1, 0f, -2.5f) }));

        Assert.Equal(ShadeAiAction.SlashDown, plan.Action);
    }

    [Fact]
    public void DoesNotSlashAtSomethingOutOfReach()
    {
        Assert.Equal(ShadeAiAction.None, ShadeAiBrain.ChooseNailDirection(new Vector2(6f, 6f), 0.5f, Tuning));
    }

    /// <summary>
    /// Regression for "regularly misses nail slashes when enemies are positioned diagonally". The
    /// side slash used to be taken whenever the enemy was inside its vertical band at all, so an
    /// enemy up and to the side got a horizontal swing that could not reach it. The axis the enemy
    /// mostly lies on now decides.
    /// </summary>
    [Fact]
    public void SwingsUpwardAtAnEnemyMostlyAboveIt()
    {
        var action = ShadeAiBrain.ChooseNailDirection(new Vector2(0.9f, 1.8f), 0.5f, Tuning);

        Assert.Equal(ShadeAiAction.SlashUp, action);
    }

    [Fact]
    public void SwingsSidewaysAtAnEnemyMostlyBesideIt()
    {
        var action = ShadeAiBrain.ChooseNailDirection(new Vector2(1.8f, 0.9f), 0.5f, Tuning);

        Assert.Equal(ShadeAiAction.SlashSide, action);
    }

    /// <summary>
    /// Squarely diagonal and out of both bands: swinging would miss, so it says so and the caller
    /// goes and lines itself up instead.
    /// </summary>
    [Fact]
    public void DeclinesToSwingAtAnAwkwardDiagonal()
    {
        var action = ShadeAiBrain.ChooseNailDirection(new Vector2(2.2f, 2.2f), 0.5f, Tuning);

        Assert.Equal(ShadeAiAction.None, action);
    }

    /// <summary>
    /// In position, in reach, and the nail is still recovering. Reported as its own reason rather
    /// than as "approaching", because a Shade standing next to an enemy doing nothing is either
    /// waiting correctly or wedged, and the two are indistinguishable without being told which.
    /// <para>
    /// The rate limit that makes this state common lives in the driver
    /// (<c>GetAiNailInterval</c>): the nail cooldown is what the game permits, not what a person
    /// achieves while also dodging, so the AI is held to a fraction of it.
    /// </para>
    /// </summary>
    [Fact]
    public void ReportsWaitingOnTheNailSeparatelyFromApproaching()
    {
        // 1.9 to the side is exactly the strike point for a 0.5-radius enemy, so the Shade is
        // standing where it wants to be and the only thing left is the cooldown.
        var plan = new ShadeAiBrain().Decide(Snapshot(new[] { Basic(1, 1.9f, -0.2f) }, nailReady: false));

        Assert.Equal(ShadeAiAction.None, plan.Action);
        Assert.Equal(ShadeAiReason.Cooldown, plan.Reason);
    }

    // --- Spells -------------------------------------------------------------------------

    [Fact]
    public void DoesNotSpendSoulOnASingleBasicEnemy()
    {
        var plan = new ShadeAiBrain().Decide(Snapshot(
            new[] { Basic(1, 0f, 4f) },
            shriek: true,
            descendingDark: true,
            projectile: true));

        Assert.Equal(ShadeAiAction.None, plan.Action);
    }

    [Fact]
    public void ShrieksAtACrowdOverhead()
    {
        var plan = new ShadeAiBrain().Decide(Snapshot(
            new[] { Basic(1, 0f, 4f), Basic(2, 1f, 5f) },
            shriek: true));

        Assert.Equal(ShadeAiAction.Shriek, plan.Action);
        Assert.Equal(ShadeAiReason.ClusterSpell, plan.Reason);
        Assert.Equal(2, plan.ReasonCount);
    }

    [Fact]
    public void ShrieksAtABossOnItsOwn()
    {
        var plan = new ShadeAiBrain().Decide(Snapshot(new[] { Boss(1, 0f, 4f) }, shriek: true));

        Assert.Equal(ShadeAiAction.Shriek, plan.Action);
        Assert.Equal(ShadeAiReason.BossSpell, plan.Reason);
    }

    /// <summary>
    /// Regression for "Shade attempted to fireball 2 enemies that were not able to be hit by the
    /// spell". Enemies behind terrain were being counted toward the cast, so the Shade emptied its
    /// meter into a wall.
    /// </summary>
    [Fact]
    public void DoesNotCountEnemiesItCannotSeeTowardACast()
    {
        var plan = new ShadeAiBrain().Decide(Snapshot(
            new[] { Basic(1, 0f, 4f), Hidden(2, 1f, 5f) },
            shriek: true));

        Assert.Equal(ShadeAiAction.None, plan.Action);
    }

    [Fact]
    public void IgnoresEnemiesOutsideTheShriekCone()
    {
        // Level with the Shade rather than above it: the cone opens upward.
        int count = ShadeAiBrain.CountInShriekCone(
            new[] { Basic(1, 9f, 0f) },
            Vector2.zero,
            Tuning.ShriekConeHeight,
            Tuning.ShriekConeHalfAngleDegrees,
            out bool hitsBoss);

        Assert.Equal(0, count);
        Assert.False(hitsBoss);
    }

    [Fact]
    public void SlamsIntoACrowdBelowIt()
    {
        var plan = new ShadeAiBrain().Decide(Snapshot(
            new[] { Basic(1, 1f, -3f), Basic(2, -2f, -5f) },
            descendingDark: true));

        Assert.Equal(ShadeAiAction.DescendingDark, plan.Action);
        Assert.Equal(2, plan.ReasonCount);
    }

    /// <summary>Descending Dark dives to the ground; anything well above the Shade is not in it.</summary>
    [Fact]
    public void DoesNotCountEnemiesAboveItAsSlamTargets()
    {
        int count = ShadeAiBrain.CountInQuakeArea(
            new[] { Basic(1, 0f, 6f) },
            Vector2.zero,
            Tuning,
            out _);

        Assert.Equal(0, count);
    }

    [Fact]
    public void FiresAProjectileThroughALineOfEnemies()
    {
        var plan = new ShadeAiBrain().Decide(Snapshot(
            new[] { Basic(1, 5f, 0f), Basic(2, 9f, 0.5f) },
            projectile: true));

        Assert.Equal(ShadeAiAction.Fireball, plan.Action);
        Assert.Equal(2, plan.ReasonCount);
    }

    [Fact]
    public void OnlyCountsProjectileTargetsOnTheSideItIsFiring()
    {
        var plan = new ShadeAiBrain().Decide(Snapshot(
            new[] { Basic(1, 5f, 0f), Basic(2, -9f, 0f) },
            projectile: true));

        Assert.Equal(ShadeAiAction.None, plan.Action);
    }

    [Fact]
    public void NeverCastsASpellItHasNotUnlocked()
    {
        var plan = new ShadeAiBrain().Decide(Snapshot(new[] { Boss(1, 0f, 4f) }));

        Assert.Equal(ShadeAiAction.None, plan.Action);
    }

    [Fact]
    public void HoldsFireWhileASpellIsStillOnCooldown()
    {
        var plan = new ShadeAiBrain().Decide(Snapshot(
            new[] { Boss(1, 0f, 4f) },
            shriek: true,
            spellsReady: false));

        Assert.Equal(ShadeAiAction.None, plan.Action);
    }

    [Fact]
    public void WillNotSpendSoulThatIsReserved()
    {
        var plan = new ShadeAiBrain().Decide(Snapshot(
            new[] { Boss(1, 0f, 4f) },
            shriek: true,
            soul: 33,
            soulReserve: 33));

        Assert.Equal(ShadeAiAction.None, plan.Action);
        Assert.Equal(ShadeAiReason.SoulReserved, plan.Reason);
    }

    [Fact]
    public void SpendsSoulOnceTheReserveIsCovered()
    {
        var plan = new ShadeAiBrain().Decide(Snapshot(
            new[] { Boss(1, 0f, 4f) },
            shriek: true,
            soul: 66,
            soulReserve: 33));

        Assert.Equal(ShadeAiAction.Shriek, plan.Action);
    }

    // --- Repositioning to cast ----------------------------------------------------------

    /// <summary>
    /// A group sits overhead but off to one side, so the cone misses from here. Stepping under them
    /// takes less time than the swing the Shade would otherwise be making, so it steps.
    /// </summary>
    [Fact]
    public void StepsUnderACrowdRatherThanSwingingAtOneOfThem()
    {
        var plan = new ShadeAiBrain().Decide(Snapshot(
            new[] { Basic(1, 5f, 4f), Basic(2, 5.5f, 5f) },
            shriek: true));

        Assert.Equal(ShadeAiReason.RepositioningToCast, plan.Reason);
        Assert.Equal(ShadeAiAction.None, plan.Action);
        Assert.True(plan.Move.x > 0f, "should be moving toward the group");
        Assert.Equal(2, plan.ReasonCount);
    }

    /// <summary>
    /// The same group, too far to reach inside one attack. Repositioning has to be cheaper than
    /// attacking to be worth it, so this falls back to closing normally.
    /// </summary>
    [Fact]
    public void WillNotCrossTheRoomForACast()
    {
        var plan = new ShadeAiBrain().Decide(Snapshot(
            new[] { Basic(1, 5f, 4f), Basic(2, 5.5f, 5f) },
            shriek: true,
            moveSpeed: 1f));

        Assert.NotEqual(ShadeAiReason.RepositioningToCast, plan.Reason);
    }

    // --- Avoidance ----------------------------------------------------------------------

    /// <summary>
    /// Standing in something that hurts stops the Shade swinging, and says so - but it keeps heading
    /// for wherever it was going, because every destination this brain produces has already been
    /// pushed clear of threats, so continuing toward one is the way out of the other.
    /// </summary>
    [Fact]
    public void StopsAttackingWhileStandingInAnAttack()
    {
        var plan = new ShadeAiBrain().Decide(Snapshot(
            new[] { Basic(1, 1.9f, 0f) },
            canTakeDamage: true,
            threats: new[] { Threat(0f, 0f, 3f) }));

        Assert.Equal(ShadeAiReason.Evading, plan.Reason);
        Assert.Equal(ShadeAiAction.None, plan.Action);
        Assert.NotEqual(Vector2.zero, plan.Move);
    }

    /// <summary>
    /// Regression for "the Shade appears to be unable to path to the target reticle". An enemy
    /// between the Shade and where it was sent used to make the trip impossible: it would approach,
    /// enter the enemy's body hitbox, be turned round, approach again, forever - and the same order
    /// completed the moment that enemy was dead.
    /// </summary>
    [Fact]
    public void StillHeadsForAnOrderedSpotPastAnEnemy()
    {
        var plan = new ShadeAiBrain().Decide(Snapshot(
            new[] { Basic(1, 0.5f, 0f) },
            canTakeDamage: true,
            threats: new[] { Threat(0f, 0f, 2f) },
            command: new Vector2(8f, 0f)));

        Assert.Equal(ShadeAiReason.Evading, plan.Reason);
        Assert.True(plan.Move.x > 0f, "should still be heading for the ordered spot, not away from it");
        Assert.Equal(new Vector2(8f, 0f), plan.DesiredPosition);
    }

    // --- Threat-aware steering ----------------------------------------------------------

    [Fact]
    public void TreatsAThreatOnThePathAsBlocking()
    {
        var threats = new[] { Threat(4f, 0f, 1f) };

        Assert.True(ShadeAiNavigator.ThreatBlocks(threats, Vector2.zero, Vector2.right, 8f, 0.4f, 0.6f));
    }

    [Fact]
    public void LetsAHeadingPastAThreatThrough()
    {
        var threats = new[] { Threat(4f, 0f, 1f) };

        Assert.False(ShadeAiNavigator.ThreatBlocks(threats, Vector2.zero, Vector2.up, 8f, 0.4f, 0.6f));
    }

    /// <summary>
    /// The way round an obstacle should be the shallowest one that clears it, not a right-angle
    /// detour. Reported as the Shade travelling to the far side of the screen to get past a platform
    /// it was sitting just under: the fan required a candidate heading to be clear for the full
    /// seven-unit look-ahead, so every shallow way round was rejected for something in the distance
    /// and only the perpendicular heading survived.
    /// <para>
    /// Steerable without an engine because <c>ShadeAiTerrain</c> answers "clear" to everything when
    /// there is no Terrain layer to query, which leaves the threat ring as the only obstacle - and
    /// that half is pure geometry.
    /// </para>
    /// </summary>
    [Fact]
    public void GoesRoundAnObstacleByTheShallowestWayThatClearsIt()
    {
        var navigator = new ShadeAiNavigator();
        // Sits squarely on the direct line, six units out, and is wide. Placed and sized so the two
        // look-aheads disagree: within seven units the first fan step still clips it, within two and
        // a half the segment stops short of it. That is exactly the case the old code got wrong.
        var threats = new[] { Threat(6f, 0f, 2.6f) };

        var heading = navigator.Steer(
            Vector2.zero,
            new Vector2(12f, 0f),
            bodyRadius: 0.3f,
            time: 1f,
            threats: threats,
            threatStandoff: 0.2f);

        Assert.True(navigator.LastPathBlocked, "the direct line should have been recognised as blocked");

        float turn = Vector2.Angle(Vector2.right, heading);
        Assert.True(turn > 1f, $"expected a detour, got the direct heading ({turn:0.0} degrees off)");
        Assert.True(turn < 30f, $"expected the first fan step, got {turn:0.0} degrees off the direct line");
    }

    /// <summary>
    /// Once the way ahead opens the Shade should turn towards it promptly. The detour used to be
    /// latched for the side-commitment window - most of a second - which is the rest of the overshoot
    /// in the report above.
    /// </summary>
    [Fact]
    public void ReturnsToTheDirectLineSoonAfterItClears()
    {
        var navigator = new ShadeAiNavigator();
        var threats = new[] { Threat(6f, 0f, 2.6f) };
        var target = new Vector2(12f, 0f);

        navigator.Steer(Vector2.zero, target, 0.3f, 1f, threats, 0.2f);
        Assert.True(navigator.LastPathBlocked);

        // The obstacle is gone. The first clear frame is still ignored - a single one is noise.
        var immediately = navigator.Steer(Vector2.zero, target, 0.3f, 1.05f, null, 0.2f);
        Assert.NotEqual(Vector2.right, immediately);

        var shortlyAfter = navigator.Steer(Vector2.zero, target, 0.3f, 1.3f, null, 0.2f);
        Assert.Equal(Vector2.right.x, shortlyAfter.x, 3);
        Assert.Equal(Vector2.right.y, shortlyAfter.y, 3);
        Assert.False(navigator.LastPathBlocked);
    }

    /// <summary>
    /// A threat the Shade is already inside blocks nothing: no heading avoids it, and treating it as
    /// blocking would reject the very headings that lead back out.
    /// </summary>
    [Fact]
    public void IgnoresAThreatItIsAlreadyInside()
    {
        var threats = new[] { Threat(0f, 0f, 3f) };

        Assert.False(ShadeAiNavigator.ThreatBlocks(threats, Vector2.zero, Vector2.right, 8f, 0.4f, 0.6f));
    }

    /// <summary>
    /// Threat avoidance pushes a destination clear of a hitbox with no idea where the walls are, so
    /// in a closed boss arena it can land one inside the wall. The Shade then grinds against the edge
    /// forever trying to reach somewhere it cannot stand.
    /// </summary>
    [Fact]
    public void PullsAnUnstandableDestinationBackIntoTheRoom()
    {
        // Everything past x = 5 is solid.
        bool Blocked(Vector2 point) => point.x > 5f;

        var resolved = ShadeAiNavigator.PullBackToStandable(Vector2.zero, new Vector2(10f, 0f), Blocked);

        Assert.True(resolved.x <= 5f, "should have been pulled back out of the wall");
        Assert.True(resolved.x > 0f, "should not have given up and stayed put");
    }

    [Fact]
    public void LeavesAStandableDestinationAlone()
    {
        var desired = new Vector2(4f, 2f);

        Assert.Equal(desired, ShadeAiNavigator.PullBackToStandable(Vector2.zero, desired, _ => false));
    }

    /// <summary>Solid all the way there: standing still beats pressing into a wall.</summary>
    [Fact]
    public void StaysPutWhenEverythingBetweenIsSolid()
    {
        Assert.Equal(Vector2.zero, ShadeAiNavigator.PullBackToStandable(Vector2.zero, new Vector2(9f, 0f), _ => true));
    }

    [Theory]
    [InlineData(0f, 5f, 5f)]
    [InlineData(5f, 3f, 3f)]
    [InlineData(20f, 0f, 10f)]
    public void MeasuresDistanceToASegment(float pointX, float pointY, float expected)
    {
        float distance = ShadeAiNavigator.DistanceToSegment(
            new Vector2(pointX, pointY),
            Vector2.zero,
            new Vector2(10f, 0f));

        Assert.Equal(expected, distance, 3);
    }

    /// <summary>Invincible, so there is nothing to dodge and the fight carries on.</summary>
    [Fact]
    public void IgnoresThreatsWhileInvincible()
    {
        var plan = new ShadeAiBrain().Decide(Snapshot(
            new[] { Basic(1, 1.9f, 0f) },
            canTakeDamage: false,
            threats: new[] { Threat(0f, 0f, 3f) }));

        Assert.Equal(ShadeAiAction.SlashSide, plan.Action);
    }

    [Fact]
    public void PushesAPositionClearOfOverlappingThreats()
    {
        var adjusted = ShadeAiBrain.AvoidThreats(
            new Vector2(1f, 0f),
            new[] { Threat(0f, 0f, 3f) },
            0.5f);

        Assert.Equal(3.5f, adjusted.magnitude, 3);
    }

    [Fact]
    public void LeavesAPositionAloneWhenNothingThreatensIt()
    {
        var desired = new Vector2(3f, 1f);

        Assert.Equal(desired, ShadeAiBrain.AvoidThreats(desired, Array.Empty<ShadeAiThreat>(), 0.5f));
        Assert.Equal(desired, ShadeAiBrain.AvoidThreats(desired, null, 0.5f));
    }

    [Fact]
    public void KnowsWhichPositionsAreSafe()
    {
        var threats = new[] { Threat(0f, 0f, 2f) };

        Assert.False(ShadeAiBrain.IsSafeAt(new Vector2(1f, 0f), threats, 0.5f));
        Assert.True(ShadeAiBrain.IsSafeAt(new Vector2(4f, 0f), threats, 0.5f));
    }

    // --- Healing ------------------------------------------------------------------------

    [Fact]
    public void HealsItselfWhenLowAndClear()
    {
        var plan = new ShadeAiBrain().Decide(Snapshot(
            new[] { Basic(1, 3f, 0f) },
            canTakeDamage: true,
            canFocusHeal: true,
            selfHealth: 0.2f));

        Assert.Equal(ShadeAiAction.Focus, plan.Action);
        Assert.Equal(ShadeAiReason.Healing, plan.Reason);
        Assert.Equal(Vector2.zero, plan.Move);
    }

    /// <summary>
    /// Focus pins the Shade in place for the whole channel, so it needs more clearance to start one
    /// than simply standing would need. It walks somewhere clear enough first rather than channelling
    /// where it is.
    /// </summary>
    [Fact]
    public void WillNotStandStillToHealInsideAnAttack()
    {
        var plan = new ShadeAiBrain().Decide(Snapshot(
            new[] { Basic(1, 3f, 0f) },
            canTakeDamage: true,
            canFocusHeal: true,
            selfHealth: 0.2f,
            threats: new[] { Threat(0f, 0f, 3f) }));

        Assert.NotEqual(ShadeAiAction.Focus, plan.Action);
        Assert.NotEqual(Vector2.zero, plan.Move);
        Assert.True(plan.DesiredPosition.magnitude > 3f, "should be heading somewhere clear of the attack");
    }

    /// <summary>
    /// A channel already under way is worth finishing. Focus drains SOUL the whole time and refunds
    /// none of it, so dropping one at eighty percent costs eighty percent of a heal and buys nothing.
    /// A threat that has drifted near but is not actually on the Shade must not cause that - which is
    /// the bug that wasted most of a meter and then had to heal all over again.
    /// </summary>
    [Fact]
    public void FinishesAChannelAThreatOnlyDriftedNear()
    {
        var brain = new ShadeAiBrain();

        // Inside the margin a fresh channel would demand, outside the one that means it is being hit.
        var nearby = new[] { Threat(2.4f, 0f, 0.5f) };

        var plan = brain.Decide(Snapshot(
            Array.Empty<ShadeAiTarget>(),
            canTakeDamage: true,
            canFocusHeal: true,
            selfHealth: 0.2f,
            threats: nearby,
            isFocusing: true));

        Assert.Equal(ShadeAiAction.Focus, plan.Action);
    }

    /// <summary>The same threat is still enough to stop one being started.</summary>
    [Fact]
    public void DoesNotStartAChannelWithAThreatThatClose()
    {
        var plan = new ShadeAiBrain().Decide(Snapshot(
            Array.Empty<ShadeAiTarget>(),
            canTakeDamage: true,
            canFocusHeal: true,
            selfHealth: 0.2f,
            threats: new[] { Threat(2.4f, 0f, 0.5f) },
            isFocusing: false));

        Assert.NotEqual(ShadeAiAction.Focus, plan.Action);
    }

    /// <summary>
    /// Healing Hornet is a side effect of the Shade healing itself near her, so when she is the one
    /// who is low the Shade has to close the distance before the channel is worth starting.
    /// </summary>
    [Fact]
    public void ClosesOnHornetBeforeHealingForHer()
    {
        var plan = new ShadeAiBrain().Decide(Snapshot(
            new[] { Basic(1, 3f, 0f) },
            shade: new Vector2(10f, 0f),
            hornet: Vector2.zero,
            canFocusHeal: true,
            hornetHealth: 0.2f));

        Assert.Equal(ShadeAiReason.Healing, plan.Reason);
        Assert.Equal(ShadeAiAction.None, plan.Action);
        Assert.True(plan.Move.x < 0f, "should be moving toward Hornet");
    }

    [Fact]
    public void DoesNotHealAtFullHealth()
    {
        var plan = new ShadeAiBrain().Decide(Snapshot(
            new[] { Basic(1, 2.5f, 0f) },
            canTakeDamage: true,
            canFocusHeal: true,
            selfHealth: 1f));

        Assert.Equal(ShadeAiAction.SlashSide, plan.Action);
    }

    [Fact]
    public void DoesNotHealWithoutTheSoulForIt()
    {
        var plan = new ShadeAiBrain().Decide(Snapshot(
            new[] { Basic(1, 2.5f, 0f) },
            canTakeDamage: true,
            canFocusHeal: true,
            selfHealth: 0.2f,
            soul: 10));

        Assert.Equal(ShadeAiAction.SlashSide, plan.Action);
    }

    [Fact]
    public void HealingIsOffWhenFocusCannotDoAnything()
    {
        Assert.False(ShadeAiBrain.ShouldHeal(Snapshot(
            Array.Empty<ShadeAiTarget>(),
            canTakeDamage: true,
            canFocusHeal: false,
            selfHealth: 0.1f)));
    }
}
