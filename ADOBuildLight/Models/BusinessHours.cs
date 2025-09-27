namespace ADOBuildLight.Models;

public class BusinessHours
{
    public int StartHour { get; set; }
    public int EndHour { get; set; }
    public List<string> DaysOfWeek { get; set; } = new List<string>();
}