using System;
using System.Numerics;
using UnityEngine;

public class ElectricMotor : ElectricLoad
{
    private const double R1 = 0.64;
    private readonly Complex X1 = new Complex(0, 1.1);
    private const double R2 = 0.332;
    //private const double RC = 0.1;
    private readonly Complex XM = new Complex(0, 26.3);

    public float RotorFrequency => Slip * VoltageSource.Frequency;
    public float Slip => ((_SynchronousSpeed - CurrentSpeed) / _SynchronousSpeed);
    public Complex MotorImpedance
    {
        get
        {
            Complex z2 = R2 / Slip + new Complex(0, 15 * Slip);
            Complex zCore = XM;//(RC * XM) / (RC + XM);
            Complex zParallel = (z2 * zCore) / (z2 + zCore);
            return (zParallel + R1 + X1);
        }
    }

    [Header("Motor")]
    public int Poles = 4;
    [Tooltip("The output of the motor, in horsepower\n1 hp = 745.7 watts")]
    public float HorsePower = 0.5f;
    [Range(0.4f, 0.98f)]
    public float SpeedPercent = 0.95f;
    public float StartTime = 1f;

    private float _Lerp;
    private float _CurrentSpeed;
    private int _SynchronousSpeed;
    private int _SlipSpeed;


    public float CurrentSpeed
    {
        get
        {
            return _CurrentSpeed;
        }
        set
        {
            if (_CurrentSpeed != value)
            {
                _CurrentSpeed = value;
                Refresh();
            }
        }
    }

    protected override void Awake()
    {
        base.Awake();
        _SynchronousSpeed = (int)((60.0 * VoltageSource.AngularVelocity) / (Math.PI * Poles));
        _SlipSpeed = (int)(_SynchronousSpeed * SpeedPercent);

    }
    protected override void VoltageChanged()
    {
        _Lerp = CurrentSpeed / _SlipSpeed;
        Refresh();
    }

    protected virtual void Refresh()
    {
        Current = CalculateCurrent(MotorImpedance, HorsePower * 745.6998715822702f);
    }

    protected void Update()
    {
        if (Voltage != 0)
            _Lerp = Mathf.Clamp(_Lerp + (Time.deltaTime / StartTime), 0, 1);
        else
            _Lerp = Mathf.Clamp(_Lerp - (Time.deltaTime / (StartTime * 1.3f)), 0, 1);


        CurrentSpeed = Mathf.Lerp(0, _SlipSpeed, _Lerp);
    }
}
