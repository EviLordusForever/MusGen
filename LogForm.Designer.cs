namespace ELFMusGen
{
	partial class LogForm
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
			this.rtb = new System.Windows.Forms.RichTextBox();
			this.SuspendLayout();
			// 
			// rtb
			// 
			this.rtb.Location = new System.Drawing.Point(40, 32);
			this.rtb.Name = "rtb";
			this.rtb.Size = new System.Drawing.Size(636, 189);
			this.rtb.TabIndex = 0;
			this.rtb.Text = "";
			// 
			// LogForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(800, 450);
			this.Controls.Add(this.rtb);
			this.Name = "LogForm";
			this.Text = "LogForm";
			this.Load += new System.EventHandler(this.LogForm_Load);
			this.ResumeLayout(false);

		}

		#endregion

		public RichTextBox rtb;
	}
}