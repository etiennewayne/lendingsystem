﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using MySql.Data.MySqlClient;


namespace LendingSystem
{
    public partial class ApplyLoanAddEdit : Form
    {

        MySqlConnection con;
        MySqlCommand cmd;
        string query;


        public long id;
        public long member_id;



        public ApplyLoanAddEdit()
        {
            InitializeComponent();

        }

        private void btnBrowseMember_Click(object sender, EventArgs e)
        {
            ApplyLoanBrowseMember frm = new ApplyLoanBrowseMember(this);
            frm.ShowDialog();
        }




        void loadDailyToFlxGrid(decimal amtToPay)
        {
            flx.Rows.Count = flx.Rows.Fixed;
            for(int row = 1; row <= numDayMonth.Value; row++)
            {
                flx.Rows.Add();
                var addedDateTime = DateTime.Now.AddDays(row);
                flx[row, "date_month"] = addedDateTime;
                flx[row, "amountToPay"] = amtToPay;
            }
           
        }

        void loadMonthlyToFlxGrid(decimal amtToPay)
        {
            flx.Rows.Count = flx.Rows.Fixed;
            for (int row = 1; row <= numDayMonth.Value; row++)
            {
                flx.Rows.Add();
                var addedDateTime = DateTime.Now.AddMonths(row);
                flx[row, "date_month"] = addedDateTime;
                flx[row, "amountToPay"] = amtToPay;
            }
        }

        private void btnCompute_Click(object sender, EventArgs e)
        {
            //compute
            try
            {
                if (String.IsNullOrEmpty(this.cmbLoanType.Text))
                {
                    Box.WarnBox("Please select loan type.");
                    return;
                }

                if (numInterest.Value < 1)
                {
                    Box.WarnBox("Please input interest.");
                    return;
                }

                if (numDayMonth.Value < 1)
                {
                    Box.WarnBox("Please input no of day or month.");
                    return;
                }


                decimal months_days, roi, amt, interest, ins;
                amt = numAmountToLoan.Value;
                interest = numInterest.Value;
                months_days = numDayMonth.Value;
                roi = amt / interest;
                ins = amt / months_days + roi;

                lblAmountToPay.Text = String.Format("{0:n}", ins);

                if (this.cmbLoanType.Text == "DAILY")
                {
                    loadDailyToFlxGrid(ins);
                }

                if (this.cmbLoanType.Text == "MONTHLY")
                {
                    loadMonthlyToFlxGrid(ins);
                }

            }
            catch (Exception er)
            {
                Box.ErrBox(er.Message);
                //throw;
            }
        }

        private void btnApply_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(txtLoanTitle.Text))
            {
                Box.WarnBox("Please input loan title.");
                return;
            }

            if(member_id < 1)
            {
                Box.WarnBox("Please select a member.");
                return;
            }

            save();

            Box.InfoBox("Loan successfully saved.");

        }


        void save()
        {
            //here na nako gebutang.. getapulan nako buhat class hahaha
            con = Connection.con();
            con.Open();
            query = "INSERT INTO loans SET member_id=?mid, loan_title = ?ltitle, loan_type=?ltype, interest=?interest, no_days_month=?ndm, amount_to_loan=?atl;" +
                "select last_insert_id();";
            cmd = new MySqlCommand(query, con);
            cmd.Parameters.AddWithValue("?mid", member_id);
            cmd.Parameters.AddWithValue("?ltitle", txtLoanTitle.Text);
            cmd.Parameters.AddWithValue("?ltype", cmbLoanType.Text);
            cmd.Parameters.AddWithValue("?interest", numInterest.Value);
            cmd.Parameters.AddWithValue("?ndm", numDayMonth.Value);
            cmd.Parameters.AddWithValue("?atl", numAmountToLoan.Value);
            id = Convert.ToInt64(cmd.ExecuteScalar());
            txtReference.Text = id.ToString("00000000");
            cmd.Dispose();

            query = "INSERT INTO loan_details SET loan_id=?lid, date_month=?dmonth, amount_to_pay=?atopay, amount_paid=?apaid, balance=?bal;" +
                "select last_insert_id();";
            cmd = new MySqlCommand(query, con);
            for(int row = 1; row < flx.Rows.Count; row++)
            {
                cmd.Parameters.AddWithValue("?lid", id);
                cmd.Parameters.AddWithValue("?dmonth", Convert.ToDateTime(flx[row, "date_month"]));
                cmd.Parameters.AddWithValue("?atopay", Convert.ToDouble(flx[row, "amountToPay"]));
                cmd.Parameters.AddWithValue("?apaid", Convert.ToDouble(flx[row, "amountPaid"]));
                cmd.Parameters.AddWithValue("?bal", Convert.ToString(flx[row, "balance"]));
                long loan_detail_id = Convert.ToInt64(cmd.ExecuteScalar());
                flx[row, "loan_detail_id"] = loan_detail_id;
                cmd.Parameters.Clear();
            }

            cmd.Dispose();
            
            con.Close();
            con.Dispose();

        }

        private void btnDebug_Click(object sender, EventArgs e)
        {
            txtLoanTitle.Text = "TEST ONLY";
            cmbLoanType.Text = "MONTHLY";
            numInterest.Value = 10;
            numDayMonth.Value = 5;
            numAmountToLoan.Value = 5000;

            btnCompute_Click(sender, e);
        }

        private void flx_AfterEdit(object sender, C1.Win.C1FlexGrid.RowColEventArgs e)
        {
            if(e.Col == 4)
            {
                double amountToPay = Convert.ToDouble(flx[flx.RowSel, "amountToPay"]);
                double amountPaid = Convert.ToDouble(flx[flx.RowSel, "amountPaid"]);

                flx[e.Row, 5] = amountToPay - amountPaid;
            }
        }
    }
}