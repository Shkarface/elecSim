using System;
using System.Numerics;
using UnityEngine.Events;

public abstract class ElectricNode : Interactable
{
    public event UnityAction OnVoltageChanged;
    public event UnityAction<Complex, Complex> OnCurrentChanged;

    private float _Voltage;
    public float Voltage
    {
        get
        {
            return _Voltage;
        }
        set
        {
            if (_Voltage != value)
            {
                _Voltage = value;
                OnVoltageChanged?.Invoke();
            }
        }
    }

    private Complex _Current;
    public Complex Current
    {
        get { return _Current; }
        protected set
        {
            if (_Current != value)
            {
                Complex oldValue = _Current;
                _Current = value;
                OnCurrentChanged?.Invoke(oldValue, _Current);
            }
        }
    }

    public float ApparentPower
    {
        get
        {
            return (float)Math.Sqrt(Math.Pow(ActivePower, 2) + Math.Pow(ReactivePower, 2));
        }
    }
    public float ActivePower
    {
        get
        {
            return Voltage * (float)((Current.Magnitude) * Math.Cos(Current.Phase));
        }
    }
    public float ReactivePower
    {
        get
        {
            return Voltage * (float)((Current.Magnitude) * Math.Sin(Current.Phase));
        }
    }
    public double PowerFactor
    {
        get
        {
            return Math.Min(Math.Cos(Math.Atan(ReactivePower / ActivePower)), 0.99);
        }
    }

    public string VoltageInfo => $"{Voltage.ToString("f2")} V";
    public string CurrentInfo
    {
        get
        {
            double current = Current.Magnitude;
            if (current < 1)
            {
                current *= 1000;
                return $"{current.ToString("f2")} mA";
            }
            return $"{current.ToString("f2")} A";
        }
    }
    public string ActivePowerInfo => $"{((ActivePower > 1000) ? $"{(ActivePower / 1000).ToString("f2")} KW" : $"{ActivePower.ToString("f2")} Watts")}";

    protected void NotifyVoltageChanged() => OnVoltageChanged?.Invoke();
}
