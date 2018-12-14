using System;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

public class CapacitorBank : ElectricNode
{
    public ElectricLine Line;
    public ElectricLine MainCapacitorLine;
    public float PowerFactorThreshold = 0.9f;
    public bool TurnedOn = true;
    public ElectricLine[] CapacitorLines;

    private Stack<ElectricLine> UsedCapacitorLines = new Stack<ElectricLine>();

    public override string GetHelpText()
    {
        if (TurnedOn)
            return $"Threshold = {PowerFactorThreshold.ToString("f2")}{Environment.NewLine}{UsedCapacitorLines.Count} Active Units{Environment.NewLine}Click to Turn Off";

        return $"Threshold = {PowerFactorThreshold.ToString("f2")}{Environment.NewLine}Click to Turn On";
    }
    public override bool Interact()
    {
        TurnedOn = !TurnedOn;
        return true;
    }

    private int _CapacitorsCount;
    private float _LastUpdateTime;

    private void Awake()
    {
        _CapacitorsCount = CapacitorLines.Length;

        OnVoltageChanged += CapacitorBank_OnVoltageChanged;
        Line.OnCurrentChanged += Line_OnCurrentChanged;

        foreach (var capacitorLine in CapacitorLines)
        {
            capacitorLine.StartingVoltage = 0;
            capacitorLine.OnCurrentChanged += CapacitorLine_OnCurrentChanged;
        }
    }
    protected void Update()
    {
        if (_LastUpdateTime + .1f > Time.time)
            return;
        Refresh();
    }
    private void Refresh()
    {
        if (_LastUpdateTime + .05f > Time.time)
            return;
        _LastUpdateTime = Time.time;
        //Debug.Log("[CapacitorBank] Refreshing...");
        var powerFactor = PowerFactor;
        bool isLeading = ReactivePower < 0;
        //Debug.Log($"[CapacitorBank] Power Factor = {powerFactor}");
        if (!isLeading && TurnedOn && powerFactor < PowerFactorThreshold && _CapacitorsCount > UsedCapacitorLines.Count)
        {
            //Debug.Log($"[CapacitorBank] Pushing Capacitor ({CapacitorLines[UsedCapacitorLines.Count]}) into system");
            UsedCapacitorLines.Push(CapacitorLines[UsedCapacitorLines.Count]);
        }
        else if (((isLeading && powerFactor < PowerFactorThreshold) || !TurnedOn) && UsedCapacitorLines.Count > 0)
        {
            //Debug.Log($"[CapacitorBank] Pulling Capacitor ({UsedCapacitorLines.Peek()}) from system");
            UsedCapacitorLines.Peek().StartingVoltage = 0;
            UsedCapacitorLines.Pop();
        }

        foreach (var usedCapacitorLine in UsedCapacitorLines)
        {
            usedCapacitorLine.StartingVoltage = Voltage;
        }
    }

    private void CapacitorLine_OnCurrentChanged(Complex oldValue, Complex newValue)
    {
        Current += (newValue - oldValue);
        //Refresh();
    }
    private void Line_OnCurrentChanged(Complex oldValue, Complex newValue)
    {
        Current += (newValue - oldValue);
        Refresh();
    }

    private void CapacitorBank_OnVoltageChanged()
    {
        Line.StartingVoltage = Voltage;
        foreach (var capacitorLine in UsedCapacitorLines)
            capacitorLine.StartingVoltage = Voltage;
    }
}
