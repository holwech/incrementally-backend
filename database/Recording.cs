using Newtonsoft.Json;

namespace models.recording
{
  public class Recording {
    [JsonProperty(PropertyName = "id")]
    public string Id { get; set; }

    [JsonProperty(PropertyName = "recording")]
    public string Data { get; set; }
  }
}