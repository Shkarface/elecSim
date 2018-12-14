using System;
using System.Numerics;
using System.Threading.Tasks;
using UnityEngine;

public class ElectricSwitch : ElectricNode
{
    [Header("Switch")]
    public SwitchData Data;
    public ElectricLine Line;
    public bool ClosedOnStart;

    private bool _IsClosed;
    public bool IsClosed
    {
        get
        {
            return _IsClosed;
        }
        set
        {
            if (_IsClosed != value)
            {
                _IsClosed = value;
                NotifyVoltageChanged();
            }
        }
    }

    protected void Awake()
    {
        OnVoltageChanged += VoltageChanged;
        if (Line != null)
            Line.OnCurrentChanged += Line_OnCurrentChanged;
    }
    protected void Start()
    {
        if (ClosedOnStart)
            IsClosed = true;
        else NotifyVoltageChanged();
    }

    private void VoltageChanged()
    {
        if (IsClosed)
            Line.StartingVoltage = Voltage;
        else
            Line.StartingVoltage = 0;
    }

    public override bool Interact()
    {
        IsClosed = !IsClosed;
        return true;
    }
    public override string GetHelpText() => $"{(!string.IsNullOrEmpty(Tooltip) ? $"{Tooltip}{Environment.NewLine}" : "")}{VoltageInfo} × {CurrentInfo}{((Current.Phase != 0.0) ? $" × {Math.Cos(Current.Phase).ToString("f2")}<sub>Pf</sub>" : "")} = {((ActivePower > 1000) ? $"{(ActivePower / 1000).ToString("f2")} KW" : $"{ActivePower.ToString("f2")} Watts")}{Environment.NewLine}Click to turn {(IsClosed ? "off" : "on")}";

    private async void Line_OnCurrentChanged(Complex oldValue, Complex newValue)
    {
        Current += (newValue - oldValue);
        if (Data != null && Data.OverloadProtection && Current.Magnitude > Data.MaxCurrent)
        {
            await Task.Delay((int)(Data.MaxCurrent * 50));
            if (Current.Magnitude > Data.MaxCurrent)
                IsClosed = false;
        }
    }

}
