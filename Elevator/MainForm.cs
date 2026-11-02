// MainForm.cs
// Purpose: Main window of the Elevator Control System (Windows Forms). It creates the two
//          elevators (A and B) and their views, handles every button click, runs the two
//          timers that drive the animation, shows the hall displays, and manages the
//          database log (write, show, delete) on a BackgroundWorker.
// Author:  Samar Maharjan | University ID: 2439489 | Student ID: 2510412186

using System.Collections.Concurrent;
using System.ComponentModel;
using System.Data;
using System.Drawing.Drawing2D;

namespace Elevator
{
    // Expected control names:
    //  Elevator A: pnlShaftA, pnlCarA, pnlCableA, lblCarIndicatorA, lblPanelDisplay,
    //              btnPanelFloor1, btnPanelFloor2, OpendoorA, ClosedoorA
    //  Elevator B: pnlShaftB, pnlCarB, pnlCableB, lblCarIndicatorB, lblPanelDisplayB,
    //              btnPanelFloor1B, btnPanelFloor2B, OpendoorB, ClosedoorB
    //  Call buttons (one per elevator per floor): btnRequestFloor1A, btnRequestFloor2A,
    //              btnRequestFloor1B, btnRequestFloor2B
    //  Shared:     btnLog, deleteLog, dgvLog, tmrElevator, tmrDoor
    //  Hall displays (one per elevator per floor): lblFloor1StatusA, lblFloor1StatusB,
    //              lblFloor2StatusA, lblFloor2StatusB

    /// <summary>
    /// Main form. It is also an observer (IElevatorObserver) so that it can refresh the
    /// hall displays and call-button colours whenever either elevator changes.
    /// </summary>
    public partial class MainForm : Form, IElevatorObserver
    {
        private const int FLOOR2_Y = 26;             // car top position at floor 2

        private readonly ElevatorLogDb db = new ElevatorLogDb();
        private readonly ElevatorDispatcher dispatcher = new ElevatorDispatcher();   // holds both units; AssignHallCall is not used by the call buttons yet

        private ElevatorUnit unitA, unitB;
        private ElevatorView viewA, viewB;
        private int floor1Y;                         // car A's designer position
        private int collapsedWidth, expandedWidth;   // form width without / with the log grid

        // Log writing: events are queued with their timestamp and written one after another
        // by a single background worker, so rows keep the order the events happened in.
        private readonly ConcurrentQueue<(int Floor, string Text, DateTime When)> logQueue = new();
        private bool logWorkerBusy;                  // UI thread only
        private bool dbErrorShown;                   // stops one pop-up per event when the DB is unavailable

        // Pixel position (car top) for a floor. Both cars share these positions, and this
        // method is passed to each ElevatorUnit as its FloorToY delegate.
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
            btnRequestFloor1A.Click -= btnRequestFloor1A_Click; btnRequestFloor1A.Click += btnRequestFloor1A_Click;
            btnRequestFloor2A.Click -= btnRequestFloor2A_Click; btnRequestFloor2A.Click += btnRequestFloor2A_Click;
            btnRequestFloor1B.Click -= btnRequestFloor1B_Click; btnRequestFloor1B.Click += btnRequestFloor1B_Click;
            btnRequestFloor2B.Click -= btnRequestFloor2B_Click; btnRequestFloor2B.Click += btnRequestFloor2B_Click;
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

            // Flat style so the Silver/Orange BackColor renders the same on every button
            foreach (Button b in new[] { btnRequestFloor1A, btnRequestFloor2A,
                                         btnRequestFloor1B, btnRequestFloor2B,
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

            btnRequestFloor1A.Text = "\u25B2 Call";       // floor 1 can only go up
            btnRequestFloor2A.Text = "\u25BC Call";       // floor 2 can only go down
            btnRequestFloor1B.Text = "\u25B2 Call";       // floor 1 can only go up
            btnRequestFloor2B.Text = "\u25BC Call";       // floor 2 can only go down

            foreach (Label l in new[] { lblFloor1StatusA, lblFloor1StatusB,
                                        lblFloor2StatusA, lblFloor2StatusB,
                                        lblPanelDisplay, lblPanelDisplayB })
                StyleDisplay(l);

            floor1Y = pnlCarA.Top;                   // floor 1 position = where the car was placed in the designer
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

            // Observers: database logger and this form's hall displays.
            // (Each ElevatorView attaches itself in its own constructor.)
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
                // The Delete key does the same as the Delete Log button
                if (ev.KeyCode == Keys.Delete) { deleteLog.PerformClick(); ev.Handled = true; }
            };
            dgvLog.RowHeadersVisible = false;
            dgvLog.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            // host = the panel that holds the two elevators. The form is as wide as that panel
            // plus the same left margin again on the right. With the grid shown, the width
            // runs to the grid's right edge plus that margin.
            Control host = pnlShaftA.Parent;
            collapsedWidth = host.Right + host.Left;
            expandedWidth = dgvLog.Right + host.Left;
            ClientSize = new Size(collapsedWidth, host.Bottom + host.Top);

            RefreshHall();   // first draw of the hall displays and call-button colours

            // These intervals must match MOVE_TICK_SEC and DOOR_TICK_SEC in ElevatorSystem.cs,
            // because EstimateSeconds() calculates its times from those constants.
            tmrElevator.Interval = 15; tmrElevator.Start();   // movement (15 ms)
            tmrDoor.Interval = 30; tmrDoor.Start();           // doors (30 ms)
        }

        // Timers: each tick is passed to every elevator, and its current state decides what to do
        private void tmrElevator_Tick(object sender, EventArgs e)
        {
            foreach (var u in dispatcher.Units) u.MoveTick();
        }

        private void tmrDoor_Tick(object sender, EventArgs e)
        {
            foreach (var u in dispatcher.Units) u.DoorTick();
        }

        // Call buttons: each button calls its own elevator only (A's buttons -> A, B's buttons -> B).
        // The dispatcher is not used here, so the nearest elevator is not chosen automatically.
        private void btnRequestFloor1A_Click(object sender, EventArgs e) => unitA.AddStop(1, "Call button pressed at floor 1");
        private void btnRequestFloor2A_Click(object sender, EventArgs e) => unitA.AddStop(2, "Call button pressed at floor 2");
        private void btnRequestFloor1B_Click(object sender, EventArgs e) => unitB.AddStop(1, "Call button pressed at floor 1");
        private void btnRequestFloor2B_Click(object sender, EventArgs e) => unitB.AddStop(2, "Call button pressed at floor 2");

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

        // Empty handlers left over from the Designer. They do nothing. To remove them, also
        // delete the matching "+= new EventHandler(...)" lines in MainForm.Designer.cs,
        // or the project won't compile.
        private void button2_Click(object sender, EventArgs e) { }
        private void lblPanelDisplay_Click(object sender, EventArgs e) { }

        // Observer: hall displays shared by both elevators.
        // Explicit interface implementation, so Update() is not part of MainForm's public API.
        void IElevatorObserver.Update(ElevatorUnit elevator, string eventText) => RefreshHall();

        // Refreshes the hall displays (name, direction arrow, floor) and the call-button colours
        private void RefreshHall()
        {
            // Safety check: do nothing until both units exist
            if (unitA == null || unitB == null) return;

            string a = HallText(unitA), b = HallText(unitB);
            lblFloor1StatusA.Text = lblFloor2StatusA.Text = a;
            lblFloor1StatusB.Text = lblFloor2StatusB.Text = b;

            // Each call button turns orange while its own elevator has a pending stop at that floor
            btnRequestFloor1A.BackColor = unitA.IsPending(1) ? Color.Orange : Color.Silver;
            btnRequestFloor2A.BackColor = unitA.IsPending(2) ? Color.Orange : Color.Silver;
            btnRequestFloor1B.BackColor = unitB.IsPending(1) ? Color.Orange : Color.Silver;
            btnRequestFloor2B.BackColor = unitB.IsPending(2) ? Color.Orange : Color.Silver;
        }

        // Text for a hall display, e.g. "A 2" or "B ▲1" (name, direction arrow, floor)
        private static string HallText(ElevatorUnit u) => $"{u.Name} {u.DirectionArrow}{u.CurrentFloor}";

        // Green-on-black display look; position and size come from the designer
        private static void StyleDisplay(Label l)
        {
            l.TextAlign = ContentAlignment.MiddleCenter;
            l.Font = new Font("Consolas", 10.5f, FontStyle.Bold);
            l.ForeColor = Color.Lime;
            l.BackColor = Color.Black;
        }

        // Turns a button into a circle of the given diameter, keeping its centre where the
        // designer put it, and shows the given text on it
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

        /// <summary>
        /// Runs database work on a BackgroundWorker so the animation never freezes.
        /// <paramref name="work"/> runs on the worker thread, and <paramref name="onSuccess"/>
        /// and <paramref name="onFinished"/> run back on the UI thread.
        /// </summary>
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
                    if (reportOnce) dbErrorShown = false;   // a write worked again, so allow a future error message
                    onSuccess?.Invoke(e.Result);
                }

                onFinished?.Invoke();
                bw.Dispose();
            };

            bw.RunWorkerAsync();
        }

        // Shows a database failure as a message box. The program keeps running.
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

        // Starts one background worker that writes every queued event, oldest first.
        // Only one worker runs at a time (logWorkerBusy), which keeps the rows in order.
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
                        dgvLog.Columns["LoggedAt"].DefaultCellStyle.Format = "dd/MM/yyyy HH:mm:ss";   // full date and time
                    if (dgvLog.Columns.Contains("EventText")) dgvLog.Columns["EventText"].FillWeight = 300;   // widest column
                    if (dgvLog.Columns.Contains("LogID")) dgvLog.Columns["LogID"].FillWeight = 40;            // narrowest column

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

            // LogID values of the selected rows (LogID is the table's primary key)
            var ids = dgvLog.SelectedRows.Cast<DataGridViewRow>()
                            .Select(r => Convert.ToInt32(r.Cells["LogID"].Value))
                            .ToList();

            string question = ids.Count == 1
                ? "Delete the selected log entry?"
                : $"Delete the {ids.Count} selected log entries?";

            if (MessageBox.Show(question, "Elevator Log", MessageBoxButtons.YesNo,
                                MessageBoxIcon.Question) != DialogResult.Yes)
                return;

            deleteLog.Enabled = false;                   // blocks repeat clicks while deleting
            RunInBackground(
                () => db.DeleteEntries(ids),
                result => ShowLog(),                     // reload so the grid matches the database
                () => deleteLog.Enabled = true);
        }
    }
}