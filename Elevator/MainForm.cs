using System.ComponentModel;
using System.Data;
using System.Drawing.Drawing2D;

namespace Elevator
{
    public partial class MainForm : Form, IElevatorObserver
    {
        private const int FLOOR1_Y = 287;
        private const int FLOOR2_Y = 26;
        private const int HALL_GAP = 170;            // space between the shafts for the hall call buttons

        private readonly ElevatorLogDb db = new ElevatorLogDb();
        private readonly ElevatorDispatcher dispatcher = new ElevatorDispatcher();

        private ElevatorUnit unitA, unitB;
        private ElevatorView viewA, viewB;
        private int expandedHeight;                  // form height when the log grid is visible

        // Passed to each ElevatorUnit to convert a floor number into a pixel Y.
        private static int FloorToY(int floor) => floor == 1 ? FLOOR1_Y : FLOOR2_Y;

        public MainForm()
        {
            InitializeComponent();
        }

        //  Start-up
        private void MainForm_Load(object sender, EventArgs e)
        {
            // "-=" before "+=" so a handler is never attached twice
            tmrElevator.Tick -= tmrElevator_Tick; tmrElevator.Tick += tmrElevator_Tick;
            tmrDoor.Tick -= tmrDoor_Tick; tmrDoor.Tick += tmrDoor_Tick;
            btnRequestFloor1.Click -= btnRequestFloor1_Click; btnRequestFloor1.Click += btnRequestFloor1_Click;
            btnRequestFloor2.Click -= btnRequestFloor2_Click; btnRequestFloor2.Click += btnRequestFloor2_Click;
            btnPanelFloor1.Click -= btnPanelFloor1_Click; btnPanelFloor1.Click += btnPanelFloor1_Click;
            btnPanelFloor2.Click -= btnPanelFloor2_Click; btnPanelFloor2.Click += btnPanelFloor2_Click;
            btnLog.Click -= btnLog_Click; btnLog.Click += btnLog_Click;

            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            StartPosition = FormStartPosition.CenterScreen;
            DoubleBuffered = true;

            // Elevator A uses the controls placed in the designer
            Control shaftA = car.Parent;
            Control host = shaftA.Parent;                // the big yellow panel
            Control panelA = btnPanelFloor1.Parent;

            // Elevator B is built in code, copying A's controls
            var shaftB = new Panel
            {
                Size = shaftA.Size,
                BackColor = shaftA.BackColor,
                Location = new Point(shaftA.Right + HALL_GAP, shaftA.Top)
            };
            host.Controls.Add(shaftB);

            var carB = new PictureBox
            {
                Size = car.Size,
                BackColor = car.BackColor,
                Location = new Point(car.Left, FLOOR1_Y)
            };
            shaftB.Controls.Add(carB);

            var panelB = new Panel
            {
                Size = panelA.Size,
                BackColor = panelA.BackColor,
                Location = new Point(shaftB.Right + 40, panelA.Top)
            };
            host.Controls.Add(panelB);

            var displayB = new Label { Location = lblPanelDisplay.Location, Size = lblPanelDisplay.Size };
            var btnB1 = CloneButton(btnPanelFloor1);
            var btnB2 = CloneButton(btnPanelFloor2);
            panelB.Controls.AddRange(new Control[] { displayB, btnB1, btnB2 });

            host.Width = panelB.Right + 30;

            ApplyTheme(host, shaftA, panelA, panelB, displayB, btnB1, btnB2);

            // Models
            int doorMax = (car.Width + 1) / 2;           // each door leaf covers half the car
            unitA = new ElevatorUnit(1, "A", doorMax, FloorToY);
            unitB = new ElevatorUnit(2, "B", doorMax, FloorToY);
            dispatcher.Add(unitA);
            dispatcher.Add(unitB);

            // Views
            viewA = new ElevatorView(unitA, shaftA, car, lblPanelDisplay, btnPanelFloor1, btnPanelFloor2);
            viewB = new ElevatorView(unitB, shaftB, carB, displayB, btnB1, btnB2);

            // Observers: DB logger and this form (hall displays)
            var logger = new LogObserver((floor, text) => WriteLog(floor, text));
            foreach (var u in dispatcher.Units)
            {
                u.Attach(logger);
                u.Attach(this);
            }

            // Panel B requests go to elevator B only
            btnB1.Click += (s, ev) => unitB.AddStop(1, "Panel request for floor 1");
            btnB2.Click += (s, ev) => unitB.AddStop(2, "Panel request for floor 2");

            // Log grid sits below the lifts, hidden until requested
            dgvLog.Location = new Point(host.Left, host.Bottom + 10);
            dgvLog.Size = new Size(host.Width, 180);
            dgvLog.Visible = false;
            StyleLogGrid();
            ClientSize = new Size(host.Right + host.Left, host.Bottom + host.Top);
            expandedHeight = dgvLog.Bottom + host.Top;

            RefreshHall();

            tmrElevator.Interval = 15; tmrElevator.Start();    // movement
            tmrDoor.Interval = 30; tmrDoor.Start();            // doors
        }

        //  Timers: each tick is passed to every unit's current state
        private void tmrElevator_Tick(object sender, EventArgs e)
        {
            foreach (var u in dispatcher.Units) u.MoveTick();
        }

        private void tmrDoor_Tick(object sender, EventArgs e)
        {
            foreach (var u in dispatcher.Units) u.DoorTick();
        }

        //  Buttons
        // --------------------
        // Hall buttons: the dispatcher chooses which elevator responds
        private void btnRequestFloor1_Click(object sender, EventArgs e) => dispatcher.AssignHallCall(1);
        private void btnRequestFloor2_Click(object sender, EventArgs e) => dispatcher.AssignHallCall(2);

        // Panel A: elevator A only
        private void btnPanelFloor1_Click(object sender, EventArgs e) => unitA.AddStop(1, "Panel request for floor 1");
        private void btnPanelFloor2_Click(object sender, EventArgs e) => unitA.AddStop(2, "Panel request for floor 2");

        // Designer leftover; safe to delete along with its line in MainForm.Designer.cs
        private void button2_Click(object sender, EventArgs e) { }

        //  Observer: hall displays shared by both elevators
        void IElevatorObserver.Update(ElevatorUnit elevator, string eventText) => RefreshHall();

        private void RefreshHall()
        {
            if (unitA == null || unitB == null) return;  // observers can fire before both units exist

            string text = $"A {unitA.DirectionArrow}{unitA.CurrentFloor}   B {unitB.DirectionArrow}{unitB.CurrentFloor}";
            lblFloor1Status.Text = text;
            lblFloor2Status.Text = text;

            // Orange while any elevator has a pending stop at that floor
            btnRequestFloor1.BackColor = dispatcher.Units.Any(u => u.IsPending(1)) ? Color.Orange : Color.Silver;
            btnRequestFloor2.BackColor = dispatcher.Units.Any(u => u.IsPending(2)) ? Color.Orange : Color.Silver;
        }

        //  Database: runs off the UI thread so the animation stays smooth
        private void RunInBackground(Func<object> work, Action<object> onSuccess, Action onFinished = null)
        {
            var bw = new BackgroundWorker();

            bw.DoWork += (s, e) => e.Result = work();        // worker thread: don't touch controls

            bw.RunWorkerCompleted += (s, e) =>               // back on the UI thread
            {
                if (e.Error != null) ShowDbError(e.Error);
                else onSuccess?.Invoke(e.Result);

                onFinished?.Invoke();
                bw.Dispose();
            };

            bw.RunWorkerAsync();
        }

        private void ShowDbError(Exception ex)
        {
            MessageBox.Show("Database problem: " + ex.Message, "Elevator Log",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void WriteLog(int floor, string text)
        {
            RunInBackground(
                () => { db.AddEntry(floor, text); return null; },
                null);
        }

        private void btnLog_Click(object sender, EventArgs e)
        {
            btnLog.Enabled = false;                          // blocks repeat clicks while loading
            RunInBackground(
                () => db.Load(),
                result =>
                {
                    dgvLog.DataSource = (DataTable)result;
                    if (dgvLog.Columns.Contains("LoggedAt"))
                        dgvLog.Columns["LoggedAt"].DefaultCellStyle.Format = "dd/MM/yyyy HH:mm:ss";
                    if (dgvLog.Columns.Contains("EventText")) dgvLog.Columns["EventText"].FillWeight = 300;
                    if (dgvLog.Columns.Contains("LogID")) dgvLog.Columns["LogID"].FillWeight = 40;

                    ClientSize = new Size(ClientSize.Width, expandedHeight);
                    dgvLog.Visible = true;
                },
                () => btnLog.Enabled = true);
        }

       
        //  Look and feel
        private static Button CloneButton(Button src) =>
            new Button { Text = src.Text, Location = src.Location, Size = src.Size };

        private void StyleRoundButton(Button b, string text, int size)
        {
            b.Text = text;
            b.Size = new Size(size, size);
            b.FlatStyle = FlatStyle.Flat;
            b.FlatAppearance.BorderSize = 0;
            b.FlatAppearance.MouseOverBackColor = Color.FromArgb(255, 210, 100);
            b.FlatAppearance.MouseDownBackColor = Color.Orange;
            b.BackColor = Color.Silver;
            b.ForeColor = Color.Black;
            b.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            b.Cursor = Cursors.Hand;
            var path = new GraphicsPath();               // elliptical clip region makes the button round
            path.AddEllipse(0, 0, b.Width, b.Height);
            b.Region = new Region(path);
        }

        private static void StyleLcd(Label l, int w, int h, float fontSize)
        {
            l.AutoSize = false;
            l.Size = new Size(w, h);
            l.BackColor = Color.Black;
            l.ForeColor = Color.LimeGreen;
            l.Font = new Font("Consolas", fontSize, FontStyle.Bold);
            l.TextAlign = ContentAlignment.MiddleCenter;
        }

        private static void CenterInParent(Control c) =>
            c.Left = (c.Parent.ClientSize.Width - c.Width) / 2;

        private void ApplyTheme(Control host, Control shaftA, Control panelA, Panel panelB,
                                Label displayB, Button btnB1, Button btnB2)
        {
            Color wall = Color.FromArgb(225, 222, 214);
            Color steel = Color.FromArgb(55, 58, 64);
            BackColor = wall;
            host.BackColor = wall;

            // Car control panels
            panelA.BackColor = steel;
            panelB.BackColor = steel;
            void Center(Control c) { if (c.Parent == panelA || c.Parent == panelB) CenterInParent(c); }
            foreach (var l in new[] { lblPanelDisplay, displayB }) { StyleLcd(l, 60, 30, 14); Center(l); }
            foreach (var b in new[] { btnPanelFloor2, btnB2 }) { StyleRoundButton(b, "2", 48); Center(b); }
            foreach (var b in new[] { btnPanelFloor1, btnB1 }) { StyleRoundButton(b, "1", 48); Center(b); }

            // Hall call stations, centred between the two shafts
            StyleRoundButton(btnRequestFloor1, "\u25B2", 44);     // floor 1: up
            StyleRoundButton(btnRequestFloor2, "\u25BC", 44);     // floor 2: down
            StyleLcd(lblFloor1Status, 130, 28, 11);
            StyleLcd(lblFloor2Status, 130, 28, 11);

            int hallX = shaftA.Right + HALL_GAP / 2;
            foreach (Control c in new Control[] { btnRequestFloor1, btnRequestFloor2, lblFloor1Status, lblFloor2Status })
                c.Left = hallX - c.Width / 2;
            lblFloor1Status.Top = btnRequestFloor1.Top - lblFloor1Status.Height - 4;
            lblFloor2Status.Top = btnRequestFloor2.Top - lblFloor2Status.Height - 4;

            btnLog.FlatStyle = FlatStyle.Flat;
            btnLog.BackColor = steel;
            btnLog.ForeColor = Color.White;
            btnLog.FlatAppearance.BorderColor = Color.Silver;
        }

        private void StyleLogGrid()
        {
            dgvLog.BorderStyle = BorderStyle.None;
            dgvLog.BackgroundColor = Color.White;
            dgvLog.RowHeadersVisible = false;
            dgvLog.ReadOnly = true;
            dgvLog.AllowUserToAddRows = false;
            dgvLog.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvLog.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvLog.EnableHeadersVisualStyles = false;
            dgvLog.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(55, 58, 64);
            dgvLog.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvLog.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(244, 244, 247);
        }
    }
}