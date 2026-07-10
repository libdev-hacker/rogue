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
            if (level != (uint) DebugSeverity.DebugSeverityNotification)
            {
                ConsoleColor textColour = (DebugSeverity) level switch
                {
                    DebugSeverity.DebugSeverityLow => ConsoleColor.White,
                    DebugSeverity.DebugSeverityMedium => ConsoleColor.Yellow,
                    DebugSeverity.DebugSeverityHigh => ConsoleColor.Red,
                    _ => ConsoleColor.White
                };

                errorMessage = $"OpenGl Message: source={(DebugSource) source}, type={(DebugType) type}, message={errorMessage}";
                
                OpenGlDebug.PrintMessage(textColour, errorMessage);
            }
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