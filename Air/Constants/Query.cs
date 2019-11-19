namespace com.b_velop.stack.Air.Constants
{
    public class Query
    {
        public const string ActiveMeasurePoints = "{ activeMeasurePoints { id isActive point { id externId } } }";
        public const string MeasurePoints = "{ measurePoints {id externId } }";

        public const string CreateMeasureValue = "mutation CreateMeasureValue($value: Float!, $point: ID!) { createEasyMeasureValue(pointId: $point, value: $value) { id } }";

        public const string CreateMeasureValueBunch = "mutation InsertMeasureValueBunch($values: [Float]!, $points: [ID]!) { createMeasureValueBunch(values: $values, points: $points) { id } }";

        public const string UpdateBatteryStateBunch = "mutation UpdateBatteryStateBunch($states: [Boolean]!, $ids: [ID]!, $timestamps: [DateTimeOffset]!) { updateBatteryStateBunch(states: $states, ids: $ids, timestamps: $timestamps) { id } }";
    }
}