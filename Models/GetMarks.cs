namespace AbyssRiaCarBot.Models;

public class GetMarks
{
    public List<Mark> Marks { get; set; }
}
public class Mark
{
    public string Name { get; set; }
    public int Value { get; set; }
}