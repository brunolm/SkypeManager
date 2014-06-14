using SkypeCommander;
using SkypeManager.Properties;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;

namespace SkypeManager
{
    public partial class FormMain : Form
    {
        public FormMain()
        {
            InitializeComponent();
        }

        private void FormMain_Load(object sender, EventArgs ev)
        {
            this.Location = Settings.Default.StartPosition;

            if (Settings.Default.StartSize.Width > 0
                && Settings.Default.StartSize.Height > 0)
            {
                this.Size = Settings.Default.StartSize;
            }

            try
            {
                bool isRunningAsAdmin = UacHelper.IsProcessElevated;

                SkypeCommands.Skype.Attach();
                SkypeCommands.LoadSettings();
                SpeechCommands.Start();

                //DebugTests();

                SkypeCommands.Skype.Error += SkypeCommands.OnError;
                SkypeCommands.Skype.MessageStatus += SkypeCommands.OnMessageStatus;
            }
            catch
            {
                MessageBox.Show("Execute this program as admin and allow in Skype", "Error");
                System.Windows.Forms.Application.Exit();
            }
        }

        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            Settings.Default.StartPosition = this.Location;
            Settings.Default.StartSize = this.Size;
            Settings.Default.Save();

            SpeechCommands.Stop();
        }


        private void DebugTests()
        {
            try
            {
                var chats = SkypeCommands.Skype.ActiveChats.OfType<SKYPE4COMLib.Chat>().ToList();
                var chat = chats.FirstOrDefault();

                if (chat == null)
                    throw new NullReferenceException("CHAT");

            }
            catch (Exception e)
            {
                txtLog.AppendText("" + e);
                txtLog.AppendText(Environment.NewLine);
            }
        }
    }
}
