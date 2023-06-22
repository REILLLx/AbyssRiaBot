using AbyssRiaCarBot.Models;
namespace AbyssRiaCarBot;

public class CarParams
{
    internal List<Model> modelsList { get; set; } = new List<Model>();
    internal GetIds idsList { get; set; } = new GetIds();
    internal string mark { get; set; } = "";
    internal string model{ get; set; } = "";
    internal int markValue{ get; set; } = 0;
    internal int modelValue{ get; set; } = 0;
    internal int startPrice{ get; set; } = 0;
    internal int endPrice{ get; set; } = 0;
    internal int startYear{ get; set; } = 0;
    internal int endYear{ get; set; } = 0;
    internal int i{ get; set; } = 0;
    internal int k{ get; set; } = 0;
    internal string choise{ get; set; } = "";
    internal string exception{ get; set; } = "Вибачте, приїхала помилка";
    internal string SearchRequest{ get; set; } = "";
    internal string stage{ get; set; } = "";
    internal string washStep{ get; set; } = "";
    internal YTSearch youtubeVideo{ get; set; }
    internal int j;
    internal string YTMark{ get; set; }
    internal string YTModel{ get; set; }
    internal int YTYear{ get; set; }
    internal string GMLocation{ get; set; }
    internal GetPlaces carsRepair{ get; set; }
    internal int id{ get; set; }
    internal List<string> favIds { get; set; }
}