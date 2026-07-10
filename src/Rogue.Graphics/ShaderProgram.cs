
using System.Text;
using Veldrid;

namespace Rogue.Graphics
{
    public class ShaderProgram
    {
        public Shader VertexShader { get; }

        public Shader FragShader { get; }

        private const string _entryPoint = "main";

        public ShaderProgram(string vertexCode, string fragmentCode, ResourceFactory factory)
        {
            ShaderDescription vertexShader = new (ShaderStages.Vertex, Encoding.UTF8.GetBytes(vertexCode), _entryPoint);
            ShaderDescription fragShader = new (ShaderStages.Fragment, Encoding.UTF8.GetBytes(fragmentCode), _entryPoint);

            this.VertexShader = factory.CreateShader(vertexShader);
            this.FragShader = factory.CreateShader(fragShader);
        }

        public Shader[] ToArray() => [this.VertexShader, this.FragShader];
    }
}