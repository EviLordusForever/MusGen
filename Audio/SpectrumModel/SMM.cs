using Extensions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tensorflow;

namespace MusGen
{
	public static class SMM //Spectrum Model Manager
	{
		private static string _path = $"{DiskE._programFiles}\\SpectrumModel.json";

		public static SpectrumModel _model = new SpectrumModel();

		public static void Init()
		{
			if (!File.Exists(_path))
				Regen();
			else if (DialogE.Ask("Regenerate spectrum model?"))
				Regen();
			else
				Load();

			void Regen()
			{
				_model._size = AP.SpectrumSizeGG;
				_model._model = new float[_model._size, _model._size];
				_model._modelN = new float[_model._size, _model._size];
				_model._modelN2 = new double[_model._size][];
				_model._modelN3 = new float[_model._size][];
				_model._mask = new int[_model._size][];
				_model._maxesForColumns = new float[_model._size];
				_model._power = new float[_model._size];
				_model._curve = new float[_model._size];

				Fill();				
				Normalize();
				FillMask();
				FindPower();
				Save();
			}

			void FillMask()
			{
				for (int fi = 0; fi < _model._size; fi++)
				{
					List<int> sis = new List<int>();

					for (int si = 0; si < _model._size; si++)
						if (_model._modelN[si, fi] > AP._spectrumModelThreshold)
							sis.add(si);

					_model._mask[fi] = sis.ToArray();
				}

				Logger.Log("Spectrum model mask was filled.");
			}

			void FindPower()
			{
				for (int fi = 0; fi < _model._size; fi++)
					for (int si = 0; si < _model._size; si++)
						_model._power[fi] += _model._model[fi, si];

				float[] gg = new float[AP.SpectrumSize];

				for (int index = 0; index < gg.Length; index++)
				{
					float widthN = AP._peakWidth_ForMultiNad_Gauss;
					widthN *= 1 / SpectrumFinder._frequenciesLg[index];
					float heighN = SpectrumFinder._fadeInLowMask[index];

					float widthL = AP._peakWidth_ForMultiNad_Gauss / AP._lc;
					widthL *= 1 / SpectrumFinder._frequenciesLg[index];
					float heighL = SpectrumFinder._fadeOutLowMask[index];

					gg[index] = widthN * heighN + widthL * heighL;
				}

				_model._maxesForColumns[0] = _model._maxesForColumns[1];

				for (int i = 0; i < _model._curve.Length; i++)
					_model._curve[i] = _model._maxesForColumns[i] / _model._max;

				_model._curve = ArrayE.SmoothArrayCopy(_model._curve, (int)(20 * AP.SpectrumSizeGG / 512f));

				float max = _model._curve.Max();

				for (int i = 0; i < _model._curve.Length; i++)
					_model._curve[i] = 1 / (_model._curve[i] / max);

				DiskE.WriteToProgramFiles("SpectrumPower", "csv", TextE.ToCsvString(_model._maxesForColumns, _model._power, gg, _model._curve), false);
				Logger.Log("Spectrum model powers were calculated.");
			}

			void Fill()
			{
				ProgressShower.Show("Generating spectrum model.");

				for (int fi = 0; fi < _model._size; fi++)
				{
					float frequency = SpectrumFinder._frequenciesLg[fi];
					float[] signal = new float[AP.FftSize * AP._lc];
					float[] signalLow = new float[AP.FftSize * AP._lc];

					for (int x = 0; x < AP.FftSize * AP._lc; x++)
					{
						float t = 1f * x / AP.SampleRate; //time in seconds
						signal[x] = MathF.Sin(2f * MathF.PI * frequency * t);
						signalLow[x] = signal[x];
					}

					float cutOff = 0.5f * (AP.SampleRate / AP._lc);

					signalLow = KaiserFilter.Make(signalLow, AP.SampleRate, cutOff, AP._kaiserFilterLength_ForProcessing, AP._kaiserFilterBeta, false);

					float[] spectrum = SpectrumFinder.Find(signal, signalLow, false);
					
					if (fi == 0)
						spectrum[5] = 0;

					int column = fi;

					for (int row = 0; row < _model._size; row++)
					{
						_model._model[row, column] = spectrum[row];
						_model._maxesForColumns[column] = Math.Max(_model._maxesForColumns[column], spectrum[row]);
					}

					_model._max = Math.Max(_model._max, _model._maxesForColumns[column]);

					ProgressShower.Set(1.0 * fi / _model._size);
				}

				ProgressShower.Close();
				Logger.Log("Spectrum model was generated.");				
			}
		}

		private static void Load()
		{
			string str = File.ReadAllText(_path);
			_model = JsonConvert.DeserializeObject<SpectrumModel>(str);
			Logger.Log("Spectrum model was loaded.", System.Windows.Media.Brushes.Cyan);
		}

		private static void Save()
		{
			string str = JsonConvert.SerializeObject(_model);
			File.WriteAllText(_path, str);
			Logger.Log("Spectrum model was saved.");
		}

		private static void Normalize()
		{
			for (int in_s = 0; in_s < _model._size; in_s++)
			{
				_model._modelN2[in_s] = new double[_model._size];
				_model._modelN3[in_s] = new float[_model._size];

				for (int in_f = 0; in_f < _model._size; in_f++)
				{
					_model._modelN[in_s, in_f] = _model._model[in_s, in_f] / _model._maxesForColumns[in_f];
					_model._modelN2[in_s][in_f] = _model._modelN[in_s, in_f];
				}
			}

			for (int in_s = 0; in_s < _model._size; in_s++)
				for (int in_f = 0; in_f < _model._size; in_f++)
					_model._modelN3[in_s][in_f] = _model._modelN[in_s, in_f];

			DiskE.WriteToProgramFiles("SpectrumModel", "csv", TextE.ToCsvString(_model._modelN3), false);			

			Logger.Log("Spectrum model was normalized.");
		}
	}
	//fifth in column & second in row =
	//fifth row & second column =
	//fifth row & second in row =
	//fifth in column & second column
}
