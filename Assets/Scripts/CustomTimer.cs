using UnityEngine;
using System;
using System.Collections.Generic;

public class CustomTimer : MonoBehaviour
{
    // Timer class to hold data for each individual timer
    private class Timer
    {
        public float Duration;
        public Action Callback;
        public float CurrentTime;
        public bool IsRunning;
    }

    // List to keep track of all active timers
    private List<Timer> timers = new List<Timer>();

    // Method to start a new timer with a specified duration and action
    public void StartTimer(float duration, Action callback)
    {
        Timer newTimer = new Timer
        {
            Duration = duration,
            Callback = callback,
            CurrentTime = 0f,
            IsRunning = true
        };

        timers.Add(newTimer);
    }

    // Unity's Update method, called every frame
    void Update()
    {
        // Iterate through all timers
        for (int i = timers.Count - 1; i >= 0; i--)
        {
            Timer timer = timers[i];

            if (!timer.IsRunning) continue;

            timer.CurrentTime += Time.deltaTime;

            if (timer.CurrentTime >= timer.Duration)
            {
                timer.Callback?.Invoke();  // Call the callback when the timer completes
                timer.IsRunning = false;   // Stop the timer
                timers.RemoveAt(i);        // Remove the timer from the list after completion
            }
        }
    }

    // Reset all timers (optional, depending on your use case)
    public void ResetAllTimers()
    {
        timers.Clear();
    }

    // Reset a specific timer by providing a new duration and callback
    public void ResetTimer(float newDuration, Action newCallback)
    {
        StartTimer(newDuration, newCallback);  // Add a new timer, reusing StartTimer
    }

    // Check if any timer is still running
    public bool AreAnyTimersRunning()
    {
        foreach (var timer in timers)
        {
            if (timer.IsRunning)
                return true;
        }
        return false;
    }
}
