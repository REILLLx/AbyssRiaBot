namespace AbyssRiaCarBot.Models;

public class GetIds
{
    public AdditionalParams additional_params { get; set; }
    public Result result { get; set; }
}
public class AdditionalParams
{
    public int lang_id { get; set; }
    public int page { get; set; }
    public int view_type_id { get; set; }
    public string target { get; set; }
    public string section { get; set; }
    public string catalog_name { get; set; }
    public bool elastica { get; set; }
    public bool nodejs { get; set; }
}
public class Qs
{
    public List<string> fields { get; set; }
    public int size { get; set; }
    public int from { get; set; }
}
public class SearchResult
{
    public List<string> ids { get; set; }
    public int count { get; set; }
    public int last_id { get; set; }
    public Qs qs { get; set; }
}
public class Result
{
    public SearchResult search_result { get; set; }
}
