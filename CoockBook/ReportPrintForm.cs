using System;
using System.Data;
using System.Windows.Forms;
using Microsoft.Reporting.WinForms;

namespace  CoockBook
{
    public partial class ReportPrintForm : Form
    {
        DBhelperSQLite db;
        String id;

        public ReportPrintForm(DBhelperSQLite db, String id)
        {
            InitializeComponent();
            this.db = db;
            this.id = id;
        }

        private void ReportPrint_Load(object sender, EventArgs e)
        {
            DataTable dt=new DataTable();
            this.Text = "Рецепт" + db.SqlGetOneData("select наименование from Рецепт where код=" + id);
            rvReport.Reset();
            //подключим шаблон отчета в xml
            this.rvReport.LocalReport.ReportEmbeddedResource = "CoockBook.Report.rdlc";

            db.FillDT("select * from Рецепт where код=" + id, dt);

            rvReport.LocalReport.DataSources.Clear();
            rvReport.LocalReport.DataSources.Add(new ReportDataSource("DataSet1", dt));         
            //обновляем его
            this.rvReport.RefreshReport();
        }
    }
}
