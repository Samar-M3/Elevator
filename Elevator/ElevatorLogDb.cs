using System.Data;
using System.Data.OleDb;

namespace Elevator
{
    public class ElevatorLogDb
    {
        private readonly OleDbDataAdapter adapter;
        private readonly DataTable table = new DataTable();
        private readonly object sync = new object();

        public ElevatorLogDb()
        {
            // relative path: the database sits next to the .exe
            string path = Path.Combine(AppContext.BaseDirectory, "Elevator.accdb");
            string connStr = $"Provider=Microsoft.ACE.OLEDB.12.0;Data Source={path};";

            adapter = new OleDbDataAdapter(
                "SELECT LogID, CurrentFloor, EventText, LoggedAt FROM ElevatorLog ORDER BY LogID",
                connStr);
            adapter.MissingSchemaAction = MissingSchemaAction.AddWithKey;
            new OleDbCommandBuilder(adapter);
        }

        public DataTable Load()
        {
            try
            {
                lock (sync)
                {
                    table.Clear();
                    adapter.Fill(table);
                    return table.Copy();
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Could not load the elevator log. " + ex.Message, ex);
            }
        }

        public void AddEntry(int floor, string text)
        {
            try
            {
                lock (sync)
                {
                    if (table.Columns.Count == 0) adapter.Fill(table);

                    DataRow row = table.NewRow();
                    row["CurrentFloor"] = floor;
                    row["EventText"] = text;
                    row["LoggedAt"] = DateTime.Now;
                    table.Rows.Add(row);

                    adapter.Update(table);
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Could not save the log entry. " + ex.Message, ex);
            }
        }
    }
}