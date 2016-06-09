using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Management;  // <=== Add Reference required!!
using System.Diagnostics;
using KeyboardApi;

namespace KeyboardDemoBackground
{
    public partial class BackgroundKeyboardDemo : Form
    {
        public BackgroundKeyboardDemo()
        {
            InitializeComponent();
        }

        private void BackgroundKeyboardDemo_Shown(object sender, EventArgs e)
        {
            try
            {
                List<VKey> keys = "sent one key at a time".Select(c => new VKey(c)).ToList();
                string message = "hello world";
                var procId = Process.GetCurrentProcess().MainWindowHandle;
                Console.WriteLine("ID: " + procId);
                Console.WriteLine("Sending background keypresses to write \"hello world\"");

                Messaging.SendChatTextSend(procId, message);

                foreach (var key in keys)
                {
                    key.PressBackground(procId);
                }
            }
            catch (InvalidOperationException)
            {
            }
        }

        private void BackgroundKeyboardDemo_KeyPress(object sender, KeyPressEventArgs e)
        {
            richTextBox1.Text += e.KeyChar;
        }
    }
}
