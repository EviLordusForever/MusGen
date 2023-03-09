using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MusGen.View.Forms;

namespace MusGen.Forms
{
	public static class FormsManager
	{
		public static MainForm _mainForm;
		public static LogForm _logForm;
		public static ProgressForm _progressForm;
		public static ManageVoiceModelsForm manageVoiceModelsForm;
		public static CreateVoiceModelForm createVoiceModelForm;
		public static AddLetterPatternForm addLetterPatternForm;
		public static VoiceChangingForm voiceChangingForm;
		public static EffectsForm effectsForm;

		public static void CloseProgressForm()
		{
			if (_progressForm != null && !_progressForm.IsDisposed)
				_progressForm.Close();
		}

		public static void OpenProgressForm()
		{
			if (_progressForm == null || _progressForm.IsDisposed)
				_progressForm = new ProgressForm();

			_progressForm.WindowState = FormWindowState.Normal;
			_progressForm.Show();
			_progressForm.BringToFront();
		}

		public static void OpenLogForm()
		{
			_mainForm.Invoke(new Action(() =>
			{
				if (_logForm == null || _logForm.IsDisposed)
				{
					_logForm = new LogForm();
					_logForm.BringToFront();
					_logForm.Show();
					_logForm.WindowState = FormWindowState.Normal;
					_logForm.Location = new Point(-7, 0);
					_logForm.rtb.ForeColor = Color.FromArgb(0, 255, 0);
				}
			}));
		}

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