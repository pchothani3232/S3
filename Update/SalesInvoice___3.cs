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
        public SalesInvoice()
        {
            InitializeComponent();
        }



        private void SalesInvoice_Load(object sender, EventArgs e)
        {        
            sqlparam obj = new sqlparam();
            obj.status = "select";
            DataTable dt = CommonHelper.GetDataTable("StateMaster_Sp", obj);
            cmbState.DataSource = dt;
            cmbState.DisplayMember = "StateName";
            cmbState.ValueMember = "State_Id";
            cmbState.SelectedIndex = -1;

            AutogenerateCode();
            SearchProductDGV.Visible = false;
        }

        void AutogenerateCode()
        {           
            sqlparam AutogenerateCode = new sqlparam();
            AutogenerateCode.status = "SalesInvoiceNumber";

            DataTable dt = CommonHelper.GetDataTable("SalesInvoice_Sp", AutogenerateCode);

            if (dt != null && dt.Rows.Count > 0)
            {
                txtSalesInvoiceNo.Text = dt.Rows[0]["SalesInvoiceNumber"].ToString();
            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.RowIndex < CustomerDetailDGV.Rows.Count)
            {
                //1...Continue
                txtCustomerName.TextChanged -= txtCustomerName_TextChanged;

                DataGridViewRow row = CustomerDetailDGV.Rows[e.RowIndex];

                txtCustomerName.Text = row.Cells["Customer_Name1"].Value?.ToString() ?? string.Empty;
                txtCustomerMobileNo.Text = row.Cells["Customer_PhoneNumber"].Value?.ToString() ?? string.Empty;
                txtCustomerAddress.Text = row.Cells["Customer_Address"].Value?.ToString() ?? string.Empty;
                cmbState.Text = row.Cells["Customer_State"].Value?.ToString() ?? string.Empty;

                //2...Continue
                txtCustomerName.TextChanged += txtCustomerName_TextChanged;

                CustomerDetailDGV.Visible = false;
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


            CustomerDetailDGV.BringToFront();

            // Clear old selected data
            ClearCustomerData();

            if (!string.IsNullOrEmpty(searchText))
            {
                sqlparam obj = new sqlparam();
                obj.status = "SearchByNameAndMobileNo";
                obj.MobileNumber = searchText;
                obj.CustomerName = searchText;
                DataTable dt = CommonHelper.GetDataTable("SalesInvoice_Sp", obj);

                if(dt!=null && dt.Rows.Count>0)
                {
                    CustomerDetailDGV.DataSource = dt;
                    CustomerDetailDGV.Visible = true;
                }
                else
                {
                    CustomerDetailDGV.DataSource = null;
                    CustomerDetailDGV.Visible = false;
                }
               
            }
            else
            {
                CustomerDetailDGV.DataSource = null;
                CustomerDetailDGV.Visible = false;
   
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
            SearchProductDGV.BringToFront();
            string searchText = txtProductName.Text.Trim();

            if (string.IsNullOrEmpty(searchText))
            {
                SearchProductDGV.Visible = false;
                return;
            }

            // Clear old product data
            ClearProductData();
  
            sqlparam obj = new sqlparam();

            obj.status = rbProductName.Checked ? "SearchByName" : "SearchByCode"; ;
            obj.Product_Name = searchText;

            DataTable dt = CommonHelper.GetDataTable("Product_Sp", obj);

            // Display in grid
            SearchProductDGV.DataSource = dt;
            SearchProductDGV.Visible = true;
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
            if (e.RowIndex >= 0 && e.RowIndex < SearchProductDGV.Rows.Count)
            {
                DataGridViewRow row = SearchProductDGV.Rows[e.RowIndex];


                txtProductName.TextChanged -= txtProductName_TextChanged;
                txtProductCode.TextChanged -= txtProductCode_TextChanged;

                txtProductName.Text = row.Cells["Product_Name"].Value?.ToString() ?? string.Empty;
                txtProductCode.Text = row.Cells["Product_Code"].Value?.ToString() ?? string.Empty;
                txtProductRate.Text = row.Cells["Product_SalesRate"].Value?.ToString() ?? string.Empty;

                lblAvailableQty.Text = row.Cells["Product_AvailableQty"].Value?.ToString() ?? string.Empty;


                txtProductName.TextChanged += txtProductName_TextChanged;
                txtProductCode.TextChanged += txtProductCode_TextChanged;

                SearchProductDGV.Visible = false;
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
            var dataGrid = SalesInvoiceDetailDGV.Rows
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
                row.CreateCells(SalesInvoiceDetailDGV); // Ensure columns already created

                txtProductName.TextChanged -= txtProductName_TextChanged;
                txtProductCode.TextChanged -= txtProductCode_TextChanged;

                //2nd Gridview data showing in 3rdGridview when i click on Add..Button
                row.Cells[SalesInvoiceDetailDGV.Columns["ProductName"].Index].Value = txtProductName.Text;
                row.Cells[SalesInvoiceDetailDGV.Columns["ProductCode"].Index].Value = txtProductCode.Text;
                row.Cells[SalesInvoiceDetailDGV.Columns["ProductSalesRate"].Index].Value = txtProductRate.Text;
                row.Cells[SalesInvoiceDetailDGV.Columns["ProductQty"].Index].Value = txtProductQty.Text;
                row.Cells[SalesInvoiceDetailDGV.Columns["ProductFreeQty"].Index].Value = txtProductFreeQty.Text;
                row.Cells[SalesInvoiceDetailDGV.Columns["Product_DiscountPercentage"].Index].Value = txtProductDiscount.Text;
                row.Cells[SalesInvoiceDetailDGV.Columns["ProductDiscount"].Index].Value = txtProductDiscount.Text;
                row.Cells[SalesInvoiceDetailDGV.Columns["ProductAvailableQty"].Index].Value = lblAvailableQty.Text;

                int qty = Convert.ToInt32(txtProductQty.Text);
                int freeQty = Convert.ToInt32(txtProductFreeQty.Text);
                decimal rate = Convert.ToDecimal(txtProductRate.Text);
                decimal discount = Convert.ToDecimal(txtProductDiscount.Text);

                decimal ProductSubTotal = (qty) * rate;
                row.Cells[SalesInvoiceDetailDGV.Columns["ProductSubTotal"].Index].Value = ProductSubTotal;

                decimal ProductDiscountAmount = (ProductSubTotal * discount) / 100;
                row.Cells[SalesInvoiceDetailDGV.Columns["ProductDiscount"].Index].Value = ProductDiscountAmount;

                decimal ProductNetAmount = ProductSubTotal - ProductDiscountAmount;
                row.Cells[SalesInvoiceDetailDGV.Columns["ProductNetAmount"].Index].Value = ProductNetAmount;


                txtProductName.TextChanged += txtProductName_TextChanged;
                txtProductCode.TextChanged += txtProductCode_TextChanged;

                SalesInvoiceDetailDGV.Rows.Add(row);

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
            if (e.RowIndex >= 0 && !SalesInvoiceDetailDGV.Rows[e.RowIndex].IsNewRow)
            {
                SalesInvoiceDetailDGV.CellValueChanged -= dataGridView3_CellValueChanged;
                var row = SalesInvoiceDetailDGV.Rows[e.RowIndex];

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

                SalesInvoiceDetailDGV.CellValueChanged += dataGridView3_CellValueChanged;
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
                 CalculateNetAmount();
            }
        }


        private void UpdateGrossAmount()
        {
            var dataGrid = SalesInvoiceDetailDGV.Rows
                .Cast<DataGridViewRow>()
                .ToList();
            txtGrossAmount.Text = dataGrid.Sum(row => Convert.ToDecimal(row.Cells["ProductSubTotal"].Value)).ToString();
        }


        private void CalculateNetAmount()
        {
            var dataGrid = SalesInvoiceDetailDGV.Rows
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
                //this.Hide();
                //this.Close();
                SalesInvoiceCustomerListPage l1 = new SalesInvoiceCustomerListPage();
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

                Allopp();
                Clear();
                AutogenerateCode();

                
                SalesInvoiceCustomerListPage l1 = new SalesInvoiceCustomerListPage();
                l1.ShowDialog();
                

                this.DialogResult = DialogResult.OK;
                this.Close(); // close after the list page is closed
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

            // Check at least one product is selected in DataGridView
            if (SalesInvoiceDetailDGV.Rows.Cast<DataGridViewRow>()
                                 .All(r => r.IsNewRow)) // means no product rows
            {
                MessageBox.Show("Please select at least one product", "Missing Data",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }


            return true;
        }


       


        void Allopp()
        {
            try
            {
                // 1. Check for existing customer                 
                int cutId = 0;
                sqlparam objCheckCust = new sqlparam();
                objCheckCust.status = "getuserinfo";
                objCheckCust.MobileNo = txtCustomerMobileNo.Text.Trim();
                objCheckCust.CustomerName = txtCustomerName.Text.Trim();

                DataTable dt = CommonHelper.GetDataTable("USP_usermaster", objCheckCust);

                if (dt != null && dt.Rows.Count > 0)
                {
                    cutId = Convert.ToInt32(dt.Rows[0][0]);  // Get CustomerId from first column
                }

                // 2. Insert customer if not found
                if (cutId == 0)
                {
                    sqlparam objInsert = new sqlparam();
                    objInsert.status = "InsertCustomerInfo";
                    objInsert.MobileNo = txtCustomerMobileNo.Text.Trim();
                    objInsert.CustomerName = txtCustomerName.Text.Trim();
                    objInsert.Customer_Address = txtCustomerAddress.Text.Trim();
                    objInsert.Customer_State_id = Convert.ToInt32(cmbState.SelectedValue);

                    DataTable dtInsertCust = CommonHelper.GetDataTable("USP_usermaster", objInsert);

                    if (dtInsertCust != null && dtInsertCust.Rows.Count > 0)
                    {
                        cutId = Convert.ToInt32(dtInsertCust.Rows[0][0]); // Get new customer ID
                    }
                }

                // 3. Insert into SalesInvoice

                int sih_id = 0;

                sqlparam salesInsert = new sqlparam();
                salesInsert.status = "InsertSalesInvoice";

                salesInsert.SalesInvoice_Date = SalesDate.Text.Trim();
                salesInsert.SalesInvoice_Number = txtSalesInvoiceNo.Text.Trim();

                salesInsert.GrossAmount = Convert.ToDecimal(txtGrossAmount.Text);
                salesInsert.DiscountPer = Convert.ToDecimal(txtDiscountValue.Text);
                salesInsert.DiscountAmount = Convert.ToDecimal(txtDiscountAmt.Text);
                salesInsert.Roundoff = Convert.ToDecimal(txtRoundOffDiscount.Text);
                salesInsert.Total_Amount = Convert.ToInt32(txtNetAmount.Text);
                salesInsert.Sate_Id = Convert.ToInt32(cmbState.SelectedValue);

                salesInsert.Customer_Id = cutId;

                DataTable dtInsertSales = CommonHelper.GetDataTable("USP_SalesInvoice", salesInsert);
                if (dtInsertSales != null && dtInsertSales.Rows.Count > 0)
                {
                    sih_id = Convert.ToInt32(dtInsertSales.Rows[0][0]); // assuming the ID is in the first column
                }
              

                // 4. Insert Invoice Details & Stock Updates
                if (sih_id > 0)
                {
                    foreach (DataGridViewRow row in SalesInvoiceDetailDGV.Rows.Cast<DataGridViewRow>().Where(r => !r.IsNewRow))
                    {
                        // Insert Invoice Detail
                     
                        sqlparam InsertInvoice = new sqlparam();
                      
                        InsertInvoice.sid_SIH_id = sih_id;
                        InsertInvoice.sid_mm_id = Convert.ToInt32(row.Cells["ProductCode"].Value);
                        InsertInvoice.sid_Qty = Convert.ToInt32(row.Cells["ProductQty"].Value);
                        InsertInvoice.sid_FreeQty = Convert.ToInt32(row.Cells["ProductFreeQty"].Value);
                        InsertInvoice.sid_Discount = Convert.ToDecimal(row.Cells["Product_DiscountPercentage"].Value);
                        InsertInvoice.sid_DiscountAmt = Convert.ToDecimal(row.Cells["ProductDiscount"].Value);
                        InsertInvoice.sid_SubTotal = Convert.ToDecimal(row.Cells["ProductSubTotal"].Value);
                        InsertInvoice.sid_NetAmount = Convert.ToDecimal(row.Cells["ProductNetAmount"].Value);

                        DataTable dtInvoice = CommonHelper.GetDataTable("SP_SalesInvoiceDetail_new", InsertInvoice);
                        

                        // Update Stock                      
                        sqlparam UpdateStock = new sqlparam();

                        UpdateStock.ProductCode = Convert.ToInt32(row.Cells["ProductCode"].Value);
                        UpdateStock.ProductQty = Convert.ToInt32(row.Cells["ProductQty"].Value ?? 0) +
                                                 Convert.ToInt32(row.Cells["ProductFreeQty"].Value ?? 0);

                        DataTable UpdateStk = CommonHelper.GetDataTable("SP_stock", UpdateStock);
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
            SalesInvoiceDetailDGV.Rows.Clear();

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

        //Customer detail keyup... Key down through
        private void txtCustomerName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Down)
            {
                if (CustomerDetailDGV.Rows.Count > 0)
                {
                    // Move focus to first cell
                    CustomerDetailDGV.CurrentCell = CustomerDetailDGV.Rows[0].Cells[0];
                    CustomerDetailDGV.Focus();
                }
                e.Handled = true;
                e.SuppressKeyPress = true; // prevent beep sound
            }

            if(e.KeyCode == Keys.Enter)
            {
                if (CustomerDetailDGV.CurrentRow != null)
                {
                    //1...Continue
                    txtCustomerName.TextChanged -= txtCustomerName_TextChanged;

                    DataGridViewRow row = CustomerDetailDGV.CurrentRow;

                    txtCustomerName.Text = row.Cells["Customer_Name1"].Value?.ToString() ?? string.Empty;
                    txtCustomerMobileNo.Text = row.Cells["Customer_PhoneNumber"].Value?.ToString() ?? string.Empty;
                    txtCustomerAddress.Text = row.Cells["Customer_Address"].Value?.ToString() ?? string.Empty;
                    cmbState.Text = row.Cells["Customer_State"].Value?.ToString() ?? string.Empty;

                    //2...Continue
                    txtCustomerName.TextChanged += txtCustomerName_TextChanged;

                    CustomerDetailDGV.Visible = false;

                    txtCustomerName.Focus();

                    e.Handled = true;
                    e.SuppressKeyPress = true; // prevent beep
                }
            }
         
        }

        private void CustomerDetailDGV_KeyPress(object sender, KeyPressEventArgs e)
        {

            if (e.KeyChar == (char)Keys.Enter && CustomerDetailDGV.Rows.Count > 0)
            {
                txtCustomerName.TextChanged -= txtCustomerName_TextChanged;

                DataGridViewRow row = CustomerDetailDGV.Rows[CustomerDetailDGV.CurrentCell.RowIndex];

                txtCustomerName.Text = row.Cells["Customer_Name1"].Value?.ToString() ?? string.Empty;
                txtCustomerMobileNo.Text = row.Cells["Customer_PhoneNumber"].Value?.ToString() ?? string.Empty;
                txtCustomerAddress.Text = row.Cells["Customer_Address"].Value?.ToString() ?? string.Empty;
                cmbState.Text = row.Cells["Customer_State"].Value?.ToString() ?? string.Empty;

                //2...Continue
                txtCustomerName.TextChanged += txtCustomerName_TextChanged;

                CustomerDetailDGV.Visible = false;
            }
          
        }
    }
}
