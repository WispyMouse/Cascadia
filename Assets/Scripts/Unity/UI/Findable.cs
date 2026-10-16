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
        public static class TITLE
        {
            public static class CANVAS
            {
                /// <summary>
                /// The game object names for the title scene's windows.
                /// </summary>
                public static readonly string[] TITLEMENUWINDOWS =
                {
                    TITLE,
                    GAMESETUP,
                    SELECTBOARD,
                    CONFIGUREPLAYERS,
                    EDITPLAYER,

                    RULESPAGEONE,
                    RULESPAGETWO,
                };

                public const string TITLE = "[FINDABLE] Title Canvas";
                public const string GAMESETUP = "[FINDABLE] Game Setup Canvas";
                public const string SELECTBOARD = "[FINDABLE] Select Board Canvas";
                public const string CONFIGUREPLAYERS = "[FINDABLE] Configure Players Canvas";
                public const string EDITPLAYER = "[FINDABLE] Edit Player Canvas";
                public const string RULESPAGEONE = "[FINDABLE] Rules Page One";
                public const string RULESPAGETWO = "[FINDABLE] Rules Page Two";
            }
        };

        public const string GAMESETUPSTARTBUTTONNAME = "[FINDABLE] Game Setup Start Button";
        public const string GAMESETUPBACKBUTTONNAME = "[FINDABLE] Game Setup Back Button";

        public const string RULESBUTTONNAME = "[FINDABLE] Rules Button";
        public const string RULESCANVASNAME = "[FINDABLE] Rules Canvas";
        public const string RULESFROMONETOTWOBUTTONNAME = "[FINDABLE] Rules From One to Two";
        public const string RULESFROMTWOTOONEBUTTONNAME = "[FINDABLE] Rules From Two to One";
        public const string RULESRETURNBUTTONNAME = "[FINDABLE] Rules Return";

        public const string SELECTBOARDBUTTONNAME = "[FINDABLE] Select Board";
        public const string SELECTBOARDBACKBUTTONNAME = "[FINDABLE] Select Board Back Button";
    }
}
