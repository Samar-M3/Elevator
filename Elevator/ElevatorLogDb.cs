//ElevatorLogDb.cs

using System.Data;
using System.Data.OleDb;

namespace Elevator
{
    public class ElevatorLogDb
    {
        private readonly OleDbDataAdapter adapter;
        private readonly DataTable table = new DataTable();
        private readonly object sync = new object();
        private readonly string connStr;

        public ElevatorLogDb()
        {
            // relative path: the database sits next to the .exe
            string path = Path.Combine(AppContext.BaseDirectory, "Elevator.accdb");
            connStr = $"Provider=Microsoft.ACE.OLEDB.12.0;Data Source={path};";

            adapter = new OleDbDataAdapter(
                "SELECT LogID, CurrentFloor, EventText, LoggedAt FROM ElevatorLog ORDER BY LogID",
                connStr);
            adapter.MissingSchemaAction = MissingSchemaAction.AddWithKey;
            new OleDbCommandBuilder(adapter);   // generates the INSERT and DELETE commands used by Update()
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

        // 'when' is captured on the UI thread at the moment of the event, so the
        // timestamp is correct even if the background write happens a little later.
        public void AddEntry(int floor, string text, DateTime when)
        {
            try
            {
                lock (sync)
                {
                    if (table.Columns.Count == 0) adapter.Fill(table);

                    DataRow row = table.NewRow();
                    row["CurrentFloor"] = floor;
                    row["EventText"] = text;
                    row["LoggedAt"] = when;
                    table.Rows.Add(row);

                    adapter.Update(table);
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Could not save the log entry. " + ex.Message, ex);
            }
        }

        // Disconnected model: mark rows as deleted in the DataTable, then DataAdapter.Update()
        // sends the DELETE commands to the database (no ExecuteNonQuery).
        public int DeleteEntries(IEnumerable<int> logIds)
        {
            try
            {
                lock (sync)
                {
                    table.Clear();
                    adapter.Fill(table);                       // make sure the rows are current

                    int deleted = 0;
                    foreach (int id in logIds)
                    {
                        DataRow row = table.Rows.Find(id);     // LogID is the primary key (AddWithKey)
                        if (row != null) { row.Delete(); deleted++; }
                    }

                    adapter.Update(table);
                    return deleted;
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Could not delete the log entry. " + ex.Message, ex);
            }
        }
    }
}