using Ramsey.Utilities.UI;

namespace Ramsey.UI
{
    public struct TextParameter
    {
        public string Name { get; set; }
        public IInputVerifier Verifier { get; set; }
        public string DefaultValue { get; set; }
    }
}
