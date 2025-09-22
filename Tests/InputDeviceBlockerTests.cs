using System;
using System.Reflection;
using System.Runtime.Serialization;
using GlobalEnums;
using UnityEngine;
using Xunit;

public class InputDeviceBlockerTests
{
    [Fact]
    public void AllowsShadeControllerInMenuStates()
    {
        using (var environment = new InputBlockerEnvironment())
        {
            environment.SetUiState(UIState.PLAYING);
            Assert.True(LegacyHelper.InputDeviceBlocker.ShouldBlockShadeDeviceInput());

            environment.SetUiState("inventory_overlay");
            Assert.False(LegacyHelper.InputDeviceBlocker.ShouldBlockShadeDeviceInput());

            environment.SetUiState("MapScreen");
            Assert.False(LegacyHelper.InputDeviceBlocker.ShouldBlockShadeDeviceInput());
        }
    }

    private sealed class InputBlockerEnvironment : IDisposable
    {
        private readonly object originalGameManager;
        private readonly object originalUiManager;
        private readonly float originalTimeScale;
        private readonly GameManager gm;
        private readonly TestUIManager ui;

        internal InputBlockerEnvironment()
        {
            originalGameManager = GetStaticField(typeof(GameManager), "_instance");
            originalUiManager = GetStaticField(typeof(UIManager), "_instance");
            originalTimeScale = Time.timeScale;

            gm = (GameManager)FormatterServices.GetUninitializedObject(typeof(GameManager));
            ui = (TestUIManager)FormatterServices.GetUninitializedObject(typeof(TestUIManager));

            SetProperty(gm, "GameState", GameState.PLAYING);
            SetProperty(gm, "ui", ui);

            SetStaticField(typeof(GameManager), "_instance", gm);
            SetStaticField(typeof(UIManager), "_instance", ui);

            Time.timeScale = 1f;
        }

        internal void SetUiState(UIState state)
        {
            ui.uiState = state;
        }

        internal void SetUiState(string stateName)
        {
            ui.uiState = stateName;
        }

        public void Dispose()
        {
            SetStaticField(typeof(GameManager), "_instance", originalGameManager);
            SetStaticField(typeof(UIManager), "_instance", originalUiManager);
            Time.timeScale = originalTimeScale;
        }

        private static object GetStaticField(Type type, string name)
        {
            var field = type.GetField(name, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
            return field?.GetValue(null);
        }

        private static void SetStaticField(Type type, string name, object value)
        {
            var field = type.GetField(name, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
            field?.SetValue(null, value);
        }

        private static void SetProperty(object target, string name, object value)
        {
            if (target == null)
            {
                return;
            }

            var property = target.GetType().GetProperty(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            property?.SetValue(target, value, null);
        }

        private sealed class TestUIManager : UIManager
        {
            public new object uiState;
        }
    }
}
