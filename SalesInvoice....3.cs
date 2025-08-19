using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Windows.Forms;

namespace DatabaseConfiguration
{
    public partial class SalesInvoice : Form
    {
        SqlConnection con = new SqlConnection(CommClass.Connection);

        int cutId = 0;
        public SalesInvoice()
        {
            InitializeComponent();
        }

        private void SalesInvoice_Load(object sender, EventArgs e)
        {
            //fillCombobox();
            //CommonHelper.FillComboBox(cmbState,"StateMaster_Sp", "StateName", "State_Id");

            sqlparam obj = new sqlparam();
            obj.status = "select";
            DataTable dt = CommonHelper.GetDataTable("StateMaster_Sp", obj);
            cmbState.DataSource = dt;
            cmbState.DisplayMember = "StateName";
            cmbState.ValueMember = "State_Id";
            cmbState.SelectedIndex = -1;

            AutogenerateCode();
            dataGridView2.Visible = false;
        }

        //void fillCombobox()
        //{
        //    con.Open();
        //    SqlCommand cmd = new SqlCommand("StateMaster_Sp", con);
        //    cmd.CommandType = CommandType.StoredProcedure;
        //    cmd.Parameters.AddWithValue("@Status", "select");

        //    SqlDataAdapter da = new SqlDataAdapter(cmd);
        //    DataTable dt = new DataTable();
        //    da.Fill(dt);           



        //    cmbState.DataSource = dt;
        //   
        //    cmbState.DisplayMember = "StateName";       
        //    cmbState.ValueMember = "State_Id";
        //    //........//
        //    cmbState.SelectedIndex = -1; // optional, to clear selection
        //    con.Close();

        //}

        void AutogenerateCode()
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("SalesInvoice_Sp", con);  //LoginCredential = stored procedure name
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Status", "SalesInvoiceNumber");
            SqlParameter p = new SqlParameter("@SalesInvoiceNumber", SqlDbType.Decimal);
            p.Direction = ParameterDirection.Output;
            cmd.Parameters.Add(p);
            cmd.ExecuteNonQuery(); // Run the stored procedure

            // Show result in textbox
            txtSalesInvoiceNo.Text = p.Value.ToString();
            con.Close();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.RowIndex < dataGridView1.Rows.Count)
            {
                //1...Continue
                txtCustomerName.TextChanged -= txtCustomerName_TextChanged;

                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];

                txtCustomerName.Text = row.Cells["Customer_Name1"].Value?.ToString() ?? string.Empty;
                txtCustomerMobileNo.Text = row.Cells["Customer_PhoneNumber"].Value?.ToString() ?? string.Empty;
                txtCustomerAddress.Text = row.Cells["Customer_Address"].Value?.ToString() ?? string.Empty;
                cmbState.Text = row.Cells["Customer_State"].Value?.ToString() ?? string.Empty;

                //2...Continue
                txtCustomerName.TextChanged += txtCustomerName_TextChanged;

                dataGridView1.Visible = false;
            }
        }


        void ClearCustomerData()
        {
            txtCustomerAddress.Clear();
            txtCustomerMobileNo.Clear();
            cmbState.SelectedIndex = -1;
        }


        //Search by CustomerName and MobileNumber
        private void txtCustomerName_TextChanged(object sender, EventArgs e)
        {
            string searchText = txtCustomerName.Text.Trim();

            // Clear old selected data
            ClearCustomerData();

            if (!string.IsNullOrEmpty(searchText))
            {
                /* Generic*/
                
                sqlparam obj = new sqlparam();
                obj.status = "SearchByNameAndMobileNo";
                obj.MobileNumber = searchText;
                obj.CustomerName = searchText;
                DataTable dt = CommonHelper.GetDataTable("SalesInvoice_Sp", obj);

                dataGridView1.DataSource = dt;
                dataGridView1.Visible = true;
                



               /*
               SqlCommand cmd = new SqlCommand("SalesInvoice_Sp", con);
               cmd.CommandType = CommandType.StoredProcedure;

               cmd.Parameters.AddWithValue("@Status", "SearchByNameAndMobileNo");
               cmd.Parameters.AddWithValue("@CustomerName", txtCustomerName.Text);
               cmd.Parameters.AddWithValue("@MobileNumber", txtCustomerName.Text);
               SqlDataAdapter da = new SqlDataAdapter(cmd);
               DataTable dt = new DataTable();
               da.Fill(dt);
               dataGridView1.DataSource = dt;

               dataGridView1.Visible = true;
               con.Close();
               */
               



            }
            else
            {
                dataGridView1.Visible = false;
            }             
        }

        void ClearProductData()
        {
            txtProductCode.Clear();
            txtProductRate.Clear();
            txtProductQty.Clear();
            txtProductDiscount.Clear();
            txtProductFreeQty.Clear();
        }

        //RadioButton....ProductName
        private void txtProductName_TextChanged(object sender, EventArgs e)
        {
            string searchText = txtProductName.Text.Trim();

            if (string.IsNullOrEmpty(searchText))
            {
                dataGridView2.Visible = false;
                return;
            }

           
            // Clear old product data
            ClearProductData();

            /*
            // Create and configure command

            SqlCommand cmd = new SqlCommand("Product_Sp", con);
            cmd.CommandType = CommandType.StoredProcedure;

            if (rbProductName.Checked)  //rbProductName is selected
            {
                cmd.Parameters.AddWithValue("@Status", "SearchByName");
                cmd.Parameters.AddWithValue("@Product_Name", searchText);
            }
            if (rbProductCode.Checked)// rbProductCode is selected
            {
                cmd.Parameters.AddWithValue("@Status", "SearchByCode");
                cmd.Parameters.AddWithValue("@Product_Code", searchText);
            }

            // Fetch data
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);

            // Display in grid
            dataGridView2.DataSource = dt;
            dataGridView2.Visible = true;

            if (con.State == ConnectionState.Open)
                con.Close();
            */





            /*Generic*/
            sqlparam obj = new sqlparam();
                   
            //SqlCommand cmd = new SqlCommand("Product_Sp", con);
            //cmd.CommandType = CommandType.StoredProcedure;

            //if (rbProductName.Checked)  //rbProductName is selected
            //{
            //    //cmd.Parameters.AddWithValue("@Status", "SearchByName");
            //    //cmd.Parameters.AddWithValue("@Product_Name", searchText);

            //    obj.status = rbProductName.Checked ? "SearchByName" : "SearchByCode"; ;
            //    obj.Product_Name = searchText;
                
            //}
            //if (rbProductCode.Checked)// rbProductCode is selected
            //{
            //    obj.status = "SearchByCode";
            //    obj.Product_Code = searchText;

            //    //cmd.Parameters.AddWithValue("@Status", "SearchByCode");
            //    //cmd.Parameters.AddWithValue("@Product_Code", searchText);
            //}

            obj.status = rbProductName.Checked ? "SearchByName" : "SearchByCode"; ;
            obj.Product_Name = searchText;

            // Fetch data
            //SqlDataAdapter da = new SqlDataAdapter(cmd);
            //DataTable dt = new DataTable();
            //da.Fill(dt);
            DataTable dt = CommonHelper.GetDataTable("Product_Sp", obj);


            // Display in grid
            dataGridView2.DataSource = dt;
            dataGridView2.Visible = true;

            if (con.State == ConnectionState.Open)
                con.Close();

        }

        //RadioButton...ProductCode
        private void txtProductCode_TextChanged(object sender, EventArgs e)
        {
        }

        private void rbProductCode_CheckedChanged(object sender, EventArgs e)
        {
        }

        private void rbProductName_CheckedChanged(object sender, EventArgs e)
        {
            if (rbProductName.Checked) //Product ..Name
            {
                txtProductName_TextChanged(null, null); // Re-run name search
            }

            if (rbProductCode.Checked) //Product...Code
            {
                txtCustomerName.Clear();
                txtProductName_TextChanged(null, null); // Re-run code search
            }
        }


        private void dataGridView2_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.RowIndex < dataGridView2.Rows.Count)
            {
                DataGridViewRow row = dataGridView2.Rows[e.RowIndex];


                txtProductName.TextChanged -= txtProductName_TextChanged;
                txtProductCode.TextChanged -= txtProductCode_TextChanged;

                txtProductName.Text = row.Cells["Product_Name"].Value?.ToString() ?? string.Empty;
                txtProductCode.Text = row.Cells["Product_Code"].Value?.ToString() ?? string.Empty;
                txtProductRate.Text = row.Cells["Product_SalesRate"].Value?.ToString() ?? string.Empty;
            
                lblAvailableQty.Text = row.Cells["Product_AvailableQty"].Value?.ToString() ?? string.Empty;


                txtProductName.TextChanged += txtProductName_TextChanged;
                txtProductCode.TextChanged += txtProductCode_TextChanged;

                dataGridView2.Visible = false;
            }
        }


        //Discount
        private bool Validate(List<DataGridViewRow> Row)
        {
            if (string.IsNullOrWhiteSpace(txtProductDiscount.Text))
            {
                txtProductDiscount.Text = "0";
            }
            if (string.IsNullOrWhiteSpace(txtProductQty.Text))
            {
                txtProductQty.Text = "0";
            }
            if (string.IsNullOrWhiteSpace(txtProductFreeQty.Text))
            {
                txtProductFreeQty.Text = "0";
            }

            if (string.IsNullOrWhiteSpace(txtProductName.Text) ||
                string.IsNullOrWhiteSpace(txtProductCode.Text))
            {
                MessageBox.Show("Please fill in Product Name and Code.", "Missing Data", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            else if (string.IsNullOrWhiteSpace(txtProductName.Text) ||
                string.IsNullOrWhiteSpace(txtProductCode.Text) ||
                string.IsNullOrWhiteSpace(txtProductRate.Text))
            {
                MessageBox.Show("Please fill in Product Name, Code, SaleRate.", "Missing Data", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            else if (!decimal.TryParse(txtProductDiscount.Text.Trim(), out decimal discount))
            {
                MessageBox.Show("Enter a valid discount number.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtProductDiscount.Focus();
                return false;
            }
            else if (discount < 0 || discount > 100)
            {
                MessageBox.Show("Discount must be between 0 and 100.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtProductDiscount.Focus();
                return false;
            }
            else if ((Convert.ToDecimal(txtProductQty.Text) + Convert.ToDecimal(txtProductFreeQty.Text)) > Convert.ToDecimal(lblAvailableQty.Text.Trim()))
            {
                MessageBox.Show("Qty is stock of out.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtProductQty.Focus();
                return false;
            }
            if (Row != null)
            {
                decimal totalgridQty = Row
                .Sum(row => Convert.ToDecimal(row.Cells["ProductQty"].Value) + Convert.ToDecimal(row.Cells["ProductFreeQty"].Value));

                if ((totalgridQty + Convert.ToInt32(txtProductQty.Text) + Convert.ToInt32(txtProductFreeQty.Text)) > Convert.ToDecimal(lblAvailableQty.Text))
                {
                    MessageBox.Show("Qty + Free Qty is greater than Available Qty. No changes applied.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtProductQty.Text = "";
                    txtProductFreeQty.Text = "";
                    txtProductQty.Focus();
                    return false;
                }
            }
            return true;
        }


        //AddButton
        private void btnAdd_Click(object sender, EventArgs e)
        {
            var dataGrid = dataGridView3.Rows
                .Cast<DataGridViewRow>()
                .Where(row => row.Cells["ProductCode"].Value?.ToString() == txtProductCode.Text.Trim())
                .ToList();
            var existingRow = dataGrid.FirstOrDefault();
            if (!Validate(dataGrid))
            {
                return;
            }

            if (existingRow != null)
            {

                decimal availableQty = Convert.ToDecimal(existingRow.Cells["ProductAvailableQty"].Value);
                decimal newQty = Convert.ToInt32(txtProductQty.Text);
                decimal newFreeQty = Convert.ToInt32(txtProductFreeQty.Text);
                decimal totalgridQty = dataGrid.Sum(row => Convert.ToDecimal(row.Cells["ProductQty"].Value) + Convert.ToDecimal(row.Cells["ProductFreeQty"].Value));

                existingRow.Cells["ProductQty"].Value = Convert.ToDecimal(existingRow.Cells["ProductQty"].Value) + newQty;
                existingRow.Cells["ProductFreeQty"].Value = Convert.ToDecimal(existingRow.Cells["ProductFreeQty"].Value) + newFreeQty;

                existingRow.Cells["ProductSalesRate"].Value = Convert.ToDecimal(existingRow.Cells["ProductSalesRate"].Value) + Convert.ToDecimal(txtProductRate.Text);

                Decimal ProductSubTotal = (newQty) * (Convert.ToDecimal(existingRow.Cells["ProductSalesRate"].Value));
                existingRow.Cells["ProductSubTotal"].Value = ProductSubTotal;

                existingRow.Cells["ProductDiscount"].Value = Convert.ToDecimal(existingRow.Cells["ProductDiscount"].Value) + Convert.ToDecimal(txtProductDiscount.Text);
                existingRow.Cells["Product_DiscountPercentage"].Value = Convert.ToDecimal(existingRow.Cells["Product_DiscountPercentage"].Value) + Convert.ToDecimal(txtProductDiscount.Text);

                decimal DiscountAmount = (ProductSubTotal * (Convert.ToDecimal(existingRow.Cells["ProductDiscount"].Value))) / 100;
                existingRow.Cells["ProductDiscount"].Value = DiscountAmount;

                decimal NetAmount = ProductSubTotal - DiscountAmount;
                existingRow.Cells["ProductNetAmount"].Value = NetAmount;

                clearSearchProduct();

                UpdateGrossAmount();
            }
            else
            {
                // Add new row if not duplicate
                DataGridViewRow row = new DataGridViewRow();
                row.CreateCells(dataGridView3); // Ensure columns already created

                txtProductName.TextChanged -= txtProductName_TextChanged;
                txtProductCode.TextChanged -= txtProductCode_TextChanged;

                //2nd Gridview data showing in 3rdGridview when i click on Add..Button
                row.Cells[dataGridView3.Columns["ProductName"].Index].Value = txtProductName.Text;
                row.Cells[dataGridView3.Columns["ProductCode"].Index].Value = txtProductCode.Text;
                row.Cells[dataGridView3.Columns["ProductSalesRate"].Index].Value = txtProductRate.Text;
                row.Cells[dataGridView3.Columns["ProductQty"].Index].Value = txtProductQty.Text;
                row.Cells[dataGridView3.Columns["ProductFreeQty"].Index].Value = txtProductFreeQty.Text;
                row.Cells[dataGridView3.Columns["Product_DiscountPercentage"].Index].Value = txtProductDiscount.Text;
                row.Cells[dataGridView3.Columns["ProductDiscount"].Index].Value = txtProductDiscount.Text;
                row.Cells[dataGridView3.Columns["ProductAvailableQty"].Index].Value = lblAvailableQty.Text;

                int qty = Convert.ToInt32(txtProductQty.Text);
                int freeQty = Convert.ToInt32(txtProductFreeQty.Text);
                decimal rate = Convert.ToDecimal(txtProductRate.Text);
                decimal discount = Convert.ToDecimal(txtProductDiscount.Text);

                decimal ProductSubTotal = (qty) * rate;
                row.Cells[dataGridView3.Columns["ProductSubTotal"].Index].Value = ProductSubTotal;

                decimal ProductDiscountAmount = (ProductSubTotal * discount) / 100;
                row.Cells[dataGridView3.Columns["ProductDiscount"].Index].Value = ProductDiscountAmount;

                decimal ProductNetAmount = ProductSubTotal - ProductDiscountAmount;
                row.Cells[dataGridView3.Columns["ProductNetAmount"].Index].Value = ProductNetAmount;


                txtProductName.TextChanged += txtProductName_TextChanged;
                txtProductCode.TextChanged += txtProductCode_TextChanged;

                dataGridView3.Rows.Add(row);

                clearSearchProduct();

                //2...GrossAmount
                UpdateGrossAmount();

                CalculateNetAmount();

            }
        }

        //Clear()...DataGridView3
        void clearSearchProduct()
        {
            txtProductName.Clear();
            txtProductCode.Clear();
            txtProductRate.Clear();
            txtProductQty.Clear();
            txtProductFreeQty.Clear();
            txtProductDiscount.Clear();
            lblAvailableQty.Text = "";
        }

        private void txtProductDiscount_Validating(object sender, CancelEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtProductDiscount.Text))
            {
                txtProductDiscount.Text = "0";
            }
        }


        public void Numeric(KeyPressEventArgs e)
        {
            // Allow only digits and control characters (like backspace)
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true; // Block the character
            }
        }

        private void txtProductQty_KeyPress(object sender, KeyPressEventArgs e)
        {
            Numeric(e);
        }
        private void txtProductDiscount_KeyPress(object sender, KeyPressEventArgs e)
        {
            Numeric(e);
        }
        private void txtProductFreeQty_KeyPress(object sender, KeyPressEventArgs e)
        {
            Numeric(e);
        }


        //data edit automatic update calculation
        private void dataGridView3_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && !dataGridView3.Rows[e.RowIndex].IsNewRow)
            {
                dataGridView3.CellValueChanged -= dataGridView3_CellValueChanged;
                var row = dataGridView3.Rows[e.RowIndex];

                // Get values from the row
                decimal.TryParse(row.Cells["ProductSalesRate"].Value?.ToString(), out decimal rate);
                int.TryParse(row.Cells["ProductQty"].Value?.ToString(), out int qty);
                int.TryParse(row.Cells["ProductFreeQty"].Value?.ToString(), out int freeQty);
                decimal.TryParse(row.Cells["Product_DiscountPercentage"].Value?.ToString(), out decimal discountPer);

                decimal subtotal = qty * rate;
                row.Cells["ProductSubTotal"].Value = subtotal;

                decimal discountAmount = (subtotal * discountPer) / 100;
                row.Cells["ProductDiscount"].Value = discountAmount;

                decimal netAmount = subtotal - discountAmount;
                row.Cells["ProductNetAmount"].Value = netAmount;

                //Update Total
                UpdateGrossAmount();
                CalculateNetAmount();

                dataGridView3.CellValueChanged += dataGridView3_CellValueChanged;
            }
        }


        private void txtDiscount_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void txtDiscount_Validating(object sender, CancelEventArgs e)
        {
            if (!FinalDiscount())
            {
                e.Cancel = true;
            }
        }

        private bool FinalDiscount()
        {
            string discountText = txtDiscountValue.Text.Trim();


            if (!decimal.TryParse(discountText, out decimal discount))
            {
                MessageBox.Show("Enter a valid discount number.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtDiscountValue.Focus();
                return false;
            }
            else if (discount < 0 || discount > 100)
            {
                MessageBox.Show("Discount must be between 0 and 100.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtDiscountValue.Focus();
                return false;
            }

            return true;
        }


        //DataGridvie3..Edit
        private void dataGridView3_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
        }

        private void txtRoundOffDiscount_TextChanged(object sender, EventArgs e)
        {
            if (FinalDiscount())
            {
                //futercal(); 
                CalculateNetAmount();
            }
        }


        private void UpdateGrossAmount()
        {
            //futercal();
            var dataGrid = dataGridView3.Rows
                .Cast<DataGridViewRow>()
                .ToList();
            txtGrossAmount.Text = dataGrid.Sum(row => Convert.ToDecimal(row.Cells["ProductSubTotal"].Value)).ToString();
        }


        private void /*futercal()*/ CalculateNetAmount()
        {
            var dataGrid = dataGridView3.Rows
                .Cast<DataGridViewRow>()
                .ToList();

            //txtGrossAmount.Text = dataGrid.Sum(row => Convert.ToDecimal(row.Cells["ProductSubTotal"].Value)).ToString();

            decimal NetAmount = Convert.ToDecimal(dataGrid.Sum(row => Convert.ToDecimal(row.Cells["ProductNetAmount"].Value)));

            if (Convert.ToDecimal(txtDiscountValue.Text.Trim()) > 0)
            {
                txtDiscountAmt.Text = Convert.ToString((NetAmount * Convert.ToDecimal(txtDiscountValue.Text.Trim())) / 100);
            }
            NetAmount = NetAmount - Convert.ToDecimal(txtDiscountAmt.Text.Trim());

            txtRoundOffDiscount.TextChanged -= txtRoundOffDiscount_TextChanged;

            txtRoundOffDiscount.Text = (Math.Round(NetAmount) - NetAmount).ToString();
            txtNetAmount.Text = Math.Round(NetAmount).ToString();

            txtRoundOffDiscount.TextChanged += txtRoundOffDiscount_TextChanged;
        }


        private void dataGridView1_CellClick_1(object sender, DataGridViewCellEventArgs e)
        {

        }


        private void txtDiscount_TextChanged(object sender, EventArgs e)
        {
            if (FinalDiscount())
            {
                CalculateNetAmount();
            }
            else
            {
                txtDiscountValue.Text = "0";
                txtDiscountAmt.Text = "0.00";
                txtNetAmount.Text = "0.00";
                txtRoundOffDiscount.Text = "+0.00";
            }
        }

        private void btnBack_Click(object sender, EventArgs e)
        {

            DialogResult result = MessageBox.Show("Do you want to go back to the previous form ? ", "Confirm Message", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                this.Hide();
                ListPage l1 = new ListPage();
                l1.ShowDialog();
            }
            return;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Do you want to Save All data? ", "Confirm Message", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                if (!ValidateCustomerDetail())
                    return;

                opp();
                //InsertGridData();
                Clear();
                AutogenerateCode();
            }
            return;
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            ClearAlldata();
        }


        private bool ValidateCustomerDetail()
        {
            if (new[] { txtCustomerName.Text, cmbState.Text, txtCustomerAddress.Text, txtCustomerMobileNo.Text }.Any(string.IsNullOrWhiteSpace))
            {
                MessageBox.Show("Please fill All Customer Detail", "Missing Data", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            return true;
        }


        void opp()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(CommClass.Connection))
                {
                    con.Open();

                    // 1. Check for existing customer
                    int cutId = 0;
                    using (SqlCommand cmd = new SqlCommand("USP_usermaster", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                      
                        cmd.Parameters.AddWithValue("@Status", "getuserinfo");
                        cmd.Parameters.AddWithValue("@MobileNo", txtCustomerMobileNo.Text.Trim());
                        cmd.Parameters.AddWithValue("@CustomerName", txtCustomerName.Text.Trim());

                        object obj = cmd.ExecuteScalar();
                        cutId = Convert.ToInt32(obj);
                    }

                    // 2. Insert customer if not found
                    if (cutId == 0)
                    {
                        using (SqlCommand cmd1 = new SqlCommand("USP_usermaster", con))
                        {
                            cmd1.CommandType = CommandType.StoredProcedure;
                            cmd1.Parameters.AddWithValue("@Status", "InsertCustomerInfo");
                            cmd1.Parameters.AddWithValue("@MobileNo", txtCustomerMobileNo.Text.Trim());
                            cmd1.Parameters.AddWithValue("@CustomerName", txtCustomerName.Text.Trim());
                            cmd1.Parameters.AddWithValue("@Customer_Address", txtCustomerAddress.Text.Trim());
                            cmd1.Parameters.AddWithValue("@Customer_State_id", Convert.ToInt32(cmbState.SelectedValue));

                            var obj1 = cmd1.ExecuteScalar();
                            cutId = Convert.ToInt32(obj1);
                        }
                    }

                    // 3. Insert into SalesInvoice
                    int sih_id = 0;
                    using (SqlCommand cmdSI = new SqlCommand("USP_SalesInvoice", con))
                    {
                        cmdSI.CommandType = CommandType.StoredProcedure;
                        cmdSI.Parameters.AddWithValue("@Status", "InsertSalesInvoice");
                        cmdSI.Parameters.AddWithValue("@SalesInvoice_Date", Convert.ToDateTime(SalesDate.Text.Trim()));
                        cmdSI.Parameters.AddWithValue("@SalesInvoice_Number", txtSalesInvoiceNo.Text.Trim());
                        cmdSI.Parameters.AddWithValue("@GrossAmount", Convert.ToDecimal(txtGrossAmount.Text.Trim()));
                        cmdSI.Parameters.AddWithValue("@DiscountPer", Convert.ToDecimal(txtDiscountValue.Text.Trim()));
                        cmdSI.Parameters.AddWithValue("@DiscountAmount", Convert.ToDecimal(txtDiscountAmt.Text.Trim()));
                        cmdSI.Parameters.AddWithValue("@Roundoff", Convert.ToDecimal(txtRoundOffDiscount.Text.Trim()));
                        cmdSI.Parameters.AddWithValue("@Total_Amount", Convert.ToDecimal(txtNetAmount.Text.Trim()));
                        cmdSI.Parameters.AddWithValue("@Sate_Id", Convert.ToInt32(cmbState.SelectedValue));
                        cmdSI.Parameters.AddWithValue("@Customer_Id", cutId);

                        sih_id = Convert.ToInt32(cmdSI.ExecuteScalar());
                    }

                    // 4. Insert Invoice Details & Stock Updates
                    if (sih_id > 0)
                    {
                        foreach (DataGridViewRow row in dataGridView3.Rows.Cast<DataGridViewRow>().Where(r => !r.IsNewRow))
                        {
                            // Insert Invoice Detail
                            using (SqlCommand cmdSD = new SqlCommand("SP_SalesInvoiceDetail_new", con))
                            {
                                cmdSD.CommandType = CommandType.StoredProcedure;
                                cmdSD.Parameters.AddWithValue("@sid_SIH_id", sih_id);
                                cmdSD.Parameters.AddWithValue("@sid_mm_id", row.Cells["ProductCode"].Value);
                                cmdSD.Parameters.AddWithValue("@sid_Qty", row.Cells["ProductQty"].Value);
                                cmdSD.Parameters.AddWithValue("@sid_FreeQty", row.Cells["ProductFreeQty"].Value);
                                cmdSD.Parameters.AddWithValue("@sid_Discount", row.Cells["Product_DiscountPercentage"].Value);
                                cmdSD.Parameters.AddWithValue("@sid_DiscountAmt", row.Cells["ProductDiscount"].Value);
                                cmdSD.Parameters.AddWithValue("@sid_SubTotal", row.Cells["ProductSubTotal"].Value);
                                cmdSD.Parameters.AddWithValue("@sid_NetAmount", row.Cells["ProductNetAmount"].Value);
                                cmdSD.ExecuteNonQuery();
                            }

                            // Update Stock
                            using (SqlCommand cmdStock = new SqlCommand("SP_stock", con))
                            {
                                cmdStock.CommandType = CommandType.StoredProcedure;
                                cmdStock.Parameters.AddWithValue("@ProductCode", row.Cells["ProductCode"].Value);
                                cmdStock.Parameters.AddWithValue("@ProductQty",Convert.ToInt32(row.Cells["ProductQty"].Value ?? 0) + Convert.ToInt32(row.Cells["ProductFreeQty"].Value ?? 0));
                                cmdStock.ExecuteNonQuery();
                            }
                        }
                    }

                    // 5. Delete products with zero quantity
                    using (SqlCommand cmd = new SqlCommand("DELETE FROM Product WHERE Product_AvailableQty = 0", con))
                    {
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Data Searching or insertion Error: " + ex.Message);
            }
        }



        public void ClearAlldata()
        {
            DialogResult result = MessageBox.Show("Do you want to Clear All data ? ", "Confirm Message", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (result == DialogResult.Yes)
            {
                Clear();
            }
            return;
        }

        public void Clear()
        {

            ClearCustomerData();
            txtCustomerName.Clear();
            //txtSalesInvoiceNo.Clear();
            //SalesDate.Select = "";

            clearSearchProduct();
            dataGridView3.Rows.Clear();

            txtGrossAmount.Clear();

            txtDiscountValue.Text = "0";
            txtDiscountAmt.Text = "0.00";
            txtRoundOffDiscount.Clear();
            txtNetAmount.Clear();
        }

        private void txtCustomerMobileNo_KeyPress(object sender, KeyPressEventArgs e)
        {

            if ((!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar)) || (txtCustomerMobileNo.Text.Length >= 10 && !char.IsControl(e.KeyChar)))
            {
                e.Handled = true;
            }

        }
    }
}
