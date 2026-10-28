using System.ComponentModel;
using System.Data;

namespace Elevator
{
    // Expected control names:
    //  Elevator A: pnlShaftA, pnlCarA, pnlCableA, lblCarIndicatorA, lblPanelDisplay,
    //              btnPanelFloor1, btnPanelFloor2
    //  Elevator B: pnlShaftB, pnlCarB, pnlCableB, lblCarIndicatorB, lblPanelDisplayB,
    //              btnPanelFloor1B, btnPanelFloor2B
    //  Shared:     btnRequestFloor1, btnRequestFloor2, btnLog, dgvLog, tmrElevator, tmrDoor
    //  Hall displays (one per elevator per floor): lblFloor1StatusA, lblFloor1StatusB,
    //              lblFloor2StatusA, lblFloor2StatusB
    public partial class MainForm : Form, IElevatorObserver
    {
        private const int FLOOR2_Y = 26;             // car top position at floor 2

        private readonly ElevatorLogDb db = new ElevatorLogDb();
        private readonly ElevatorDispatcher dispatcher = new ElevatorDispatcher();

        private ElevatorUnit unitA, unitB;
        private ElevatorView viewA, viewB;
        private int floor1Y;                         // car A's designer position
        private int collapsedWidth, expandedWidth;   // form width without / with the log grid

        private int FloorToY(int floor) => floor == 1 ? floor1Y : FLOOR2_Y;

        public MainForm()
        {
            InitializeComponent();
        }

        // Start-up
        private void MainForm_Load(object sender, EventArgs e)
        {
            // "-=" before "+=" prevents handlers from being attached twice
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

            // Flat style is required for BackColor changes to show
            foreach (Button b in new[] { btnRequestFloor1, btnRequestFloor2,
                                         btnPanelFloor1, btnPanelFloor2,
                                         btnPanelFloor1B, btnPanelFloor2B })
            {
                b.FlatStyle = FlatStyle.Flat;
                b.BackColor = Color.Silver;
            }

            btnRequestFloor1.Text = "\u25B2 Call";       // floor 1 can only go up
            btnRequestFloor2.Text = "\u25BC Call";       // floor 2 can only go down

            foreach (Label l in new[] { lblFloor1StatusA, lblFloor1StatusB,
                                        lblFloor2StatusA, lblFloor2StatusB,
                                        lblPanelDisplay, lblPanelDisplayB })
                StyleDisplay(l);

            floor1Y = pnlCarA.Top;
            int doorMax = (pnlCarA.Width + 1) / 2;   // each door leaf covers half the car
            unitA = new ElevatorUnit(1, "A", doorMax, FloorToY);
            unitB = new ElevatorUnit(2, "B", doorMax, FloorToY);
            dispatcher.Add(unitA);
            dispatcher.Add(unitB);

            viewA = new ElevatorView(unitA, pnlShaftA, pnlCarA, pnlCableA,
                                     lblPanelDisplay, lblCarIndicatorA,
                                     btnPanelFloor1, btnPanelFloor2);
            viewB = new ElevatorView(unitB, pnlShaftB, pnlCarB, pnlCableB,
                                     lblPanelDisplayB, lblCarIndicatorB,
                                     btnPanelFloor1B, btnPanelFloor2B);

            // Observers: database logger and this form's hall displays
            var logger = new LogObserver((floor, text) => WriteLog(floor, text));
            foreach (var u in dispatcher.Units)
            {
                u.Attach(logger);
                u.Attach(this);
            }

            // Panel B requests go to elevator B only
            btnPanelFloor1B.Click += (s, ev) => unitB.AddStop(1, "Panel request for floor 1");
            btnPanelFloor2B.Click += (s, ev) => unitB.AddStop(2, "Panel request for floor 2");

            // Log grid is hidden until requested
            dgvLog.Visible = false;
            dgvLog.ReadOnly = true;
            dgvLog.AllowUserToAddRows = false;
            dgvLog.RowHeadersVisible = false;
            dgvLog.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            Control host = pnlShaftA.Parent;
            collapsedWidth = host.Right + host.Left;
            expandedWidth = dgvLog.Right + host.Left;
            ClientSize = new Size(collapsedWidth, host.Bottom + host.Top);

            RefreshHall();

            tmrElevator.Interval = 15; tmrElevator.Start();   // movement
            tmrDoor.Interval = 30; tmrDoor.Start();           // doors
        }

        // Timers
        private void tmrElevator_Tick(object sender, EventArgs e)
        {
            foreach (var u in dispatcher.Units) u.MoveTick();
        }

        private void tmrDoor_Tick(object sender, EventArgs e)
        {
            foreach (var u in dispatcher.Units) u.DoorTick();
        }

        // Buttons
        private void btnRequestFloor1_Click(object sender, EventArgs e) => dispatcher.AssignHallCall(1);
        private void btnRequestFloor2_Click(object sender, EventArgs e) => dispatcher.AssignHallCall(2);

        private void btnPanelFloor1_Click(object sender, EventArgs e) => unitA.AddStop(1, "Panel request for floor 1");
        private void btnPanelFloor2_Click(object sender, EventArgs e) => unitA.AddStop(2, "Panel request for floor 2");

        // Designer leftover; safe to delete together with its line in MainForm.Designer.cs
        private void button2_Click(object sender, EventArgs e) { }

        // Observer: hall displays shared by both elevators
        void IElevatorObserver.Update(ElevatorUnit elevator, string eventText) => RefreshHall();

        private void RefreshHall()
        {
            if (unitA == null || unitB == null) return;   // observers can fire before both units exist

            string a = HallText(unitA), b = HallText(unitB);
            lblFloor1StatusA.Text = lblFloor2StatusA.Text = a;
            lblFloor1StatusB.Text = lblFloor2StatusB.Text = b;

            // Orange while any elevator has a pending stop at that floor
            btnRequestFloor1.BackColor = dispatcher.Units.Any(u => u.IsPending(1)) ? Color.Orange : Color.Silver;
            btnRequestFloor2.BackColor = dispatcher.Units.Any(u => u.IsPending(2)) ? Color.Orange : Color.Silver;
        }

        private static string HallText(ElevatorUnit u) => $"{u.Name} {u.DirectionArrow}{u.CurrentFloor}";

        // Green-on-black display look; position and size come from the designer
        private static void StyleDisplay(Label l)
        {
            l.TextAlign = ContentAlignment.MiddleCenter;
            l.Font = new Font("Consolas", 10.5f, FontStyle.Bold);
            l.ForeColor = Color.Lime;
            l.BackColor = Color.Black;
        }

        // Database access runs off the UI thread to keep the animation smooth
        private void RunInBackground(Func<object> work, Action<object> onSuccess, Action onFinished = null)
        {
            var bw = new BackgroundWorker();

            bw.DoWork += (s, e) => e.Result = work();    // worker thread: don't touch controls

            bw.RunWorkerCompleted += (s, e) =>           // UI thread
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
            btnLog.Enabled = false;                      // blocks repeat clicks while loading
            RunInBackground(
                () => db.Load(),
                result =>
                {
                    dgvLog.DataSource = (DataTable)result;
                    if (dgvLog.Columns.Contains("LoggedAt"))
                        dgvLog.Columns["LoggedAt"].DefaultCellStyle.Format = "dd/MM/yyyy HH:mm:ss";
                    if (dgvLog.Columns.Contains("EventText")) dgvLog.Columns["EventText"].FillWeight = 300;
                    if (dgvLog.Columns.Contains("LogID")) dgvLog.Columns["LogID"].FillWeight = 40;

                    ClientSize = new Size(expandedWidth, ClientSize.Height);   // widen the form to show the grid
                    dgvLog.Visible = true;
                },
                () => btnLog.Enabled = true);
        }

        private void lblPanelDisplay_Click(object sender, EventArgs e)
        {

        }

        
    }
}