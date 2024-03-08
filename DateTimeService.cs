public class DateTimeService{
    public static string CurrentTimeStamp (){
        return DateTime.Now.ToString("yyyyMMddHHMMss");
    }
    public static string GetCurrentDate(){
        return  DateTime.Now.ToString("yyyyMMdd");
    }
        public static string GetCurrentTime(){
        return  DateTime.Now.ToString("HHMMss");
    }
    public static int ConvertToInt(string value){
        return int.Parse(value);
    }
    public static int ConvertTimeInMinutes(string time){
        var hours = int.Parse(time[..2]);
        var minutes = int.Parse(time[2..4]);
        return hours * 60 + minutes;    
    }
    public static string GetNiceDate(string date){
        return date[4..6] + "/" + date[6..8] + "/" + date[..4];
    }
    public static string getNiceTime(string time){
        return time[..2] + ":" + time[2..4] + ":" + time[4..6]; 
    }
}