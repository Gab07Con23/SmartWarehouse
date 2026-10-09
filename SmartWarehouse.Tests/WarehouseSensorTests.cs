using Microsoft.VisualStudio.TestTools.UnitTesting;
using SmartWarehouse.Core;

namespace SmartWarehouse.Tests;

[TestClass]
public class WarehouseSensorTests
{
    [TestMethod]
    public void Constructor_ValidInputs_InitializesCorrectly()
    {
        // Arrange
        string sensorId = "SENSOR-001";
        string locationTag = "Cold-Vault-01";

        // Act
        WarehouseSensor sensor = new WarehouseSensor(sensorId, locationTag);

        // Assert
        Assert.AreEqual("SENSOR-001", sensor.SensorId);
        Assert.AreEqual("Cold-Vault-01", sensor.LocationTag);
        Assert.AreEqual(0.0, sensor.CurrentTemperature);
        Assert.IsFalse(sensor.IsActive);
        Assert.IsFalse(sensor.IsAlertTriggered);
        Assert.AreEqual(4.0, sensor.CriticalThresholdCelsius);
    }

    [DataTestMethod]
    [DataRow(null)]
    [DataRow("")]
    [DataRow("   ")]
    public void Constructor_InvalidSensorId_ThrowsArgumentException(string? sensorId)
    {
        // Arrange
        string locationTag = "Cold-Vault-01";

        // Act
        Action act = () => new WarehouseSensor(sensorId!, locationTag);

        // Assert
        Assert.ThrowsExactly<ArgumentException>(act);
    }

    [TestMethod]
    [DataRow(null)]
    [DataRow("")]
    [DataRow("   ")]
    public void Constructor_InvalidLocationTag_ThrowsArgumentException(string? locationTag)
    {
        // Arrange
        string sensorId = "SENSOR-001";

        // Act
        Action act = () => new WarehouseSensor(sensorId, locationTag!);

        // Assert
        Assert.ThrowsExactly<ArgumentException>(act);
    }

     [TestMethod]
    public void Activate_InactiveSensor_SetsActiveToTrue()
    {
        // Arrange
        WarehouseSensor sensor = new WarehouseSensor("SENSOR-001", "Cold-Vault-01");

        // Act
        sensor.Activate();

        // Assert
        Assert.IsTrue(sensor.IsActive);
    }

    [TestMethod]
    public void Deactivate_ActiveSensor_SetsActiveToFalse()
    {
        // Arrange
        WarehouseSensor sensor = new WarehouseSensor("SENSOR-001", "Cold-Vault-01");
        sensor.Activate();

        // Act
        sensor.Deactivate();

        // Assert
        Assert.IsFalse(sensor.IsActive);
    }

    [TestMethod]
    public void Deactivate_AlertTriggeredSensor_ClearsAlert()
    {
        // Arrange
        WarehouseSensor sensor = new WarehouseSensor("SENSOR-001", "Cold-Vault-01");
        sensor.Activate();
        sensor.RecordReading(5.0);
        Assert.IsTrue(sensor.IsAlertTriggered);

        // Act
        sensor.Deactivate();

        // Assert
        Assert.IsFalse(sensor.IsActive);
        Assert.IsFalse(sensor.IsAlertTriggered);
    }

     [TestMethod]
    public void RecordReading_InactiveSensor_ThrowsInvalidOperationException()
    {
        // Arrange
        WarehouseSensor sensor = new WarehouseSensor("SENSOR-001", "Cold-Vault-01");

        // Act
        Action act = () => sensor.RecordReading(5.0);

        // Assert
        Assert.ThrowsExactly<InvalidOperationException>(act);
    }

    [TestMethod]
    [DataRow(3.9, false)]
    [DataRow(4.0, true)]
    [DataRow(4.1, true)]
    public void RecordReading_ValidTemperature_UpdatesAlert(double temperature, bool expectedAlert)
    {
        // Arrange
        WarehouseSensor sensor = new WarehouseSensor("SENSOR-001", "Cold-Vault-01");
        sensor.Activate();

        // Act
        sensor.RecordReading(temperature);

        // Assert
        Assert.AreEqual(temperature, sensor.CurrentTemperature);
        Assert.AreEqual(expectedAlert, sensor.IsAlertTriggered);
    }

     [TestMethod]
    [DataRow(-50.0)]
    [DataRow(80.0)]
    public void RecordReading_BoundaryTemperature_AcceptsReading(double temperature)
    {
        // Arrange
        WarehouseSensor sensor = new WarehouseSensor("SENSOR-001", "Cold-Vault-01");
        sensor.Activate();

        // Act
        sensor.RecordReading(temperature);

        // Assert
        Assert.AreEqual(temperature, sensor.CurrentTemperature);
    }

    [TestMethod]
    [DataRow(-50.1)]
    [DataRow(80.1)]
    public void RecordReading_OutOfRangeTemperature_ThrowsException(double temperature)
    {
        // Arrange
        WarehouseSensor sensor = new WarehouseSensor("SENSOR-001", "Cold-Vault-01");
        sensor.Activate();
        Action act = () => sensor.RecordReading(temperature);

        // Act

        // Assert
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(act);
    }

     [TestMethod]
    public void UpdateThreshold_RaisesThreshold_ClearsAlert()
    {
        // Arrange
        WarehouseSensor sensor = new WarehouseSensor("SENSOR-001", "Cold-Vault-01");
        sensor.Activate();
        sensor.RecordReading(10.0);

        // Act
        sensor.UpdateThreshold(15.0);

        // Assert
        Assert.AreEqual(15.0, sensor.CriticalThresholdCelsius);
        Assert.IsFalse(sensor.IsAlertTriggered);
    }

    [TestMethod]
    public void UpdateThreshold_LowersThreshold_TriggersAlert()
    {
        // Arrange
        WarehouseSensor sensor = new WarehouseSensor("SENSOR-001", "Cold-Vault-01");
        sensor.Activate();
        sensor.RecordReading(10.0);
        sensor.UpdateThreshold(15.0);

        // Act
        sensor.UpdateThreshold(5.0);

        // Assert
        Assert.AreEqual(5.0, sensor.CriticalThresholdCelsius);
        Assert.IsTrue(sensor.IsAlertTriggered);
    }

    [TestMethod]
    [DataRow(-30.0)]
    [DataRow(50.0)]
    public void UpdateThreshold_BoundaryValue_AcceptsThreshold(double threshold)
    {
        // Arrange
        WarehouseSensor sensor = new WarehouseSensor("SENSOR-001", "Cold-Vault-01");

        // Act
        sensor.UpdateThreshold(threshold);

        // Assert
        Assert.AreEqual(threshold, sensor.CriticalThresholdCelsius);
    }

    [TestMethod]
    [DataRow(-30.1)]
    [DataRow(50.1)]
    public void UpdateThreshold_OutOfRange_ThrowsException(double threshold)
    {
        // Arrange
        WarehouseSensor sensor = new WarehouseSensor("SENSOR-001", "Cold-Vault-01");

        // Act
        Action act = () => sensor.UpdateThreshold(threshold);

        // Assert
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(act);
    }
}