namespace AbyssRiaCarBot.Models;

public class GetModels
{
    public List<Model> modelsList { get; set; }
}
public class Model
{
    public string name { get; set; }
    public int value { get; set; }
}