using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace GenepackReprocessor;

public static class GenepackLogCurve
{
    public static readonly SimpleCurve ComplexityToCreationHoursCurve = new SimpleCurve
    {
        new CurvePoint(0f,  1f),
        new CurvePoint(4f,  11f),
        new CurvePoint(8f,  16f),
        new CurvePoint(12f, 19f),
        new CurvePoint(16f, 21f),
        new CurvePoint(20f, 22f),
        new CurvePoint(24f, 23f)
    };
}

public static class GenepackExpCurves
{
    public static readonly SimpleCurve ComplexityToCreationHoursCurve = new SimpleCurve
    {
        new CurvePoint(0f,  1f),
        new CurvePoint(4f,  6f),
        new CurvePoint(8f,  11f),
        new CurvePoint(12f, 18f),
        new CurvePoint(16f, 25f),
        new CurvePoint(20f, 32f),
        new CurvePoint(24f, 40f)
    };
}

public static class GenepackLinCurves
{
    public static readonly SimpleCurve ComplexityToCreationHoursCurve = new SimpleCurve
    {
        new CurvePoint(0f,  2f),
        new CurvePoint(4f,  6f),
        new CurvePoint(8f,  10f),
        new CurvePoint(12f, 14f),
        new CurvePoint(16f, 18f),
        new CurvePoint(20f, 22f),
        new CurvePoint(24f, 26f)
    };
}