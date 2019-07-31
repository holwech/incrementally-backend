using Newtonsoft.Json;

namespace models.recording
{
  public class Recording {
    [JsonProperty(PropertyName = "CreatedBy")]
    public string CreatedBy { get; set; }

    [JsonProperty(PropertyName = "givenName")]
    public string GivenName { get; set; }

    [JsonProperty(PropertyName = "surname")]
    public string Surname { get; set; }
    
    [JsonProperty(PropertyName = "id")]
    public string Id { get; set; }

    [JsonProperty(PropertyName = "recording")]
    public string Data { get; set; }
  }
}