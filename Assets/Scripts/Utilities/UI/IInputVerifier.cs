namespace Ramsey.Utilities.UI
{
    public interface IInputVerifier
    {
        bool IsValid(string str, out string reason);
        object Parse(string str);

        public class Float : IInputVerifier
        {
            public Float(float? min = null, float? max = null)
            {
                Min = min;
                Max = max;
            }

            public float? Min { get; }
            public float? Max { get; }

            public bool IsValid(string str, out string reason) 
            {
                reason = null;

                if(!float.TryParse(str, out var val))
                {
                    reason = "Invalid number";
                    return false;
                }

                if(Min != null && val < Min) 
                {
                    reason = "Must be greater than " + Min;
                    return false;
                }
                if(Max != null && val > Max) 
                {
                    reason = "Must be less than " + Max;
                    return false;
                }

                return true;
            }

            public object Parse(string str)
                => float.Parse(str);
        }

        public class Integer : IInputVerifier
        {
            public Integer(int? min = null, int? max = null)
            {
                Min = min;
                Max = max;
            }

            public int? Min { get; }
            public int? Max { get; }

            public bool IsValid(string str, out string reason) 
            {
                reason = null;

                if(!int.TryParse(str, out var val))
                {
                    reason = "Invalid number";
                    return false;
                }

                if(Min != null && val < Min) 
                {
                    reason = "Must be greater than " + Min;
                    return false;
                }
                if(Max != null && val > Max) 
                {
                    reason = "Must be less than " + Max;
                    return false;
                }

                return true;
            }

            public object Parse(string str)
                => int.Parse(str);
        }
    }
}
