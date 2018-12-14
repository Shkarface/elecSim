using System.Numerics;
using UnityEngine;

public class Capacitor : ElectricLoad
{
    public string CapacitanceInfo
    {
        get
        {
            double capacitance = Capacitance;
            if (capacitance < 1)
            {
                capacitance *= 1000;
                if (capacitance < 1)
                {
                    capacitance *= 1000;
                    return $"{capacitance.ToString("f2")} µFarad";
                }
                else
                    return $"{capacitance.ToString("f2")} mFarad";
            }
            return $"{capacitance.ToString("f2")} Farad";
        }
    }

    [Tooltip("The capacitance of the capacitor, in Farads")]
    public float Capacitance = 1;
    protected override void VoltageChanged()
    {
        //var impedance = new Complex(LoadData.Real, LoadData.Imaginary);
        //double magnitude, phase;
        //if (Voltage > 0)
        //{
        //    magnitude = LoadData.RequiredPower / (Voltage * Math.Cos(impedance.Phase));
        //    phase = impedance.Phase;
        //}
        //else magnitude = phase = 0;
        Current = Voltage / new Complex(0, VoltageSource.AngularVelocity * Capacitance); ;
    }

    public override string GetHelpText() => $"Capacitance = {CapacitanceInfo}";


}
