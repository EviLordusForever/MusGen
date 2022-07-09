using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELFVoiceChanger.Forms
{
	public static class FormsManager
	{
		public static MainForm mainForm;
		public static ManageVoiceModelsForm manageVoiceModelsForm;
		public static CreateVoiceModelForm createVoiceModelForm;

		public static void OpenManageVoiceModelForm()
		{
			if (manageVoiceModelsForm == null || manageVoiceModelsForm.IsDisposed)
				manageVoiceModelsForm = new ManageVoiceModelsForm();

			manageVoiceModelsForm.WindowState = FormWindowState.Normal;
			manageVoiceModelsForm.Show();
			manageVoiceModelsForm.BringToFront();
		}

		public static void OpenCreateVoiceModelForm()
		{
			if (createVoiceModelForm == null || createVoiceModelForm.IsDisposed)
				createVoiceModelForm = new CreateVoiceModelForm();

			createVoiceModelForm.WindowState = FormWindowState.Normal;
			createVoiceModelForm.Show();
			createVoiceModelForm.BringToFront();
		}
	}
}
