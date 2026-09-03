#nullable enable

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace LegacyoftheAbyss.Shade
{
    /// <summary>One <see cref="Shadow"/> or outline copied off a borrowed label.</summary>
    internal struct UiShadowStyle
    {
        public Type Type;
        public Color EffectColor;
        public Vector2 EffectDistance;
        public bool UseGraphicAlpha;
    }

    /// <summary>
    /// Everything about how one of the game's own <see cref="Text"/> labels is drawn.
    /// <para>
    /// Both the settings menu and the inventory pane build their rows by cloning a game prefab and
    /// then writing their own text into it, and both have to put the prefab's typography back
    /// afterwards or the row comes out in Unity's default Arial. They had grown identical copies of
    /// this record and of the capture below.
    /// </para>
    /// </summary>
    internal struct UiTextStyle
    {
        public Font? Font;
        public int FontSize;
        public FontStyle FontStyle;
        public TextAnchor Alignment;
        public Color Color;
        public bool RichText;
        public bool BestFit;
        public int BestFitMin;
        public int BestFitMax;
        public float LineSpacing;
        public bool AlignByGeometry;
        public HorizontalWrapMode HorizontalOverflow;
        public VerticalWrapMode VerticalOverflow;
        public List<UiShadowStyle>? Shadows;
    }

    internal static class UiTextStyles
    {
        /// <summary>
        /// The shadows and outlines on a label. Guarded throughout: this runs against prefabs the
        /// game owns, and one that has been part-destroyed should cost a row its drop shadow rather
        /// than throw out of a menu build.
        /// </summary>
        internal static List<UiShadowStyle> CaptureShadows(Graphic graphic)
        {
            var list = new List<UiShadowStyle>();
            if (graphic == null)
            {
                return list;
            }

            try
            {
                foreach (var shadow in graphic.GetComponents<Shadow>())
                {
                    if (shadow == null)
                    {
                        continue;
                    }

                    list.Add(new UiShadowStyle
                    {
                        Type = shadow.GetType(),
                        EffectColor = shadow.effectColor,
                        EffectDistance = shadow.effectDistance,
                        UseGraphicAlpha = shadow.useGraphicAlpha
                    });
                }
            }
            catch
            {
            }

            return list;
        }

        /// <summary>Reads a label's typography, shadows included.</summary>
        internal static UiTextStyle Capture(Text text)
        {
            return new UiTextStyle
            {
                Font = text.font,
                FontSize = text.fontSize,
                FontStyle = text.fontStyle,
                Alignment = text.alignment,
                Color = text.color,
                RichText = text.supportRichText,
                BestFit = text.resizeTextForBestFit,
                BestFitMin = text.resizeTextMinSize,
                BestFitMax = text.resizeTextMaxSize,
                LineSpacing = text.lineSpacing,
                AlignByGeometry = text.alignByGeometry,
                HorizontalOverflow = text.horizontalOverflow,
                VerticalOverflow = text.verticalOverflow,
                Shadows = CaptureShadows(text)
            };
        }
    }
}
