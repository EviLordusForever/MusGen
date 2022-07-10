namespace ELFVoiceChanger.Forms
{
	partial class VoiceChangingForm
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
			this.panel1 = new System.Windows.Forms.Panel();
			this.voiceModelsExport = new System.Windows.Forms.ComboBox();
			this.voiceModelsOrig = new System.Windows.Forms.ComboBox();
			this.label4 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.selectAudioFileButton = new System.Windows.Forms.Button();
			this.button1 = new System.Windows.Forms.Button();
			this.exportFileName = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
			this.panel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// panel1
			// 
			this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.panel1.Controls.Add(this.voiceModelsExport);
			this.panel1.Controls.Add(this.voiceModelsOrig);
			this.panel1.Controls.Add(this.label4);
			this.panel1.Controls.Add(this.label2);
			this.panel1.Controls.Add(this.selectAudioFileButton);
			this.panel1.Controls.Add(this.button1);
			this.panel1.Controls.Add(this.exportFileName);
			this.panel1.Controls.Add(this.label3);
			this.panel1.Controls.Add(this.label1);
			this.panel1.Location = new System.Drawing.Point(12, 12);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(625, 137);
			this.panel1.TabIndex = 7;
			this.panel1.Paint += new System.Windows.Forms.PaintEventHandler(this.panel1_Paint);
			// 
			// voiceModelsExport
			// 
			this.voiceModelsExport.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.voiceModelsExport.FormattingEnabled = true;
			this.voiceModelsExport.Location = new System.Drawing.Point(151, 56);
			this.voiceModelsExport.Name = "voiceModelsExport";
			this.voiceModelsExport.Size = new System.Drawing.Size(468, 23);
			this.voiceModelsExport.TabIndex = 21;
			// 
			// voiceModelsOrig
			// 
			this.voiceModelsOrig.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.voiceModelsOrig.FormattingEnabled = true;
			this.voiceModelsOrig.Location = new System.Drawing.Point(151, 30);
			this.voiceModelsOrig.Name = "voiceModelsOrig";
			this.voiceModelsOrig.Size = new System.Drawing.Size(468, 23);
			this.voiceModelsOrig.TabIndex = 20;
			// 
			// label4
			// 
			this.label4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
			this.label4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.label4.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
			this.label4.Location = new System.Drawing.Point(4, 82);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(141, 23);
			this.label4.TabIndex = 19;
			this.label4.Text = "Export File Name:";
			this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// label2
			// 
			this.label2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
			this.label2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.label2.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
			this.label2.Location = new System.Drawing.Point(4, 56);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(141, 23);
			this.label2.TabIndex = 18;
			this.label2.Text = "Export Voice Model:";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// selectAudioFileButton
			// 
			this.selectAudioFileButton.FlatAppearance.BorderColor = System.Drawing.Color.Black;
			this.selectAudioFileButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.selectAudioFileButton.Location = new System.Drawing.Point(151, 4);
			this.selectAudioFileButton.Name = "selectAudioFileButton";
			this.selectAudioFileButton.Size = new System.Drawing.Size(468, 23);
			this.selectAudioFileButton.TabIndex = 17;
			this.selectAudioFileButton.Text = "Select Audio File";
			this.selectAudioFileButton.UseVisualStyleBackColor = true;
			this.selectAudioFileButton.Click += new System.EventHandler(this.selectAudioFileButton_Click);
			// 
			// button1
			// 
			this.button1.FlatAppearance.BorderColor = System.Drawing.Color.Black;
			this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.button1.Location = new System.Drawing.Point(4, 108);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(615, 23);
			this.button1.TabIndex = 14;
			this.button1.Text = "Export";
			this.button1.UseVisualStyleBackColor = true;
			// 
			// exportFileName
			// 
			this.exportFileName.Location = new System.Drawing.Point(151, 82);
			this.exportFileName.Name = "exportFileName";
			this.exportFileName.Size = new System.Drawing.Size(468, 23);
			this.exportFileName.TabIndex = 10;
			// 
			// label3
			// 
			this.label3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
			this.label3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.label3.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
			this.label3.Location = new System.Drawing.Point(4, 30);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(141, 23);
			this.label3.TabIndex = 9;
			this.label3.Text = "Origin Voice Model:";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// label1
			// 
			this.label1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
			this.label1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.label1.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
			this.label1.Location = new System.Drawing.Point(4, 4);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(141, 23);
			this.label1.TabIndex = 1;
			this.label1.Text = "Original Audio File:";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// openFileDialog1
			// 
			this.openFileDialog1.FileName = "openFileDialog1";
			this.openFileDialog1.FileOk += new System.ComponentModel.CancelEventHandler(this.openFileDialog1_FileOk);
			// 
			// VoiceChangingForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(649, 163);
			this.Controls.Add(this.panel1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.Name = "VoiceChangingForm";
			this.Text = "Change Voice";
			this.Load += new System.EventHandler(this.VoiceChangingForm_Load);
			this.panel1.ResumeLayout(false);
			this.panel1.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private Panel panel1;
		private Button button1;
		public TextBox exportFileName;
		private Label label3;
		private Label label1;
		private Label label2;
		private Button selectAudioFileButton;
		private Label label4;
		public ComboBox voiceModelsExport;
		public ComboBox voiceModelsOrig;
		private OpenFileDialog openFileDialog1;
	}
}