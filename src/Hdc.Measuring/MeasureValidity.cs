namespace Hdc.Measuring
{
    public enum MeasureValidity
    {
        Valid,
        Alarm, // over range
        Wait,
        Error,
    }
}