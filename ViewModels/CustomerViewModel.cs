public class CustomerViewModel
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string PhoneNumber { get; set; }

    public decimal DueAmount { get; set; }
    public string DueLabel { get; set; }
    public string DueStatus { get; set; }

    public Color AmountColor { get; set; }
    public Color StatusColor { get; set; }
}
