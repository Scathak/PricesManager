#pragma warning disable CS8618
#pragma warning disable CS8603

namespace PricesManager
{
    using System;
    using System.Collections.Generic;
    using System.Text.Json;
    using System.Text.Json.Serialization;
    using System.Globalization;

    public partial class Container
    {
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("defaultPrice")]
        public decimal? _defaultPrice { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("defaultCurrency")]
        public string _defaultCurrency { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("currentCountry")]
        public string _currentCountry { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("lastUpdateDateAndTime")]
        public string _lastUpdateDateAndTime { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("pricesStorage")]
        public Dictionary<string, List<priceValueWithPeriod>> _pricesStorage { get; set; }
    }

    public partial class priceValueWithPeriod
    {
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("price")]
        public decimal? price { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("startTime")]
        public string startTime { get; set; }

		[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("durationMinutes")]
        public decimal? durationMinutes { get; set; }
    }

    public partial class Container
    {
        public static Container FromJson(string json) => JsonSerializer.Deserialize<Container>(json, PricesManager.Converter.Settings);
    }
	public partial class Container
    {
		public Container(){

			_currentCountry = "";
			_defaultCurrency = "";
			_defaultPrice = 0;
			_lastUpdateDateAndTime = "";
			_pricesStorage ??= new Dictionary<string, List<priceValueWithPeriod>>();  
		}
		public bool AddPriceOnExistingDate(string date, priceValueWithPeriod priceWithPeriod){
			if (!_pricesStorage.ContainsKey(date)) return false;

			_pricesStorage[date].Add(priceWithPeriod);
			return true;
		}
		public bool OpenNewDate(string date){
			if (_pricesStorage.ContainsKey(date)) return false;
			
			_pricesStorage.Add(date, new List<priceValueWithPeriod>());
			return true;
		}
		public decimal? GetPriceOnDateTime(string date, string time){
			if (!_pricesStorage.ContainsKey(date)) return decimal.MaxValue;

			var munuteNumber = DateTimeService.ConvertTimeInMinutes(time);
			foreach(var priceItem in _pricesStorage[date].AsEnumerable()){
				var minuteStartTime = DateTimeService.ConvertTimeInMinutes(priceItem.startTime);
				if (minuteStartTime <= munuteNumber &&
					munuteNumber < minuteStartTime + priceItem.durationMinutes )
				{
					return priceItem.price;
				}
			}
			return _defaultPrice;
		}
		public IEnumerable<priceValueWithPeriod> GetEnumeratorPricesOnDate(string date){
			if(!_pricesStorage.ContainsKey(date)) return default;
			
			return _pricesStorage[date].AsEnumerable();
		}
		public decimal? GetDefaultPrice(){
			return _defaultPrice;
		}
		public void SetDefaultPrice(decimal value){
			_defaultPrice = value;
		}	
		public string GetDefaultCurrency(){
			return _defaultCurrency;
		}
		public void SetDefaultCurrency(string value){
			_defaultCurrency = value;
		}
		public string GetCurrentCountry(){
			return _currentCountry;
		}
		public void SetCurrentCountry(string value){
			_currentCountry = value;
		}
		public string GetLastUpdateDateAndTime(){
			return _lastUpdateDateAndTime;
		}
		public void SetUpDate(string date){
			_lastUpdateDateAndTime = date;
		}
    }

    public static class Serialize
    {
        public static string ToJson(this Container self) => JsonSerializer.Serialize(self, PricesManager.Converter.Settings);
    }

    internal static class Converter
    {
        public static readonly JsonSerializerOptions Settings = new(JsonSerializerDefaults.General)
        {
            Converters =
            {
                new DateOnlyConverter(),
                new TimeOnlyConverter(),
                IsoDateTimeOffsetConverter.Singleton
            },
        };
    }
    
    public class DateOnlyConverter : JsonConverter<DateOnly>
    {
        private readonly string serializationFormat;
        public DateOnlyConverter() : this(null) { }

        public DateOnlyConverter(string? serializationFormat)
        {
            this.serializationFormat = serializationFormat ?? "yyyy-MM-dd";
        }

        public override DateOnly Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var value = reader.GetString();
            return DateOnly.Parse(value!);
        }

        public override void Write(Utf8JsonWriter writer, DateOnly value, JsonSerializerOptions options)
            => writer.WriteStringValue(value.ToString(serializationFormat));
    }

    public class TimeOnlyConverter : JsonConverter<TimeOnly>
    {
        private readonly string serializationFormat;

        public TimeOnlyConverter() : this(null) { }

        public TimeOnlyConverter(string? serializationFormat)
        {
            this.serializationFormat = serializationFormat ?? "HH:mm:ss.fff";
        }

        public override TimeOnly Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var value = reader.GetString();
            return TimeOnly.Parse(value!);
        }

        public override void Write(Utf8JsonWriter writer, TimeOnly value, JsonSerializerOptions options)
            => writer.WriteStringValue(value.ToString(serializationFormat));
    }

    internal class IsoDateTimeOffsetConverter : JsonConverter<DateTimeOffset>
    {
        public override bool CanConvert(Type t) => t == typeof(DateTimeOffset);

        private const string DefaultDateTimeFormat = "yyyy'-'MM'-'dd'T'HH':'mm':'ss.FFFFFFFK";

        private DateTimeStyles _dateTimeStyles = DateTimeStyles.RoundtripKind;
        private string? _dateTimeFormat;
        private CultureInfo? _culture;

        public DateTimeStyles DateTimeStyles
        {
            get => _dateTimeStyles;
            set => _dateTimeStyles = value;
        }

        public string? DateTimeFormat
        {
            get => _dateTimeFormat ?? string.Empty;
            set => _dateTimeFormat = (string.IsNullOrEmpty(value)) ? null : value;
        }

        public CultureInfo Culture
        {
            get => _culture ?? CultureInfo.CurrentCulture;
            set => _culture = value;
        }

        public override void Write(Utf8JsonWriter writer, DateTimeOffset value, JsonSerializerOptions options)
        {
            string text;


            if ((_dateTimeStyles & DateTimeStyles.AdjustToUniversal) == DateTimeStyles.AdjustToUniversal
                || (_dateTimeStyles & DateTimeStyles.AssumeUniversal) == DateTimeStyles.AssumeUniversal)
            {
                value = value.ToUniversalTime();
            }

            text = value.ToString(_dateTimeFormat ?? DefaultDateTimeFormat, Culture);

            writer.WriteStringValue(text);
        }

        public override DateTimeOffset Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            string? dateText = reader.GetString();

            if (string.IsNullOrEmpty(dateText) == false)
            {
                if (!string.IsNullOrEmpty(_dateTimeFormat))
                {
                    return DateTimeOffset.ParseExact(dateText, _dateTimeFormat, Culture, _dateTimeStyles);
                }
                else
                {
                    return DateTimeOffset.Parse(dateText, Culture, _dateTimeStyles);
                }
            }
            else
            {
                return default;
            }
        }
        public static readonly IsoDateTimeOffsetConverter Singleton = new IsoDateTimeOffsetConverter();
    }

public interface IGetPrice
{
	public decimal GetPriceOnTimeStamp(string date, string time);
	public decimal GetCurrentPrice();
	public decimal[] GetArrayMinutesPricesOnDate(string date);
}

public class PriceReader : IGetPrice
{
	private Container _pricesContainer;
	public PriceReader(Container pricesContainer){
		_pricesContainer = pricesContainer;
	}
	public decimal GetPriceOnTimeStamp(string date, string time){
		return (decimal)_pricesContainer.GetPriceOnDateTime( date, time );
	}
	public decimal GetCurrentPrice(){
		return (decimal)_pricesContainer.GetPriceOnDateTime( DateTimeService.GetCurrentDate(), DateTimeService.GetCurrentTime() );
	}
	public decimal[] GetArrayMinutesPricesOnDate(string date){
		var pricesCollection = _pricesContainer.GetEnumeratorPricesOnDate(date);
		if (pricesCollection == null) return default;

		var interval = new TimeSpan(days: 1, hours: 0, minutes: 0, seconds: 0, milliseconds: 0);
		var output = new decimal[(int)interval.TotalMinutes];

		foreach(var pricesSet in pricesCollection){
			var startIndex = DateTimeService.ConvertTimeInMinutes(pricesSet.startTime);
			for(var j = 0; j < pricesSet.durationMinutes; j++){
				if(pricesSet.price == null) { output[j + startIndex] = 0;}
				else
					output[j + startIndex] = (decimal)pricesSet.price;
			}
		}

		for(var i = 0; i < output.Length; i++){
			if(output[i] == 0){
				output[i] = (decimal)_pricesContainer.GetDefaultPrice();
			}
		}
		return output;
	}
} 
}
#pragma warning restore CS8618
#pragma warning restore CS8603