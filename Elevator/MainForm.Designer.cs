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
            lblFloor1StatusB = new Label();
            groupBox1 = new GroupBox();
            lblPanelDisplayB = new Label();
            btnPanelFloor1B = new Button();
            btnPanelFloor2B = new Button();
            lblFloor2StatusB = new Label();
            pnlShaftA = new Panel();
            panel19 = new Panel();
            panel20 = new Panel();
            panel21 = new Panel();
            panel22 = new Panel();
            pnlCableA = new Panel();
            pnlCarA = new Panel();
            panel25 = new Panel();
            panel26 = new Panel();
            panel27 = new Panel();
            lblCarIndicatorA = new Label();
            panel28 = new Panel();
            panel29 = new Panel();
            pnlShaftB = new Panel();
            panel8 = new Panel();
            panel9 = new Panel();
            panel10 = new Panel();
            panel11 = new Panel();
            pnlCableB = new Panel();
            pnlCarB = new Panel();
            panel14 = new Panel();
            panel15 = new Panel();
            panel16 = new Panel();
            lblCarIndicatorB = new Label();
            panel17 = new Panel();
            panel18 = new Panel();
            btnLog = new Button();
            grpControlPanel = new GroupBox();
            lblPanelDisplay = new Label();
            btnPanelFloor1 = new Button();
            btnPanelFloor2 = new Button();
            lblFloor1StatusA = new Label();
            lblFloor2StatusA = new Label();
            btnRequestFloor2 = new Button();
            btnRequestFloor1 = new Button();
            tmrElevator = new System.Windows.Forms.Timer(components);
            dgvLog = new DataGridView();
            tmrDoor = new System.Windows.Forms.Timer(components);
            Car = new Panel();
            panel3 = new Panel();
            panel2 = new Panel();
            mirror = new Panel();
            lblCarIndicator = new Label();
            panel1 = new Panel();
            panel4 = new Panel();
            cable = new Panel();
            guiderail1 = new Panel();
            panel6 = new Panel();
            Door1 = new Panel();
            panel5 = new Panel();
            OpendoorA = new Button();
            ClosedoorA = new Button();
            OpendoorB = new Button();
            ClosedoorB = new Button();
            MainPanel.SuspendLayout();
            groupBox1.SuspendLayout();
            pnlShaftA.SuspendLayout();
            panel20.SuspendLayout();
            pnlCarA.SuspendLayout();
            panel28.SuspendLayout();
            pnlShaftB.SuspendLayout();
            panel9.SuspendLayout();
            pnlCarB.SuspendLayout();
            panel17.SuspendLayout();
            grpControlPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvLog).BeginInit();
            Car.SuspendLayout();
            panel1.SuspendLayout();
            guiderail1.SuspendLayout();
            SuspendLayout();
            // 
            // MainPanel
            // 
            MainPanel.BackColor = SystemColors.Info;
            MainPanel.Controls.Add(lblFloor1StatusB);
            MainPanel.Controls.Add(groupBox1);
            MainPanel.Controls.Add(lblFloor2StatusB);
            MainPanel.Controls.Add(pnlShaftA);
            MainPanel.Controls.Add(pnlShaftB);
            MainPanel.Controls.Add(btnLog);
            MainPanel.Controls.Add(grpControlPanel);
            MainPanel.Controls.Add(lblFloor1StatusA);
            MainPanel.Controls.Add(lblFloor2StatusA);
            MainPanel.Controls.Add(btnRequestFloor2);
            MainPanel.Controls.Add(btnRequestFloor1);
            MainPanel.Location = new Point(12, 0);
            MainPanel.Name = "MainPanel";
            MainPanel.Size = new Size(818, 429);
            MainPanel.TabIndex = 0;
            // 
            // lblFloor1StatusB
            // 
            lblFloor1StatusB.BackColor = SystemColors.ControlText;
            lblFloor1StatusB.Font = new Font("Consolas", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblFloor1StatusB.ForeColor = Color.Lime;
            lblFloor1StatusB.Location = new Point(425, 326);
            lblFloor1StatusB.Name = "lblFloor1StatusB";
            lblFloor1StatusB.Size = new Size(47, 27);
            lblFloor1StatusB.TabIndex = 12;
            lblFloor1StatusB.Text = "status";
            lblFloor1StatusB.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // groupBox1
            // 
            groupBox1.BackColor = Color.Gray;
            groupBox1.Controls.Add(ClosedoorB);
            groupBox1.Controls.Add(OpendoorB);
            groupBox1.Controls.Add(lblPanelDisplayB);
            groupBox1.Controls.Add(btnPanelFloor1B);
            groupBox1.Controls.Add(btnPanelFloor2B);
            groupBox1.Location = new Point(652, 91);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(148, 257);
            groupBox1.TabIndex = 11;
            groupBox1.TabStop = false;
            // 
            // lblPanelDisplayB
            // 
            lblPanelDisplayB.AutoSize = true;
            lblPanelDisplayB.Location = new Point(44, 33);
            lblPanelDisplayB.Name = "lblPanelDisplayB";
            lblPanelDisplayB.Size = new Size(56, 20);
            lblPanelDisplayB.TabIndex = 2;
            lblPanelDisplayB.Text = "display";
            // 
            // btnPanelFloor1B
            // 
            btnPanelFloor1B.Location = new Point(23, 122);
            btnPanelFloor1B.Name = "btnPanelFloor1B";
            btnPanelFloor1B.Size = new Size(94, 29);
            btnPanelFloor1B.TabIndex = 1;
            btnPanelFloor1B.Text = "1st Floor";
            btnPanelFloor1B.UseVisualStyleBackColor = true;
            // 
            // btnPanelFloor2B
            // 
            btnPanelFloor2B.Location = new Point(23, 73);
            btnPanelFloor2B.Name = "btnPanelFloor2B";
            btnPanelFloor2B.Size = new Size(94, 29);
            btnPanelFloor2B.TabIndex = 0;
            btnPanelFloor2B.Text = "2nd Floor";
            btnPanelFloor2B.UseVisualStyleBackColor = true;
            // 
            // lblFloor2StatusB
            // 
            lblFloor2StatusB.BackColor = SystemColors.ControlText;
            lblFloor2StatusB.Font = new Font("Consolas", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblFloor2StatusB.ForeColor = Color.Lime;
            lblFloor2StatusB.Location = new Point(425, 91);
            lblFloor2StatusB.Name = "lblFloor2StatusB";
            lblFloor2StatusB.Size = new Size(47, 27);
            lblFloor2StatusB.TabIndex = 10;
            lblFloor2StatusB.Text = "status";
            lblFloor2StatusB.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // pnlShaftA
            // 
            pnlShaftA.BackColor = SystemColors.ControlLightLight;
            pnlShaftA.Controls.Add(panel19);
            pnlShaftA.Controls.Add(panel20);
            pnlShaftA.Controls.Add(pnlCableA);
            pnlShaftA.Controls.Add(pnlCarA);
            pnlShaftA.Location = new Point(189, 0);
            pnlShaftA.Name = "pnlShaftA";
            pnlShaftA.Size = new Size(137, 426);
            pnlShaftA.TabIndex = 8;
            // 
            // panel19
            // 
            panel19.BackColor = SystemColors.ControlDark;
            panel19.Location = new Point(123, 0);
            panel19.Name = "panel19";
            panel19.Size = new Size(14, 425);
            panel19.TabIndex = 3;
            // 
            // panel20
            // 
            panel20.BackColor = SystemColors.ControlDark;
            panel20.Controls.Add(panel21);
            panel20.Controls.Add(panel22);
            panel20.Location = new Point(0, 0);
            panel20.Name = "panel20";
            panel20.Size = new Size(14, 425);
            panel20.TabIndex = 2;
            // 
            // panel21
            // 
            panel21.Location = new Point(3, 8);
            panel21.Name = "panel21";
            panel21.Size = new Size(121, 145);
            panel21.TabIndex = 5;
            // 
            // panel22
            // 
            panel22.Location = new Point(3, 281);
            panel22.Name = "panel22";
            panel22.Size = new Size(121, 145);
            panel22.TabIndex = 4;
            // 
            // pnlCableA
            // 
            pnlCableA.BackColor = SystemColors.ActiveCaptionText;
            pnlCableA.Location = new Point(68, 176);
            pnlCableA.Margin = new Padding(3, 0, 3, 3);
            pnlCableA.Name = "pnlCableA";
            pnlCableA.Size = new Size(4, 125);
            pnlCableA.TabIndex = 1;
            // 
            // pnlCarA
            // 
            pnlCarA.BackColor = SystemColors.Info;
            pnlCarA.Controls.Add(panel25);
            pnlCarA.Controls.Add(panel26);
            pnlCarA.Controls.Add(panel27);
            pnlCarA.Controls.Add(lblCarIndicatorA);
            pnlCarA.Controls.Add(panel28);
            pnlCarA.ForeColor = SystemColors.HotTrack;
            pnlCarA.Location = new Point(15, 296);
            pnlCarA.Name = "pnlCarA";
            pnlCarA.Size = new Size(109, 129);
            pnlCarA.TabIndex = 0;
            // 
            // panel25
            // 
            panel25.BackColor = Color.Gray;
            panel25.Location = new Point(0, 98);
            panel25.Name = "panel25";
            panel25.Size = new Size(108, 4);
            panel25.TabIndex = 4;
            // 
            // panel26
            // 
            panel26.BackColor = SystemColors.WindowFrame;
            panel26.Dock = DockStyle.Bottom;
            panel26.Location = new Point(0, 114);
            panel26.Name = "panel26";
            panel26.Size = new Size(109, 15);
            panel26.TabIndex = 3;
            // 
            // panel27
            // 
            panel27.BackColor = SystemColors.GradientActiveCaption;
            panel27.Location = new Point(25, 43);
            panel27.Name = "panel27";
            panel27.Size = new Size(60, 35);
            panel27.TabIndex = 1;
            // 
            // lblCarIndicatorA
            // 
            lblCarIndicatorA.AutoSize = true;
            lblCarIndicatorA.BackColor = SystemColors.Desktop;
            lblCarIndicatorA.Font = new Font("Consolas", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblCarIndicatorA.ForeColor = Color.FromArgb(128, 255, 128);
            lblCarIndicatorA.Location = new Point(42, 16);
            lblCarIndicatorA.Name = "lblCarIndicatorA";
            lblCarIndicatorA.Size = new Size(16, 18);
            lblCarIndicatorA.TabIndex = 2;
            lblCarIndicatorA.Text = "1";
            // 
            // panel28
            // 
            panel28.BackColor = SystemColors.GrayText;
            panel28.Controls.Add(panel29);
            panel28.Dock = DockStyle.Top;
            panel28.Location = new Point(0, 0);
            panel28.Name = "panel28";
            panel28.Size = new Size(109, 13);
            panel28.TabIndex = 1;
            // 
            // panel29
            // 
            panel29.BackColor = SystemColors.ButtonHighlight;
            panel29.Location = new Point(25, 3);
            panel29.Name = "panel29";
            panel29.Size = new Size(60, 6);
            panel29.TabIndex = 0;
            // 
            // pnlShaftB
            // 
            pnlShaftB.BackColor = SystemColors.ControlLightLight;
            pnlShaftB.Controls.Add(panel8);
            pnlShaftB.Controls.Add(panel9);
            pnlShaftB.Controls.Add(pnlCableB);
            pnlShaftB.Controls.Add(pnlCarB);
            pnlShaftB.Location = new Point(499, 3);
            pnlShaftB.Name = "pnlShaftB";
            pnlShaftB.Size = new Size(137, 426);
            pnlShaftB.TabIndex = 7;
            // 
            // panel8
            // 
            panel8.BackColor = SystemColors.ControlDark;
            panel8.Location = new Point(123, 0);
            panel8.Name = "panel8";
            panel8.Size = new Size(14, 425);
            panel8.TabIndex = 3;
            // 
            // panel9
            // 
            panel9.BackColor = SystemColors.ControlDark;
            panel9.Controls.Add(panel10);
            panel9.Controls.Add(panel11);
            panel9.Location = new Point(0, 0);
            panel9.Name = "panel9";
            panel9.Size = new Size(14, 425);
            panel9.TabIndex = 2;
            // 
            // panel10
            // 
            panel10.Location = new Point(3, 8);
            panel10.Name = "panel10";
            panel10.Size = new Size(121, 145);
            panel10.TabIndex = 5;
            // 
            // panel11
            // 
            panel11.Location = new Point(3, 281);
            panel11.Name = "panel11";
            panel11.Size = new Size(121, 145);
            panel11.TabIndex = 4;
            // 
            // pnlCableB
            // 
            pnlCableB.BackColor = SystemColors.ActiveCaptionText;
            pnlCableB.Location = new Point(68, 176);
            pnlCableB.Margin = new Padding(3, 0, 3, 3);
            pnlCableB.Name = "pnlCableB";
            pnlCableB.Size = new Size(4, 125);
            pnlCableB.TabIndex = 1;
            // 
            // pnlCarB
            // 
            pnlCarB.BackColor = SystemColors.Info;
            pnlCarB.Controls.Add(panel14);
            pnlCarB.Controls.Add(panel15);
            pnlCarB.Controls.Add(panel16);
            pnlCarB.Controls.Add(lblCarIndicatorB);
            pnlCarB.Controls.Add(panel17);
            pnlCarB.ForeColor = SystemColors.HotTrack;
            pnlCarB.Location = new Point(15, 297);
            pnlCarB.Name = "pnlCarB";
            pnlCarB.Size = new Size(109, 129);
            pnlCarB.TabIndex = 0;
            // 
            // panel14
            // 
            panel14.BackColor = Color.Gray;
            panel14.Location = new Point(0, 98);
            panel14.Name = "panel14";
            panel14.Size = new Size(108, 4);
            panel14.TabIndex = 4;
            // 
            // panel15
            // 
            panel15.BackColor = SystemColors.WindowFrame;
            panel15.Dock = DockStyle.Bottom;
            panel15.Location = new Point(0, 114);
            panel15.Name = "panel15";
            panel15.Size = new Size(109, 15);
            panel15.TabIndex = 3;
            // 
            // panel16
            // 
            panel16.BackColor = SystemColors.GradientActiveCaption;
            panel16.Location = new Point(25, 43);
            panel16.Name = "panel16";
            panel16.Size = new Size(60, 35);
            panel16.TabIndex = 1;
            // 
            // lblCarIndicatorB
            // 
            lblCarIndicatorB.AutoSize = true;
            lblCarIndicatorB.BackColor = SystemColors.Desktop;
            lblCarIndicatorB.Font = new Font("Consolas", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblCarIndicatorB.ForeColor = Color.FromArgb(128, 255, 128);
            lblCarIndicatorB.Location = new Point(42, 16);
            lblCarIndicatorB.Name = "lblCarIndicatorB";
            lblCarIndicatorB.Size = new Size(16, 18);
            lblCarIndicatorB.TabIndex = 2;
            lblCarIndicatorB.Text = "1";
            // 
            // panel17
            // 
            panel17.BackColor = SystemColors.GrayText;
            panel17.Controls.Add(panel18);
            panel17.Dock = DockStyle.Top;
            panel17.Location = new Point(0, 0);
            panel17.Name = "panel17";
            panel17.Size = new Size(109, 13);
            panel17.TabIndex = 1;
            // 
            // panel18
            // 
            panel18.BackColor = SystemColors.ButtonHighlight;
            panel18.Location = new Point(25, 3);
            panel18.Name = "panel18";
            panel18.Size = new Size(60, 6);
            panel18.TabIndex = 0;
            // 
            // btnLog
            // 
            btnLog.Location = new Point(28, 383);
            btnLog.Name = "btnLog";
            btnLog.Size = new Size(94, 29);
            btnLog.TabIndex = 6;
            btnLog.Text = "Log";
            btnLog.UseVisualStyleBackColor = true;
            btnLog.Click += btnLog_Click;
            // 
            // grpControlPanel
            // 
            grpControlPanel.BackColor = Color.Gray;
            grpControlPanel.Controls.Add(ClosedoorA);
            grpControlPanel.Controls.Add(OpendoorA);
            grpControlPanel.Controls.Add(lblPanelDisplay);
            grpControlPanel.Controls.Add(btnPanelFloor1);
            grpControlPanel.Controls.Add(btnPanelFloor2);
            grpControlPanel.Location = new Point(15, 91);
            grpControlPanel.Name = "grpControlPanel";
            grpControlPanel.Size = new Size(154, 257);
            grpControlPanel.TabIndex = 5;
            grpControlPanel.TabStop = false;
            // 
            // lblPanelDisplay
            // 
            lblPanelDisplay.AutoSize = true;
            lblPanelDisplay.Location = new Point(44, 37);
            lblPanelDisplay.Name = "lblPanelDisplay";
            lblPanelDisplay.Size = new Size(56, 20);
            lblPanelDisplay.TabIndex = 2;
            lblPanelDisplay.Text = "display";
            lblPanelDisplay.Click += lblPanelDisplay_Click;
            // 
            // btnPanelFloor1
            // 
            btnPanelFloor1.Location = new Point(28, 122);
            btnPanelFloor1.Name = "btnPanelFloor1";
            btnPanelFloor1.Size = new Size(94, 29);
            btnPanelFloor1.TabIndex = 1;
            btnPanelFloor1.Text = "1st Floor";
            btnPanelFloor1.UseVisualStyleBackColor = true;
            // 
            // btnPanelFloor2
            // 
            btnPanelFloor2.Location = new Point(28, 73);
            btnPanelFloor2.Name = "btnPanelFloor2";
            btnPanelFloor2.Size = new Size(94, 29);
            btnPanelFloor2.TabIndex = 0;
            btnPanelFloor2.Text = "2nd Floor";
            btnPanelFloor2.UseVisualStyleBackColor = true;
            // 
            // lblFloor1StatusA
            // 
            lblFloor1StatusA.AutoSize = true;
            lblFloor1StatusA.BackColor = SystemColors.Desktop;
            lblFloor1StatusA.Font = new Font("Consolas", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblFloor1StatusA.ForeColor = Color.Lime;
            lblFloor1StatusA.Location = new Point(343, 330);
            lblFloor1StatusA.Name = "lblFloor1StatusA";
            lblFloor1StatusA.Size = new Size(56, 18);
            lblFloor1StatusA.TabIndex = 4;
            lblFloor1StatusA.Text = "status";
            lblFloor1StatusA.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // lblFloor2StatusA
            // 
            lblFloor2StatusA.BackColor = SystemColors.ControlText;
            lblFloor2StatusA.Font = new Font("Consolas", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblFloor2StatusA.ForeColor = Color.Lime;
            lblFloor2StatusA.Location = new Point(343, 91);
            lblFloor2StatusA.Name = "lblFloor2StatusA";
            lblFloor2StatusA.Size = new Size(47, 27);
            lblFloor2StatusA.TabIndex = 3;
            lblFloor2StatusA.Text = "status";
            lblFloor2StatusA.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // btnRequestFloor2
            // 
            btnRequestFloor2.Location = new Point(353, 140);
            btnRequestFloor2.Name = "btnRequestFloor2";
            btnRequestFloor2.Size = new Size(94, 29);
            btnRequestFloor2.TabIndex = 2;
            btnRequestFloor2.Text = "call";
            btnRequestFloor2.UseVisualStyleBackColor = true;
            // 
            // btnRequestFloor1
            // 
            btnRequestFloor1.Location = new Point(353, 363);
            btnRequestFloor1.Name = "btnRequestFloor1";
            btnRequestFloor1.Size = new Size(94, 29);
            btnRequestFloor1.TabIndex = 1;
            btnRequestFloor1.Text = "call";
            btnRequestFloor1.UseVisualStyleBackColor = true;
            // 
            // dgvLog
            // 
            dgvLog.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvLog.Location = new Point(850, 16);
            dgvLog.Name = "dgvLog";
            dgvLog.ReadOnly = true;
            dgvLog.RowHeadersWidth = 51;
            dgvLog.Size = new Size(499, 413);
            dgvLog.TabIndex = 1;
            dgvLog.Visible = false;
            // 
            // Car
            // 
            Car.BackColor = SystemColors.Info;
            Car.Controls.Add(panel3);
            Car.Controls.Add(panel2);
            Car.Controls.Add(mirror);
            Car.Controls.Add(lblCarIndicator);
            Car.Controls.Add(panel1);
            Car.ForeColor = SystemColors.HotTrack;
            Car.Location = new Point(14, 297);
            Car.Name = "Car";
            Car.Size = new Size(109, 129);
            Car.TabIndex = 0;
            // 
            // panel3
            // 
            panel3.BackColor = Color.Gray;
            panel3.Location = new Point(0, 98);
            panel3.Name = "panel3";
            panel3.Size = new Size(108, 4);
            panel3.TabIndex = 4;
            // 
            // panel2
            // 
            panel2.BackColor = SystemColors.WindowFrame;
            panel2.Dock = DockStyle.Bottom;
            panel2.Location = new Point(0, 114);
            panel2.Name = "panel2";
            panel2.Size = new Size(109, 15);
            panel2.TabIndex = 3;
            // 
            // mirror
            // 
            mirror.BackColor = SystemColors.GradientActiveCaption;
            mirror.Location = new Point(25, 43);
            mirror.Name = "mirror";
            mirror.Size = new Size(60, 35);
            mirror.TabIndex = 1;
            // 
            // lblCarIndicator
            // 
            lblCarIndicator.AutoSize = true;
            lblCarIndicator.BackColor = SystemColors.Desktop;
            lblCarIndicator.Font = new Font("Consolas", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblCarIndicator.ForeColor = Color.FromArgb(128, 255, 128);
            lblCarIndicator.Location = new Point(42, 16);
            lblCarIndicator.Name = "lblCarIndicator";
            lblCarIndicator.Size = new Size(16, 18);
            lblCarIndicator.TabIndex = 2;
            lblCarIndicator.Text = "1";
            // 
            // panel1
            // 
            panel1.BackColor = SystemColors.GrayText;
            panel1.Controls.Add(panel4);
            panel1.Dock = DockStyle.Top;
            panel1.Location = new Point(0, 0);
            panel1.Name = "panel1";
            panel1.Size = new Size(109, 13);
            panel1.TabIndex = 1;
            // 
            // panel4
            // 
            panel4.BackColor = SystemColors.ButtonHighlight;
            panel4.Location = new Point(25, 3);
            panel4.Name = "panel4";
            panel4.Size = new Size(60, 6);
            panel4.TabIndex = 0;
            // 
            // cable
            // 
            cable.BackColor = SystemColors.ActiveCaptionText;
            cable.Location = new Point(68, 176);
            cable.Margin = new Padding(3, 0, 3, 3);
            cable.Name = "cable";
            cable.Size = new Size(4, 125);
            cable.TabIndex = 1;
            // 
            // guiderail1
            // 
            guiderail1.BackColor = SystemColors.ControlDark;
            guiderail1.Controls.Add(panel6);
            guiderail1.Controls.Add(Door1);
            guiderail1.Location = new Point(0, 0);
            guiderail1.Name = "guiderail1";
            guiderail1.Size = new Size(14, 425);
            guiderail1.TabIndex = 2;
            // 
            // panel6
            // 
            panel6.Location = new Point(3, 8);
            panel6.Name = "panel6";
            panel6.Size = new Size(121, 145);
            panel6.TabIndex = 5;
            // 
            // Door1
            // 
            Door1.Location = new Point(3, 281);
            Door1.Name = "Door1";
            Door1.Size = new Size(121, 145);
            Door1.TabIndex = 4;
            // 
            // panel5
            // 
            panel5.BackColor = SystemColors.ControlDark;
            panel5.Location = new Point(123, 0);
            panel5.Name = "panel5";
            panel5.Size = new Size(14, 425);
            panel5.TabIndex = 3;
            // 
            // OpendoorA
            // 
            OpendoorA.Location = new Point(13, 176);
            OpendoorA.Name = "OpendoorA";
            OpendoorA.Size = new Size(94, 29);
            OpendoorA.TabIndex = 3;
            OpendoorA.Text = "<>";
            OpendoorA.UseVisualStyleBackColor = true;
            // 
            // ClosedoorA
            // 
            ClosedoorA.Location = new Point(28, 222);
            ClosedoorA.Name = "ClosedoorA";
            ClosedoorA.Size = new Size(94, 29);
            ClosedoorA.TabIndex = 4;
            ClosedoorA.Text = "><";
            ClosedoorA.UseVisualStyleBackColor = true;
            // 
            // OpendoorB
            // 
            OpendoorB.Location = new Point(23, 157);
            OpendoorB.Name = "OpendoorB";
            OpendoorB.Size = new Size(94, 29);
            OpendoorB.TabIndex = 4;
            OpendoorB.Text = "<>";
            OpendoorB.UseVisualStyleBackColor = true;
            // 
            // ClosedoorB
            // 
            ClosedoorB.Location = new Point(23, 208);
            ClosedoorB.Name = "ClosedoorB";
            ClosedoorB.Size = new Size(94, 29);
            ClosedoorB.TabIndex = 5;
            ClosedoorB.Text = "><";
            ClosedoorB.UseVisualStyleBackColor = true;
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
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            pnlShaftA.ResumeLayout(false);
            panel20.ResumeLayout(false);
            pnlCarA.ResumeLayout(false);
            pnlCarA.PerformLayout();
            panel28.ResumeLayout(false);
            pnlShaftB.ResumeLayout(false);
            panel9.ResumeLayout(false);
            pnlCarB.ResumeLayout(false);
            pnlCarB.PerformLayout();
            panel17.ResumeLayout(false);
            grpControlPanel.ResumeLayout(false);
            grpControlPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)dgvLog).EndInit();
            Car.ResumeLayout(false);
            Car.PerformLayout();
            panel1.ResumeLayout(false);
            guiderail1.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private Panel MainPanel;
        private Panel shaftpanel;
        private Button btnPanelFloor1;
        private Button btnRequestFloor1;
        private Button btnRequestFloor2;
        private Label lblFloor1StatusA;
        private GroupBox grpControlPanel;
        private Button btnPanelFloor2;
        private Label lblPanelDisplay;
        private Button btnLog;
        private System.Windows.Forms.Timer tmrElevator;
        private DataGridView dgvLog;
        private System.Windows.Forms.Timer tmrDoor;
        private Panel Car;
        private Label lblCarIndicator;
        private Panel panel1;
        private Panel panel2;
        private Panel mirror;
        private Panel panel3;
        private Panel cable;
        private Panel panel4;
        private Panel panel5;
        private Panel guiderail1;
        private Panel Door1;
        private Panel panel6;
        private Panel pnlShaftB;
        private Panel panel8;
        private Panel panel9;
        private Panel panel10;
        private Panel panel11;
        private Panel pnlCableB;
        private Panel pnlCarB;
        private Panel panel14;
        private Panel panel15;
        private Panel panel16;
        private Label lblCarIndicatorB;
        private Panel panel17;
        private Panel panel18;
        private Panel pnlShaftA;
        private Panel panel19;
        private Panel panel20;
        private Panel panel21;
        private Panel panel22;
        private Panel pnlCableA;
        private Panel pnlCarA;
        private Panel panel25;
        private Panel panel26;
        private Panel panel27;
        private Label lblCarIndicatorA;
        private Panel panel28;
        private Panel panel29;
        private Label lblFloor1StatusSh2;
        private Label lblFloor2StatusB;
        private Label lblFloor2StatusA;
        private GroupBox groupBox1;
        private Label lblPanelDisplayB;
        private Button btnPanelFloor1B;
        private Button btnPanelFloor2B;
        private Label lblFloor1StatusB;
        private Button ClosedoorB;
        private Button OpendoorB;
        private Button ClosedoorA;
        private Button OpendoorA;
    }
}
