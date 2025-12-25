namespace vyaparsathi.Models;

public class KpiItem
{
    public string Title { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public Color ValueColor { get; set; }
    public string Icon { get; set; } = string.Empty;
}