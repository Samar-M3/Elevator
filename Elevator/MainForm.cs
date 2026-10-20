namespace Elevator
{
    public partial class MainForm : Form
    // TODO: constant for the floor 1 Y value (287)
    // TODO: constant for the floor 2 Y value (26)
    
    {
        private const int FLOOR1_Y = 287;
        private const int FLOOR2_Y = 26;

        private int currentFloor = 1;
        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            car.Top = FLOOR1_Y;
            UpdateDisplays();
        }

        private void UpdateDisplays()
        {
            lblPanelDisplay.Text = currentFloor.ToString();
            lblFloor1Status.Text = (currentFloor == 1) ? "Elevator here" : "";
            lblFloor2Status.Text = (currentFloor == 2) ? "Elevator here" : "";
        }

        // All four floor buttons share one method (no duplication)
        private const int STEP = 4;                 // pixels per tick
        private int targetY;
        private bool isMoving = false;

        private delegate void ArrivedHandler();     // delegate type
        private ArrivedHandler onArrived;           // called when the car arrives

        private void MoveToFloor(int floor)
        {
            if (isMoving || floor == currentFloor) return;   // ignore clicks while moving

            isMoving = true;
            targetY = (floor == 1) ? FLOOR1_Y : FLOOR2_Y;

            // The delegate holds what should happen on arrival
            onArrived = delegate ()
            {
                currentFloor = floor;
                UpdateDisplays();
            };

            tmrElevator.Interval = 15;   // ms
            tmrElevator.Start();
        }

        private void tmrElevator_Tick(object sender, EventArgs e)
        {
            if (car.Top < targetY)
                car.Top = Math.Min(car.Top + STEP, targetY);
            else
                car.Top = Math.Max(car.Top - STEP, targetY);

            if (car.Top == targetY)
            {
                tmrElevator.Stop();
                isMoving = false;
                onArrived?.Invoke();    // run the delegate
            }
        }


        private void btnRequestFloor1_Click(object sender, EventArgs e) => MoveToFloor(1);
        private void btnRequestFloor2_Click(object sender, EventArgs e) => MoveToFloor(2);
        private void btnPanelFloor1_Click(object sender, EventArgs e) => MoveToFloor(1);
        private void btnPanelFloor2_Click(object sender, EventArgs e) => MoveToFloor(2);

        private void button2_Click(object sender, EventArgs e)
        {

        }



        private void btnLog_Click(object sender, EventArgs e)
        {

        }
    }
}
