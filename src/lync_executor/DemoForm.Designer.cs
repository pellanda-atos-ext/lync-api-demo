namespace lync_executor
{
	partial class DemoForm
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.dataGridView1 = new System.Windows.Forms.DataGridView();
			this.ColumnContact = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.ColumnPhone1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.ColumnPhone2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.ColumnPhone3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.ColumnPhone4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
			((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
			this.SuspendLayout();
			// 
			// dataGridView1
			// 
			this.dataGridView1.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
			this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ColumnContact,
            this.ColumnPhone1,
            this.ColumnPhone2,
            this.ColumnPhone3,
            this.ColumnPhone4});
			this.dataGridView1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.dataGridView1.Location = new System.Drawing.Point(0, 0);
			this.dataGridView1.Name = "dataGridView1";
			this.dataGridView1.ReadOnly = true;
			this.dataGridView1.Size = new System.Drawing.Size(836, 450);
			this.dataGridView1.TabIndex = 0;
			// 
			// ColumnContact
			// 
			this.ColumnContact.HeaderText = "Contact";
			this.ColumnContact.Name = "ColumnContact";
			this.ColumnContact.ReadOnly = true;
			// 
			// ColumnPhone1
			// 
			this.ColumnPhone1.HeaderText = "Phone 1";
			this.ColumnPhone1.Name = "ColumnPhone1";
			this.ColumnPhone1.ReadOnly = true;
			// 
			// ColumnPhone2
			// 
			this.ColumnPhone2.HeaderText = "Phone 2";
			this.ColumnPhone2.Name = "ColumnPhone2";
			this.ColumnPhone2.ReadOnly = true;
			// 
			// ColumnPhone3
			// 
			this.ColumnPhone3.HeaderText = "Phone 3";
			this.ColumnPhone3.Name = "ColumnPhone3";
			this.ColumnPhone3.ReadOnly = true;
			// 
			// ColumnPhone4
			// 
			this.ColumnPhone4.HeaderText = "Phone 4";
			this.ColumnPhone4.Name = "ColumnPhone4";
			this.ColumnPhone4.ReadOnly = true;
			// 
			// DemoForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(836, 450);
			this.Controls.Add(this.dataGridView1);
			this.Name = "DemoForm";
			this.Text = "DemoForm";
			((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.DataGridView dataGridView1;
		private System.Windows.Forms.DataGridViewTextBoxColumn ColumnContact;
		private System.Windows.Forms.DataGridViewTextBoxColumn ColumnPhone1;
		private System.Windows.Forms.DataGridViewTextBoxColumn ColumnPhone2;
		private System.Windows.Forms.DataGridViewTextBoxColumn ColumnPhone3;
		private System.Windows.Forms.DataGridViewTextBoxColumn ColumnPhone4;
	}
}