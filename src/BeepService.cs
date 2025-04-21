using System;
using System.Threading;
using System.Threading.Tasks;

namespace BeepGUIApp.Src
{
    public class BeepService
    {
        private CancellationTokenSource? cancellationTokenSource;
        private Task? beepTask;
        private int beepCount;

        public bool IsRunning => beepTask != null && !beepTask.IsCompleted;

        public void Start(BeepConfig config, Action<int>? onBeep = null)
        {
            if (IsRunning) return;

            cancellationTokenSource = new CancellationTokenSource();
            beepTask = Task.Run(() =>
                {
                    Random rng = new Random();
                    beepCount = 0;

                    try
                    {
                        while (!cancellationTokenSource.Token.IsCancellationRequested)
                        {
                            int waitTime = rng.Next(config.MinSeconds, config.MaxSeconds + 1);
                            Thread.Sleep(waitTime * 1000);
                            Console.Beep(config.BeepFrequency, config.BeepLength);
                            beepCount++;
                            onBeep?.Invoke(beepCount);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"BeepService Error: {ex.Message}");
                    }
                }
            );
        }

        public void Stop()
        {
            if (cancellationTokenSource != null)
            {
                cancellationTokenSource.Cancel();
                cancellationTokenSource.Dispose();
                cancellationTokenSource = null;
            }

            beepTask = null;
        }
    }
}