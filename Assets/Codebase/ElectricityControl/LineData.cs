using System;
using UnityEngine;

[CreateAssetMenu]
public class LineData : ScriptableObject
{
    [Tooltip("the cross-sectional area of the specimen (A in mm^2)")]
    public float CrossSectionalArea = 0.1f;
    [Tooltip("the electrical resistivity (ρ - rho)")]
    public float ElectricalResistivity = 0.1f;
    public int NumberOfLines = 2;
    [Space(20)]
    public float ResistancePerMeter = 0.0f;
    [Tooltip("The price for per meter length in $USD")]
    public float PricePerMeter = 0.6f;

    protected void OnValidate()
    {
        ResistancePerMeter = ElectricalResistivity / (CrossSectionalArea * 10e-6f) * NumberOfLines;
    }
}
