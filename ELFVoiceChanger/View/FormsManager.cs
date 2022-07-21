using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ELFVoiceChanger.View.Forms;

namespace ELFVoiceChanger.Forms
{
	public static class FormsManager
	{
		public static MainForm mainForm;
		public static ManageVoiceModelsForm manageVoiceModelsForm;
		public static CreateVoiceModelForm createVoiceModelForm;
		public static AddLetterPatternForm addLetterPatternForm;
		public static VoiceChangingForm voiceChangingForm;
		public static EffectsForm effectsForm;

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

		public static void OpenAddLetterPatternForm()
		{
			if (addLetterPatternForm == null || addLetterPatternForm.IsDisposed)
				addLetterPatternForm = new AddLetterPatternForm();

			addLetterPatternForm.WindowState = FormWindowState.Normal;
			addLetterPatternForm.Show();
			addLetterPatternForm.BringToFront();
		}
		public static void OpenVoiceChangingForm()
		{
			if (voiceChangingForm == null || voiceChangingForm.IsDisposed)
				voiceChangingForm = new VoiceChangingForm();

			voiceChangingForm.WindowState = FormWindowState.Normal;
			voiceChangingForm.Show();
			voiceChangingForm.BringToFront();
		}

		public static void OpenEffectsForm()
		{
			if (effectsForm == null || effectsForm.IsDisposed)
				effectsForm = new EffectsForm();

			effectsForm.WindowState = FormWindowState.Normal;
			effectsForm.Show();
			effectsForm.BringToFront();
		}

		public static void CloseAddLetterPatternForm()
		{
			if (addLetterPatternForm != null && !addLetterPatternForm.IsDisposed)
				addLetterPatternForm.Close();
		}

		public static void CloseCreateVoiceModelForm()
		{
			if (createVoiceModelForm != null && !createVoiceModelForm.IsDisposed)
				createVoiceModelForm.Close();
		}
	}
}