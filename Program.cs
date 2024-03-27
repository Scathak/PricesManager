using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging; 
using Microsoft.Extensions.DependencyInjection;
using PricesManager;
using ExternalInterfaces;

internal partial class Program
{
    private static void Main(string[] args)
    {
        var loggingConfiguration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("PriceManagerLog.json", optional: false, reloadOnChange: true)
                .Build();

        IServiceCollection serviceCollection = new ServiceCollection();
        serviceCollection.AddLogging(
            builder => builder
                .AddConfiguration(loggingConfiguration.GetSection("Logging"))
                .AddFilter("Microsoft", LogLevel.Warning)
                .AddFilter("System", LogLevel.Warning)
                .AddFilter("SampleApp.Program", LogLevel.Debug)                
                //.AddJsonConsole()
                .AddConsole()
        );
        var loggerFactory = serviceCollection.BuildServiceProvider().GetService<ILoggerFactory>();
        var logger = loggerFactory.CreateLogger("Main program test");
        int eventInformationId = 0;
        int eventErrorId = 0;
        logger.Log(LogLevel.Information, eventInformationId++, "The dynamic prices manager v1.0");


        var dynamicPrices = new Container();
    
        dynamicPrices.SetDefaultCurrency("USD");
        dynamicPrices.SetDefaultPrice(8.0M);
        dynamicPrices.SetCurrentCountry("USA");
        dynamicPrices.OpenNewDate(DateTimeService.GetCurrentDate());
        var dateToWrite = DateTimeService.GetCurrentDate(); 
        dynamicPrices.AddPriceOnExistingDate(
            dateToWrite, 
            new priceValueWithPeriod() {price = 3.0M, durationMinutes = 60, startTime = "180000" });
        dynamicPrices.AddPriceOnExistingDate(
            dateToWrite, 
            new priceValueWithPeriod() {price = 9.0M, durationMinutes = 30, startTime = "090000" });       
        dynamicPrices.AddPriceOnExistingDate(
            dateToWrite, 
            new priceValueWithPeriod() {price = 55.0M, durationMinutes = 1, startTime = "100000" });     
        
        logger.Log(LogLevel.Information, eventInformationId++, $"The new date {DateTimeService.GetNiceDate(dateToWrite)} is open for adding prices");


        dynamicPrices.OpenNewDate(DateTimeService.GetCurrentDate());   

        var nextDate = DateTime.Now.AddDays(2).ToString("yyyyMMdd");
        dynamicPrices.OpenNewDate(nextDate);
        dynamicPrices.AddPriceOnExistingDate(
            nextDate, 
            new priceValueWithPeriod() {price = 88.0M, durationMinutes = 60, startTime = "123000" });

        var priceReader = new PriceReader(dynamicPrices);
        var dateToPrint = DateTime.Now.AddDays(0).ToString("yyyyMMdd");
        var MinutesPrices = priceReader.GetArrayMinutesPricesOnDate(dateToPrint);

        if(MinutesPrices != null) {
          //  foreach(var priceItem in MinutesPrices) Console.Write(priceItem + " ,");
        } else {
            logger.Log(LogLevel.Error,eventErrorId++, $"There is no prices for this date: {DateTimeService.GetNiceDate(dateToPrint)}");
        }

        var customIntervalPrices = priceReader.GetArrayPricesOnDate(dateToPrint, intervalInMinutes: 30);
        if(MinutesPrices != null) {
            foreach( var priceItem in customIntervalPrices) Console.Write(priceItem + " ,");
        } else{
            logger.Log(LogLevel.Error,eventErrorId++, $"There is no prices for this date: {DateTimeService.GetNiceDate(dateToPrint)}");
        }
        
        var fileName = "testjson.json";
        var FileWriter = new FileWriter(fileName);
        FileWriter.WriteAll(dynamicPrices);
        logger.Log(LogLevel.Information, eventInformationId++, $"Prices were saved in JSON file: {fileName}");

        var FileReader = new FileReader(fileName);
		dynamicPrices = StringService.GetStrings(FileReader);
        logger.Log(LogLevel.Information, eventInformationId++, $"Prices were read from JSON file: {fileName}");

        Console.WriteLine();

        var testDate = dynamicPrices._pricesStorage.Keys.ElementAt(0);
        var testTime = "100100";
        Console.WriteLine("On the date: " + DateTimeService.GetNiceDate(testDate) + 
                        " at a time: " + DateTimeService.getNiceTime(testTime) + 
                        " price was: " + dynamicPrices.GetPriceOnDateTime(testDate, testTime) +
                         dynamicPrices.GetDefaultCurrency());
        Console.WriteLine("Price on current minute: " + priceReader.GetCurrentPrice()+
                         dynamicPrices.GetDefaultCurrency());

        var test = new CacheReader(priceReader);
        var test2 = test.GetJSON(dateToPrint, intervalInMinutes: 30);
    }
}