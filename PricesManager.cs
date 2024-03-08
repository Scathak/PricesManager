public class priceValueWithPeriod
{
    public decimal price {get; set;}
    public int durationMinutes {get; set;}
    public string startTime {get; set;}
}
public class PricesContainer
{
	private decimal _defaultPrice;
	private string _defaultCurrency;
	private string _currentCountry;
	private string _lastUpdateDateAndTime;
	private Dictionary <string, List<priceValueWithPeriod>> _pricesStorage;

public PricesContainer (decimal defaultPrice,
						string defaultCurrency,
						string currentCountry
						)
{
	_defaultPrice = defaultPrice;
	_defaultCurrency = defaultCurrency;
	_currentCountry = currentCountry;
	_pricesStorage ??= new Dictionary <string, List<priceValueWithPeriod>> ();
}
	public void SetUpDate(string date){
		_lastUpdateDateAndTime = date;
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
	public decimal GetPriceOnDateTime(string date, string time){
		if (!_pricesStorage.ContainsKey(date)) return 0.0M;
		var munuteNumber = DateTimeService.ConvertTimeInMinutes(time);
		foreach(var priceitem in _pricesStorage[date].AsEnumerable()){
			var minuteStartTime = DateTimeService.ConvertTimeInMinutes(priceitem.startTime);
			if (minuteStartTime <= munuteNumber &&
				munuteNumber < minuteStartTime + priceitem.durationMinutes )
			{
				return priceitem.price;
			}
		}
		return _defaultPrice;
	}
	public IEnumerable<priceValueWithPeriod> GetEnumeratorPricesOnDate(string date){
		return _pricesStorage[date].AsEnumerable();
	}
	public decimal GetDefaultPrice(){
		return _defaultPrice;
	}	
	public string GetDefaultCurrency(){
		return _defaultCurrency;
	}
	public string GetCurrentCountry(){
		return _currentCountry;
	}
	public string GetLastUpdateDateAndTime(){
		return _lastUpdateDateAndTime;
	}
}
public interface IGetPrice
{
	public decimal GetPriceOnTimeStamp(string date, string time);
	public decimal GetCurrentPrice();
	public decimal[] GetArrayMinutesPricesOnDate(string date);
}

public class PriceReader : IGetPrice
{
	private PricesContainer _pricesContainer;
	public PriceReader(PricesContainer pricesContainer){
		_pricesContainer = pricesContainer;
	}
	public decimal GetPriceOnTimeStamp(string date, string time){
		return _pricesContainer.GetPriceOnDateTime( date, time );
	}
	public decimal GetCurrentPrice(){
		return _pricesContainer.GetPriceOnDateTime( DateTimeService.GetCurrentDate(), DateTimeService.GetCurrentTime() );
	}
	public decimal[] GetArrayMinutesPricesOnDate(string date){
		var output = new decimal[24*60];
	
		foreach(var pricesSet in _pricesContainer.GetEnumeratorPricesOnDate(date)){
			var startIndex = DateTimeService.ConvertTimeInMinutes(pricesSet.startTime);
			for(var j = 0; j < pricesSet.durationMinutes; j++){
				output[j + startIndex] = pricesSet.price;
			}
		}
		for(var i = 0; i < output.Length; i++){
			if(output[i] == 0){
				output[i] = _pricesContainer.GetDefaultPrice();
			}
		}
		return output;
	}
} 