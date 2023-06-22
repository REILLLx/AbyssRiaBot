namespace AbyssRiaCarBot.Models;

public class GetPlaces
{
    public List<object> HtmlAttributions { get; set; }
    public List<Place> Results { get; set; }
    public string Status { get; set; }
}
public class Place
{
    public string BusinessStatus { get; set; }
    public string FormattedAddress { get; set; }
    public Geometry Geometry { get; set; }
    public string Icon { get; set; }
    public string IconBackgroundColor { get; set; }
    public string IconMaskBaseUri { get; set; }
    public string Name { get; set; }
    public OpeningHours OpeningHours { get; set; }
    public List<Photo> Photos { get; set; }
    public string PlaceId { get; set; }
    public PlusCode PlusCode { get; set; }
    public double Rating { get; set; }
    public string Reference { get; set; }
    public List<string> Types { get; set; }
    public int UserRatingsTotal { get; set; }
}
public class Geometry
{
    public Location Location { get; set; }
    public Viewport Viewport { get; set; }
}

public class Location
{
    public double Lat { get; set; }
    public double Lng { get; set; }
}

public class Viewport
{
    public Location Northeast { get; set; }
    public Location Southwest { get; set; }
}

public class OpeningHours
{
    public bool OpenNow { get; set; }
}

public class Photo
{
    public int Height { get; set; }
    public List<string> HtmlAttributions { get; set; }
    public string PhotoReference { get; set; }
    public int Width { get; set; }
}

public class PlusCode
{
    public string CompoundCode { get; set; }
    public string GlobalCode { get; set; }
}
