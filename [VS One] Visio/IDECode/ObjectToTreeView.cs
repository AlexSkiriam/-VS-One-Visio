using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace _VS_One__Visio
{
    public static class ObjectToTreeView
    {
        private sealed class IndexContainer
        {
            private int _n;
            public int Inc() => _n++;
        }

        private static void FillTreeView(TreeNode node, JToken tok, Stack<IndexContainer> s, bool getOnlyPhrasesTree = false)
        {
            if (tok.Type == JTokenType.Object)
            {
                TreeNode n = node;
                if (tok.Parent != null)
                {
                    if (tok.Parent.Type == JTokenType.Property)
                    {
                        n = node.Nodes.Add($"{((JProperty)tok.Parent).Name}");
                    }
                    else
                    {
                        Match match = Regex.Match(tok.ToString(), @"""name""\s*:\s*""([^""]+)""");

                        string treeText = "";

                        if (tok.SelectToken("name") != null) treeText = tok.SelectToken("name").ToString();
                        else treeText = tok.ToString();

                        n = node.Nodes.Add(treeText);
                    }
                }
                s.Push(new IndexContainer());
                foreach (var p in tok.Children<JProperty>())
                {
                    FillTreeView(n, p.Value, s);
                }
                s.Pop();
            }
            else if (tok.Type == JTokenType.Array)
            {
                TreeNode n = node;
                if (tok.Parent != null)
                {
                    n = (tok.Parent.Type == JTokenType.Property) ? node.Nodes.Add($"{((JProperty)tok.Parent).Name}") : node.Nodes.Add($"[{s.Peek().Inc()}]");
                }
                s.Push(new IndexContainer());
                foreach (var p in tok)
                {
                    FillTreeView(n, p, s);
                }
                s.Pop();
            }
            else
            {
                var name = string.Empty;
                var value = JsonConvert.SerializeObject(((JValue)tok).Value);

                name = (tok.Parent.Type == JTokenType.Property) ? $"{((JProperty)tok.Parent).Name} : {value}" : $"[{s.Peek().Inc()}] : {value}";

                node.Nodes.Add(name);
            }
        }

        public static void SetObjectAsJson<T>(this TreeView tv, T obj, bool getOnlyPhrasesTree = false)
        {
            tv.BeginUpdate();
            try
            {
                tv.Nodes.Clear();

                var s = new Stack<IndexContainer>();
                s.Push(new IndexContainer());
                FillTreeView(tv.Nodes.Add("Объекты"), JsonConvert.DeserializeObject<JToken>(JsonConvert.SerializeObject(obj)), s);
                s.Pop();
            }
            finally
            {
                tv.EndUpdate();
            }
        }
    }
}
