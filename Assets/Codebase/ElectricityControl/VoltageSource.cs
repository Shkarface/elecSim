using System;
using UnityEngine;

public class VoltageSource : ElectricNode
{
    public static double AngularVelocity { get; private set; }
    public static ushort Frequency { get; private set; }

    [Header("Source Information")]
    public ElectricLine Line;
    public int Amplitude;
    public ushort SignalFrequency;
    public bool SimulateAC;
    public bool EnvironmentalEffects;

    private AnimationCurve Curve;
    private float _LastEnvironmentEffectTime;
    private float _LastEnvironmentalVoltage;

    protected void Awake()
    {
        Voltage = Amplitude;
        Frequency = SignalFrequency;
        AngularVelocity = 2 * Math.PI * Frequency;
        var period = 1.0f / Frequency;
        var keyframes = new Keyframe[] {
            new Keyframe(0,Amplitude),
            new Keyframe(0.25f *period,0),
            new Keyframe(0.5f * period,-Amplitude),
            new Keyframe(0.75f * period,0),
            new Keyframe(period,Amplitude)
        };
        Curve = new AnimationCurve(keyframes);
        Curve.postWrapMode = WrapMode.Loop;
        //if (Curve.keys.Length > 1)
        //    Frequency = 1.0f / (Curve.keys[Curve.keys.Length - 1].time - Curve.keys[0].time);
        //else Frequency = 0;
        OnVoltageChanged += VoltageSource_OnVoltageChanged;
    }
    protected void Start()
    {
        NotifyVoltageChanged();
    }
    protected void Update()
    {
        if (SimulateAC)
            Voltage = Curve.Evaluate(Time.time);
        if (EnvironmentalEffects)
        {
            if (_LastEnvironmentEffectTime + 0.5f < Time.time)
            {
                _LastEnvironmentEffectTime = Time.time;
                if (UnityEngine.Random.Range(0, 100) > 50)
                {
                    float voltage = Voltage - _LastEnvironmentalVoltage;
                    _LastEnvironmentalVoltage = UnityEngine.Random.Range(-5f, 0f);
                    voltage += _LastEnvironmentalVoltage;
                    Voltage = voltage;
                }
            }
        }
    }

    private void VoltageSource_OnVoltageChanged()
    {
        Line.StartingVoltage = Voltage;
    }

    public override string GetHelpText() => $"{(!string.IsNullOrEmpty(Tooltip) ? $"{Tooltip}{Environment.NewLine}" : "")}Amplitude = {Amplitude} V {((SignalFrequency > 0) ? $"AC {Environment.NewLine}Frequency = {SignalFrequency} Hz" : "DC")}";
    public override bool Interact() => false;
}
