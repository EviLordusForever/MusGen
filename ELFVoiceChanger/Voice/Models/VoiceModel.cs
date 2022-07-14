using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELFVoiceChanger.Voice.Models
{
	public class VoiceModel
	{
		public string name { get; set; }
		public List<LetterPattern> letterPatterns { get; set; }
	}
}
