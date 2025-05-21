namespace Programowanie_wspolbiezne_projekt
{
    partial class Form1
    {
        private System.Windows.Forms.Panel panelQueue; //kolejka
        private System.Windows.Forms.Panel panelGates; //bramki 
        private System.Windows.Forms.Panel stadiumPanel; //Zielony stadion
        private System.Windows.Forms.Label labelStadium; 
        private System.Windows.Forms.Panel notAdmittedPanel; //kolejki odbita
        private System.Windows.Forms.TextBox txtCapacity; //ustawianie pojemnosc
        private System.Windows.Forms.Button btnStart; //przycisk start
        private System.Windows.Forms.Button btnRestart; //przycisk restart

        private void InitializeComponent()
        {
            //inicjalizacja tworzenie obiektow w apliakcji
            this.panelQueue = new System.Windows.Forms.Panel();
            this.panelGates = new System.Windows.Forms.Panel();
            this.stadiumPanel = new System.Windows.Forms.Panel();
            this.labelStadium = new System.Windows.Forms.Label();
            this.notAdmittedPanel = new System.Windows.Forms.Panel();
            this.txtCapacity = new System.Windows.Forms.TextBox();
            this.btnStart = new System.Windows.Forms.Button();
            this.btnRestart = new System.Windows.Forms.Button();
            this.SuspendLayout();

            //Wyglad kolejki na stadion
            this.panelQueue.AutoScroll = true;
            this.panelQueue.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelQueue.Location = new System.Drawing.Point(10, 140);
            this.panelQueue.Name = "panelQueue";
            this.panelQueue.Size = new System.Drawing.Size(600, 80);
            this.panelQueue.TabIndex = 0;
            this.panelQueue.Padding = new Padding(5);

            //Wyglad bramek
            this.panelGates.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelGates.Location = new System.Drawing.Point(620, 10);
            this.panelGates.Name = "panelGates";
            this.panelGates.Size = new System.Drawing.Size(130, 400);
            this.panelGates.TabIndex = 1;

            //Stadion wyglad
            this.stadiumPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.stadiumPanel.Location = new System.Drawing.Point(760, 10);
            this.stadiumPanel.Name = "stadiumPanel";
            this.stadiumPanel.Size = new System.Drawing.Size(200, 400);
            this.stadiumPanel.BackColor = System.Drawing.Color.Green;
            this.stadiumPanel.TabIndex = 2;

            //Napis na stadionie
            this.labelStadium.AutoSize = true;
            this.labelStadium.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.labelStadium.ForeColor = System.Drawing.Color.White;
            this.labelStadium.BackColor = System.Drawing.Color.Transparent;
            this.labelStadium.Location = new System.Drawing.Point(20, 180);
            this.labelStadium.Name = "labelStadium";
            this.labelStadium.Size = new System.Drawing.Size(160, 25);
            this.labelStadium.TabIndex = 3;
            this.labelStadium.Text = "Wolnych miejsc: 20";
            this.stadiumPanel.Controls.Add(this.labelStadium);

            //Odbici kibice
            this.notAdmittedPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.notAdmittedPanel.Location = new System.Drawing.Point(10, 230);
            this.notAdmittedPanel.Name = "notAdmittedPanel";
            this.notAdmittedPanel.Size = new System.Drawing.Size(600, 80);
            this.notAdmittedPanel.TabIndex = 4;
            this.notAdmittedPanel.BackColor = System.Drawing.Color.LightGray;

            //Podawanie pojemnosc
            this.txtCapacity.Location = new System.Drawing.Point(10, 20);
            this.txtCapacity.Name = "txtCapacity";
            this.txtCapacity.Size = new System.Drawing.Size(100, 23);
            this.txtCapacity.TabIndex = 5;
            this.txtCapacity.Text = "20";

            //Przycisk start
            this.btnStart.Location = new System.Drawing.Point(120, 20);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(75, 23);
            this.btnStart.TabIndex = 6;
            this.btnStart.Text = "Start";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.BtnStart_Click);

            //Przycisk restart
            this.btnRestart.Location = new System.Drawing.Point(200, 20);
            this.btnRestart.Name = "btnRestart";
            this.btnRestart.Size = new System.Drawing.Size(75, 23);
            this.btnRestart.TabIndex = 7;
            this.btnRestart.Text = "Restart";
            this.btnRestart.UseVisualStyleBackColor = true;
            this.btnRestart.Click += new System.EventHandler(this.BtnRestart_Click);
            this.btnRestart.Enabled = false;

            //Wyglad całego okienka 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(980, 450);
            this.Controls.Add(this.btnRestart);
            this.Controls.Add(this.btnStart);
            this.Controls.Add(this.txtCapacity);
            this.Controls.Add(this.notAdmittedPanel);
            this.Controls.Add(this.panelQueue);
            this.Controls.Add(this.panelGates);
            this.Controls.Add(this.stadiumPanel);
            this.Name = "Form1";
            this.Text = "Projekt Ostateczny";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}