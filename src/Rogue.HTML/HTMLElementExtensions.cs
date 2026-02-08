using System.Diagnostics;
using System.Xml;

namespace Rogue.HTML
{
    public static class HTMLElementExtensions
    {
        public static void PopulateAttributes(this HTMLElement element, XmlReader reader)
        {
            if (reader.HasAttributes)
            {
                for (int i = 0; i < reader.AttributeCount; i++)
                {
                    reader.MoveToAttribute(i);
                    element.AddAttribute(reader.Name, reader.Value);
                }
                reader.MoveToElement();
            }
        }
    }
}