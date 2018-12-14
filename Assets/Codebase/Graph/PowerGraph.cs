using UnityEngine;

public enum PowerType
{
    Apparent,
    Active,
    Reactive
}

public class PowerGraph : BaseGraph
{
    [Header("Power")]
    public ElectricNode Node;

    float[] _PowerValues = new float[3];
    protected override void NewValueRequest()
    {
        _PowerValues[0] = Node.ApparentPower;
        _PowerValues[1] = Node.ActivePower;
        _PowerValues[2] = Node.ReactivePower;
        NewPoint(_PowerValues);
    }
}
