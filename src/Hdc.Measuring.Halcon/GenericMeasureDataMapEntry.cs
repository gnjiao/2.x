using System;
using System.Collections;
using Hdc.Mv.Inspection;

namespace Hdc.Measuring
{
    [Serializable]
    public class GenericMeasureDataMapEntry : IMeasureDataMapEntry
    {
        public string ResultType { get; set; }

        public int ResultIndex { get; set; }

        public string PropertyName { get; set; }

        public MeasureOutput GetMeasureData(InspectionResult inspectionResult)
        {
            try
            {
                var resultList = inspectionResult.GetPropertyValue<IList>(ResultType);
                var result = resultList[ResultIndex];

                var value = result.GetPropertyValue(PropertyName);

                MeasureOutput output = null;

                if (value is double)
                {
                    var doubleValue = Convert.ToDouble(value);
                    output = new MeasureOutput { Value = doubleValue };

                    string format = $"GetMeasureData: " +
                                    $"\n - Type: {ResultType}, " +
                                    $"\n - Index: {ResultIndex}, " +
                                    $"\n - Property: {PropertyName}, " +
                                    $"\n - Value: {doubleValue:00.000 mm}";
                    Console.WriteLine(format);
                }
                else if(value is string)
                {
                    var stringValue = Convert.ToString(value);
                    output = new MeasureOutput { Message = stringValue };

                    string format = $"GetMeasureData: " +
                                    $"\n - Type: {ResultType}, " +
                                    $"\n - Index: {ResultIndex}, " +
                                    $"\n - Property: {PropertyName}, " +
                                    $"\n - Message: {stringValue}";
                    Console.WriteLine(format);
                }
                else
                {
                    throw new InvalidOperationException($"GetMeasureData error: {PropertyName}");
                }

                return output;
            }
            catch (Exception ex)
            {
                var err = ex.Message;
                var output = new MeasureOutput { Value = 99.999, Validity = MeasureValidity.Error };

                string format = $"GetMeasureData: error" +
                               $"\n - Type: {ResultType}, " +
                               $"\n - Index: {ResultIndex}, " +
                               $"\n - Property: {PropertyName}, " +
                               $"\n - Value: 99.999";
                Console.WriteLine(format);
                return output;
            }

        }

        public int InspectionResultIndex { get; set; } = 0;
    }
}