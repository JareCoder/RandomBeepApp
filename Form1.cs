namespace BeepGUIApp;
using BeepGUIApp.Src;


public partial class Form1 : Form
{
    private BeepConfig currentConfig;
    private BeepService beepService = new BeepService();

    private NumericUpDown numericMinSeconds;
    private NumericUpDown numericMaxSeconds;
    private NumericUpDown numericBeepLength;
    private NumericUpDown numericBeepFreq;

    private Label appStatusLabel;
    private Label settingsSaveLabel;
    private Label beepCountLabel;

    private Button saveButton;
    private Button startButton;
    private Button stopButton;

    public Form1()
    {
        InitializeComponent();
        currentConfig = ConfigManager.Load();



        this.Text = "Random Beep App";

        appStatusLabel = new Label
        {
            Text = "Stopped!",
            Left = 10,
            Top = 10,
            Width = 230,
            ForeColor = Color.Red
        };

        this.Controls.Add(appStatusLabel);

        // Settings boxes
        Label labelMin = new Label { Text = "Min Seconds", Left = 10, Top = 40, Width = 100 };
        numericMinSeconds = new NumericUpDown { Name = "numericMinSeconds", Left = 120, Top = 40, Width = 100, Minimum = 1, Maximum = 600 };

        Label labelMax = new Label { Text = "Max Seconds", Left = 10, Top = 70, Width = 100 };
        numericMaxSeconds = new NumericUpDown { Name = "numericMaxSeconds", Left = 120, Top = 70, Width = 100, Minimum = 1, Maximum = 600 };

        Label labelLength = new Label { Text = "Beep Length (ms)", Left = 10, Top = 100, Width = 100 };
        numericBeepLength = new NumericUpDown { Name = "numericBeepLength", Left = 120, Top = 100, Width = 100, Minimum = 10, Maximum = 5000 };

        Label labelFreq = new Label { Text = "Beep Frequency (Hz)", Left = 10, Top = 130, Width = 100 };
        numericBeepFreq = new NumericUpDown { Name = "numericBeepFreq", Left = 120, Top = 130, Width = 100, Minimum = 37, Maximum = 32767 };

        this.Controls.AddRange(new Control[]
        {
            labelMin, numericMinSeconds,
            labelMax, numericMaxSeconds,
            labelLength, numericBeepLength,
            labelFreq, numericBeepFreq
        });

        // Settings save text and button
        settingsSaveLabel = new Label
        {
            Text = "",
            Left = 10,
            Top = 160,
            Width = 250,
            ForeColor = Color.Green,
            Visible = false
        };


        saveButton = new Button { Text = "Save", Left = 10, Top = 185, Width = 100 };
        saveButton.Click += SaveButton_Click;

        this.Controls.AddRange(new Control[]
        {
            settingsSaveLabel, saveButton
        });

        // Start and stop buttons & beep count label

        startButton = new Button { Text = "Start", Left = 120, Top = 185, Width = 100 };
        startButton.Click += StartButton_Click;

        stopButton = new Button { Text = "Stop", Left = 230, Top = 185, Width = 100, Enabled = false };
        stopButton.Click += StopButton_Click;

        beepCountLabel = new Label
        {
            Text = "Beep Count: 0",
            Left = 10,
            Top = 215,
            Width = 230,
            ForeColor = Color.Blue
        };

        this.Controls.AddRange([
            startButton, stopButton,
            beepCountLabel
        ]);

        DisplaySettings();

    }

    private void DisplaySettings()
    {
        numericMinSeconds.Value = currentConfig.MinSeconds;
        numericMaxSeconds.Value = currentConfig.MaxSeconds;
        numericBeepLength.Value = currentConfig.BeepLength;
        numericBeepFreq.Value = currentConfig.BeepFrequency;
    }

    private void SaveButton_Click(object? sender, EventArgs e)
    {
        currentConfig.MinSeconds = (int)numericMinSeconds.Value;
        currentConfig.MaxSeconds = (int)numericMaxSeconds.Value;
        currentConfig.BeepLength = (int)numericBeepLength.Value;
        currentConfig.BeepFrequency = (int)numericBeepFreq.Value;

        try
        {
            ConfigManager.Save(currentConfig);
            ShowSaveText("Settings saved!", settingsSaveLabel);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Failed to save settings: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void ShowSaveText(string message, Label label)
    {
        label.Text = message;
        label.Visible = true;

        // Timer to hide the label after 1.5 seconds
        var timer = new System.Windows.Forms.Timer();
        timer.Interval = 1500;
        timer.Tick += (s, e) =>
        {
            label.Visible = false;
            timer.Stop();
            timer.Dispose();
        };
        timer.Start();
    }

    private void UpdateAppStatus(bool isRunning)
    {
        appStatusLabel.Text = isRunning ? "Running..." : "Stopped!";
        appStatusLabel.ForeColor = isRunning ? Color.Green : Color.Red;

        saveButton.Enabled = !isRunning;
        startButton.Enabled = !isRunning;
        stopButton.Enabled = isRunning;
    }

    private void StartButton_Click(object? sender, EventArgs e)
    {
        UpdateAppStatus(true);

        beepCountLabel.Text = "Beep Count: 0";
        beepService.Start(currentConfig, (count) =>
        {
            this.Invoke(() =>
            {
                beepCountLabel.Text = $"Beep Count: {count}";
            });
        });
    }

    private void StopButton_Click(object? sender, EventArgs e)
    {
        beepService.Stop();
        UpdateAppStatus(false);
    }


}