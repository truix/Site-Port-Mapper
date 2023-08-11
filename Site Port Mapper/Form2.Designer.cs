namespace Site_Port_Mapper
{
    partial class Form2
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.switches_list_box = new System.Windows.Forms.CheckedListBox();
            this.console_list_box = new System.Windows.Forms.ListBox();
            this.button1 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // switches_list_box
            // 
            this.switches_list_box.CheckOnClick = true;
            this.switches_list_box.FormattingEnabled = true;
            this.switches_list_box.Location = new System.Drawing.Point(515, 12);
            this.switches_list_box.Name = "switches_list_box";
            this.switches_list_box.Size = new System.Drawing.Size(273, 310);
            this.switches_list_box.TabIndex = 0;
            // 
            // console_list_box
            // 
            this.console_list_box.FormattingEnabled = true;
            this.console_list_box.HorizontalScrollbar = true;
            this.console_list_box.ItemHeight = 15;
            this.console_list_box.Location = new System.Drawing.Point(12, 12);
            this.console_list_box.Name = "console_list_box";
            this.console_list_box.Size = new System.Drawing.Size(497, 334);
            this.console_list_box.TabIndex = 1;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(515, 323);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(273, 23);
            this.button1.TabIndex = 2;
            this.button1.Text = "Start";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // Form2
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 356);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.console_list_box);
            this.Controls.Add(this.switches_list_box);
            this.Name = "Form2";
            this.Text = "Switch Selection";
            this.Load += new System.EventHandler(this.Form2_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private CheckedListBox switches_list_box;
        private ListBox console_list_box;
        private Button button1;
    }
}