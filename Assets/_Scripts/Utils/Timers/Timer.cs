using BetterTimers;
using System;
using UnityEngine;

public abstract class Timer : IDisposable
{
    #region Properties
    public float CurrentTime { get; set; }
    public bool IsRunning { get; private set; }

    public Action OnTimerStart = delegate { };
    public Action OnTimerStop = delegate { };

    public float Progress => Mathf.Clamp01(CurrentTime / initialTime);
    #endregion

    protected float initialTime;

    protected Timer(float value)
    {
        this.initialTime = value;
        CurrentTime = initialTime;
    }

    public void Start()
    {
        Reset();
        if (!IsRunning)
        {
            IsRunning = true;
            TimerManager.RegisterTimer(this);
            OnTimerStart.Invoke();
            ExtraStartOperation();
        }
    }

    public void Stop()
    {
        if (IsRunning)
        {
            IsRunning = false;
            TimerManager.DeregisterTimer(this);
            OnTimerStop.Invoke();
            ExtraStopOperation();
        }
    }

    public void Resume() => IsRunning = true;
    public void Pause() => IsRunning = false;

    public abstract void Tick();
    public abstract bool IsFinished { get; }


    public virtual void Reset() => CurrentTime = initialTime;
    public virtual void Reset(float newValue)
    {
        initialTime = newValue;
        Reset();
    }
    public virtual void ExtraStopOperation() { }
    public virtual void ExtraStartOperation() { }

    #region GarbageCollection
    bool disposed;
    ~Timer()
    {
        Dispose(false);
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposed) return;

        if (disposing)
        {
            TimerManager.DeregisterTimer(this);
        }

        disposed = true;
    }
    #endregion
}


/// <summary>
/// A timer that counts down from a starting time to zero.
/// Useful for cooldowns, ability durations, or delayed triggers.
/// </summary>
public class CountDownTimer : Timer
{
    public CountDownTimer(float value) : base(value) { }

    public override bool IsFinished => CurrentTime <= 0f;

    public override void Tick()
    {
        if (!IsRunning) return;
        CurrentTime -= Time.deltaTime;

        if (CurrentTime <= 0f)
        {
            Stop();
        }
    }
}


/// <summary>
/// A timer that waits for a delay period before beginning its countdown.
/// Useful for timed activation after a short wait (e.g., explosions, delayed events).
/// </summary>
public class DelayedTimer : Timer
{
    float delay;
    float waitTime;
    public DelayedTimer(float value, float delay) : base(value)
    {
        this.delay = delay;
        this.waitTime = delay;
    }

    public override bool IsFinished => CurrentTime <= 0f;
    public bool IsWaiting => waitTime > 0f;

    public override void Tick()
    {
        if (!IsRunning) return;
        if (waitTime >= 0f)
        {
            waitTime -= Time.deltaTime;
        }
        else
        {
            CurrentTime -= Time.deltaTime;

            if (CurrentTime <= 0f)
            {
                Stop();
            }
        }
    }

    public override void Reset()
    {
        waitTime = delay;
        CurrentTime = initialTime;
    }
}


/// <summary>
/// A repeating timer that automatically restarts after each completed loop.
/// Useful for periodic updates, AI ticks, or repeating actions.
/// If loopAmount is set to 0 or a negative number it will loop forever (int.MaxValue)
/// </summary>
public class LoopTimer : Timer
{
    public Action<int> OnLoop = delegate { };
    public LoopTimer(float value, int loopAmount) : base(value)
    {
        if (loopAmount <= 0)
            this.loopAmount = int.MaxValue;
        else
            this.loopAmount = loopAmount;
    }
    private int loopCount;
    private int loopAmount;

    public override bool IsFinished => loopCount >= loopAmount;

    public override void Tick()
    {
        if (!IsRunning) return;

        CurrentTime -= Time.deltaTime;

        if (loopCount >= loopAmount)
        {
            Stop();
        }

        if (CurrentTime <= 0f)
        {
            OnLoop.Invoke(loopCount);
            loopCount++;
            Reset();
        }
    }

    public override void ExtraStartOperation() => loopCount = 0;

}


/// <summary>
/// A timer that counts upward from zero to its target duration.
/// Useful for tracking elapsed time or measuring performance intervals.
/// </summary>
public class CountUpTimer : Timer
{
    public CountUpTimer(float value) : base(value)
    {
        CurrentTime = 0f;
    }

    public override bool IsFinished => CurrentTime >= initialTime;

    public override void Tick()
    {
        if (!IsRunning) return;
        CurrentTime += Time.deltaTime;

        if (CurrentTime >= initialTime)
        {
            Stop();
        }
    }


    public override void Reset() => CurrentTime = 0f;
}


/// <summary>
/// A timer that evaluates an <see cref="AnimationCurve"/> over a given duration,
/// providing smooth interpolated values over time.
/// 
/// This timer is often used for:
/// - Tweening values (e.g., fading alpha, scaling, moving UI elements)
/// - Smooth blending (e.g., transition curves, easing functions)
/// - Procedural effects (e.g., pulsing lights, gradual color changes)
/// 
/// The <see cref="OnValueChanged"/> event is invoked every frame while the timer runs,
/// sending the evaluated curve value (typically in the 0–1 range).
/// </summary>
public class CurveTimer : Timer
{
    private readonly AnimationCurve curve;
    public event Action<float> OnValueChanged = delegate { };

    public CurveTimer(float duration, AnimationCurve curve) : base(duration)
    {
        this.curve = curve;
        CurrentTime = 0f;
    }

    public override bool IsFinished => CurrentTime >= initialTime;

    public override void Tick()
    {
        if (!IsRunning) return;
        CurrentTime += Time.deltaTime;
        float value = curve.Evaluate(Progress);
        OnValueChanged.Invoke(value);

        if (IsFinished) Stop();
    }

    public override void Reset() => CurrentTime = 0f;
}

