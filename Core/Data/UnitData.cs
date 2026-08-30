using System.Text.Json.Serialization;

namespace KingdomCore.Data
{
    public enum UnitType
    {
        Vagrant,
        Archer,
        Builder,
        Knight
    }

    // POCO (Plain Old CLR Object) pur, indépendant de Godot.
    public class UnitData
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("type")]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public UnitType Type { get; set; }

        [JsonPropertyName("health")]
        public int Health { get; set; }

        [JsonPropertyName("movement_speed")]
        public float MovementSpeed { get; set; }

        [JsonPropertyName("coin_cost")]
        public int CoinCost { get; set; }
    }
}
