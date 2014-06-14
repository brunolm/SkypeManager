using System;
using System.Collections.Generic;
using System.Linq;
using System.Speech.Synthesis;
using System.Text;
using System.Threading.Tasks;

namespace SkypeCommander
{
    public static class SpeechManager
    {
        public static void Speak(string text)
        {
            PromptBuilder builder = new PromptBuilder();

            builder.StartVoice(VoiceGender.Female, VoiceAge.Teen);
            builder.AppendText(text);
            builder.EndVoice();

            SpeechSynthesizer synthesizer = new SpeechSynthesizer();
            synthesizer.Speak(builder);
            synthesizer.Dispose();
        }
    }
}
