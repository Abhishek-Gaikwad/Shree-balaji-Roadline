namespace Trans9.Models
{
    public class Constants {
        public static string dev = "vyankateshsoft_sblbkp";
        public static string prod = "vyankateshsoft_sbl";
    }
    public class DropDown
    {
        public dynamic Id { get; set; }
        public string Name { get; set; }
    }


    public class QueryResult
    {
        public int errorCode { get; set; }
        public string? id { get; set; }
        public string Message { get; set; }
    }
}
