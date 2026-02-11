using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace Rogue.Graphics
{
    public static class Shader
    {
        public static Matrix4 Orthogonal { get => field.Transposed(); set; }

        public static int CreateShader(string vertex, string fragment)
        {
            int vertexShader = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(vertexShader, vertex);

            int fragShader = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(fragShader, fragment);

            // Compiling Vertex Shader
            GL.CompileShader(vertexShader);

            int isCompiled, isLinked;
            int handle;

            GL.GetShader(vertexShader, ShaderParameter.CompileStatus, out isCompiled);
            if (isCompiled == 0)
            {
                Shader.PrintError(GL.GetShaderInfoLog(vertexShader));
                return -1;
            }

            // Compiling Fragment Shader
            GL.CompileShader(fragShader);

            GL.GetShader(fragShader, ShaderParameter.CompileStatus, out isCompiled);
            if (isCompiled == 0)
            {
                Shader.PrintError(GL.GetShaderInfoLog(fragShader));
                return -1;
            }

            handle = GL.CreateProgram();

            GL.AttachShader(handle, vertexShader);
            GL.AttachShader(handle, fragShader);

            GL.LinkProgram(handle);

            GL.GetProgram(handle, GetProgramParameterName.LinkStatus, out isLinked);
            if (isLinked == 0)
            {
                Shader.PrintError(GL.GetProgramInfoLog(handle));
                return -1;
            }

            // Cleanup
            GL.DetachShader(handle, vertexShader);
            GL.DetachShader(handle, fragShader);

            GL.DeleteShader(vertexShader);
            GL.DeleteShader(fragShader);
            
            return handle;
        }

        public static int GetAttributeLocation(int shader, string name)
        {
            return GL.GetAttribLocation(shader, name);
        }

        public static int GetUniformLocation(int shader, string name)
        {
            return GL.GetUniformLocation(shader, name);
        }

        private static void PrintError(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(message);
            Console.ForegroundColor = ConsoleColor.White;
        }
    }
}