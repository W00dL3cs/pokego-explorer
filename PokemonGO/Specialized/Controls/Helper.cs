using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PokemonGO.Specialized.Controls
{
    public static class Helper
    {
        private delegate void SetBoolCallback(Control Control, bool Value);
        private delegate void SetStringCallback(Control Control, string Value);

        public static void ChangeText(Control Control, string Value)
        {
            try
            {
                if (Control.InvokeRequired)
                {
                    var Callback = new SetStringCallback(ChangeText);

                    Control.Invoke(Callback, Control, Value);
                }
                else
                {
                    Control.Text = Value;
                }
            }
            catch { }
        }

        public static void SetVisible(Control Control, bool Visible = true)
        {
            try
            {
                if (Control.InvokeRequired)
                {
                    var Callback = new SetBoolCallback(SetVisible);

                    Control.Invoke(Callback, Control, Visible);
                }
                else
                {
                    Control.Visible = Visible;
                }
            }
            catch { }
        }

        internal static void SetEnabled(Control Control, bool Enabled = true)
        {
            try
            {
                if (Control.InvokeRequired)
                {
                    var Callback = new SetBoolCallback(SetEnabled);

                    Control.Invoke(Callback, Control, Enabled);
                }
                else
                {
                    Control.Visible = Enabled;
                }
            }
            catch { }
        }
    }
}
