using System.Globalization;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Ramsey.Utilities.UI
{
    public interface IInputVerifier
    {
        bool IsValid(string str, out string reason);
        object Parse(string str);

        public class None : IInputVerifier
        {
            public bool IsValid(string str, out string reason)
            {
                reason = null;
                return true;
            }

            public object Parse(string str) 
                => str;
        }

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

                if(!float.TryParse(str, NumberStyles.Float, CultureInfo.InvariantCulture, out var val))
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
                => float.Parse(str, NumberStyles.Float, CultureInfo.InvariantCulture);
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

                if(!int.TryParse(str, NumberStyles.Integer, CultureInfo.InvariantCulture, out var val))
                {
                    reason = "Invalid integer";
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
                => int.Parse(str, NumberStyles.Integer, CultureInfo.InvariantCulture);
        }
    }
}
