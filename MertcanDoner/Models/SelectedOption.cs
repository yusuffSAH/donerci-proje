using MertcanDoner.Models;

public class SelectedOption
{
    public int Id { get; set; }

    public int OrderItemId { get; set; }
    public OrderItem OrderItem { get; set; }

    public string Name { get; set; }
}
