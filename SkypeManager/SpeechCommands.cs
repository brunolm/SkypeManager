namespace SkypeManager
{
    using System;
    using SKYPE4COMLib;
    using SkypeCommander;
    using System.ComponentModel;
    using System.Linq;
    using System.Speech.Recognition;
    using System.IO;
    using System.Reflection;
    using System.Collections.Generic;

    public class SpeechCommands
    {
        public static SpeechRecognitionEngine mainSpeech;
        public static Grammar mainGrammar;
        public static Grammar commandsGrammar;

        public enum VoiceCommands
        {
            [Description("Bot I Summon Thee")]
            BotISummonThee,
            [Description("Fuck")]
            Fuck,
            [Description("Goodbye bot")]
            Exit,
        }

        private static void LoadMainGrammar()
        {
            Choices choices = new Choices();
            choices.Add(VoiceCommands.BotISummonThee.GetDescription());
            mainGrammar = new Grammar(new GrammarBuilder(choices));
        }

        private static void LoadCommandsGrammar()
        {
            Choices choices = new Choices();

            var allVoiceCommands = typeof(VoiceCommands).GetEnumValuesWithDescription<VoiceCommands>();

            foreach (var item in allVoiceCommands)
            {
                if (item.Key != VoiceCommands.BotISummonThee)
                    choices.Add(item.Value);
            }

            commandsGrammar = new Grammar(new GrammarBuilder(choices));
        }

        public static void Start()
        {
            LoadMainGrammar();
            LoadCommandsGrammar();

            mainSpeech = new SpeechRecognitionEngine();
            mainSpeech.LoadGrammar(mainGrammar);
            mainSpeech.SetInputToDefaultAudioDevice();
            mainSpeech.SpeechRecognized += mainSpeech_SpeechRecognized;

            mainSpeech.RecognizeAsync(RecognizeMode.Multiple);
        }
        public static void Stop()
        {
            try
            {
                mainSpeech.UnloadAllGrammars();
                mainSpeech.RecognizeAsyncStop();
            }
            catch { }
        }

        static void mainSpeech_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            if (!mainSpeech.Grammars.Contains(commandsGrammar))
            {
                mainSpeech.LoadGrammar(commandsGrammar);
                SpeechManager.Speak("Systems online");
                return;
            }

            var chat = SkypeCommands.Skype.ActiveChats.OfType<IChat>().FirstOrDefault();

            if (chat == null)
                return;

            var commandKv = typeof(VoiceCommands).GetEnumValuesWithDescription<VoiceCommands>()
                .FirstOrDefault(o => o.Value == e.Result.Text);

            var command = commandKv.Key;

            switch (command)
            {
                case VoiceCommands.Fuck:
                    SpeechManager.Speak("Hey! You should not say fuck!");
                    break;
                case VoiceCommands.Exit:
                    mainSpeech.UnloadGrammar(commandsGrammar);
                    SpeechManager.Speak("Systems offline");
                    break;
            }
        }
    }
}
