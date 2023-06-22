using System.Text;
using AbyssRiaCarBot.Models;
using Newtonsoft.Json;

namespace AbyssRiaCarBot.Controllers;

public class GeneralClient
{
    private HttpClient _httpClient;
    private string _myApiAdress;
    public GeneralClient()
    {
        _httpClient = new HttpClient();
        _myApiAdress = Constants.MyApiAdress;
        _httpClient.BaseAddress = new Uri(_myApiAdress);
    }
    public async Task<GetInfo> GetInfo(string id)
    {
        var responce = await _httpClient.GetAsync($"/GetInfo?id={id}");
        responce.EnsureSuccessStatusCode();
        var content = await responce.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<GetInfo>(content);
        return result;
    }
    public async Task<List<Mark>> GetMarks()
    {
        var responce = await _httpClient.GetAsync($"/GetMarks");
        var content = await responce.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<List<Mark>>(content);
        return result;
    }
    public async Task<List<Model>> GetModels(int MarkId)
    {
        var responce5 = await _httpClient.GetAsync($"/GetModels?markId={MarkId}");
        responce5.EnsureSuccessStatusCode();
        var content5 = await responce5.Content.ReadAsStringAsync();
        var result5 = JsonConvert.DeserializeObject<List<Model>>(content5);
        return result5;
    }
    public async Task<GetIds> GetIds()
    {
        var response = await _httpClient.GetAsync($"/GetIds");
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<GetIds>(content);
        return result;
    }
    public async Task<GetIds> GetIds(int markaId, int modelId, int price_ot, int price_do, int s_years, int po_years)
    {
        var response = await _httpClient.GetAsync($"/GetIds?markaId={markaId}&modelId={modelId}&price_ot={price_ot}&price_do={price_do}&s_years={s_years}&po_years={po_years}");
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<GetIds>(content);
        return result;
    }
    public async Task<GetIds> GetIdsModel(int modelId)
    {
        var response = await _httpClient.GetAsync($"/GetIds?modelId={modelId}");
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<GetIds>(content);
        return result;
    }
    public async Task<GetIds> GetIdsMark(int markId)
    {
        var response = await _httpClient.GetAsync($"/GetIds?markaId={markId}");
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<GetIds>(content);
        return result;
    }
    public async Task<GetIds> GetIdsMarkNModel(int markId, int modelId)
    {
        var response = await _httpClient.GetAsync($"/GetIds?markaId={markId}&modelId={modelId}");
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<GetIds>(content);
        return result;
    }
    public async Task<YTSearch> GetVideo(string car)
    {
        var responce = await _httpClient.GetAsync($"/YTSearch?car={car}");
        responce.EnsureSuccessStatusCode();
        var content = await responce.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<YTSearch>(content);
        return result;
    }
    public async Task<GetPlaces> GetPlaces(string area)
    {
        var responce5 = await _httpClient.GetAsync($"/GetPlaces?area={area}");
        responce5.EnsureSuccessStatusCode();
        var content5 = await responce5.Content.ReadAsStringAsync();
        var result5 = JsonConvert.DeserializeObject<GetPlaces>(content5);
        return result5;
    }
    public async Task AddId(string id, long userId)
    {
        HttpClient client = new HttpClient();
        await client.PostAsync($"{_myApiAdress}/AddId?Id={id}&UserId={userId}", null);
    }
    public async Task<List<string>> GetDBIds (long userId)
    {
        var responce = await _httpClient.GetAsync($"/GetDBIds?UserId={userId}");
        responce.EnsureSuccessStatusCode();
        var content = await responce.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<List<string>>(content);
        return result;
    }
    public async Task DeleteIdBD(string id, long userId)
    {
        HttpClient client = new HttpClient();
        await _httpClient.DeleteAsync($"{_myApiAdress}/DeleteIdBD?id={id}&userId={userId}");
    }
}