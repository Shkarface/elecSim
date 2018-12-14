using System;
using System.IO;
using System.Numerics;
using System.Text;
using TMPro;
using UnityEngine;

public class Multimeter : Interactable
{
    public TextMeshProUGUI UI;
    public ElectricNode Terminal;

    private float _RefreshTimer,_KWMeterTimer, _KWH;
    private int _LineIndex = 0;

    public override string GetHelpText() => string.Empty;
    public override bool Interact() => false;

    protected void Awake()
    {
        File.WriteAllText("report.csv", $"Index,Time,ApparentPower,ActivePower,ReactivePower,Current,Voltage,PowerFactor{Environment.NewLine}");
    }

    protected void Update()
    {
        if (_KWMeterTimer + 1f < Time.time)
        {
            _KWMeterTimer = Time.time;
            _KWH += (Terminal.ActivePower / 1000) / 60f;
        }
        if (_RefreshTimer + 0.25f < Time.time)
        {
            _RefreshTimer = Time.time;
            Refresh();

        }
    }
    private void Refresh()
    {
        if (Terminal.Voltage != 0)
        {
            StringBuilder info = new StringBuilder();
            float reactivePower = Terminal.ReactivePower;

            info.AppendLine(Terminal.VoltageInfo);
            info.AppendLine($"{VoltageSource.AngularVelocity / (2.0 * Math.PI)} Hz");

            info.AppendLine(Terminal.CurrentInfo);

            info.AppendLine($"{(Terminal.ApparentPower / 1000).ToString("f2")} kVA");
            info.AppendLine(Terminal.ActivePowerInfo);
            info.AppendLine($"{((reactivePower > 1000) ? $"{(reactivePower / 1000).ToString("f2")} kVAR" : $"{reactivePower.ToString("f2")} VAR")}");
            info.AppendLine($"{Terminal.PowerFactor.ToString("f2")} <sub>{((reactivePower > 0) ? "Lagging" : "Leading")}</sub>");
            info.AppendLine($"{_KWH.ToString("f2")} <size=80%>KW/min");
            info.AppendLine($"{(_KWH * 15).ToString("n0")} IQD");

            UI.text = info.ToString();
            UI.transform.parent.gameObject.SetActive(true);
            File.AppendAllText("report.csv", $"{++_LineIndex},{DateTime.Now},{Terminal.ApparentPower},{Terminal.ActivePower},{reactivePower},{Terminal.Current.Magnitude},{Terminal.Voltage},{Terminal.PowerFactor}{Environment.NewLine}");
        }
        else
            UI.transform.parent.gameObject.SetActive(false);
    }
}
