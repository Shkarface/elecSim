using System.Numerics;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Assertions;

public class Refrigerator : ElectricLoad
{
    protected Complex Impedance = new Complex(10, 25);

    [Header("Electric Properties")]
    [SerializeField] ElectricMotor Motor;
    [SerializeField] float RequiredWattage = 10;
    [Header("Temperature Properties")]
    [Range(-30, 30)]
    [SerializeField] float Temperature = 25;
    [SerializeField] float FreezingTemperature = 0;
    [SerializeField] float CooldownTemperature = 5f;
    [SerializeField] float FreezingRate = 0.1f;
    [SerializeField] float DefreezingRate = 0.05f;


    public bool IsMotorRunning
    {
        get
        {
            return Mathf.Abs(Motor.Voltage) > 0;
        }
        private set
        {
            Motor.Voltage = (value) ? Voltage : 0f;
        }
    }
    public bool ShouldMotorRun => ((IsMotorRunning && Temperature >= FreezingTemperature) || (!IsMotorRunning && Temperature >= FreezingTemperature + CooldownTemperature));

    private Complex _OldSelfCurrent = new Complex(0, 0);


    public override string GetHelpText() => string.Empty;
    public override bool Interact() => false;

    protected override void Awake()
    {
        base.Awake();
        Assert.IsNotNull(Motor);

        Motor.OnCurrentChanged += Motor_OnCurrentChanged;
        UpdateAsync();
    }
    protected async void UpdateAsync()
    {
        while (true)
        {
            await Task.Delay(1 * 1000);
            if (IsMotorRunning)
                Temperature -= FreezingRate;
            else
                Temperature += DefreezingRate;

            Temperature = Mathf.Clamp(Temperature, -30f, 30f);
            IsMotorRunning = ShouldMotorRun;
        }
    }
    protected override void VoltageChanged()
    {
        IsMotorRunning = ShouldMotorRun;
        if (Mathf.Abs(Voltage) > 0)
        {
            Current -= _OldSelfCurrent;
            _OldSelfCurrent = CalculateCurrent(Impedance, RequiredWattage);
            Current += _OldSelfCurrent;
        }
        else
            Current = 0;
    }
    private void Motor_OnCurrentChanged(Complex oldValue, Complex newValue)
    {
        Current += (newValue - oldValue);
    }
}
