using System.Reflection;

namespace Elevator
{
    // Drives the controls of one elevator. As an observer, it refreshes them
    // whenever the ElevatorUnit changes. Built from Panels and Labels only.
    public class ElevatorView : IElevatorObserver
    {
        public ElevatorUnit Unit { get; }

        private readonly Panel shaft;            // dark panel the car moves in
        private readonly Panel car;              // the car
        private readonly Panel cable;            // thin panel, its height follows the car
        private readonly Label display;          // display on the car control panel
        private readonly Label carIndicator;     // floor indicator inside the car
        private readonly Button btnFloor1, btnFloor2;
        private readonly Func<int, int> floorToY;

        private Panel doorL, doorR;                              // car doors
        private readonly Panel[] hallDoorL = new Panel[3];       // shaft doors, indexed by floor (1, 2)
        private readonly Panel[] hallDoorR = new Panel[3];

        public ElevatorView(ElevatorUnit unit, Panel shaft, Panel car, Panel cable,
                            Label display, Label carIndicator,
                            Button btnFloor1, Button btnFloor2)
        {
            Unit = unit;
            this.shaft = shaft;
            this.car = car;
            this.cable = cable;
            this.display = display;
            this.carIndicator = carIndicator;
            this.btnFloor1 = btnFloor1;
            this.btnFloor2 = btnFloor2;
            floorToY = unit.FloorToY;

            // DoubleBuffered is protected, so set it by reflection to reduce flicker
            foreach (Control c in new Control[] { shaft, car })
                typeof(Control).GetProperty("DoubleBuffered",
                    BindingFlags.Instance | BindingFlags.NonPublic)?.SetValue(c, true);

            cable.Left = car.Left + car.Width / 2 - cable.Width / 2;
            cable.Top = 0;

            BuildDoors();

            car.Top = unit.Y;
            cable.Height = car.Top;
            cable.SendToBack();

            unit.Attach(this);
            Update(unit, null);
        }

        public void Update(ElevatorUnit e, string eventText)
        {
            if (car.Top != e.Y)
            {
                car.Top = e.Y;
                cable.Height = car.Top;
            }
            ApplyDoors();

            string indicator = e.DirectionArrow + e.CurrentFloor;
            display.Text = indicator;
            carIndicator.Text = indicator;

            btnFloor1.BackColor = e.IsPending(1) ? Color.Orange : Color.Silver;
            btnFloor2.BackColor = e.IsPending(2) ? Color.Orange : Color.Silver;
        }

        // A door leaf is a Panel whose width changes
        private static Panel MakeDoor(Control parent, Color color)
        {
            var p = new Panel { BackColor = color, BorderStyle = BorderStyle.FixedSingle };
            parent.Controls.Add(p);
            p.BringToFront();
            return p;
        }

        private void BuildDoors()
        {
            doorL = MakeDoor(car, Color.Silver);
            doorR = MakeDoor(car, Color.Silver);

            for (int f = 1; f <= 2; f++)
            {
                hallDoorL[f] = MakeDoor(shaft, Color.SlateGray);
                hallDoorR[f] = MakeDoor(shaft, Color.SlateGray);
            }
            ApplyDoors();
        }

        private static void SetLeaves(Panel left, Panel right, int x, int y, int totalW, int h, int leafW)
        {
            left.SetBounds(x, y, leafW, h);
            right.SetBounds(x + totalW - leafW, y, leafW, h);
            left.Visible = right.Visible = leafW > 0;
        }

        private void ApplyDoors()
        {
            int half = (car.Width + 1) / 2;
            int leaf = half - Unit.DoorOpen;

            SetLeaves(doorL, doorR, 0, 0, car.Width, car.Height, leaf);

            for (int f = 1; f <= 2; f++)
            {
                int w = (f == Unit.CurrentFloor) ? leaf : half;     // only the car's floor opens
                SetLeaves(hallDoorL[f], hallDoorR[f], car.Left, floorToY(f), car.Width, car.Height, w);
            }
        }
    }
}