namespace Cascadia.Unity.UI
{
    using UnityEngine;
    using UnityEngine.EventSystems;

    /// <summary>
    /// The UIToolkit can find these buttons during tests.
    /// The assumption is that the button will have a predictable name, and that this will report on that.
    /// That helps the toolkit find buttons based on strings.
    /// </summary>
    public class Findable : MonoBehaviour
    {
        public const string STARTBUTTONNAME = "[FINDABLE] Start Button";
        public const string TITLECANVASNAME = "[FINDABLE] Title Canvas";
        public const string SELECTPLAYMODECANVASNAME = "[FINDABLE] Select Play Mode Canvas";
        public const string SELECTBOARDCANVASNAME = "[FINDABLE] Select Board Canvas";
        public const string CONFIGUREPLAYERSCANVASNAME = "[FINDABLE] Configure Players Canvas";
        public const string EDITPLAYERCANVASNMAE = "[FINDABLE] Edit Player Canvas";

        public const string RULESBUTTONNAME = "[FINDABLE] Rules Button";
        public const string RULESCANVASNAME = "[FINDABLE] Rules Canvas";
        public const string RULESPAGEONENAME = "[FINDABLE] Rules Page One";
        public const string RULESPAGETWONAME = "[FINDABLE] Rules Page Two";
        public const string RULESFROMONETOTWOBUTTONNAME = "[FINDABLE] Rules From One to Two";
        public const string RULESFROMTWOTOONEBUTTONNAME = "[FINDABLE] Rules From Two to One";
        public const string RULESRETURNBUTTONNAME = "[FINDABLE] Rules Return";
    }
}
