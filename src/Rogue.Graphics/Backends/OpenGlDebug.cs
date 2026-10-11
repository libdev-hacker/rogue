using System.Runtime.InteropServices;

using Veldrid.OpenGLBinding;

namespace Rogue.Graphics.Backends
{
    public static class OpenGlDebug
    {
        public static unsafe void DebugCallback(
            uint source,
            uint type,
            uint id,
            uint level,
            uint messageLength,
            byte* message
        )
        {
            string errorMessage = Marshal.PtrToStringAnsi((nint) message, (int) messageLength);
            ConsoleColor textColour = (DebugSeverity) level switch
            {
                DebugSeverity.DebugSeverityLow => ConsoleColor.Green,
                DebugSeverity.DebugSeverityMedium => ConsoleColor.Yellow,
                DebugSeverity.DebugSeverityHigh => ConsoleColor.Red,
                DebugSeverity.DebugSeverityNotification => ConsoleColor.White,
                _ => ConsoleColor.Magenta
            };

            errorMessage = $"OpenGl Message: source={(DebugSource) source}, type={(DebugType) type}, message={errorMessage}";
            
            OpenGlDebug.PrintMessage(textColour, errorMessage);
        }

        private static void PrintMessage(ConsoleColor colour, string message)
        {
            ConsoleColor previousColour = Console.ForegroundColor;
            Console.ForegroundColor = colour;
            Console.WriteLine(message);
            Console.ForegroundColor = previousColour;
        }
    }
}