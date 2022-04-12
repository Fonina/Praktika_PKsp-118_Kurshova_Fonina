using System;
using System.Data.SQLite;
using System.Data;
using System.Windows.Forms;

public class DBhelperSQLite
{
        SQLiteConnection cn;
        String dbFile;
        String ConStr;

        public bool GetStatusConnect()
        {
            if (cn == null) return false;
            if (cn.State != ConnectionState.Open) return false;

            return true;
        }

        public bool Connect(string dbFileName)
        {
            dbFile = dbFileName;
            ConStr = "Data Source=" + dbFileName + "; foreign keys=true;";
            cn = new SQLiteConnection(ConStr);
            try
            {
                cn.Open();
            }
            catch (Exception e)
            {
                MessageBox.Show("Ошибка соединения с БД! Подробности:" + e.Message.ToString());
                return false;
            }

            return true;
        }

        public bool Disconnect()
        {
            if (!GetStatusConnect()) return true;

            try
            {
                cn.Close();
            }
            catch (Exception e)
            {
                MessageBox.Show("Ошибка отключения от БД! Подробности:" + e.Message.ToString());
                return false;
            }
            return true;
        }

        public bool FillDT(String SqlCmd, DataTable dt)
        {
            if (!GetStatusConnect()) return false;

            try
            {
                SQLiteDataAdapter da = new SQLiteDataAdapter(SqlCmd, ConStr);
                da.Fill(dt);
            }
            catch (Exception e)
            {
                MessageBox.Show("Ошибка заполнения из БД! Подробности:" + e.Message.ToString());
                return false;
            }
            return true;
        }

        public bool FillDataGrid(String SqlCmd, DataGridView dg)
        {
            if (!GetStatusConnect()) return false;

            try
            {
                SQLiteDataAdapter da = new SQLiteDataAdapter(SqlCmd, ConStr);
                DataSet ds = new DataSet();
                da.Fill(ds, "tbl");
                dg.AutoGenerateColumns = true;
                dg.DataSource = ds.Tables["tbl"];
                dg.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                dg.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                dg.ReadOnly = true;

#if !DEBUG
                 dg.Columns[0].Visible = false;
#endif
            }
            catch (Exception e)
            {
                MessageBox.Show("Ошибка заполнения таблицы " + dg.Name + " из БД! Подробности:" + e.Message.ToString());
                return false;
            }
            return true;
        }

        public bool SqlCmd(String SqlCmd)
        {
            if (!GetStatusConnect()) return false;

            try
            {
                using (var transaction = cn.BeginTransaction())
                {
                    SQLiteCommand cmd = cn.CreateCommand();
                    cmd.CommandText = SqlCmd;
                    cmd.ExecuteNonQuery();

                    transaction.Commit();
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("Ошибка выполнения команды: " + SqlCmd + "! Подробности:" + e.Message.ToString());
                return false;
            }
            return true;
        }

        public String SqlGetOneData(String SqlCmd, int position = 0)
        {
            if (!GetStatusConnect()) return "";

            try
            {
                SQLiteDataAdapter da = new SQLiteDataAdapter(SqlCmd, ConStr);
                DataTable ds = new DataTable();
                da.Fill(ds);

                if (ds.Rows.Count == 0) return "";

                return ds.Rows[0][position].ToString();
            }
            catch (Exception e)
            {
                MessageBox.Show("Ошибка заполнения получения данных из БД! Подробности:" + e.Message.ToString());
                return "";
            }
        }
    }
