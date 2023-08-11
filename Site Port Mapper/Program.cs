namespace Site_Port_Mapper
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            Application.Run(new Selection());
        }
    }
    public class SwitchInformation
    {
        public int id { get; set; }
        public string name { get; set; }
        public string location { get; set; }
        public string mac { get; set; }
        public string type { get; set; }
        public string ip { get; set; }

    }
    public class SwitchList
    {
        public List<SwitchInformation> Switches { get; set; }
    }
    public class LocationInformation
    {
        public string LocationName { get; set; }
        public string LocationTemplate { get; set; }
    }
    public class LocationList
    {
        public List<LocationInformation> Locations { get; set; }
    }

}