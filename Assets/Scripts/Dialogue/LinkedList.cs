using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CustomUtility
{
    public class LinkedList
    {
        public Node head;
        public Dictionary<string, Node> nodes = new Dictionary<string, Node>();
        Queue<Node> upcomingNodes = new Queue<Node>();

        public LinkedList(Subtegral.DialogueSystem.DataContainers.DialogueContainer dialogue)
        {
            nodes.Add(dialogue.NodeLinks[0].BaseNodeGUID, new Node(dialogue.NodeLinks[0].BaseNodeGUID, "HEAD", new List<string>()));
            head = nodes[dialogue.NodeLinks[0].BaseNodeGUID];

            upcomingNodes.Enqueue(head);
            Node currentNode, tempNode;

            while(upcomingNodes.Count > 0)
            {
                currentNode = upcomingNodes.Dequeue();

                foreach (var node in dialogue.DialogueNodeData)
                {
                    if(node.NodeGUID == currentNode.GUID)
                    {
                        currentNode.dialogueText = node.DialogueText;
                        break;
                    }
                }

                foreach (var link in dialogue.NodeLinks)
                {
                    if (link.BaseNodeGUID == currentNode.GUID)
                    {
                        currentNode.options.Add(link.PortName);

                        if(!nodes.ContainsKey(link.TargetNodeGUID))
                        {
                            tempNode = new Node(link.TargetNodeGUID);

                            upcomingNodes.Enqueue(tempNode);
                            nodes.Add(tempNode.GUID, tempNode);
                        }

                        currentNode.nexts.Add(nodes[link.TargetNodeGUID]); ;


                    }
                }
            }
        }
    }
}


