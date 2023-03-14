using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    internal class OpenList
    {
        private List<ListEntry> openList;
        private Dictionary<Node, ListEntry> openDictionary;

        public OpenList()
        {
            openList = new List<ListEntry>();
            openDictionary = new Dictionary<Node, ListEntry>();
        }

        public void Sort()
        {
            openList.Sort(delegate (ListEntry x, ListEntry y)
            {
                return (x.Distance+x.s).CompareTo(y.Distance+y.s);
            });
        }

        public bool Add(ListEntry entry)
        {
            if (
                (openDictionary.ContainsKey(entry.NodeEntry)) || 
                (openList.Contains(entry))
               )
            {
                return false;
            }
                
            openDictionary.Add(entry.NodeEntry, entry);
            openList.Add(entry);

            return true;
        }

        public bool IsInOpen(Node n)
        {
            return openDictionary.ContainsKey(n);
        }

        public ListEntry GetBest()
        {
            if(openList.Count <= 0)
                return null;

            Sort();
            ListEntry result = openList[0];
            openList.RemoveAt(0);
            openDictionary.Remove(result.NodeEntry);
            
            return result;
        }

        public ListEntry Get(Node n)
        {
            return openDictionary[n];
        }
    }
}
