using System.Text.Json;
using PricesManager;

public interface IReader
{
	public Container? ReadAll();
}
public interface IWriter
{
	public void WriteAll(Container container);
}
public class FileWriter : IWriter
{
	private readonly string _fileName;
	public FileWriter(string fileName){
		_fileName = fileName;
	}
	public void WriteAll(Container container)
	{
		var serializeOptions = new JsonSerializerOptions
		{
			PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
			WriteIndented = true,
			IncludeFields = true
		};
		container.SetUpDate(DateTimeService.CurrentTimeStamp());
		string jsonString = JsonSerializer.Serialize(container, serializeOptions);
		var pathtoJson = Path.Combine(
			Path.Combine(
			Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName
			, "DataStorage"
			,_fileName)
		);
		File.WriteAllText(pathtoJson, jsonString);
	}
}
public class FileReader : IReader
{
	private readonly string _fileName;
	public FileReader(string fileName){
		_fileName = fileName;
	}
	public Container? ReadAll()
	{
		try
		{
			using var r = new StreamReader(
				Path.Combine(
					Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName
					, "DataStorage"
					,_fileName)
				);
			var json = r.ReadToEnd();

			Container? pricesContainer = 
                JsonSerializer.Deserialize<Container>(json);
			return pricesContainer;
		}
		catch (FileNotFoundException e)
		{
			Console.WriteLine($"The json file was not found: '{e}'");
		}
		catch (DirectoryNotFoundException e)
		{
			Console.WriteLine($"The directory was not found: '{e}'");
		}
		catch (IOException e)
		{
			Console.WriteLine($"The json file could not be opened: '{e}'");
		}
		return null;
	}
}
public class StringService
{
	public static Container? GetStrings(IReader reader)
	{
		return reader.ReadAll();
	}
}

