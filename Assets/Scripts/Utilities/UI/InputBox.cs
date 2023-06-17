using TextInput = TMPro.TMP_InputField;
using Text = TMPro.TMP_Text;
using UnityEngine;

namespace Ramsey.Utilities.UI 
{
    public class InputBox
    {
        public Transform MainTransform => Textbox.transform;
        public GameObject GameObject => Textbox.gameObject;

        public TextInput Textbox { get; }

        public IInputVerifier Verifier { get; }
        public Text Title { get; }
        public Text Error { get; }

        public string InputRaw => Textbox.text;
        public bool InputValid => Verifier.IsValid(InputRaw, out _);
        public string InputInvalidReason 
        {
            get 
            {
                Verifier.IsValid(InputRaw, out var r);
                return r;
            }
        }

        public bool InputValidReason(out string reason)
            => Verifier.IsValid(InputRaw, out reason);

        public object Input => Verifier.Parse(InputRaw);

        public InputBox(GameObject head, string name, IInputVerifier verifier) 
        {
            Textbox = head.GetComponent<TextInput>();
            Verifier = verifier;

            Title = MainTransform.GetChild(1).GetComponent<Text>();
            Error = MainTransform.GetChild(2).GetComponent<Text>();

            Title.text = name;

            Error.text = "";

            Textbox.onEndEdit.AddListener(str => 
            {
                if(!InputValidReason(out var reason))
                {
                    Error.text = reason;
                }
            });

            Textbox.onValueChanged.AddListener(str => 
            {
                Error.text = "";
            });
        }
    }
}