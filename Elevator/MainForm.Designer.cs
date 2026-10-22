namespace Elevator
{
    partial class MainForm
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
            components = new System.ComponentModel.Container();
            MainPanel = new Panel();
            btnLog = new Button();
            grpControlPanel = new GroupBox();
            lblPanelDisplay = new Label();
            btnPanelFloor1 = new Button();
            btnPanelFloor2 = new Button();
            lblFloor1Status = new Label();
            lblFloor2Status = new Label();
            btnRequestFloor2 = new Button();
            btnRequestFloor1 = new Button();
            shaftpanel = new Panel();
            car = new PictureBox();
            tmrElevator = new System.Windows.Forms.Timer(components);
            dgvLog = new DataGridView();
            tmrDoor = new System.Windows.Forms.Timer(components);
            MainPanel.SuspendLayout();
            grpControlPanel.SuspendLayout();
            shaftpanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)car).BeginInit();
            ((System.ComponentModel.ISupportInitialize)dgvLog).BeginInit();
            SuspendLayout();
            // 
            // MainPanel
            // 
            MainPanel.BackColor = SystemColors.Info;
            MainPanel.Controls.Add(btnLog);
            MainPanel.Controls.Add(grpControlPanel);
            MainPanel.Controls.Add(lblFloor1Status);
            MainPanel.Controls.Add(lblFloor2Status);
            MainPanel.Controls.Add(btnRequestFloor2);
            MainPanel.Controls.Add(btnRequestFloor1);
            MainPanel.Controls.Add(shaftpanel);
            MainPanel.Location = new Point(12, 0);
            MainPanel.Name = "MainPanel";
            MainPanel.Size = new Size(776, 450);
            MainPanel.TabIndex = 0;
            // 
            // btnLog
            // 
            btnLog.Location = new Point(134, 379);
            btnLog.Name = "btnLog";
            btnLog.Size = new Size(94, 29);
            btnLog.TabIndex = 6;
            btnLog.Text = "Log";
            btnLog.UseVisualStyleBackColor = true;
            btnLog.Click += btnLog_Click;
            // 
            // grpControlPanel
            // 
            grpControlPanel.Controls.Add(lblPanelDisplay);
            grpControlPanel.Controls.Add(btnPanelFloor1);
            grpControlPanel.Controls.Add(btnPanelFloor2);
            grpControlPanel.Location = new Point(127, 102);
            grpControlPanel.Name = "grpControlPanel";
            grpControlPanel.Size = new Size(107, 194);
            grpControlPanel.TabIndex = 5;
            grpControlPanel.TabStop = false;
            // 
            // lblPanelDisplay
            // 
            lblPanelDisplay.AutoSize = true;
            lblPanelDisplay.Location = new Point(23, 29);
            lblPanelDisplay.Name = "lblPanelDisplay";
            lblPanelDisplay.Size = new Size(56, 20);
            lblPanelDisplay.TabIndex = 2;
            lblPanelDisplay.Text = "display";
            // 
            // btnPanelFloor1
            // 
            btnPanelFloor1.Location = new Point(6, 155);
            btnPanelFloor1.Name = "btnPanelFloor1";
            btnPanelFloor1.Size = new Size(94, 29);
            btnPanelFloor1.TabIndex = 1;
            btnPanelFloor1.Text = "1st Floor";
            btnPanelFloor1.UseVisualStyleBackColor = true;
            // 
            // btnPanelFloor2
            // 
            btnPanelFloor2.Location = new Point(7, 86);
            btnPanelFloor2.Name = "btnPanelFloor2";
            btnPanelFloor2.Size = new Size(94, 29);
            btnPanelFloor2.TabIndex = 0;
            btnPanelFloor2.Text = "2nd Floor";
            btnPanelFloor2.UseVisualStyleBackColor = true;
            // 
            // lblFloor1Status
            // 
            lblFloor1Status.AutoSize = true;
            lblFloor1Status.Location = new Point(504, 349);
            lblFloor1Status.Name = "lblFloor1Status";
            lblFloor1Status.Size = new Size(47, 20);
            lblFloor1Status.TabIndex = 4;
            lblFloor1Status.Text = "status";
            // 
            // lblFloor2Status
            // 
            lblFloor2Status.AutoSize = true;
            lblFloor2Status.Location = new Point(504, 87);
            lblFloor2Status.Name = "lblFloor2Status";
            lblFloor2Status.Size = new Size(47, 20);
            lblFloor2Status.TabIndex = 3;
            lblFloor2Status.Text = "status";
            // 
            // btnRequestFloor2
            // 
            btnRequestFloor2.Location = new Point(488, 110);
            btnRequestFloor2.Name = "btnRequestFloor2";
            btnRequestFloor2.Size = new Size(94, 29);
            btnRequestFloor2.TabIndex = 2;
            btnRequestFloor2.Text = "call";
            btnRequestFloor2.UseVisualStyleBackColor = true;
            // 
            // btnRequestFloor1
            // 
            btnRequestFloor1.Location = new Point(491, 383);
            btnRequestFloor1.Name = "btnRequestFloor1";
            btnRequestFloor1.Size = new Size(94, 29);
            btnRequestFloor1.TabIndex = 1;
            btnRequestFloor1.Text = "call";
            btnRequestFloor1.UseVisualStyleBackColor = true;
            // 
            // shaftpanel
            // 
            shaftpanel.BackColor = SystemColors.ControlLightLight;
            shaftpanel.Controls.Add(car);
            shaftpanel.Location = new Point(296, 12);
            shaftpanel.Name = "shaftpanel";
            shaftpanel.Size = new Size(165, 426);
            shaftpanel.TabIndex = 0;
            // 
            // car
            // 
            car.BackColor = SystemColors.ActiveCaptionText;
            car.Location = new Point(28, 287);
            car.Name = "car";
            car.Size = new Size(106, 113);
            car.TabIndex = 0;
            car.TabStop = false;
            // 
            // dgvLog
            // 
            dgvLog.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvLog.Location = new Point(821, 29);
            dgvLog.Name = "dgvLog";
            dgvLog.ReadOnly = true;
            dgvLog.RowHeadersWidth = 51;
            dgvLog.Size = new Size(429, 301);
            dgvLog.TabIndex = 1;
            dgvLog.Visible = false;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1383, 646);
            Controls.Add(dgvLog);
            Controls.Add(MainPanel);
            Name = "MainForm";
            Text = "Elevator Control";
            Load += MainForm_Load;
            MainPanel.ResumeLayout(false);
            MainPanel.PerformLayout();
            grpControlPanel.ResumeLayout(false);
            grpControlPanel.PerformLayout();
            shaftpanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)car).EndInit();
            ((System.ComponentModel.ISupportInitialize)dgvLog).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private Panel MainPanel;
        private Panel shaftpanel;
        private Button btnPanelFloor1;
        private Button btnRequestFloor1;
        private Button btnRequestFloor2;
        private Label lblFloor1Status;
        private Label lblFloor2Status;
        private GroupBox grpControlPanel;
        private Button btnPanelFloor2;
        private Label lblPanelDisplay;
        private Button btnLog;
        public PictureBox car;
        private System.Windows.Forms.Timer tmrElevator;
        private DataGridView dgvLog;
        private System.Windows.Forms.Timer tmrDoor;
    }
}
