namespace ELFVoiceChanger.Forms
{
	partial class ManageVoiceModelsForm
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
			this.voiceModelsBox = new System.Windows.Forms.ComboBox();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.letterPatternBox = new System.Windows.Forms.ComboBox();
			this.panel1 = new System.Windows.Forms.Panel();
			this.button4 = new System.Windows.Forms.Button();
			this.button1 = new System.Windows.Forms.Button();
			this.voiceModelNameBox = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this.label6 = new System.Windows.Forms.Label();
			this.panel2 = new System.Windows.Forms.Panel();
			this.button5 = new System.Windows.Forms.Button();
			this.button3 = new System.Windows.Forms.Button();
			this.button2 = new System.Windows.Forms.Button();
			this.letterPatternName = new System.Windows.Forms.TextBox();
			this.label4 = new System.Windows.Forms.Label();
			this.panel1.SuspendLayout();
			this.panel2.SuspendLayout();
			this.SuspendLayout();
			// 
			// voiceModelsBox
			// 
			this.voiceModelsBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.voiceModelsBox.FormattingEnabled = true;
			this.voiceModelsBox.Location = new System.Drawing.Point(113, 4);
			this.voiceModelsBox.Name = "voiceModelsBox";
			this.voiceModelsBox.Size = new System.Drawing.Size(473, 23);
			this.voiceModelsBox.TabIndex = 0;
			this.voiceModelsBox.SelectedIndexChanged += new System.EventHandler(this.voiceModelsBox_SelectedIndexChanged);
			// 
			// label1
			// 
			this.label1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
			this.label1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.label1.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
			this.label1.Location = new System.Drawing.Point(4, 4);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(103, 23);
			this.label1.TabIndex = 1;
			this.label1.Text = "Voice Model:";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// label2
			// 
			this.label2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
			this.label2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.label2.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
			this.label2.Location = new System.Drawing.Point(4, 4);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(103, 23);
			this.label2.TabIndex = 2;
			this.label2.Text = "Letter Pattern:";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// letterPatternBox
			// 
			this.letterPatternBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.letterPatternBox.FormattingEnabled = true;
			this.letterPatternBox.Location = new System.Drawing.Point(113, 4);
			this.letterPatternBox.Name = "letterPatternBox";
			this.letterPatternBox.Size = new System.Drawing.Size(473, 23);
			this.letterPatternBox.TabIndex = 3;
			// 
			// panel1
			// 
			this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.panel1.Controls.Add(this.button4);
			this.panel1.Controls.Add(this.button1);
			this.panel1.Controls.Add(this.voiceModelNameBox);
			this.panel1.Controls.Add(this.label3);
			this.panel1.Controls.Add(this.label1);
			this.panel1.Controls.Add(this.voiceModelsBox);
			this.panel1.Location = new System.Drawing.Point(12, 12);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(592, 111);
			this.panel1.TabIndex = 6;
			// 
			// button4
			// 
			this.button4.FlatAppearance.BorderColor = System.Drawing.Color.Black;
			this.button4.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.button4.Location = new System.Drawing.Point(4, 82);
			this.button4.Name = "button4";
			this.button4.Size = new System.Drawing.Size(582, 23);
			this.button4.TabIndex = 15;
			this.button4.Text = "Delete This Voice Model";
			this.button4.UseVisualStyleBackColor = true;
			this.button4.Click += new System.EventHandler(this.button4_Click);
			// 
			// button1
			// 
			this.button1.FlatAppearance.BorderColor = System.Drawing.Color.Black;
			this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.button1.Location = new System.Drawing.Point(4, 56);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(582, 23);
			this.button1.TabIndex = 14;
			this.button1.Text = "Create New Voice Model";
			this.button1.UseVisualStyleBackColor = true;
			this.button1.Click += new System.EventHandler(this.button1_Click);
			// 
			// voiceModelNameBox
			// 
			this.voiceModelNameBox.Location = new System.Drawing.Point(113, 30);
			this.voiceModelNameBox.Name = "voiceModelNameBox";
			this.voiceModelNameBox.Size = new System.Drawing.Size(473, 23);
			this.voiceModelNameBox.TabIndex = 10;
			this.voiceModelNameBox.TextChanged += new System.EventHandler(this.voiceModelNameBox_TextChanged);
			// 
			// label3
			// 
			this.label3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
			this.label3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.label3.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
			this.label3.Location = new System.Drawing.Point(4, 30);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(103, 23);
			this.label3.TabIndex = 9;
			this.label3.Text = "Name:";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// label6
			// 
			this.label6.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
			this.label6.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.label6.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
			this.label6.Location = new System.Drawing.Point(4, 56);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(103, 23);
			this.label6.TabIndex = 2;
			this.label6.Text = "Audio File:";
			this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// panel2
			// 
			this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.panel2.Controls.Add(this.button5);
			this.panel2.Controls.Add(this.button3);
			this.panel2.Controls.Add(this.button2);
			this.panel2.Controls.Add(this.label6);
			this.panel2.Controls.Add(this.letterPatternName);
			this.panel2.Controls.Add(this.label2);
			this.panel2.Controls.Add(this.label4);
			this.panel2.Controls.Add(this.letterPatternBox);
			this.panel2.Location = new System.Drawing.Point(12, 178);
			this.panel2.Name = "panel2";
			this.panel2.Size = new System.Drawing.Size(592, 138);
			this.panel2.TabIndex = 7;
			// 
			// button5
			// 
			this.button5.FlatAppearance.BorderColor = System.Drawing.Color.Black;
			this.button5.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.button5.Location = new System.Drawing.Point(4, 109);
			this.button5.Name = "button5";
			this.button5.Size = new System.Drawing.Size(582, 23);
			this.button5.TabIndex = 14;
			this.button5.Text = "Delete This Letter Pattern";
			this.button5.UseVisualStyleBackColor = true;
			// 
			// button3
			// 
			this.button3.FlatAppearance.BorderColor = System.Drawing.Color.Black;
			this.button3.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.button3.Location = new System.Drawing.Point(113, 56);
			this.button3.Name = "button3";
			this.button3.Size = new System.Drawing.Size(473, 23);
			this.button3.TabIndex = 13;
			this.button3.Text = "Select Audio File";
			this.button3.UseVisualStyleBackColor = true;
			// 
			// button2
			// 
			this.button2.FlatAppearance.BorderColor = System.Drawing.Color.Black;
			this.button2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.button2.Location = new System.Drawing.Point(4, 82);
			this.button2.Name = "button2";
			this.button2.Size = new System.Drawing.Size(582, 23);
			this.button2.TabIndex = 11;
			this.button2.Text = "Add New Letter Pattern";
			this.button2.UseVisualStyleBackColor = true;
			this.button2.Click += new System.EventHandler(this.button2_Click);
			// 
			// letterPatternName
			// 
			this.letterPatternName.Location = new System.Drawing.Point(113, 30);
			this.letterPatternName.Name = "letterPatternName";
			this.letterPatternName.Size = new System.Drawing.Size(473, 23);
			this.letterPatternName.TabIndex = 12;
			// 
			// label4
			// 
			this.label4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
			this.label4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.label4.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
			this.label4.Location = new System.Drawing.Point(4, 30);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(103, 23);
			this.label4.TabIndex = 11;
			this.label4.Text = "Name:";
			this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// ManageVoiceModelsForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(616, 328);
			this.Controls.Add(this.panel2);
			this.Controls.Add(this.panel1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.Name = "ManageVoiceModelsForm";
			this.Text = "Manage Voice Models";
			this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.ManageVoiceModelsForm_FormClosed);
			this.Load += new System.EventHandler(this.ManageVoiceModelsForm_Load);
			this.panel1.ResumeLayout(false);
			this.panel1.PerformLayout();
			this.panel2.ResumeLayout(false);
			this.panel2.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion
		private Label label1;
		private Label label2;
		private Panel panel1;
		private Button button1;
		private Label label6;
		private Label label3;
		private Panel panel2;
		private Button button3;
		private Button button2;
		private Label label4;
		private Button button4;
		private Button button5;
		public ComboBox voiceModelsBox;
		public ComboBox letterPatternBox;
		public TextBox voiceModelNameBox;
		public TextBox letterPatternName;
	}
}