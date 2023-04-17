using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;

namespace RitualRewards.Sinnamon_Ritual;

public class Sinnamon_GameCondition_Aurora : GameCondition
{
    private int curColorIndex = -1;

    private int prevColorIndex = -1;

    private float curColorTransition;

    private static readonly Color[] Colors = new Color[8]
    {
            new Color(0f, 0.5f, 0f),
            new Color(0.1f, 0.5f, 0f),
            new Color(0f, 0.5f, 0.2f),
            new Color(0.3f, 0.5f, 0.3f),
            new Color(0f, 0.2f, 0.5f),
            new Color(0f, 0f, 0.5f),
            new Color(0.5f, 0f, 0f),
            new Color(0.3f, 0f, 0.5f)
    };

    public Color CurrentColor => Color.Lerp(Colors[prevColorIndex], Colors[curColorIndex], curColorTransition);

    private int TransitionDurationTicks => !Permanent ? 280 : 3750;

    public override int TransitionTicks => 200;

    public override void Init()
    {
        base.Init();
        curColorIndex = Rand.Range(0, Colors.Length);
        prevColorIndex = curColorIndex;
        curColorTransition = 1f;
    }

    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_Values.Look(ref curColorIndex, "curColorIndex", 0);
        Scribe_Values.Look(ref prevColorIndex, "prevColorIndex", 0);
        Scribe_Values.Look(ref curColorTransition, "curColorTransition", 0f);
    }

    private int GetNewColorIndex()
    {
        return (from x in Enumerable.Range(0, Colors.Length)
                where x != curColorIndex
                select x).RandomElement();
    }

    public override void GameConditionTick()
    {
        curColorTransition += 1f / TransitionDurationTicks;
        if (curColorTransition >= 1f)
        {
            prevColorIndex = curColorIndex;
            curColorIndex = GetNewColorIndex();
            curColorTransition = 0f;
        }
    }

    public override SkyTarget? SkyTarget(Map map)
    {
        Color currentColor = CurrentColor;
        return new SkyTarget(colorSet: new SkyColorSet(Color.Lerp(Color.white, currentColor, 0.075f) * Brightness(map), new Color(0.92f, 0.92f, 0.92f), Color.Lerp(Color.white, currentColor, 0.025f) * Brightness(map), 1f), glow: Mathf.Max(GenCelestial.CurCelestialSunGlow(map), 0.25f), lightsourceShineSize: 1f, lightsourceShineIntensity: 1f);
    }

    private static float Brightness(Map map)
    {
        return Mathf.Max(0.73f, GenCelestial.CurCelestialSunGlow(map));
    }

    public override float SkyGazeChanceFactor(Map map)
    {
        return 8f;
    }

    public override float SkyGazeJoyGainFactor(Map map)
    {
        return 5f;
    }

    public override float SkyTargetLerpFactor(Map map)
    {
        return GameConditionUtility.LerpInOutValue(this, TransitionTicks);
    }
}
