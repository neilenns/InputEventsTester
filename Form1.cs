using Microsoft.FlightSimulator.SimConnect;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text.Json;

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
        public List<InputEvent> events = new();
        bool subscribed = false;
        const int WM_USER_SIMCONNECT = 0x0402;
        string aircraftName = string.Empty;
        bool connected = false;

        enum RequestID
        {
            GetInputEvents,
            GetInputEventValue,
            GetAircraftTitle,
        }

        enum Definitions
        {
            Struct1
        }

        public enum InputEventType
        {
            DOUBLE,
            STRING
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private struct Struct1
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x100)]
            public string title;
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
            SaveButton.Enabled = enabled;
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
            // Not connected so connect
            if (simConnect == null && !connected)
            {
                try
                {
                    simConnect = new SimConnect("Managed Data Request", this.Handle, WM_USER_SIMCONNECT, null, 0);
                    simConnect.RegisterStruct<SIMCONNECT_RECV_GET_INPUT_EVENT, GetInputEventDouble>(InputEventType.DOUBLE);
                    simConnect.RegisterStruct<SIMCONNECT_RECV_GET_INPUT_EVENT, GetInputEventString>(InputEventType.STRING);

                    simConnect.AddToDataDefinition(Definitions.Struct1, "Title", null, SIMCONNECT_DATATYPE.STRING256, 0.0f, SimConnect.SIMCONNECT_UNUSED);
                    simConnect.RegisterDataDefineStruct<Struct1>(Definitions.Struct1);

                    simConnect.OnRecvOpen += SimConnect_OnRecvOpen;
                    simConnect.OnRecvEnumerateInputEvents += SimConnect_OnRecvEnumerateInputEvents;
                    simConnect.OnRecvGetInputEvent += SimConnect_OnRecvGetInputEvent;
                    simConnect.OnRecvEnumerateInputEventParams += SimConnect_OnRecvEnumerateInputEventParams;
                    simConnect.OnRecvSubscribeInputEvent += SimConnect_OnRecvSubscribeInputEvent;
                    simConnect.OnRecvSimobjectDataBytype += SimConnect_OnRecvSimobjectDataBytype;
                    simConnect.OnRecvException += SimConnect_OnRecvException;
                }
                catch (COMException ex)
                {
                    Debug.WriteLine(ex.Message);
                }
            }
            // Connected so disconnect
            else if (simConnect != null && connected)
            {
                connectButton.Text = "Connect";
                connected = false;
                simConnect = null;
                SetButtonStates(false);
            }
        }

        private void SimConnect_OnRecvException(SimConnect sender, SIMCONNECT_RECV_EXCEPTION data)
        {
            Debug.WriteLine($"Exception received: {(uint)data.dwException}");
        }

        private void SimConnect_OnRecvSimobjectDataBytype(SimConnect sender, SIMCONNECT_RECV_SIMOBJECT_DATA_BYTYPE data)
        {
            if (data.dwRequestID == (uint)RequestID.GetAircraftTitle)
            {
                Struct1 struct1 = (Struct1)data.dwData[0];

                aircraftName = struct1.title;
                Debug.WriteLine($"Current aircraft: {aircraftName}");
            }
        }

        private void SimConnect_OnRecvOpen(SimConnect sender, SIMCONNECT_RECV_OPEN data)
        {
            if (simConnect == null)
            {
                return;
            }

            Debug.WriteLine("Connected to simulator.");
            connectButton.Text = "Disconnect";
            connected = true;
            SetButtonStates(false);

            simConnect.RequestDataOnSimObjectType(RequestID.GetAircraftTitle, Definitions.Struct1, 0, SIMCONNECT_SIMOBJECT_TYPE.USER);
        }

        private void SimConnect_OnRecvSubscribeInputEvent(SimConnect sender, SIMCONNECT_RECV_SUBSCRIBE_INPUT_EVENT data)
        {
            switch (data.eType)
            {
                case SIMCONNECT_INPUT_EVENT_TYPE.DOUBLE:
                    {
                        double d = (double)data.Value[0];
                        string details = $"Received {events.FirstOrDefault(eventItem => eventItem.Hash == data.Hash)}: {d}";
                        Debug.WriteLine(details);
                        eventList.Items.Add(details);
                        break;
                    }
                case SIMCONNECT_INPUT_EVENT_TYPE.STRING:
                    {
                        SimConnect.InputEventString str = (SimConnect.InputEventString)data.Value[0];
                        string details = $"Received {events.FirstOrDefault(eventItem => eventItem.Hash == data.Hash)}: {str.value}";
                        Debug.WriteLine(details);
                        eventList.Items.Add(details);
                        break;
                    }
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
                    InputEvent newEvent = new InputEvent()
                    {
                        Name = descriptor.Name,
                        Hash = descriptor.Hash,
                    };
                    eventsComboBox.Items.Add(newEvent);
                    events.Add(newEvent);
                }
            }

            listenButton.Enabled = true;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
        }

        private void GetInputEventsButton_Click(object sender, EventArgs e)
        {
            if (simConnect == null)
            {
                return;
            }

            listenButton.Enabled = false;
            SaveButton.Enabled = true;
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

        private void ListenButton_Click(object sender, EventArgs e)
        {
            if (simConnect == null || events.Count == 0)
            {
                return;
            }

            if (subscribed)
            {
                listenButton.Text = "Subscribe";
                simConnect.UnsubscribeInputEvent(0);
                subscribed = false;
            }
            else
            {
                listenButton.Text = "Stop listening";
                simConnect.SubscribeInputEvent(0);
                subscribed = true;
            }
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            // Create a SaveFileDialog instance
            using SaveFileDialog saveFileDialog = new();

            // Set properties of the dialog
            saveFileDialog.Filter = "JSON Files (*.json)|*.json|All Files (*.*)|*.*"; // Filter for file types
            saveFileDialog.FileName = $"{aircraftName}.json"; // Default file name

            // Show the dialog
            DialogResult result = saveFileDialog.ShowDialog();

            // Check if the user clicked the 'Save' button
            if (result == DialogResult.OK)
            {
                // Get the selected file name
                string fileName = saveFileDialog.FileName;

                string json = JsonSerializer.Serialize(events, new JsonSerializerOptions
                {
                    WriteIndented = true,
                    Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
                });

                File.WriteAllText(fileName, json);
            }
        }
    }
}