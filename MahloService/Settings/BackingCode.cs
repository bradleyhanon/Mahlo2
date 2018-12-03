using Newtonsoft.Json;

namespace MahloService.Settings
{
  public class BackingCode
  {
    public string Code { get; set; }
    public string Backing { get; set; }

    public static BackingCode FromString(string s)
    {
      return JsonConvert.DeserializeObject<BackingCode>(s);
    }

    public override string ToString()
    {
      return JsonConvert.SerializeObject(this).Replace('"', '\'');
    }
  }
}