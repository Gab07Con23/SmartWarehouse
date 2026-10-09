namespace SmartWarehouse.Core;


public class WarehouseSensor
{
    public string SensorId { get; private set; }

    public string LocationTag{ get; private set; }

    public double CurrentTemperature { get; private set; }

    public bool IsActive { get; private set; }

    public bool IsAlertTriggered { get; private set; }

    public double CriticalThresholdCelsius { get; private set; }


    public WarehouseSensor(
        string sensorId,
        string locationTag,
        double criticalThreshold = 4.0)
    {
        if (string.IsNullOrWhiteSpace(sensorId))
            {
                throw new ArgumentException("Sensor  ID cannot be empty.");
            }        
        
        if (string.IsNullOrWhiteSpace(locationTag))
            {
                throw new ArgumentException("Location tag cannot be empty.");
            }
        
        SensorId = sensorId;
        LocationTag = locationTag;

        CurrentTemperature = 0.0;
        IsActive = false;
        IsAlertTriggered = false;

        CriticalThresholdCelsius = criticalThreshold;
    }

    public void Activate()
    {
        IsActive = true;
    }

    public void Deactivate()
    {
        IsActive = false;
        IsAlertTriggered = false;
    }

    public void RecordReading(double newTemperature)
    {
        if (!IsActive)
        {
            throw new InvalidOperationException("Sensor must be active.");
        }

        if (newTemperature < -50.0 || newTemperature > 80.0)
        {
            throw new ArgumentOutOfRangeException(nameof(newTemperature));
        }

        CurrentTemperature = newTemperature;

        if (newTemperature >= CriticalThresholdCelsius)
        {
            IsAlertTriggered = true;
        }
        else
        {
            IsAlertTriggered = false;
        }
    }

    public void UpdateThreshold(double newThreshold)
    {
        if (newThreshold < -30.0 || newThreshold > 50.0)
        {
            throw new ArgumentOutOfRangeException(nameof(newThreshold));
        }

        CriticalThresholdCelsius = newThreshold;

        IsAlertTriggered = 
            CurrentTemperature >= CriticalThresholdCelsius;
    }
}
