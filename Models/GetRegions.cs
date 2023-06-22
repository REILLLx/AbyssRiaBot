namespace AbyssRiaCarBot.Models;

public class GetRegions
{
    public List<Region> regionsList { get; set; }
}
public class Region
{
    public string name { get; set; }
    public int value { get; set; }
}
