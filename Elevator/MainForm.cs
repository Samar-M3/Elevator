//MainForm.cs is a C# Windows Forms application that simulates the operation of two elevators (A and B) in a building. The code defines the MainForm class, which implements the IElevatorObserver interface to receive updates from the elevator units. The form includes various controls for displaying elevator status, requesting floors, and logging events.

using System.Collections.Concurrent;
using System.ComponentModel;
using System.Data;
using System.Drawing.Drawing2D;

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

        // Log writing: events are queued with their timestamp and written one after another
        // by a single background worker, so rows keep the order the events happened in.
        private readonly ConcurrentQueue<(int Floor, string Text, DateTime When)> logQueue = new();
        private bool logWorkerBusy;                  // UI thread only
        private bool dbErrorShown;                   // stops one pop-up per event when the DB is unavailable

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
            btnPanelFloor1B.Click -= btnPanelFloor1B_Click; btnPanelFloor1B.Click += btnPanelFloor1B_Click;
            btnPanelFloor2B.Click -= btnPanelFloor2B_Click; btnPanelFloor2B.Click += btnPanelFloor2B_Click;
            OpendoorA.Click -= OpendoorA_Click; OpendoorA.Click += OpendoorA_Click;
            ClosedoorA.Click -= ClosedoorA_Click; ClosedoorA.Click += ClosedoorA_Click;
            OpendoorB.Click -= OpendoorB_Click; OpendoorB.Click += OpendoorB_Click;
            ClosedoorB.Click -= ClosedoorB_Click; ClosedoorB.Click += ClosedoorB_Click;
            btnLog.Click -= btnLog_Click; btnLog.Click += btnLog_Click;
            deleteLog.Click -= deleteLog_Click; deleteLog.Click += deleteLog_Click;

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

            // Round floor buttons with a number instead of text
            MakeRound(btnPanelFloor1, "1");
            MakeRound(btnPanelFloor2, "2");
            MakeRound(btnPanelFloor1B, "1");
            MakeRound(btnPanelFloor2B, "2");

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

            // Log grid is hidden until requested
            dgvLog.Visible = false;
            deleteLog.Visible = false;
            dgvLog.ReadOnly = true;
            dgvLog.AllowUserToAddRows = false;
            dgvLog.AllowUserToDeleteRows = false;
            dgvLog.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvLog.MultiSelect = true;
            dgvLog.KeyDown += (s, ev) =>
            {
                if (ev.KeyCode == Keys.Delete) { deleteLog.PerformClick(); ev.Handled = true; }
            };
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

        // Hall call buttons: the dispatcher picks the elevator
        private void btnRequestFloor1_Click(object sender, EventArgs e) => dispatcher.AssignHallCall(1);
        private void btnRequestFloor2_Click(object sender, EventArgs e) => dispatcher.AssignHallCall(2);

        // Control panel A: requests go to elevator A only
        private void btnPanelFloor1_Click(object sender, EventArgs e) => unitA.AddStop(1, "Panel request for floor 1");
        private void btnPanelFloor2_Click(object sender, EventArgs e) => unitA.AddStop(2, "Panel request for floor 2");

        // Control panel B: requests go to elevator B only
        private void btnPanelFloor1B_Click(object sender, EventArgs e) => unitB.AddStop(1, "Panel request for floor 1");
        private void btnPanelFloor2B_Click(object sender, EventArgs e) => unitB.AddStop(2, "Panel request for floor 2");

        // Door buttons: A and B each control their own elevator
        private void OpendoorA_Click(object sender, EventArgs e) => unitA.OpenDoors("Open-door button pressed");
        private void ClosedoorA_Click(object sender, EventArgs e) => unitA.CloseDoors("Close-door button pressed");
        private void OpendoorB_Click(object sender, EventArgs e) => unitB.OpenDoors("Open-door button pressed");
        private void ClosedoorB_Click(object sender, EventArgs e) => unitB.CloseDoors("Close-door button pressed");

        // Designer leftovers (empty). To remove them, also delete the matching
        // "+= new EventHandler(...)" lines in MainForm.Designer.cs, or the project won't compile.
        private void button2_Click(object sender, EventArgs e) { }
        private void lblPanelDisplay_Click(object sender, EventArgs e) { }

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

        private static void MakeRound(Button b, string text, int diameter = 40)
        {
            Point centre = new Point(b.Left + b.Width / 2, b.Top + b.Height / 2);

            b.Size = new Size(diameter, diameter);
            b.Location = new Point(centre.X - diameter / 2, centre.Y - diameter / 2);

            b.Text = text;
            b.Font = new Font("Segoe UI", 12f, FontStyle.Bold);
            b.TextAlign = ContentAlignment.MiddleCenter;
            b.FlatStyle = FlatStyle.Flat;
            b.FlatAppearance.BorderSize = 0;

            using (var path = new GraphicsPath())
            {
                path.AddEllipse(0, 0, diameter, diameter);
                b.Region = new Region(path);               // clips the button to a circle
            }

            // Smooth dark ring on top of the clipped edge
            b.Paint += (s, e) =>
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                using (var pen = new Pen(Color.DimGray, 3))
                    e.Graphics.DrawEllipse(pen, 1, 1, diameter - 3, diameter - 3);
            };
        }

        // Database access runs off the UI thread to keep the animation smooth.
        // reportOnce: for background log writes, show a failure only once until a write succeeds again.
        private void RunInBackground(Func<object> work, Action<object> onSuccess,
                                     Action onFinished = null, bool reportOnce = false)
        {
            var bw = new BackgroundWorker();

            bw.DoWork += (s, e) => e.Result = work();    // worker thread: don't touch controls

            bw.RunWorkerCompleted += (s, e) =>           // UI thread
            {
                if (e.Error != null)
                {
                    if (!reportOnce) ShowDbError(e.Error);
                    else if (!dbErrorShown)
                    {
                        dbErrorShown = true;             // set first: the message box is modal
                        ShowDbError(e.Error);
                    }
                }
                else
                {
                    if (reportOnce) dbErrorShown = false;
                    onSuccess?.Invoke(e.Result);
                }

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

        // Called on the UI thread for every loggable event: the time is taken now,
        // the actual write is done in the background, in the order the events happened.
        private void WriteLog(int floor, string text)
        {
            logQueue.Enqueue((floor, text, DateTime.Now));
            if (!logWorkerBusy) DrainLogQueue();
        }

        private void DrainLogQueue()
        {
            logWorkerBusy = true;
            RunInBackground(
                () =>
                {
                    while (logQueue.TryDequeue(out var item))
                        db.AddEntry(item.Floor, item.Text, item.When);
                    return null;
                },
                null,
                () =>
                {
                    logWorkerBusy = false;
                    if (!logQueue.IsEmpty) DrainLogQueue();   // events arrived while the worker was finishing
                },
                reportOnce: true);
        }

        private void btnLog_Click(object sender, EventArgs e) => ShowLog();

        // Loads the log from the database into the grid (also used after a delete)
        private void ShowLog()
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
                    deleteLog.Visible = true;
                },
                () => btnLog.Enabled = true);
        }

        // Deletes the selected log row(s) after confirmation
        private void deleteLog_Click(object sender, EventArgs e)
        {
            if (dgvLog.SelectedRows.Count == 0)
            {
                MessageBox.Show("Select the log row(s) you want to delete first.", "Elevator Log",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var ids = dgvLog.SelectedRows.Cast<DataGridViewRow>()
                            .Select(r => Convert.ToInt32(r.Cells["LogID"].Value))
                            .ToList();

            string question = ids.Count == 1
                ? "Delete the selected log entry?"
                : $"Delete the {ids.Count} selected log entries?";

            if (MessageBox.Show(question, "Elevator Log", MessageBoxButtons.YesNo,
                                MessageBoxIcon.Question) != DialogResult.Yes)
                return;

            deleteLog.Enabled = false;
            RunInBackground(
                () => db.DeleteEntries(ids),
                result => ShowLog(),                     // reload so the grid matches the database
                () => deleteLog.Enabled = true);
        }
    }
}