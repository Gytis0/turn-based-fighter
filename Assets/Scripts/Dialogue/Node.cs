using System.Collections.Generic;
public class Node
{
    public Node(string _guid, string _text, List<string> _options)
    {
        GUID = _guid;
        dialogueText = _text;
        options = _options;
    }
    public Node(string _guid, string _text)
    {
        GUID = _guid;
        dialogueText = _text;
    }
    public Node(string _guid)
    {
        GUID = _guid;
    }
    public Node()
    {
        dialogueText = "HEAD";
    }

    public string GUID;
    public string dialogueText;
    public List<string> options = new List<string>();
    public List<Node> nexts = new List<Node>();
}
