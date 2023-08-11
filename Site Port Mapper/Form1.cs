using System.Drawing.Imaging;
using System.Drawing.Printing;
using Renci.SshNet;
using Renci.SshNet.Sftp;
using System.Net;
using System.Threading;
using System.Net.NetworkInformation;
using Site_Port_Mapper;
namespace Site_Port_Mapper
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
       
        private void pictureBox1_Click(object sender, EventArgs e)
        {
            var size = SwitchImage1.ClientSize;
            size.Height += 118;
            SwitchImage1.ClientSize = size;
        }
        public struct Port_Info
        {
            public Port_Info(int port, Point pos, bool active,int slot,bool gig, bool poe)
            {
                Port = port;
                Pos = pos;
                Active = active;
                Slot = slot;
                IsGiG = gig;
                PoE = poe;
            }
            public int Port;
            public Point Pos;
            public int Slot;
            public bool PoE;
            public bool IsGiG;
            public bool Active;
        }
        public SwitchInformation swinfo = new SwitchInformation();
        public LocationInformation locationinfo = new LocationInformation();
        public Stack swStack = new Stack();
        public Random rnd = new Random();
        public int utilizedports = 0;
        public int emptyports = 0;
        public ExtremeSwitchConnection swCon;
        public Form2 main_form;
        public struct Stack
        {
           public Switch[] Switches;
            public string ip;
        }
        public struct Switch
        {
            public Port_Info[] Ports;
        }

        public struct OpenPort
        {
            public int Port;
            public int slot;
        }
        
        List<OpenPort> CleanPorts(string input)
        {

            if (input.Length <= 0)
                return new List<OpenPort>();

            List<OpenPort> openPorts = new List<OpenPort>();

            var parse = input.Split('\n', StringSplitOptions.RemoveEmptyEntries);
          
            for (int i = 0; i < parse.Length; i++)
            {
                var line = parse[i].Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
                var info = line[0].Split(':');
                OpenPort openPort = new OpenPort();
                if (info.Count() > 1)
                {
                    openPort.slot = Int32.Parse(info[0]);
                    openPort.Port = Int32.Parse(info[1]);
                }
                else
                {
                    openPort.Port = Int32.Parse(info[0]);
                    openPort.slot = 1;
                }

                openPorts.Add(openPort);

            }
         

            return openPorts;
        }
        List<OpenPort> CleanPower(string input, int slot = 1)
        {

            if (input.Length <= 0)
            {
                if (!swCon.GetPOEStatus().Contains((ExtremeStackNumbers)slot))
                {
                    main_form.Update("Port mapper recieved no informaton from PoE Module on Slot: " + slot + "-> Failed Response (inline-power)");
                    return new List<OpenPort>();
                }
            }
            if (input.Contains("Timeout"))
            {
                main_form.Update("Port mapper recieved a Timeout from PoEon Slot: " + slot + ", This means PoE is not working on target device");
                return new List<OpenPort>();
            }

            List <OpenPort> openPorts = new List<OpenPort>();

            var parse = input.Split('\n', StringSplitOptions.RemoveEmptyEntries);
            
            for (int i = 0; i < parse.Length - 1; i++)
            {
                var line = parse[i].Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
                var info = line[0].Split(':');
                OpenPort openPort = new OpenPort();
                if (info.Count() > 1)
                {
                    openPort.slot = Int32.Parse(info[0]);
                    openPort.Port = Int32.Parse(info[1]);
                }
                else
                {
                    openPort.Port = Int32.Parse(info[0]);
                    openPort.slot = 1;
                }

                openPorts.Add(openPort);

            }
            

            return openPorts;
        }
        List<OpenPort> CleanSpeed(string input)
        {

            if (input.Length <= 0)
                return new List<OpenPort>();

            List<OpenPort> openPorts = new List<OpenPort>();

            var parse = input.Split('\n', StringSplitOptions.RemoveEmptyEntries);
           
            for (int i = 0; i < parse.Length - 1; i++)
            {
                var line = parse[i].Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
                var info = line[0].Split(':');
                OpenPort openPort = new OpenPort();
                if (info.Count() > 1)
                { 
                    openPort.slot = Int32.Parse(info[0]);
                    openPort.Port = Int32.Parse(info[1]);
                }
                else
                {
                    openPort.Port = Int32.Parse(info[0]);
                    openPort.slot = 1;
                }
                if (line[2] == "100")
                openPorts.Add(openPort);

            }
            

            return openPorts;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.Text = swStack.ip;
            swCon = new ExtremeSwitchConnection(swStack.ip);

            main_form.Update("Connecting to " + swStack.ip);

            if (!swCon.Connect())
            {
                MessageBox.Show("Failed to connect to switch: " + swStack.ip);
                this.Close();
                return;
            }

            main_form.Update("Getting Switch Type");
            var portsize = swCon.GetStackPortCount();

            main_form.Update("Getting Stack Size");
            int largestswitch = swCon.GetStackSize();
            

            main_form.Update("Getting Active Ports");
            var data = CleanPorts(swCon.Run("sh ports info port-number | i active"));

   
            data = data.Where(o => o.Port <= portsize).ToList();

            main_form.Update("Getting Port Speed");
            var datasp = CleanSpeed(swCon.Run("sh ports ut bandwidth port-number | i 100"));

            if (portsize == 8) this.SwitchImage1.Image = Properties.Resources._8port__1_;
            else if (portsize == 24) this.SwitchImage1.Image = Properties.Resources._24_port;
            else this.SwitchImage1.Image = Properties.Resources._8port;
            bool Has_POE = swCon.HasPOE();
            var datapw = new List<OpenPort>();

            if (Has_POE)
            {
                main_form.Update("Getting Port PoE Status (This may take awhile)");
               

                var command = "sh inline-power info ports 1-48 | i delivering";

                if (largestswitch > 1)
                    command = "sh inline-power info ports 1:1-1:48  | i delivering";

                if (portsize < 24)
                    command = "sh inline-power info ports 1-8  | i delivering";
                if (portsize < 40 && portsize > 18)
                    command = "sh inline-power info ports 1-24  | i delivering";

                //segment out into each indiviual switch
                 datapw = CleanPower(swCon.Run(command));

                if (largestswitch >= 2)
                    datapw.AddRange(CleanPower(swCon.Run("sh inline-power info ports 2:1-2:48 | i delivering"), 2));

                if (largestswitch >= 3)
                    datapw.AddRange(CleanPower(swCon.Run("sh inline-power info ports 3:1-3:48  | i delivering"), 3));

                if (largestswitch >= 4)
                    datapw.AddRange(CleanPower(swCon.Run("sh inline-power info ports 4:1-4:48 | i delivering"), 4));
            }
            main_form.Update("Disconnecting from " + swStack.ip);
            swCon.Disconnect();


            main_form.Update("Resolving Informaton");
            swStack.Switches = new Switch[largestswitch];
            for (int x = 0; x < swStack.Switches.Length; x++)
            {
                var z = 0;
                swStack.Switches[x].Ports = new Port_Info[48];
                if (portsize < 24)
                    swStack.Switches[x].Ports = new Port_Info[8];
                if (portsize < 32 && portsize > 16)
                    swStack.Switches[x].Ports = new Port_Info[24];

                for (int i = 0; i < swStack.Switches[x].Ports.Length; i++)
                {
                    Point Pos = new Point();
                    var exists = data.Where(o => o.Port == i + 1).ToList().Where(o => o.slot == x + 1).ToList();
                    var active = (exists.Count() >= 1);
                    var ispoe = false;
                    if (Has_POE)
                    {
                        var power = datapw.Where(o => o.Port == i + 1).ToList().Where(o => o.slot == x + 1).ToList();
                        ispoe = power.Count() >= 1;
                    }
                    var speed = datasp.Where(o => o.Port == i + 1).ToList().Where(o => o.slot == x + 1).ToList();
                    var isgig = !(speed.Count() >= 1);
                    utilizedports = data.Count();

                    emptyports = (largestswitch * swStack.Switches[x].Ports.Length) - data.Count();

                    if (z > 7)
                        z = 0;

                    if (i > 15 && i < 32)
                        Pos = new Point(Groups[1].X + (z * Port.Width), Groups[1].Y + (x * 118));
                    else if (i > 31)
                        Pos = new Point(Groups[2].X + (z * Port.Width), Groups[2].Y + (x * 118));
                    else
                        Pos = new Point(Groups[0].X + (z * Port.Width), Groups[0].Y + (x * 118));

                    if (i % 2 != 0)
                        Pos.Y += Port.Height;

                    Port_Info port = new Port_Info(i + 1, Pos, active, x + 1, isgig, ispoe);
                    swStack.Switches[x].Ports[i] = port;
                    if (i % 2 != 0)
                        z++;
                }
            }
            main_form.Update("Done");
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            //ip
            var textwidth = TextRenderer.MeasureText(e.Graphics, swStack.ip, new Font("Tahoma", 20), new Size(int.MaxValue, int.MaxValue), TextFormatFlags.NoPadding);
            g.DrawString(swStack.ip, new Font("Tahoma", 20), Brushes.Black, new PointF((SwitchImage1.Size.Width / 2) - textwidth.Width, 120 + (118 * (swStack.Switches.Length - 1))));

            //active
            g.FillRectangle(new SolidBrush(Color.Green), 0, 117 + (118 * (swStack.Switches.Length - 1)), Port.Width, Port.Height);
            g.DrawRectangle(new Pen(Color.Black), 0, 117 + (118 * (swStack.Switches.Length - 1)), Port.Width, Port.Height);
            g.DrawString(" Active", new Font("Tahoma", 8), Brushes.White, new PointF(0, 127 + (118 * (swStack.Switches.Length - 1))));

            //100 mbps
            g.FillRectangle(new SolidBrush(Color.Yellow), 39, 117 + (118 * (swStack.Switches.Length - 1)), Port.Width, Port.Height);
            g.DrawRectangle(new Pen(Color.Black), 39, 117 + (118 * (swStack.Switches.Length - 1)), Port.Width, Port.Height);
            g.DrawString(" 100", new Font("Tahoma", 8), Brushes.Black, new PointF(39, 120 + (118 * (swStack.Switches.Length - 1))));
            g.DrawString(" mbps", new Font("Tahoma", 8), Brushes.Black, new PointF(39, 132 + (118 * (swStack.Switches.Length - 1))));

            //POE
            g.FillRectangle(new SolidBrush(Color.Orange), 78, 117 + (118 * (swStack.Switches.Length - 1)), Port.Width, Port.Height);
            g.DrawRectangle(new Pen(Color.Black), 78, 117 + (118 * (swStack.Switches.Length - 1)), Port.Width, Port.Height);
            g.DrawRectangle(new Pen(Color.Black), 78, 117 + (118 * (swStack.Switches.Length - 1)), Port.Width, Port.Height);
            g.DrawString("  PoE", new Font("Tahoma", 8), Brushes.Black, new PointF(78, 127 + (118 * (swStack.Switches.Length - 1))));

            //POE+100
            g.FillRectangle(new SolidBrush(Color.Purple), 117, 117 + (118 * (swStack.Switches.Length - 1)), Port.Width, Port.Height);
            g.DrawRectangle(new Pen(Color.Black), 117, 117 + (118 * (swStack.Switches.Length - 1)), Port.Width, Port.Height);
            g.DrawString(" PoE", new Font("Tahoma", 8), Brushes.White, new PointF(117, 120 + (118 * (swStack.Switches.Length - 1))));
            g.DrawString("+100", new Font("Tahoma", 8), Brushes.White, new PointF(117, 132 + (118 * (swStack.Switches.Length - 1))));

            //Inactive
            g.FillRectangle(new SolidBrush(Color.Gray), 156, 117 + (118 * (swStack.Switches.Length - 1)), Port.Width, Port.Height);
            g.DrawRectangle(new Pen(Color.Black), 156, 117 + (118 * (swStack.Switches.Length - 1)), Port.Width, Port.Height);
            g.DrawString(" Open", new Font("Tahoma", 8), Brushes.White, new PointF(156, 127 + (118 * (swStack.Switches.Length - 1))));

            //active Ports
            var textactive = TextRenderer.MeasureText(e.Graphics, "Active Ports: " + utilizedports.ToString(), new Font("Tahoma", 12), new Size(int.MaxValue, int.MaxValue), TextFormatFlags.NoPadding);
            g.DrawString("Active Ports: " + utilizedports.ToString(), new Font("Tahoma", 12), Brushes.Black, new PointF((SwitchImage1.Size.Width) - textactive.Width - 30, 130 + (118 * (swStack.Switches.Length - 1))));

            //inactive Ports
            var textinactive = TextRenderer.MeasureText(e.Graphics, "Unused Ports: " + emptyports.ToString(), new Font("Tahoma", 12), new Size(int.MaxValue, int.MaxValue), TextFormatFlags.NoPadding);
            g.DrawString("Unused Ports: " + emptyports.ToString(), new Font("Tahoma", 12), Brushes.Black, new PointF((SwitchImage1.Size.Width) - textactive.Width - textinactive.Width - 40 , 130 + (118 * (swStack.Switches.Length - 1))));

            //total ports
            var texttotal = TextRenderer.MeasureText(e.Graphics, "Total Ports: " + (emptyports + utilizedports).ToString(), new Font("Tahoma", 12), new Size(int.MaxValue, int.MaxValue), TextFormatFlags.NoPadding);
            g.DrawString("Total Ports: " + (emptyports + utilizedports).ToString(), new Font("Tahoma", 12), Brushes.Black, new PointF((SwitchImage1.Size.Width) - textwidth.Width - textinactive.Width - texttotal.Width - 30, 130 + (118 * (swStack.Switches.Length - 1))));

            

        }

        private void SwitchImage1_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            SolidBrush solBrushGreen = new SolidBrush(Color.Green);
            SolidBrush solBrushGray= new SolidBrush(Color.Gray);
            SolidBrush solBrushYellow = new SolidBrush(Color.Yellow);
            SolidBrush solBrushOrange = new SolidBrush(Color.Orange);
            SolidBrush solBrushPurple = new SolidBrush(Color.Purple);
            Pen selPenBlack = new Pen(Color.Black);
            SwitchImage1.Size = new Size(SwitchImage1.Size.Width, 118 * swStack.Switches.Length);
            Size = new Size(SwitchImage1.Size.Width, 40 + (118 * swStack.Switches.Length) + TextRenderer.MeasureText(e.Graphics, swStack.ip, new Font("Tahoma", 20), new Size(int.MaxValue, int.MaxValue), TextFormatFlags.NoPadding).Height);
            foreach (var sw in swStack.Switches)
            {
                foreach (var p in sw.Ports)
                {
                    var color = solBrushGray;
                    var BrushColor = Brushes.White;
                    if (p.Active)
                    {
                        color = solBrushGreen;

                        if (!p.IsGiG)
                            color = solBrushYellow;

                        if (p.PoE)
                            color = solBrushOrange;

                        if (!p.IsGiG && p.PoE)
                            color = solBrushPurple;

                        
                    }
                    if (color == solBrushYellow)
                        BrushColor = Brushes.Black;
                    g.FillRectangle(color, p.Pos.X, p.Pos.Y, Port.Width, Port.Height);                 
                    g.DrawRectangle(selPenBlack, p.Pos.X, p.Pos.Y, Port.Width, Port.Height);
                    g.DrawString(p.Slot.ToString() + ":" + p.Port.ToString(), new Font("Tahoma", 12), BrushColor, new PointF(p.Pos.X, p.Pos.Y + 4));
                }
                
            }



            timer1.Enabled = true;
        }
       static Size Port = new Size(39, 35);
       static Point[] Groups = new Point[] { new Point(108, 25), new Point(108 + (8 * Port.Width) + 15, 25), new Point(108 + (16 * Port.Width) + 30, 25) };

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            
        }

        private Bitmap exportBitmap;
        private PrintPreviewDialog printdiag = new PrintPreviewDialog();
        private void button1_Click(object sender, EventArgs e)
        {
           

        }

        private async void Form1_Shown(object sender, EventArgs e)
        {
            //Thread t = new Thread(() => {
               
        }
        private bool ticked = false;
        private void timer1_Tick(object sender, EventArgs e)
        {

            if (ticked)
            {
                using (Graphics gfx = this.CreateGraphics())
                {
                    using (Bitmap bmp = new Bitmap(this.Bounds.Width, this.Bounds.Height, gfx))
                    {
                        this.DrawToBitmap(bmp, new Rectangle(0, 0, this.Bounds.Width + 10, this.Bounds.Height));
                        Directory.CreateDirectory(Directory.GetCurrentDirectory() + "\\" + locationinfo.LocationName);
                        bmp.Save(Directory.GetCurrentDirectory() + "\\" + locationinfo.LocationName + "\\" + swStack.ip + " " + DateTime.Now.ToString("d-M-yy H-m tt") + ".jpg", ImageFormat.Jpeg);
                    }
                }
              
                this.Close();
            }
            else ticked = true;
            
        }
    }
}