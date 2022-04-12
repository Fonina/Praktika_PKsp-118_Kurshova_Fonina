using System;
using System.Drawing;
using System.Windows.Forms;
using System.IO;

namespace CoockBook
{
    public partial class Form1 : Form
    {
        DBhelperSQLite db;
        bool newRecipe = false;

        public Form1()
        {
            InitializeComponent();
            db = new DBhelperSQLite();
            if(!db.Connect(Directory.GetCurrentDirectory() + "\\db.sqlite3"))
                Application.Exit();
        }

        bool checkDgvSelected(DataGridView dgv, int position, string messErr = "", bool errShow = false)
        {
            try
            {
                if (dgv.SelectedRows.Count == 0)
                {
                    if (errShow) MessageBox.Show(messErr);
                    return false;
                }

                if (dgv[0, dgv.SelectedRows[0].Index].Value.ToString() == "")
                {
                    if (errShow) MessageBox.Show(messErr);
                    return false;
                }
            }
            catch (Exception)
            {
                if (errShow) MessageBox.Show(messErr + "\n Ошибка проверки ключа в " + dgv.Name);
                return false;
            }

            return true;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            db.FillDataGrid("select *from 'Рецепт'", dgMain);
            if (dgMain.Rows.Count != 0)
            {
                dgMain.Rows[0].Selected = true;
                dgMain_CellClick(null, null);
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            tbFind.Text = "";
        }

        private void tbFind_TextChanged(object sender, EventArgs e)
        {
            string str="like '%" + tbFind.Text + "%'";
            db.FillDataGrid("select *from 'Рецепт' where ингридиенты " + str + " or наименование "+str, dgMain);
        }

        private void dgMain_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            tbName.Text = tbIngridients.Text = tbInstructions.Text = "";

             if (!checkDgvSelected(dgMain,0)) return;

             tbName.Text = db.SqlGetOneData("select наименование from Рецепт where код=" + dgMain[0, dgMain.SelectedRows[0].Index].Value.ToString());
             tbIngridients.Text = db.SqlGetOneData("select ингридиенты from Рецепт where код=" + dgMain[0, dgMain.SelectedRows[0].Index].Value.ToString());
             tbInstructions.Text = db.SqlGetOneData("select инструкция from Рецепт where код=" + dgMain[0, dgMain.SelectedRows[0].Index].Value.ToString());
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (!newRecipe)
            {
                if (!checkDgvSelected(dgMain, 0, "Рецепт не выбран!", true)) return;

                if (db.SqlCmd("update Рецепт set наименование='" + tbName.Text + "', ингридиенты='" + tbIngridients.Text + "', инструкция='" +
                    tbInstructions.Text + "' where код=" + dgMain[0, dgMain.SelectedRows[0].Index].Value.ToString()))
                    tbFind_TextChanged(null, null);
            }
            else
            {
                if (db.SqlCmd("insert into Рецепт('наименование','ингридиенты','инструкция')  values('" + tbName.Text + "','"
                    + tbIngridients.Text + "','" + tbInstructions.Text + "');"))
                {
                    tbFind_TextChanged(null, null);

                    if (dgMain.Rows.Count != 0)
                    {
                        btnCreate_Click(null, null);
                        dgMain.Rows[dgMain.Rows.Count-1].Selected = true;
                        dgMain_CellClick(null, null);
                    }
                }
            }
        }

        private void btnCreate_Click(object sender, EventArgs e)
        {
            tbName.Text = tbIngridients.Text = tbInstructions.Text = "";
            if (!newRecipe)
            {
                btnCreate.Text = "Отмена";
                btnCreate.BackColor = Color.Red;
                newRecipe = true;
                btnDel.Enabled = false;
            }
            else
            {
                btnCreate.Text = "Новый";
                btnCreate.BackColor = SystemColors.Control;
                newRecipe = false;
                btnDel.Enabled = true;
            }

        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            db.Disconnect();
            e.Cancel = false;
        }

        private void btnDel_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Удалить рецепт " + tbName.Text + "?",
                          "Подтвердите действие", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                if (!checkDgvSelected(dgMain, 0, "Выберите рецепт!", true)) return;
                db.SqlCmd("delete from Рецепт where код=" + dgMain[0, dgMain.SelectedRows[0].Index].Value.ToString());
            }
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            if (!checkDgvSelected(dgMain, 0, "Рецепт не выбран!", true)) return;

            ReportPrintForm rp = new ReportPrintForm(db, dgMain[0, dgMain.SelectedRows[0].Index].Value.ToString());
            rp.ShowDialog(this);
        }
    }
}
