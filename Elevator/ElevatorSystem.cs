using System;
using System.Collections.Generic;
using System.Linq;

namespace Elevator
{
    public interface IElevatorObserver
    {
        void Update(ElevatorUnit elevator, string eventText);
    }

    // Writes loggable events to the database through a delegate
    public class LogObserver : IElevatorObserver
    {
        private readonly Action<int, string> write;
        public LogObserver(Action<int, string> write) { this.write = write; }

        public void Update(ElevatorUnit e, string eventText)
        {
            if (eventText != null) write(e.CurrentFloor, $"Elevator {e.Name}: {eventText}");
        }
    }

    // State pattern: each state decides how the elevator reacts to ticks and requests
    public interface IElevatorState
    {
        string Name { get; }
        void Enter(ElevatorUnit e);
        void MoveTick(ElevatorUnit e);                  // movement timer
        void DoorTick(ElevatorUnit e);                  // door timer
        void Request(ElevatorUnit e, int floor);
    }

    public abstract class ElevatorStateBase : IElevatorState
    {
        public abstract string Name { get; }
        public virtual void Enter(ElevatorUnit e) { }
        public virtual void MoveTick(ElevatorUnit e) { }
        public virtual void DoorTick(ElevatorUnit e) { }
        public virtual void Request(ElevatorUnit e, int floor) => e.Enqueue(floor);
    }

    public class IdleState : ElevatorStateBase
    {
        public override string Name => "Idle";

        public override void Request(ElevatorUnit e, int floor)
        {
            if (floor == e.CurrentFloor) e.SetState(new DoorsOpeningState());   // already here: just open
            else { e.Enqueue(floor); e.StartNextOrIdle(); }                     // go there
        }
    }

    public class MovingState : ElevatorStateBase
    {
        public override string Name => "Moving";

        public override void Request(ElevatorUnit e, int floor)
        {
            if (floor != e.TargetFloor) e.Enqueue(floor);       // never disturb the current trip
        }

        public override void MoveTick(ElevatorUnit e)
        {
            int targetY = e.FloorToY(e.TargetFloor);
            if (e.Y < targetY) e.Y = Math.Min(e.Y + ElevatorUnit.STEP, targetY);
            else e.Y = Math.Max(e.Y - ElevatorUnit.STEP, targetY);
            e.Notify(null);

            if (e.Y == targetY)
            {
                e.CurrentFloor = e.TargetFloor;
                e.Notify($"Arrived at floor {e.CurrentFloor}");
                e.SetState(new DoorsOpeningState());
            }
        }
    }

    public class DoorsOpeningState : ElevatorStateBase
    {
        public override string Name => "Doors opening";

        public override void Request(ElevatorUnit e, int floor)
        {
            if (floor != e.CurrentFloor) e.Enqueue(floor);
        }

        public override void DoorTick(ElevatorUnit e)
        {
            e.DoorOpen = Math.Min(e.DoorOpen + ElevatorUnit.DOOR_STEP, e.DoorMax);
            e.Notify(null);
            if (e.DoorOpen >= e.DoorMax)
            {
                e.Notify($"Doors opened at floor {e.CurrentFloor}");
                e.SetState(new DoorsOpenState());
            }
        }
    }

    public class DoorsOpenState : ElevatorStateBase
    {
        public override string Name => "Doors open";

        public override void Enter(ElevatorUnit e) => e.HoldTicks = ElevatorUnit.HOLD_TICKS;

        public override void Request(ElevatorUnit e, int floor)
        {
            if (floor == e.CurrentFloor) e.HoldTicks = ElevatorUnit.HOLD_TICKS;   // keep doors open longer
            else e.Enqueue(floor);
        }

        public override void DoorTick(ElevatorUnit e)
        {
            if (--e.HoldTicks <= 0) e.SetState(new DoorsClosingState());
        }
    }

    public class DoorsClosingState : ElevatorStateBase
    {
        public override string Name => "Doors closing";

        public override void Request(ElevatorUnit e, int floor)
        {
            if (floor == e.CurrentFloor) e.SetState(new DoorsOpeningState());    // reopen, like a real door sensor
            else e.Enqueue(floor);
        }

        public override void DoorTick(ElevatorUnit e)
        {
            e.DoorOpen = Math.Max(e.DoorOpen - ElevatorUnit.DOOR_STEP, 0);
            e.Notify(null);
            if (e.DoorOpen == 0)
            {
                e.Notify($"Doors closed at floor {e.CurrentFloor}");
                e.StartNextOrIdle();
            }
        }
    }

    // One elevator: the context of the State pattern and the subject of the Observer pattern
    public class ElevatorUnit
    {
        public const int STEP = 4;                 // pixels per movement tick
        public const int DOOR_STEP = 3;            // pixels per door tick
        public const int HOLD_TICKS = 70;          // door ticks the doors stay open
        public const double MOVE_TICK_SEC = 0.015; // movement timer interval (15 ms)
        public const double DOOR_TICK_SEC = 0.030; // door timer interval (30 ms)

        public int Id { get; }
        public string Name { get; }
        public int DoorMax { get; }                // fully open = half the car width
        public Func<int, int> FloorToY { get; }    // floor number -> pixel position

        public int CurrentFloor { get; set; } = 1; // last floor reached (departure floor while moving)
        public int TargetFloor { get; set; } = 1;
        public int Y { get; set; }
        public int DoorOpen { get; set; }
        public int HoldTicks { get; set; }
        public IElevatorState State { get; private set; } = new IdleState();

        private readonly List<int> stops = new List<int>();            // pending stops, in order
        private readonly List<IElevatorObserver> observers = new List<IElevatorObserver>();

        public ElevatorUnit(int id, string name, int doorMax, Func<int, int> floorToY)
        {
            Id = id; Name = name; DoorMax = doorMax; FloorToY = floorToY;
            Y = floorToY(1);
        }

        public void Attach(IElevatorObserver o) => observers.Add(o);
        public void Detach(IElevatorObserver o) => observers.Remove(o);
        public void Notify(string eventText)
        {
            foreach (var o in observers.ToList()) o.Update(this, eventText);   // copy: observers may change during notify
        }

        public void SetState(IElevatorState s)
        {
            State = s;
            s.Enter(this);
            Notify(null);
        }

        public void MoveTick() => State.MoveTick(this);
        public void DoorTick() => State.DoorTick(this);

        public void AddStop(int floor, string logText = null)
        {
            if (logText != null) Notify(logText);
            State.Request(this, floor);
            Notify(null);                          // refresh the button lights
        }

        public void Enqueue(int floor)
        {
            if (!stops.Contains(floor)) stops.Add(floor);
        }

        public void StartNextOrIdle()
        {
            while (stops.Count > 0 && stops[0] == CurrentFloor) stops.RemoveAt(0);   // drop stops for the current floor
            if (stops.Count == 0) { SetState(new IdleState()); return; }

            TargetFloor = stops[0];
            stops.RemoveAt(0);
            SetState(new MovingState());
        }

        public int Direction => State is MovingState ? Math.Sign(TargetFloor - CurrentFloor) : 0;
        public string DirectionArrow => Direction > 0 ? "\u25B2" : Direction < 0 ? "\u25BC" : "";
        public string StatusText => State is MovingState ? (Direction > 0 ? "Going up" : "Going down") : State.Name;
        public int PendingCount => stops.Count + (State is MovingState ? 1 : 0);

        // queued, or the current destination
        public bool IsPending(int floor) =>
            stops.Contains(floor) || (State is MovingState && TargetFloor == floor);

        public bool IsServing(int floor) =>
            IsPending(floor) ||
            (CurrentFloor == floor && !(State is IdleState) && !(State is MovingState));

        // Time estimates in seconds, derived from the timer intervals
        private double Travel(int fromFloor, int toFloor) =>
            Math.Abs(FloorToY(fromFloor) - FloorToY(toFloor)) / (double)STEP * MOVE_TICK_SEC;
        private double CloseFrom(int open) => open / (double)DOOR_STEP * DOOR_TICK_SEC;
        private double FullDoorCycle =>
            DoorMax / (double)DOOR_STEP * DOOR_TICK_SEC + HOLD_TICKS * DOOR_TICK_SEC + CloseFrom(DoorMax);

        public double EstimateSeconds(int floor)
        {
            double t = 0;
            int pos = CurrentFloor;                // where it will be when free to depart

            if (State is MovingState)
            {
                pos = TargetFloor;
                t = Math.Abs(FloorToY(TargetFloor) - Y) / (double)STEP * MOVE_TICK_SEC;
                if (floor == pos) return t;
                t += FullDoorCycle;
            }
            else if (State is DoorsOpeningState)
            {
                if (floor == CurrentFloor) return 0;
                t = (DoorMax - DoorOpen) / (double)DOOR_STEP * DOOR_TICK_SEC
                    + HOLD_TICKS * DOOR_TICK_SEC + CloseFrom(DoorMax);
            }
            else if (State is DoorsOpenState)
            {
                if (floor == CurrentFloor) return 0;
                t = HoldTicks * DOOR_TICK_SEC + CloseFrom(DoorMax);
            }
            else if (State is DoorsClosingState)
            {
                if (floor == CurrentFloor) return 0;
                t = CloseFrom(DoorOpen);
            }

            foreach (int s in stops)               // walk the queue in order
            {
                t += Travel(pos, s);
                pos = s;
                if (s == floor) return t;
                t += FullDoorCycle;
            }
            return t + Travel(pos, floor);         // new request goes to the end of the queue
        }
    }

    // Decides which elevator answers a call from outside
    public class ElevatorDispatcher
    {
        private readonly List<ElevatorUnit> units = new List<ElevatorUnit>();
        public IReadOnlyList<ElevatorUnit> Units => units;

        public void Add(ElevatorUnit u) => units.Add(u);

        public ElevatorUnit AssignHallCall(int floor)
        {
            // Already being handled: don't send a second elevator
            ElevatorUnit already = units.FirstOrDefault(u => u.IsServing(floor));
            if (already != null)
            {
                already.AddStop(floor);
                return already;
            }

            // Otherwise shortest estimated time (ties: fewest pending stops, then lowest id)
            ElevatorUnit best = units
                .OrderBy(u => u.EstimateSeconds(floor))
                .ThenBy(u => u.PendingCount)
                .ThenBy(u => u.Id)
                .First();

            double eta = best.EstimateSeconds(floor);
            best.AddStop(floor, $"Hall call at floor {floor} assigned here (ETA {eta:0.0} s)");
            return best;
        }
    }
}