using Newtonsoft.Json;

namespace models.recording
{
  public class RecordingEntry {
    [JsonProperty(PropertyName = "createdBy")]
    public string CreatedBy { get; set; }

    [JsonProperty(PropertyName = "givenName")]
    public string GivenName { get; set; }

    [JsonProperty(PropertyName = "surname")]
    public string Surname { get; set; }
    
    [JsonProperty(PropertyName = "id")]
    public string Id { get; set; }

    [JsonProperty(PropertyName = "title")]
    public string Title { get; set; }

    [JsonProperty(PropertyName = "description")]
    public string Description { get; set; }

    [JsonProperty(PropertyName = "recording")]
    public string Recording { get; set; }
  }
}