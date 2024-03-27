using PricesManager;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ExternalInterfaces
{
    public interface DataProvider
    {
        public string GetJSON(string date, int intervalInMinutes);
    }
    public partial class DataSet
    {
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("x")]
        public long? X { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("y")]
        public double? Y { get; set; }
    }
    public static class Serialize
    {
        public static string ToJson(this DataSet[] self) => JsonSerializer.Serialize(self);
    }
    public class CacheReader : DataProvider
    {

        PriceReader _priceReader;
        public CacheReader(PriceReader reader){
            _priceReader ??= reader;
        }
        public string GetJSON(string date, int intervalInMinutes){
            var pricesArray = _priceReader.GetArrayPricesOnDate(date, intervalInMinutes);
            var dataset = new List<DataSet>();
            var i = 0;
            foreach(var item in pricesArray)
            {
                var element = new DataSet{X=(long)item, Y=i};
                dataset.Add(element);
                i++;
            }
            return  Serialize.ToJson(dataset.ToArray());
        }
    }
}