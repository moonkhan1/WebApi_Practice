namespace WebApi_Practice
{
    public class WeatherForecast
    {
        public DateTime Date { get; set; }

        public int TemperatureC { get; set; }

        public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);

        public string? Summary { get; set; }
    }

    public class User : IUser
    {
        public string Name { get; set; }
        public string GatFullName()
        {
            return "Filankes Filankesov";
        }

        public string SetFullName(string fullname)
        {
            Name = fullname;

            return fullname;
        }
    }

    public interface IUser
    {
        string GatFullName();
        string SetFullName(string fullname);
    }
}