using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using UnityEngine;
using LocationType = NomaiText.Location;

namespace GhostTranslations
{
    internal class UntranslatableNode : XmlNode
    {
        public UntranslatableNode() : base()
        {
        }

        public override string Name => "Untranslatable";

        public override XmlNodeType NodeType => XmlNodeType.Text;

        public override string LocalName => "Untranslatable";

        public override string InnerText { get => UITextLibrary.GetString(UITextType.TranslatorUntranslatableWarning); set { } }

        public override string InnerXml => string.Empty;
        public override string OuterXml => string.Empty;

        public override bool IsReadOnly => true;

        public override string Value
        {
            get => string.Empty;
            set
            {
            }
        }

        public override System.Xml.XPath.XPathNavigator CreateNavigator() => null;

        public override XmlNode ParentNode => null;

        public override XmlNodeList ChildNodes => null;

        public override XmlNode PreviousSibling => null;

        public override XmlNode NextSibling => null;

        public override XmlAttributeCollection Attributes => null;

        public override XmlDocument OwnerDocument => null;

        public override XmlNode FirstChild => null;

        public override XmlNode LastChild => null;

        public override XmlNode InsertBefore(XmlNode newChild, XmlNode refChild) { return null; }

        public override XmlNode InsertAfter(XmlNode newChild, XmlNode refChild) { return null; }

        public override XmlNode ReplaceChild(XmlNode newChild, XmlNode oldChild) { return null; }

        public override XmlNode RemoveChild(XmlNode oldChild) { return null; }

        public override XmlNode PrependChild(XmlNode newChild) { return null; }

        public override XmlNode AppendChild(XmlNode newChild) { return null; }

        public override bool HasChildNodes => false;

        public override void Normalize() { }

        public override bool Supports(string feature, string version) => false;

        public override string NamespaceURI => string.Empty;

        public override string Prefix
        {
            get => string.Empty;
            set
            {
            }
        }

        public override XmlNode CloneNode(bool deep) => new UntranslatableNode();

        public override void WriteContentTo(XmlWriter w) { }

        public override void WriteTo(XmlWriter w) { }


        public override System.Xml.Schema.IXmlSchemaInfo SchemaInfo => null;

        public override string BaseURI => string.Empty;

        public override void RemoveAll() { }

        public override string GetNamespaceOfPrefix(string prefix) => string.Empty;

        public override string GetPrefixOfNamespace(string namespaceURI) => string.Empty;

        public override XmlElement this[string name] => null;

        public override XmlElement this[string localname, string ns] => null;

        public override XmlNode PreviousText => null;
    }

    public class NomaiXml
    {
        public class TextBlock
        {
            private int id = -1;
            private int parentId = -1;
            private LocationType location = LocationType.UNSPECIFIED;
            private string text = string.Empty;

            private StringBuilder builder = new StringBuilder();

            public TextBlock(int id, string text)
            {
                this.id = id;
                this.text = text;
            }

            public TextBlock(int id, int parentId, string text) : this(id, text)
            {
                this.parentId = parentId;
            }

            public TextBlock(int id, string text, LocationType location) : this(id, text)
            {
                this.location = location;
            }

            public TextBlock(int id, int parentId, string text, LocationType location) : this(id, parentId, text)
            {
                this.location = location;
            }

            public int Id
            {
                get => id;
                set => id = value;
            }
            public int ParentId
            {
                get => parentId;
                set => parentId = value;
            }
            public LocationType Location
            {
                get => location;
                set => location = value;
            }
            public string Text
            {
                get => text;
                set => text = value;
            }

            public override string ToString()
            {
                builder.Clear();
                builder.Append("<TextBlock>\n");
                builder.Append("<ID>");
                builder.Append(id);
                builder.Append("</ID>\n");
                if (parentId != -1)
                {
                    builder.Append("<ParentID>");
                    builder.Append(parentId);
                    builder.Append("</ParentID>\n");
                }
                if (location == LocationType.A)
                    builder.Append("<LocationA/>\n");
                else if (location == LocationType.B)
                    builder.Append("<LocationB/>\n");
                builder.Append("<Text>");
                builder.Append(text.Replace("<color=", "<![CDATA[<color=").Replace("</color>", "</color>]]>"));
                builder.Append("</Text>\n");
                builder.Append("</TextBlock>\n");
                return builder.ToString();
            }
        }

        public class ShipLogConditions
        {
            private LocationType location = LocationType.UNSPECIFIED;
            private List<RevealFact> revealFacts = new List<RevealFact>();

            public LocationType Location
            {
                get => location;
                set => location = value;
            }

            public List<RevealFact> RevealFacts
            {
                get => revealFacts;
                set => revealFacts = value;
            }

            private StringBuilder builder = new StringBuilder();

            public ShipLogConditions(List<RevealFact> revealFacts)
            {
                this.revealFacts = revealFacts;
            }

            public ShipLogConditions(LocationType location, List<RevealFact> revealFacts) : this(revealFacts)
            {
                this.location = location;
            }

            public ShipLogConditions(params RevealFact[] revealFacts) : this((revealFacts ?? new RevealFact[0]).ToList())
            {

            }

            public ShipLogConditions(LocationType location, params RevealFact[] revealFacts) : this(location, (revealFacts ?? new RevealFact[0]).ToList())
            {

            }

            public override string ToString()
            {
                builder.Clear();
                builder.Append("<ShipLogConditions>\n");
                if (location == LocationType.A)
                    builder.Append("<LocationA/>\n");
                else if (location == LocationType.B)
                    builder.Append("<LocationB/>\n");
                if (revealFacts != null)
                {
                    foreach (RevealFact fact in revealFacts)
                        builder.Append(fact.ToString());
                }
                builder.Append("</ShipLogConditions>\n");
                return builder.ToString();
            }

            public class RevealFact
            {
                private string factId = string.Empty;
                private List<int> conditions = new List<int>();

                private StringBuilder builder = new StringBuilder();

                public string FactId
                {
                    get => factId;
                    set => factId = value;
                }
                public List<int> Conditions
                {
                    get => conditions;
                    set => conditions = value;
                }

                public RevealFact(string factId, List<int> conditions)
                {
                    this.factId = factId;
                    this.conditions = conditions;
                }

                public RevealFact(string factId, params int[] conditions) : this(factId, (conditions ?? new int[0]).ToList())
                {
                }

                public override string ToString()
                {
                    builder.Clear();
                    builder.Append("<RevealFact>\n");
                    builder.Append("<FactID>");
                    builder.Append(factId);
                    builder.Append("</FactID>\n");
                    builder.Append("<Condition>");
                    if (conditions.Count == 0)
                        builder.Append(1);
                    else if (conditions.Count == 1)
                        builder.Append(conditions[0]);
                    else
                        builder.Append(string.Join(", ", conditions));
                    builder.Append("</Condition>\n");
                    builder.Append("</RevealFact>\n");
                    return builder.ToString();
                }
            }
        }

        private List<TextBlock> textBlocks = new List<TextBlock>();
        private List<ShipLogConditions> shipLogConditions = new List<ShipLogConditions>();

        public NomaiXml(List<TextBlock> textBlocks, List<ShipLogConditions> shipLogConditions) : this(textBlocks)
        {
            if (shipLogConditions != null)
                this.shipLogConditions = shipLogConditions;
        }

        public NomaiXml(List<TextBlock> textBlocks, ShipLogConditions shipLogConditions) : this(textBlocks, new List<ShipLogConditions>(1) { shipLogConditions })
        {
        }

        public NomaiXml(List<TextBlock> textBlocks)
        {
            if (textBlocks == null)
                throw new ArgumentNullException(nameof(textBlocks));

            this.textBlocks = textBlocks;
        }

        public NomaiXml(TextBlock[] textBlocks, ShipLogConditions[] shipLogConditions) : this((textBlocks ?? new TextBlock[0]).ToList(), (shipLogConditions ?? new ShipLogConditions[0]).ToList())
        {

        }

        public NomaiXml(TextBlock[] textBlocks, ShipLogConditions shipLogConditions) : this((textBlocks ?? new TextBlock[0]).ToList(), new List<ShipLogConditions>(1) { shipLogConditions })
        {

        }

        public NomaiXml(params TextBlock[] textBlocks) : this((textBlocks ?? new TextBlock[0]).ToList())
        {

        }

        public NomaiXml(TextBlock textBlock, ShipLogConditions shipLogConditions) : this(new List<TextBlock>(1) { textBlock }, new List<ShipLogConditions>(1) { shipLogConditions })
        {

        }

        public NomaiXml(TextBlock textBlock) : this(new List<TextBlock>(1) { textBlock })
        {

        }

        public NomaiXml(List<string> texts)
        {
            if (texts == null)
                throw new ArgumentNullException(nameof(texts));

            List<TextBlock> textBlocks = new List<TextBlock>(texts.Count);
            int i = 0;
            foreach (string text in texts)
            {
                int parent = i++;
                int current = i;
                if (parent == 0)
                    textBlocks.Add(new TextBlock(current, text));
                else
                    textBlocks.Add(new TextBlock(current, parent, text));
                i = current;
            }
            this.textBlocks = textBlocks;
        }

        public NomaiXml(params string[] texts) : this((texts ?? new string[0]).ToList())
        {

        }

        public NomaiXml(List<string> texts, List<ShipLogConditions> shipLogConditions) : this(texts)
        {
            if (shipLogConditions != null)
                this.shipLogConditions = shipLogConditions;
        }

        public NomaiXml(string[] texts, ShipLogConditions[] shipLogConditions) : this((texts ?? new string[0]).ToList(), (shipLogConditions ?? new ShipLogConditions[0]).ToList())
        {
        }

        public NomaiXml(string[] texts, ShipLogConditions shipLogConditions) : this((texts ?? new string[0]).ToList(), new List<ShipLogConditions>(1) { shipLogConditions })
        {
        }

        public NomaiXml(string text, ShipLogConditions shipLogConditions) : this(new TextBlock(1, text), shipLogConditions)
        {
        }

        public NomaiXml(string text) : this(new TextBlock(1, text))
        {
        }

        private StringBuilder builder = new StringBuilder();

        public override string ToString()
        {
            builder.Clear();
            builder.Append("<NomaiObject>\n");
            if (textBlocks != null)
            {
                foreach (TextBlock textBlock in textBlocks)
                    builder.Append(textBlock.ToString());
            }
            if (shipLogConditions != null)
            {
                foreach (ShipLogConditions shipLogConditions in shipLogConditions)
                    builder.Append(shipLogConditions.ToString());
            }
            builder.Append("</NomaiObject>");
            return builder.ToString();
        }

        public TextAsset ToTextAsset() => new TextAsset(this.ToString());

        public static explicit operator TextAsset(NomaiXml xml) => xml.ToTextAsset();

        public XmlDocument ToXmlDocument()
        {
            XmlDocument document = new XmlDocument();
            document.LoadXml(this.ToString());
            return document;
        }

        public static explicit operator XmlDocument(NomaiXml xml) => xml.ToXmlDocument();
    }
}
