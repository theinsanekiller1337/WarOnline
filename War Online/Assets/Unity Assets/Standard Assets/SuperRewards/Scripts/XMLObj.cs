using System.Collections.Generic;

namespace Playerize
{
	/// <summary>
	/// For holding parsed XML data.
	/// </summary>
	class XMLNode
	{
		public XMLNode parent;
		public List<XMLNode> children;
		public string name;
		public string value;
		public Dictionary<string, string> attr;

		public XMLNode (XMLNode parent, string name, Dictionary<string, string> attributes)
		{
			this.parent = parent;
			this.name = name;
			this.attr = attributes;
			children = new List<XMLNode>();
		}

		public XMLNode this[string name]
		{
			get {
				foreach (var child in children) {
					if (child.name == name) {
						return child;
					}
				}

				return null;
			}

			set {
				for (var i = 0; i < children.Count; i++) {
					if (children[i].name == name) {
						children[i] = value;
						return;
					}
				}
			}
		}
	}

	class XMLObj : XMLParser.IContentHandler
	{
		Stack<XMLNode> nodes = new Stack<XMLNode>();
		List<XMLNode> finishedNodes = new List<XMLNode>();
		public XMLNode data { get; private set; }

		public XMLObj () {}

		public void OnEndParsing (XMLParser parser)
		{
			if (finishedNodes.Count > 0) {
				data = finishedNodes[finishedNodes.Count - 1];
			}
		}

		public void OnStartElement (string name, XMLParser.IAttrList attrs)
		{
			var attributes = new Dictionary<string, string>();

			foreach (var attr in attrs.Names) {
				attributes.Add(attr, attrs.GetValue(attr));
			}

			XMLNode parent = null;

			if (nodes.Count > 0) {
				parent = nodes.Peek();
			}

			var child = new XMLNode(parent, name, attributes);

			if (parent != null) {
				parent.children.Add(child);
			}

			nodes.Push(child);
		}

		public void OnEndElement (string name)
		{
			finishedNodes.Add(nodes.Pop());
		}

		// Process text contained by elements
		public void OnChars (string text)
		{
			var node = nodes.Peek();
			node.value = text;
		}

		public void OnStartParsing (XMLParser parser) {}
		public void OnProcessingInstruction (string name, string text) {}
		public void OnIgnorableWhitespace (string text) {}
	}
}
