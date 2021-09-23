using Newtonsoft.Json;

namespace WorldManagementApi.Messaging.Configurations
{
  [JsonObject("rabbit_mq_exchanges")]
  public class RabbitMQExchanges
  {
    [JsonProperty("peopleExchange")]
    public string PeopleExchange { get; set; }
    [JsonProperty("karmaExchange")]
    public string KarmaExchange { get; set; }
    }
}
