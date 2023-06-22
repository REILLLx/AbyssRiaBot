namespace AbyssRiaCarBot.Models;

public class GetInfo
{
    public int userId { get; set; }
    public int chipsCount { get; set; }
    public string locationCityName { get; set; }
    public bool auctionPossible { get; set; }
    public bool exchangePossible { get; set; }
    public bool realtyExchange { get; set; }
    public string exchangeType { get; set; }
    public int exchangeTypeId { get; set; }
    public string addDate { get; set; }
    public string updateDate { get; set; }
    public string expireDate { get; set; }
    public bool userHideADSStatus { get; set; }
    public UserPhoneData userPhoneData { get; set; }
    public int USD { get; set; }
    public int UAH { get; set; }
    public int EUR { get; set; }
    public bool isAutoAddedByPartner { get; set; }
    public int partnerId { get; set; }
    public LevelData levelData { get; set; }
    public AutoData autoData { get; set; }
    public string markName { get; set; }
    public int markId { get; set; }
    public string modelName { get; set; }
    public int modelId { get; set; }
    public PhotoData photoData { get; set; }
    public string linkToView { get; set; }
    public string title { get; set; }
    public StateData stateData { get; set; }
    public OldTopData oldTop { get; set; }
    public bool canSetSpecificPhoneToAdvert { get; set; }
    public int dontComment { get; set; }
    public int sendComments { get; set; }
    public List<string> badges { get; set; }
    public CheckedVinData checkedVin { get; set; }
    public int isLeasing { get; set; }
    public DealerData dealer { get; set; }
    public bool withInfoBar { get; set; }
    public string infoBarText { get; set; }
    public List<string> optionStyles { get; set; }
}
public class UserPhoneData
{
    public string phoneId { get; set; }
    public string phone { get; set; }
}

public class LevelData
{
    public int level { get; set; }
    public int label { get; set; }
    public string hotType { get; set; }
    public string expireDate { get; set; }
}

public class AutoData
{
    public string description { get; set; }
    public string version { get; set; }
    public bool onModeration { get; set; }
    public int year { get; set; }
    public int autoId { get; set; }
    public int statusId { get; set; }
    public bool withVideo { get; set; }
    public string race { get; set; }
    public int raceInt { get; set; }
    public string fuelName { get; set; }
    public string gearboxName { get; set; }
    public bool isSold { get; set; }
    public string mainCurrency { get; set; }
    public bool fromArchive { get; set; }
    public int categoryId { get; set; }
    public int custom { get; set; }
}

public class PhotoData
{
    public int count { get; set; }
    public string seoLinkM { get; set; }
    public string seoLinkSX { get; set; }
    public string seoLinkB { get; set; }
    public string seoLinkF { get; set; }
}

public class StateData
{
    public string name { get; set; }
    public string regionName { get; set; }
    public string linkToCatalog { get; set; }
    public string title { get; set; }
    public int stateId { get; set; }
}

public class OldTopData
{
    public bool isActive { get; set; }
    public string expireDate { get; set; }
}

public class CheckedVinData
{
    public bool isShow { get; set; }
    public string linkToReport { get; set; }
    public bool hasRestrictions { get; set; }
    public string checkDate { get; set; }
    public bool isChecked { get; set; }
}

public class DealerData
{
    public string link { get; set; }
    public string logo { get; set; }
    public string type { get; set; }
    public int id { get; set; }
    public string name { get; set; }
    public int packageId { get; set; }
    public int typeId { get; set; }
}
