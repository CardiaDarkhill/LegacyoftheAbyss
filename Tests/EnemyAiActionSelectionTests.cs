using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Xunit;

/// <summary>
/// Guards the assumption the whole Shade-aggro redirect rests on: that Silksong's enemy targeting is
/// reachable through PlayMaker's <c>ActionCategory("Enemy AI")</c> action set, and that those actions
/// name their target in a public <c>FsmGameObject</c> field.
/// <para>
/// This deliberately re-derives the selection with plain reflection rather than calling into
/// <c>LegacyHelper.EnemyAiRetargeting</c>, whose <c>TargetMethods</c> is private to a Harmony patch
/// and only meaningful inside a patched process. If a game update renames the category, moves the
/// actions to another assembly, or changes the field type, the redirect quietly stops finding
/// anything to patch and enemies go back to ignoring the Shade with no error anywhere - which is
/// exactly the kind of silent regression worth a test.
/// </para>
/// </summary>
public class EnemyAiActionSelectionTests
{
    /// <summary>Assembly-CSharp, reached without naming it: HeroController lives there.</summary>
    private static Assembly GameAssembly => typeof(HeroController).Assembly;

    [Fact]
    public void EnemyAiActionsExistInTheGameAssembly()
    {
        var actions = FindEnemyAiActions();

        // 63 at time of writing. The floor is deliberately well below that - this is checking the
        // category still exists and is populated, not pinning an exact count that every patch would
        // churn.
        Assert.True(actions.Count >= 20, $"Expected the Enemy AI action category to be populated, found {actions.Count}");
    }

    [Fact]
    public void EnemyAiActionsExposeTheirTargetAsAnFsmGameObjectField()
    {
        var withTargets = FindEnemyAiActions().Where(HasRedirectableTargetField).ToList();

        Assert.True(withTargets.Count >= 10, $"Expected Enemy AI actions naming a GameObject target, found {withTargets.Count}");
    }

    /// <summary>
    /// The specific actions that make an enemy move toward, turn to face, or shoot at something. If
    /// any of these stops being redirectable, that capability is gone even if the counts above still
    /// look healthy.
    /// </summary>
    [Theory]
    [InlineData("ChaseObject")]
    [InlineData("ChaseObjectV3")]
    [InlineData("FaceObjectV4")]
    [InlineData("FireAtTarget")]
    [InlineData("DistanceFlyV3")]
    [InlineData("GetAngleToTarget2D")]
    public void KnownTargetingActionsAreStillRedirectable(string actionName)
    {
        var action = FindEnemyAiActions().SingleOrDefault(t => t.Name == actionName);

        Assert.NotNull(action);
        Assert.True(HasRedirectableTargetField(action), $"{actionName} no longer names its target in an FsmGameObject field");
    }

    private static bool HasRedirectableTargetField(Type type)
    {
        return type.GetFields(BindingFlags.Public | BindingFlags.Instance)
            .Any(f => f.FieldType.Name == "FsmGameObject" &&
                      !f.Name.StartsWith("store", StringComparison.OrdinalIgnoreCase));
    }

    private static List<Type> FindEnemyAiActions()
    {
        return SafeGetTypes(GameAssembly)
            .Where(t => t != null && !t.IsAbstract && IsFsmStateAction(t) && IsEnemyAiAction(t))
            .ToList();
    }

    private static bool IsFsmStateAction(Type type)
    {
        for (Type current = type.BaseType; current != null; current = current.BaseType)
        {
            if (current.Name == "FsmStateAction")
            {
                return true;
            }
        }

        return false;
    }

    private static bool IsEnemyAiAction(Type type)
    {
        try
        {
            foreach (var attribute in type.GetCustomAttributes(false))
            {
                if (attribute == null || attribute.GetType().Name != "ActionCategoryAttribute")
                {
                    continue;
                }

                var attributeType = attribute.GetType();
                bool matches = attributeType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                        .Any(p => p.PropertyType == typeof(string) && (string)p.GetValue(attribute, null) == "Enemy AI")
                    || attributeType.GetFields(BindingFlags.Public | BindingFlags.Instance)
                        .Any(f => f.FieldType == typeof(string) && (string)f.GetValue(attribute) == "Enemy AI");

                if (matches)
                {
                    return true;
                }
            }
        }
        catch
        {
        }

        return false;
    }

    private static IEnumerable<Type> SafeGetTypes(Assembly assembly)
    {
        try
        {
            return assembly.GetTypes();
        }
        catch (ReflectionTypeLoadException ex)
        {
            return ex.Types.Where(t => t != null);
        }
    }
}
