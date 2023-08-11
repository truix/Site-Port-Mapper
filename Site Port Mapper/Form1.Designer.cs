namespace Site_Port_Mapper
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.SwitchImage1 = new System.Windows.Forms.PictureBox();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.SwitchImage1)).BeginInit();
            this.SuspendLayout();
            // 
            // SwitchImage1
            // 
            this.SwitchImage1.BackgroundImage = global::Site_Port_Mapper.Properties.Resources._8port;
            this.SwitchImage1.Location = new System.Drawing.Point(0, -1);
            this.SwitchImage1.Name = "SwitchImage1";
            this.SwitchImage1.Size = new System.Drawing.Size(1192, 120);
            this.SwitchImage1.TabIndex = 0;
            this.SwitchImage1.TabStop = false;
            this.SwitchImage1.Click += new System.EventHandler(this.pictureBox1_Click);
            this.SwitchImage1.Paint += new System.Windows.Forms.PaintEventHandler(this.SwitchImage1_Paint);
            // 
            // timer1
            // 
            this.timer1.Interval = 1500;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1193, 119);
            this.Controls.Add(this.SwitchImage1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Form1";
            this.Text = "Network Services Port Mapper";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.Shown += new System.EventHandler(this.Form1_Shown);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.Form1_Paint);
            ((System.ComponentModel.ISupportInitialize)(this.SwitchImage1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private PictureBox SwitchImage1;
        private System.Windows.Forms.Timer timer1;
    }
}