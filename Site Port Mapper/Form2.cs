using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;
using Site_Port_Mapper;
namespace Site_Port_Mapper
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
            
        }
        private void Form2_Load(object sender, EventArgs e)
        {
            GetSwitches();
        }
        public void GetSwitches()
        {
            WebClient client = new WebClient();

            string json = client.DownloadString("http://testwebapp.xyz/index.php?p=getallknownswitches&subnet=" + Location.LocationTemplate);
            if (json != null)
                Switches = JsonSerializer.Deserialize<SwitchList>(json);


            switches_list_box.Items.Clear();

            foreach (SwitchInformation loc in Switches.Switches)
            {        
                    switches_list_box.Items.Add(loc.name, CheckState.Checked);
            }

        }
        public void Update(string label)
        {
          console_list_box.Items.Add(label);
          console_list_box.TopIndex = console_list_box.Items.Count - 1;
        }
        public LocationInformation Location = new LocationInformation();
        public SwitchList Switches = new SwitchList();

        private void button1_Click(object sender, EventArgs e)
        {
            var to_sort = Switches.Switches;
            button1.Enabled = false;
            foreach (string name in switches_list_box.Items)
            {
                if (switches_list_box.GetItemChecked(switches_list_box.FindStringExact(name)))
                {
                    continue;
                }

                to_sort.RemoveAll(i => i.name == name);
            }
            foreach (var x in to_sort)
            {          
                {
                    using (Form1 frm = new Form1())
                    {
                        frm.main_form = this;
                        frm.swStack.ip = x.ip;
                        frm.locationinfo = Location;
                        frm.swinfo = x;
                        frm.ShowDialog();

                    }
                }
            }
            MessageBox.Show("Exported JPG to: " + Directory.GetCurrentDirectory() + "\\" + Location.LocationName);
            button1.Enabled = true;
        }
    }
}
