using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager Instance;

    public TextMeshProUGUI UIText;
    private ElectricLine[] _Lines;
    private ElectricLoad[] _Loads;

    protected void Awake()
    {
        Instance = this;
        _Lines = FindObjectsOfType<ElectricLine>();
        _Loads = FindObjectsOfType<ElectricLoad>();
    }

    protected void Start()
    {
        StringBuilder info = new StringBuilder();
        info.AppendLine($"Wires:");
        Dictionary<string, List<ElectricLine>> _WireGrouping = new Dictionary<string, List<ElectricLine>>();
        float totalLength = 0;
        float totalPrice = 0;
        foreach (var line in _Lines)
        {
            totalLength += line.LocalLength;
            totalPrice += line.LocalLength * line.WireType.PricePerMeter;
            string key = $"{line.WireType.NumberOfLines}x{line.WireType.CrossSectionalArea}";
            if (_WireGrouping.ContainsKey(key))
                _WireGrouping[key].Add(line);
            else
                _WireGrouping.Add(key, new List<ElectricLine>() { line });
        }
        foreach (var key in _WireGrouping.Keys)
        {
            float length = 0, price = 0;

            foreach (var line in _WireGrouping[key])
            {
                length += line.LocalLength;
                price += line.LocalLength * line.WireType.PricePerMeter;
            }

            info.AppendLine($"\tWire ({key}mm<sup>2</sup>):");
            info.AppendLine($"\t\tLength:\t{length.ToString("f2")} m");
            info.AppendLine($"\t\tCost:\t\t{(price * 1250).ToString("n0")} IQD (${price.ToString("f2")})");
        }
        info.AppendLine($"\t------------------------------------------------");
        info.AppendLine($"\tTotal Length:\t{totalLength.ToString("f2")} m");
        info.AppendLine($"\tTotal Cost:\t\t{(totalPrice * 1250).ToString("n0")} IQD (${totalPrice.ToString("f2")})");

        UIText.text = info.ToString();
    }
}
