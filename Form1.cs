using Microsoft.FlightSimulator.SimConnect;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace InputEventsTester
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    struct GetInputEventDouble
    {
        public double value;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    struct GetInputEventString
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
        public string value;
    }

    public partial class Form1 : Form
    {
        const int WM_USER_SIMCONNECT = 0x0402;
        enum RequestID
        {
            GetInputEvents = 1,
            GetInputEventValue = 2,
        }

        public enum InputEventType
        {
            DOUBLE,
            STRING
        }

        SimConnect? simConnect = null;

        public Form1()
        {
            InitializeComponent();
            SetButtonStates(false);
        }

        private void SetButtonStates(bool enabled)
        {
            getInputEventsButton.Enabled = simConnect != null;
            getParametersButton.Enabled = enabled;
            getCurrentValueButton.Enabled = enabled;
            submitButton.Enabled = enabled;
        }

        protected override void DefWndProc(ref Message m)
        {
            if (m.Msg == WM_USER_SIMCONNECT)
            {
                try
                {
                    simConnect?.ReceiveMessage();
                }
                catch
                {
                    Debug.WriteLine("Caught an exception when trying to process a SIMCONNECT message. This probably means the sim disconnected.");
                }
            }
            else
            {
                base.DefWndProc(ref m);
            }
        }

        private void ConnectButton_MouseClick(object sender, MouseEventArgs e)
        {
            try
            {
                simConnect = new SimConnect("Managed Data Request", this.Handle, WM_USER_SIMCONNECT, null, 0);
                simConnect.RegisterStruct<SIMCONNECT_RECV_GET_INPUT_EVENT, GetInputEventDouble>(InputEventType.DOUBLE);
                simConnect.RegisterStruct<SIMCONNECT_RECV_GET_INPUT_EVENT, GetInputEventString>(InputEventType.STRING);

                simConnect.OnRecvEnumerateInputEvents += SimConnect_OnRecvEnumerateInputEvents;
                simConnect.OnRecvGetInputEvent += SimConnect_OnRecvGetInputEvent;
                simConnect.OnRecvEnumerateInputEventParams += SimConnect_OnRecvEnumerateInputEventParams;
                Debug.WriteLine("Connected to simulator");
                SetButtonStates(false);
            }
            catch (COMException ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        private void SimConnect_OnRecvEnumerateInputEventParams(SimConnect sender, SIMCONNECT_RECV_ENUMERATE_INPUT_EVENT_PARAMS data)
        {
            parametersTextBox.Text = data.Value;
        }

        private void SimConnect_OnRecvGetInputEvent(SimConnect sender, SIMCONNECT_RECV_GET_INPUT_EVENT data)
        {
            try
            {
                currentValueTextBox.Text = ((GetInputEventDouble)data.Value[0]).value.ToString();
            }
            catch (COMException ex)
            {
                Debug.WriteLine($"{ex.Message}");
                currentValueTextBox.Text = "0";
            }
        }

        private void SimConnect_OnRecvEnumerateInputEvents(SimConnect sender, SIMCONNECT_RECV_ENUMERATE_INPUT_EVENTS data)
        {
            // This is gross but I hate winforms databinding and couldn't get it to work so I'm just gonna
            // bash all the items directly into the listbox.
            eventsComboBox.Items.Clear();

            foreach (object item in data.rgData)
            {
                if (item is SIMCONNECT_INPUT_EVENT_DESCRIPTOR descriptor)
                {
                    eventsComboBox.Items.Add(new InputEvent()
                    {
                        Name = descriptor.Name,
                        Hash = descriptor.Hash,
                    });
                }
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            simConnect?.Dispose();
        }

        private void GetInputEventsButton_Click(object sender, EventArgs e)
        {
            if (simConnect == null)
            {
                return;
            }

            simConnect.EnumerateInputEvents(RequestID.GetInputEvents);
        }

        private void NewValueTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Allow digits, decimal point, and negative sign (if needed)
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.') && (e.KeyChar != '-'))
            {
                e.Handled = true; // Mark the event as handled to suppress the character input.
            }
        }

        private void GetCurrentValueButton_Click(object sender, EventArgs e)
        {
            ulong hash = Helpers.StringToULong(hashTextBox.Text);

            if (hash == 0 || simConnect == null)
            {
                return;
            }

            simConnect.GetInputEvent(RequestID.GetInputEventValue, hash);
        }

        private void EventsComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            InputEvent selectedEvent = (InputEvent)eventsComboBox.SelectedItem;

            if (selectedEvent == null || selectedEvent.Hash == null)
            {
                return;
            }

            hashTextBox.Text = selectedEvent.Hash.ToString();
            SetButtonStates(!String.IsNullOrEmpty(hashTextBox.Text));
        }

        private void GetParametersButton_Click(object sender, EventArgs e)
        {
            ulong hash = Helpers.StringToULong(hashTextBox.Text);

            if (hash == 0 || simConnect == null)
            {
                return;
            }

            simConnect.EnumerateInputEventParams(hash);
        }

        private void SubmitButton_Click(object sender, EventArgs e)
        {
            ulong hash = Helpers.StringToULong(hashTextBox.Text);

            if (hash == 0 || simConnect == null)
            {
                return;
            }

            // This only works because for now there are no InputEvents that take strings. They're all doubles.
            var value = Convert.ToDouble(newValueTextBox.Text);

            simConnect.SetInputEvent(hash, value);
        }

        private void HashTextBox_TextChanged(object sender, EventArgs e)
        {
            SetButtonStates(!String.IsNullOrEmpty(hashTextBox.Text));
        }
    }
}