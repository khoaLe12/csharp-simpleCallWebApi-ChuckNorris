using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ChuckNorrisAPI;
class Fact
{
    public int Id { get; set; }

    [JsonPropertyName("id")]
    [MaxLength(40)]
    public string ChuckNorrisId { get; set; } = string.Empty;

    [JsonPropertyName("url")]
    [MaxLength(1024)]
    public string Url { get; set; } = string.Empty;

    [JsonPropertyName("value")]
    public string Joke { get; set; } = string.Empty;
}
