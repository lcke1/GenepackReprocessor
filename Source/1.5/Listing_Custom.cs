using System;
using System.Collections.Generic;
using System.Reflection;
using LudeonTK;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Noise;
using Verse.Sound;

namespace GenepackReprocessor;

[StaticConstructorOnStartup]
public class Listing_Custom : Listing_Standard
{
    public override void Begin(Rect rect)
    {
        base.Begin(rect);
        //this.SetLabelScrollbarPosition(100f, 10f, new Vector2(10f, 20f));
    }

    public void NGTextFieldNumeric<T>(ref T val, ref string buffer, float min = 0f, float max = 1E+09f) where T : struct
    {
        Rect rect = GetRect(Text.LineHeight);
        if (!BoundingRectCached.HasValue || rect.Overlaps(BoundingRectCached.Value))
        {
            Widgets.TextFieldNumeric(rect, ref val, ref buffer, min, max);
        }
        //Gap(verticalSpacing);
    }

    public Listing_Custom BeginSection(float height, float sectionBorder = 4f, float bottomBorder = 4f)
    {
        Rect rect = GetRect(height + sectionBorder + bottomBorder);
        Widgets.DrawMenuSection(rect);
        Listing_Custom listing_Standard = new Listing_Custom();
        Rect rect2 = new Rect(rect.x + sectionBorder, rect.y + sectionBorder, rect.width - sectionBorder * 2f, rect.height - (sectionBorder + bottomBorder));
        listing_Standard.Begin(rect2);
        return listing_Standard;
    }

    public void EndSection(Listing_Custom listing)
    {
        listing.End();
    }

    /// <summary>
    /// My own num text field, it allows for multiple rows of left justified text and input boxes.
    /// Requires a call to Gap() after a row is drawn.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="rect">Needed for where to draw elements.</param>
    /// <param name="cols">How many columns will be fit on this row.</param>
    /// <param name="index">Which column is this for.</param>
    /// <param name="label">Title of the field.</param>
    /// <param name="val">Field value.</param>
    /// <param name="buffer">Where the string entered is saved to.</param>
    /// <param name="min">Minimal value enterable.</param>
    /// <param name="max">Maximal value enterable.</param>
    /// <param name="labelFieldSplit">From 0.0-1.0, how much of the rect is for the label, the rest will be the field.</param>
    /// <param name="rightBuff">From 0.0-1.0, How much space on the right of the text field should be empty.</param>
    /// <param name="tooltip">When hovering this text is displayed.</param>
    /// <param name="tooltipDelay">How long in seconds before the tooltip appears, if there's a tooltip.</param>
    public void NGTextFieldNumericLabeled<T>(Rect rect, int cols, int index, string label, ref T val, ref string buffer, float min = 0f, float max = 1E+09f,
        float labelFieldSplit = 0.5f, float rightBuff = 0.0f, string tooltip = null, float tooltipDelay = 0.5f) where T : struct
    {
        // Magic number, a small buffer to the size of each entry
        float buff = 10f;

        // Split the rect into a left and right side, so we can make the label stick to the left side!
        // Calculate the correct area
        float columnWidth = (rect.xMax - rect.xMin) / cols;
        rect.xMin = rect.xMin + (columnWidth * index);
        if (index > 0) { rect.xMin += buff; }
        rect.xMax = rect.xMin + columnWidth;
        if (index < cols) { rect.xMax -= buff; }

        Rect rectField = rect;
        Rect toolTipRect = rect;

        rect.xMax = rect.xMin + ((rect.xMax - rect.xMin) * labelFieldSplit);
        rectField.xMin = rect.xMax;
        rectField.xMax = rectField.xMax - columnWidth * rightBuff;

        // Split the rect into a left and right side, so we can make the label stick to the left side!
        if (!BoundingRectCached.HasValue || rect.Overlaps(BoundingRectCached.Value))
        {
            Widgets.LabelFit(rect, label);
            Widgets.TextFieldNumeric(rectField, ref val, ref buffer, min, max);
        }
        // Tooltip
        if (!tooltip.NullOrEmpty())
        {
            if (Mouse.IsOver(toolTipRect))
            {
                Widgets.DrawHighlight(toolTipRect);
            }
            TipSignal tip = (new TipSignal(tooltip, tooltipDelay));
            TooltipHandler.TipRegion(toolTipRect, tip);
        }
    }

    public Rect GetRectLine()
    {
        return GetRect(Text.LineHeight);
    }
    public void Gap()
    {
        Gap(verticalSpacing);
    }

    public void NGCheckboxLabeled(Rect rect, int cols, int index, string label, ref bool checkOn, float rightBuff = 0.0f,
        string tooltip = null, float tooltipDelay = 0.5f)
    {
        // Magic number, a small buffer to the size of each entry
        float buff = 10f;
        // Split the rect into a left and right side, so we can make the label stick to the left side!
        // Calculate the correct area
        float columnWidth = (rect.xMax - rect.xMin) / cols;
        rect.xMin = rect.xMin + (columnWidth * index);
        if (index > 0) { rect.xMin += buff; }
        rect.xMax = rect.xMin + columnWidth;
        if (index < cols) { rect.xMax -= buff; }
        rect.xMax = rect.xMax - columnWidth * rightBuff;

        {
            if (!tooltip.NullOrEmpty())
            {
                if (Mouse.IsOver(rect))
                {
                    Widgets.DrawHighlight(rect);
                }
                TipSignal tip = (new TipSignal(tooltip, tooltipDelay));
                TooltipHandler.TipRegion(rect, tip);
            }
            Widgets.CheckboxLabeled(rect, label, ref checkOn);
        }
    }

    public bool NGRadioButton(Rect rect, int cols, int index, string label, bool active, float rightBuff = 0.0f,
        string tooltip = null, float tooltipDelay = 0.5f, bool disabled = false)
    {
        // Magic number, a small buffer to the size of each entry
        float buff = 10f;
        // Split the rect into a left and right side, so we can make the label stick to the left side!
        // Calculate the correct area
        float columnWidth = (rect.xMax - rect.xMin) / cols;
        rect.xMin = rect.xMin + (columnWidth * index);
        if (index > 0) { rect.xMin += buff; }
        rect.xMax = rect.xMin + columnWidth;
        if (index < cols) { rect.xMax -= buff; }
        rect.xMax = rect.xMax - columnWidth * rightBuff;

        if (BoundingRectCached.HasValue && !rect.Overlaps(BoundingRectCached.Value))
        {
            return false;
        }
        if (!tooltip.NullOrEmpty())
        {
            if (Mouse.IsOver(rect))
            {
                Widgets.DrawHighlight(rect);
            }
            TipSignal tip = (new TipSignal(tooltip, tooltipDelay));
            TooltipHandler.TipRegion(rect, tip);
        }
        bool result = Widgets.RadioButtonLabeled(rect, label, active, disabled);
        return result;
    }

    public void ColLabel(Rect rect, int cols, int index, string label,
        string tooltip = null, float tooltipDelay = 0.5f)
    {
        // Magic number, a small buffer to the size of each entry
        float buff = 10f;
        // Split the rect into a left and right side, so we can make the label stick to the left side!
        // Calculate the correct area
        float columnWidth = (rect.xMax - rect.xMin) / cols;
        rect.xMin = rect.xMin + (columnWidth * index);
        if (index > 0) { rect.xMin += buff; }
        rect.xMax = rect.xMin + columnWidth;
        if (index < cols) { rect.xMax -= buff; }

        if (!tooltip.NullOrEmpty())
        {
            if (Mouse.IsOver(rect))
            {
                Widgets.DrawHighlight(rect);
            }
            TipSignal tip = (new TipSignal(tooltip, tooltipDelay));
            TooltipHandler.TipRegion(rect, tip);
        }
        Widgets.Label(rect, label);
    }


    public bool CButtonTextLabeledPct(Rect rect, int cols, int index, string label, 
        string buttonLabel, float labelPct, TextAnchor anchor = TextAnchor.UpperLeft, 
        string highlightTag = null, string tooltip = null, Texture2D labelIcon = null)
    {
        // Magic number, a small buffer to the size of each entry
        float buff = 10f;
        // Split the rect into a left and right side, so we can make the label stick to the left side!
        // Calculate the correct area
        float columnWidth = (rect.xMax - rect.xMin) / cols;
        rect.xMin = rect.xMin + (columnWidth * index);
        if (index > 0) { rect.xMin += buff; }
        rect.xMax = rect.xMin + columnWidth;
        if (index < cols) { rect.xMax -= buff; }

        Rect rect2 = rect.RightPart(1f - labelPct);
        rect2.height = 30f;

        if (highlightTag != null)
        {
            UIHighlighter.HighlightOpportunity(rect, highlightTag);
        }
        bool result = false;
        Rect rect3 = rect.LeftPart(labelPct);
        if (!BoundingRectCached.HasValue || rect.Overlaps(BoundingRectCached.Value))
        {
            Text.Anchor = anchor;
            Widgets.Label(rect3, label);
            result = Widgets.ButtonText(rect2, buttonLabel.Truncate(rect2.width - 20f));
            Text.Anchor = TextAnchor.UpperLeft;
        }
        if (labelIcon != null)
        {
            GUI.DrawTexture(new Rect(Text.CalcSize(label).x + 10f, rect3.y + (rect3.height - Text.LineHeight) / 2f, Text.LineHeight, Text.LineHeight), labelIcon);
        }
        if (!tooltip.NullOrEmpty())
        {
            if (Mouse.IsOver(rect3))
            {
                Widgets.DrawHighlight(rect3);
            }
            TooltipHandler.TipRegion(rect3, tooltip);
        }
        return result;
    }

    public bool CButtonText(Rect rect, int cols, int index, string label, 
        string highlightTag = null,
        string tooltip = null, float tooltipDelay = 0.5f)
    {
        // Magic number, a small buffer to the size of each entry
        float buff = 10f;
        // Split the rect into a left and right side, so we can make the label stick to the left side!
        // Calculate the correct area
        float columnWidth = (rect.xMax - rect.xMin) / cols;
        rect.xMin = rect.xMin + (columnWidth * index);
        if (index > 0) { rect.xMin += buff; }
        rect.xMax = rect.xMin + columnWidth;
        if (index < cols) { rect.xMax -= buff; }

        bool result = false;
        // if (!BoundingRectCached.HasValue || rect.Overlaps(BoundingRectCached.Value))
        {
            result = Widgets.ButtonText(rect, label);
            if (highlightTag != null)
            {
                UIHighlighter.HighlightOpportunity(rect, highlightTag);
            }
        }
        return result;
    }

    // Custom Sec
    public Listing_Custom CBeginSection(Rect rec, float height, float sectionBorder = 4f, float bottomBorder = 4f)
    {
        Widgets.DrawMenuSection(rec);
        Listing_Custom listing_Standard = new Listing_Custom();
        Rect rect2 = new Rect(rec.x + sectionBorder, rec.y + sectionBorder, rec.width - sectionBorder * 2f, rec.height - (sectionBorder + bottomBorder));
        listing_Standard.Begin(rect2);
        return listing_Standard;
    }
}
