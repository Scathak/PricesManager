
internal class Program
{
    private static void Main(string[] args)
    {
        Console.WriteLine("The dynamic prices manager v1.0");
        var dynamicPrices = new PricesContainer(defaultPrice: 2.0M, defaultCurrency: "USD", currentCountry: "USA");
    
        dynamicPrices.SetUpDate(DateTimeService.CurrentTimeStamp());
        dynamicPrices.OpenNewDate("20240308");
        dynamicPrices.AddPriceOnExistingDate(
            "20240308", 
            new priceValueWithPeriod() {price = 3.0M, durationMinutes = 60, startTime = "183000" });
        dynamicPrices.AddPriceOnExistingDate(
            "20240308", 
            new priceValueWithPeriod() {price = 9.0M, durationMinutes = 30, startTime = "093000" });       
        dynamicPrices.AddPriceOnExistingDate(
            "20240308", 
            new priceValueWithPeriod() {price = 55.0M, durationMinutes = 1, startTime = "100100" });                    
        var priceReader = new PriceReader(dynamicPrices);
        var Minutesprices = priceReader.GetArrayMinutesPricesOnDate("20240308");
        foreach(var priceItem in Minutesprices){
            Console.Write(priceItem + " ,");
        }
        Console.WriteLine();
        var testTime = "100100";
        var testDate = "20240308";
        Console.WriteLine("On the date: " + DateTimeService.GetNiceDate(testDate) + 
                        " at a time: " + DateTimeService.getNiceTime(testTime) + 
                        " price was: " + dynamicPrices.GetPriceOnDateTime(testDate, testTime) +
                         dynamicPrices.GetDefaultCurrency());
        Console.WriteLine("Price on current minute: " + priceReader.GetCurrentPrice()+
                         dynamicPrices.GetDefaultCurrency());
    }
}