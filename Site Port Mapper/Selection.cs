using System.Text.Json;
using System.Text.Json.Serialization;
using System.Net;
namespace Site_Port_Mapper
{
    public partial class Selection : Form
    {
        public Selection()
        {
            InitializeComponent();
            ReloadLocations();
        }
        public void ReloadLocations()
        {

            WebClient client = new WebClient();

            string json = client.DownloadString("hindex.php?p=smgetinv");
            if (json != null)
                Locations = JsonSerializer.Deserialize<LocationList>(json);


            comboBox1.Items.Clear();

            foreach (LocationInformation loc in Locations.Locations)
            {
                comboBox1.Items.Add(loc.LocationName);
            }

        }
        private void button1_Click(object sender, EventArgs e)
        {
            LocationInformation Location = Locations.Locations.Where(f => f.LocationName.Equals(comboBox1.SelectedItem)).FirstOrDefault();
            using (Form2 frm = new Form2())
            {
                
                frm.Location = Location;
                frm.ShowDialog();

            }

        }
        public LocationList Locations = new LocationList();
    }
}