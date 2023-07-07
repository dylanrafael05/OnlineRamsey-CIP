using Ramsey.Utilities.UI;

namespace Ramsey.Utilities.UI
{
    public struct TextParameter
    {
        /// <summary>
        /// The name of the parameter displayed on the main menu's sidebar.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// The verifier used to make sure the string you're inputting into the textbox is of the correct type you want.
        /// </summary>
        public IInputVerifier Verifier { get; set; }
        /// <summary>
        /// The value the string defaults to.
        /// </summary>
        public string DefaultValue { get; set; }
    }
}
