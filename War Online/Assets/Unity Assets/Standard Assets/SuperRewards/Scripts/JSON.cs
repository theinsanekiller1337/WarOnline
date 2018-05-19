using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Playerize
{
	enum JSONBinaryTag
	{
		Array       = 1,
		Class       = 2,
		Value       = 3,
		IntValue    = 4,
		DoubleValue = 5,
		BoolValue   = 6,
		FloatValue  = 7,
	}

	/// <summary>
	/// A simple JSON parser / builder.
	/// Based on SimpleJSON: http://wiki.unity3d.com/index.php/SimpleJSON
	/// </summary>
	static class JSON
	{
		public static JSONNode Parse (string json)
		{
			return JSONNode.Parse(json);
		}
	}

	class JSONNode
	{
		#region Common Interface

		public virtual void Add (string key, JSONNode item){ }
		public virtual JSONNode this[int index]  { get { return null; } set {} }
		public virtual JSONNode this[string key] { get { return null; } set {} }
		public virtual string Value              { get { return "";   } set {} }
		public virtual int Count                 { get { return 0;    } }

		public virtual void Add (JSONNode item)
		{
			Add("", item);
		}

		public virtual JSONNode Remove (string key) { return null; }
		public virtual JSONNode Remove (int index) { return null; }
		public virtual JSONNode Remove (JSONNode node) { return node; }

		public virtual IEnumerable<JSONNode> Children { get { yield break;} }

		public IEnumerable<JSONNode> DeepChildren
		{
			get
			{
				foreach (var child in Children) {
					foreach (var deepChild in child.DeepChildren) {
						yield return deepChild;
					}
				}
			}
		}

		public override string ToString ()
		{
			return "JSONNode";
		}

		public virtual string ToString (string prefix)
		{
			return "JSONNode";
		}

		#endregion

		#region Typecasting Properties

		public virtual int AsInt
		{
			get {
				int v = 0;
				int.TryParse(Value, out v);
				return v;
			}
			set { Value = value.ToString(); }
		}

		public virtual float AsFloat
		{
			get {
				float v = 0;
				float.TryParse(Value, out v);
				return v;
			}
			set { Value = value.ToString(); }
		}

		public virtual double AsDouble
		{
			get {
				double v = 0;
				double.TryParse(Value, out v);
				return v;
			}
			set { Value = value.ToString(); }
		}

		public virtual bool AsBool
		{
			get {
				bool v = false;

				if (bool.TryParse(Value, out v)) {
					return v;
				} else {
					// String cannot be parsed, so judge truthiness based on string contents.
					return !string.IsNullOrEmpty(Value);
				}
			}
			set { Value = (value) ? "true" : "false"; }
		}

		public virtual JSONArray AsArray
		{
			get { return this as JSONArray; }
		}

		public virtual JSONClass AsObject
		{
			get { return this as JSONClass; }
		}

		#endregion

		#region Operators

		public static implicit operator JSONNode (string s)
		{
			return new JSONData(s);
		}

		public static implicit operator string (JSONNode d)
		{
			return (d == null) ? null : d.Value;
		}

		public static bool operator == (JSONNode a, object b)
		{
			if (b == null && a is JSONLazyCreator) {
				return true;
			} else {
				return object.ReferenceEquals(a, b);
			}
		}

		public static bool operator != (JSONNode a, object b)
		{
			return !(a == b);
		}

		public override bool Equals (object obj)
		{
			return object.ReferenceEquals(this, obj);
		}

		public override int GetHashCode ()
		{
			return base.GetHashCode();
		}

		#endregion

		public static string Escape (string text)
		{
			string result = "";

			foreach (char c in text) {
				switch (c) {
					case '\\' : result += "\\\\"; break;
					case '\"' : result += "\\\""; break;
					case '\n' : result += "\\n" ; break;
					case '\r' : result += "\\r" ; break;
					case '\t' : result += "\\t" ; break;
					case '\b' : result += "\\b" ; break;
					case '\f' : result += "\\f" ; break;
					default   : result += c     ; break;
				}
			}

			return result;
		}

		public static JSONNode Parse (string json)
		{
			Stack<JSONNode> stack = new Stack<JSONNode>();
			JSONNode ctx = null;
			int i = 0;
			string token = "";
			string tokenName = "";
			bool quoteMode = false;

			while (i < json.Length) {
				switch (json[i]) {
					case '{':
						if (quoteMode) {
							token += json[i];
							break;
						}

						stack.Push(new JSONClass());

						if (ctx != null) {
							tokenName = tokenName.Trim();
							if (ctx is JSONArray) {
								ctx.Add(stack.Peek());
							} else if (tokenName != "") {
								ctx.Add(tokenName, stack.Peek());
							}
						}

						tokenName = "";
						token = "";
						ctx = stack.Peek();
						break;
					case '[':
						if (quoteMode) {
							token += json[i];
							break;
						}

						stack.Push(new JSONArray());

						if (ctx != null) {
							tokenName = tokenName.Trim();

							if (ctx is JSONArray) {
								ctx.Add(stack.Peek());
							} else if (tokenName != "") {
								ctx.Add(tokenName, stack.Peek());
							}
						}

						tokenName = "";
						token = "";
						ctx = stack.Peek();
						break;
					case '}':
					case ']':
						if (quoteMode) {
							token += json[i];
							break;
						}

						if (stack.Count == 0) {
							throw new Exception("JSON Parse: Too many closing brackets");
						}

						stack.Pop();

						if (token != "") {
							tokenName = tokenName.Trim();

							if (ctx is JSONArray) {
								ctx.Add(token);
							} else if (tokenName != "") {
								ctx.Add(tokenName, token);
							}
						}

						tokenName = "";
						token = "";

						if (stack.Count > 0) {
							ctx = stack.Peek();
						}

						break;
					case ':':
						if (quoteMode) {
							token += json[i];
							break;
						}

						tokenName = token;
						token = "";
						break;
					case '"':
						quoteMode ^= true;
						break;
					case ',':
						if (quoteMode) {
							token += json[i];
							break;
						}

						if (token != "") {
							if (ctx is JSONArray) {
								ctx.Add(token);
							} else if (tokenName != "") {
								ctx.Add(tokenName, token);
							}
						}

						tokenName = "";
						token = "";
						break;
					case '\r':
					case '\n':
						break;
					case ' ':
					case '\t':
						if (quoteMode) {
							token += json[i];
						}

						break;
					case '\\':
						++i;

						if (quoteMode) {
							char c = json[i];

							switch (c) {
								case 't': token += '\t'; break;
								case 'r': token += '\r'; break;
								case 'n': token += '\n'; break;
								case 'b': token += '\b'; break;
								case 'f': token += '\f'; break;
								case 'u':
									string s = json.Substring(i + 1, 4);
									token += (char)int.Parse(s, System.Globalization.NumberStyles.AllowHexSpecifier);
									i += 4;
									break;
								default: token += c; break;
							}
						}

						break;
					default:
						token += json[i];
						break;
				}

				++i;
			}

			if (quoteMode) {
				throw new Exception("JSON Parse: Quotation marks seems to be messed up.");
			}

			return ctx;
		}

		public virtual void Serialize (BinaryWriter writer) {}

		public static JSONNode Deserialize (BinaryReader reader)
		{
			JSONBinaryTag type = (JSONBinaryTag)reader.ReadByte();

			switch (type) {
				case JSONBinaryTag.Array:
					JSONArray arr = new JSONArray();

					for (int i = 0; i < reader.ReadInt32(); i++) {
						arr.Add(Deserialize(reader));
					}

					return arr;
				case JSONBinaryTag.Class:
					JSONClass cls = new JSONClass();

					for (int i = 0; i < reader.ReadInt32(); i++) {
						string key = reader.ReadString();
						var val = Deserialize(reader);
						cls.Add(key, val);
					}

					return cls;
				case JSONBinaryTag.Value:
					return new JSONData(reader.ReadString());
				case JSONBinaryTag.IntValue:
					return new JSONData(reader.ReadInt32());
				case JSONBinaryTag.DoubleValue:
					return new JSONData(reader.ReadDouble());
				case JSONBinaryTag.BoolValue:
					return new JSONData(reader.ReadBoolean());
				case JSONBinaryTag.FloatValue:
					return new JSONData(reader.ReadSingle());
				default:
					throw new Exception("Error deserializing JSON. Unknown tag: " + type);
			}
		}
	}

	class JSONArray : JSONNode, IEnumerable
	{
		List<JSONNode> m_List = new List<JSONNode>();

		public override JSONNode this[int index]
		{
			get {
				if (index < 0 || index >= m_List.Count) {
					return new JSONLazyCreator(this);
				}

				return m_List[index];
			}
			set {
				if (index < 0 || index >= m_List.Count) {
					m_List.Add(value);
				} else {
					m_List[index] = value;
				}
			}
		}

		public override JSONNode this[string key]
		{
			get { return new JSONLazyCreator(this); }
			set { m_List.Add(value); }
		}

		public override int Count
		{
			get { return m_List.Count; }
		}

		public override void Add (string key, JSONNode item)
		{
			m_List.Add(item);
		}

		public override JSONNode Remove (int index)
		{
			if (index < 0 || index >= m_List.Count) {
				return null;
			}

			JSONNode node = m_List[index];
			m_List.RemoveAt(index);
			return node;
		}

		public override JSONNode Remove (JSONNode node)
		{
			m_List.Remove(node);
			return node;
		}

		public override IEnumerable<JSONNode> Children
		{
			get {
				foreach (JSONNode node in m_List) {
					yield return node;
				}
			}
		}

		public IEnumerator GetEnumerator ()
		{
			foreach (JSONNode node in m_List) {
				yield return node;
			}
		}

		public override string ToString ()
		{
			string result = "[ ";

			foreach (JSONNode node in m_List) {
				if (result.Length > 2) {
					result += ", ";
				}

				result += node.ToString();
			}

			result += " ]";
			return result;
		}

		public override string ToString (string prefix)
		{
			string result = "[ ";

			foreach (JSONNode node in m_List) {
				if (result.Length > 3) {
					result += ", ";
				}

				result += "\n" + prefix + "   ";
				result += node.ToString(prefix + "   ");
			}

			result += "\n" + prefix + "]";
			return result;
		}

		public override void Serialize (BinaryWriter writer)
		{
			writer.Write((byte)JSONBinaryTag.Array);
			writer.Write(m_List.Count);

			for (int i = 0; i < m_List.Count; i++) {
				m_List[i].Serialize(writer);
			}
		}
	}

	class JSONClass : JSONNode, IEnumerable
	{
		Dictionary<string, JSONNode> m_Dict = new Dictionary<string, JSONNode>();

		public override JSONNode this[string key]
		{
			get {
				if (m_Dict.ContainsKey(key)) {
					return m_Dict[key];
				} else {
					return new JSONLazyCreator(this, key);
				}
			}
			set {
				if (m_Dict.ContainsKey(key)) {
					m_Dict[key] = value;
				} else {
					m_Dict.Add(key,value);
				}
			}
		}

		public override JSONNode this[int index]
		{
			get {
				if (index < 0 || index >= m_Dict.Count) {
					return null;
				}

				return m_Dict.ElementAt(index).Value;
			}
			set {
				if (index < 0 || index >= m_Dict.Count) {
					return;
				}

				string key = m_Dict.ElementAt(index).Key;
				m_Dict[key] = value;
			}
		}

		public override int Count
		{
			get { return m_Dict.Count; }
		}

		public override void Add (string key, JSONNode item)
		{
			if (!string.IsNullOrEmpty(key)) {
				if (m_Dict.ContainsKey(key)) {
					m_Dict[key] = item;
				} else {
					m_Dict.Add(key, item);
				}
			} else {
				m_Dict.Add(Guid.NewGuid().ToString(), item);
			}
		}

		public override JSONNode Remove (string key)
		{
			if (!m_Dict.ContainsKey(key)) {
				return null;
			}

			JSONNode node = m_Dict[key];
			m_Dict.Remove(key);
			return node;
		}

		public override JSONNode Remove (int index)
		{
			if (index < 0 || index >= m_Dict.Count) {
				return null;
			}

			var item = m_Dict.ElementAt(index);
			m_Dict.Remove(item.Key);
			return item.Value;
		}

		public override JSONNode Remove (JSONNode node)
		{
			try {
				var item = m_Dict.Where(k => k.Value == node).First();
				m_Dict.Remove(item.Key);
				return node;
			} catch {
				return null;
			}
		}

		public override IEnumerable<JSONNode> Children
		{
			get {
				foreach (KeyValuePair<string, JSONNode> kvp in m_Dict) {
					yield return kvp.Value;
				}
			}
		}

		public IEnumerator GetEnumerator ()
		{
			foreach (KeyValuePair<string, JSONNode> kvp in m_Dict) {
				yield return kvp;
			}
		}

		public override string ToString ()
		{
			string result = "{";

			foreach (KeyValuePair<string, JSONNode> kvp in m_Dict) {
				if (result.Length > 2) {
					result += ", ";
				}

				result += "\"" + Escape(kvp.Key) + "\":" + kvp.Value.ToString();
			}

			result += "}";
			return result;
		}

		public override string ToString (string prefix)
		{
			string result = "{ ";

			foreach (KeyValuePair<string, JSONNode> kvp in m_Dict) {
				if (result.Length > 3) {
					result += ", ";
				}

				result += "\n" + prefix + "   ";
				result += "\"" + Escape(kvp.Key) + "\" : " + kvp.Value.ToString(prefix + "   ");
			}

			result += "\n" + prefix + "}";
			return result;
		}

		public override void Serialize (BinaryWriter writer)
		{
			writer.Write((byte)JSONBinaryTag.Class);
			writer.Write(m_Dict.Count);

			foreach (string key in m_Dict.Keys) {
				writer.Write(key);
				m_Dict[key].Serialize(writer);
			}
		}
	}

	class JSONData : JSONNode
	{
		string m_Data;

		public override string Value
		{
			get { return m_Data; }
			set { m_Data = value; }
		}

		public JSONData (string data)
		{
			m_Data = data;
		}

		public JSONData (float data)
		{
			AsFloat = data;
		}

		public JSONData (double data)
		{
			AsDouble = data;
		}

		public JSONData (bool data)
		{
			AsBool = data;
		}

		public JSONData (int data)
		{
			AsInt = data;
		}

		public override string ToString ()
		{
			return "\"" + Escape(m_Data) + "\"";
		}

		public override string ToString (string prefix)
		{
			return "\"" + Escape(m_Data) + "\"";
		}

		public override void Serialize (BinaryWriter writer)
		{
			var data = new JSONData("");

			data.AsInt = AsInt;

			if (data.m_Data == this.m_Data) {
				writer.Write((byte)JSONBinaryTag.IntValue);
				writer.Write(AsInt);
				return;
			}

			data.AsFloat = AsFloat;

			if (data.m_Data == this.m_Data) {
				writer.Write((byte)JSONBinaryTag.FloatValue);
				writer.Write(AsFloat);
				return;
			}

			data.AsDouble = AsDouble;

			if (data.m_Data == this.m_Data) {
				writer.Write((byte)JSONBinaryTag.DoubleValue);
				writer.Write(AsDouble);
				return;
			}

			data.AsBool = AsBool;

			if (data.m_Data == this.m_Data) {
				writer.Write((byte)JSONBinaryTag.BoolValue);
				writer.Write(AsBool);
				return;
			}

			writer.Write((byte)JSONBinaryTag.Value);
			writer.Write(m_Data);
		}
	}

	class JSONLazyCreator : JSONNode
	{
		JSONNode m_Node = null;
		string m_Key = null;

		public JSONLazyCreator (JSONNode node)
		{
			m_Node = node;
			m_Key  = null;
		}

		public JSONLazyCreator (JSONNode node, string key)
		{
			m_Node = node;
			m_Key = key;
		}

		void Set (JSONNode aVal)
		{
			if (m_Key == null) {
				m_Node.Add(aVal);
			} else {
				m_Node.Add(m_Key, aVal);
			}

			m_Node = null; // Be GC friendly.
		}

		public override JSONNode this[int index]
		{
			get { return new JSONLazyCreator(this); }
			set {
				var arr = new JSONArray();
				arr.Add(value);
				Set(arr);
			}
		}

		public override JSONNode this[string key]
		{
			get { return new JSONLazyCreator(this, key); }
			set {
				var cls = new JSONClass();
				cls.Add(key, value);
				Set(cls);
			}
		}

		public override void Add (JSONNode item)
		{
			var arr = new JSONArray();
			arr.Add(item);
			Set(arr);
		}

		public override void Add (string key, JSONNode item)
		{
			var cls = new JSONClass();
			cls.Add(key, item);
			Set(cls);
		}

		public static bool operator == (JSONLazyCreator a, object b)
		{
			if (b == null) {
				return true;
			}

			return object.ReferenceEquals(a,b);
		}

		public static bool operator != (JSONLazyCreator a, object b)
		{
			return !(a == b);
		}

		public override bool Equals (object obj)
		{
			if (obj == null) {
				return true;
			}

			return object.ReferenceEquals(this, obj);
		}

		public override int GetHashCode ()
		{
			return base.GetHashCode();
		}

		public override string ToString ()
		{
			return "";
		}

		public override string ToString (string prefix)
		{
			return "";
		}

		public override int AsInt
		{
			get {
				JSONData data = new JSONData(0);
				Set(data);
				return 0;
			}
			set {
				JSONData data = new JSONData(value);
				Set(data);
			}
		}

		public override float AsFloat
		{
			get {
				JSONData data = new JSONData(0.0f);
				Set(data);
				return 0.0f;
			}
			set {
				JSONData data = new JSONData(value);
				Set(data);
			}
		}

		public override double AsDouble
		{
			get {
				JSONData data = new JSONData(0.0);
				Set(data);
				return 0.0;
			}
			set {
				JSONData data = new JSONData(value);
				Set(data);
			}
		}

		public override bool AsBool
		{
			get {
				JSONData data = new JSONData(false);
				Set(data);
				return false;
			}
			set {
				JSONData data = new JSONData(value);
				Set(data);
			}
		}

		public override JSONArray AsArray
		{
			get {
				JSONArray arr = new JSONArray();
				Set(arr);
				return arr;
			}
		}

		public override JSONClass AsObject
		{
			get {
				JSONClass cls = new JSONClass();
				Set(cls);
				return cls;
			}
		}
	}
}
