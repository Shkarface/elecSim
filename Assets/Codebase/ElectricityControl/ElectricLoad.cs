using System;
using System.Numerics;

public abstract class ElectricLoad : ElectricNode
{
    protected abstract void VoltageChanged();

    protected virtual void Awake() => OnVoltageChanged += VoltageChanged;

    protected virtual Complex CalculateCurrent(Complex impedance, float requiredWattage)
    {
        double magnitude, phase;
        if (Math.Abs(Voltage) > 0)
        {
            magnitude = requiredWattage / (Voltage * Math.Cos(impedance.Phase));
            phase = impedance.Phase;
        }
        else magnitude = phase = 0;

        return new Complex(magnitude * Math.Cos(impedance.Phase), magnitude * Math.Sin(impedance.Phase));
    }

    public override bool Interact() => false;
    public override string GetHelpText() => $"Voltage = {Voltage.ToString("f3")} V{Environment.NewLine}Current = {Current.ToString("f3")} A{Environment.NewLine}Wattage = {ActivePower.ToString("f2")} Watts";
}
