using System;
using System.IO;

public class KeyValuePair
{
    public string Key { get; }
    public string Value { get; }

    public KeyValuePair(string key, string value)
    {
        Key = key;
        Value = value;
    }
}

public class LinkedListNode
{
    public KeyValuePair Pair { get; }
    public LinkedListNode Next { get; set; }
    public LinkedListNode Prev { get; set; }

    public LinkedListNode(KeyValuePair pair, LinkedListNode next = null, LinkedListNode prev = null)
    {
        Pair = pair;
        Next = next;
        Prev = prev;
    }
}

public class LinkedList
{
    public LinkedListNode _first;
    public LinkedListNode _last;

    public void Add(KeyValuePair pair)
    {
        if (_first == null)
        {
            _first = new LinkedListNode(pair);
            _last = _first;
            return;
        }

        LinkedListNode tmp  = new LinkedListNode(pair, null, _last);
        _last.Next = tmp;
        _last = tmp;
    }

    public void RemoveByKey(string key)
    {
        if (_first == null)
            return;

        if (_first.Pair.Key == key)
        {
            _first = _first.Next;
            if (_first != null)
            {
                _first.Prev = null;
            }
            else
            {
                _last = null;
            }
            return;
        }

        LinkedListNode curr = _first;
        while (curr.Next != null)
        {
            if (curr.Next.Pair.Key == key)
            {
                curr.Next = curr.Next.Next;
                if (curr.Next != null)
                {
                    curr.Next.Prev = curr;
                }
                else
                {
                    _last = curr;
                }
                return;
            }
            curr = curr.Next;
        }
    }

    public KeyValuePair GetItemWithKey(string key)
    {
        LinkedListNode curr = _first;
        while (curr != null)
        {
            if (curr.Pair.Key == key) return curr.Pair;
            curr = curr.Next;
        }
        return null;
    }
}

public class StringsDictionary
{
    private const int InitialSize = 10;
    private LinkedList[] _buckets = new LinkedList[InitialSize];
    private const double LF = 0.75;

    private int _c;

    public void Add(string key, string value)
    {
        int i = Math.Abs(CalculateHash(key)) % _buckets.Length;

        if (i >= _buckets.Length)
        {
            Array.Resize(ref _buckets, i + 1);
        }

        if (_buckets[i] == null)
        {
            _buckets[i] = new LinkedList();
        }
        _buckets[i].Add(new KeyValuePair(key, value));
        _c++;
	
        if ((double)_c / _buckets.Length >= LF)
        {
            Resize();
        }
    }

    private void Resize()
    {
        int nsize = _buckets.Length * 2;
        LinkedList[] nBuckets = new LinkedList[nsize];

        foreach (var buck in _buckets)
        {
            if (buck != null)
            {
                LinkedListNode curr = buck._first;
                while (curr != null)
                {
                    int nK = Math.Abs(curr.Pair.Key.GetHashCode()) % nsize;
                    if (nBuckets[nK] == null)
                    {
                        nBuckets[nK] = new LinkedList();
                    }
                    nBuckets[nK].Add(curr.Pair);
                    curr = curr.Next;
                }
            }
        }

        _buckets = nBuckets;
    }

    public void Remove(string key)
    {
        int i = CalculateHash(key) % _buckets.Length;

        if (i < 0 || i >= _buckets.Length || _buckets[i] == null)
        {
            return;
        }

        _buckets[i].RemoveByKey(key);
        _c--;
    }

    public string Get(string key)
    {
        int i = CalculateHash(key) % _buckets.Length;

        if (i < 0 || i >= _buckets.Length || _buckets[i] == null)
        {
            return null;
        }

        KeyValuePair pair = _buckets[i].GetItemWithKey(key);
        if (pair != null)
        {
            return pair.Value;
        }

        return null;
    }

    public int GetBuckSize()
    {
        return _buckets.Length;
    }

    public LinkedList GetLL(int i)
    {
        if (i < 0 || i >= _buckets.Length)
        {
            throw new ArgumentOutOfRangeException(nameof(i));
        }
        return _buckets[i];
    }

    private int CalculateHash(string key)
    {
        return Math.Abs(key.GetHashCode());
    }
}


class Program
{
    static void Main()
    {
        StringsDictionary dictionary = new StringsDictionary();

        try
        {
            string[] ls = File.ReadAllLines("dictionary-2.txt");
            foreach (string l in ls)
            {
                string[] parts = l.Split(';');
                if (parts.Length >= 2)
                {
                    string key = parts[0].Trim().ToUpper();
                    string[] defpart = parts[1].Split(new[] { "Defn:" }, StringSplitOptions.None);
                    if (defpart.Length >= 2)
                    {
                        string definition = defpart[1].Trim();
                        dictionary.Add(key, definition);
                    }
                }
            }
        }
        catch (IOException)
        {
            return;
        }

        /*
        for (int i = 0; i < dictionary.GetBuckSize(); i++)
        {
            LinkedList ll = dictionary.GetLL(i);
            LinkedListNode curr = ll._first;
            while (curr != null)
            {
                Console.WriteLine($"key: {curr.Pair.Key}, val: {curr.Pair.Value}");
                curr = curr.Next;
            }
        }
        */
        
        while (true)
        {
            Console.Write("search key: ");
            string key = Console.ReadLine().Trim().ToUpper();
            if (key == "exit") break;

            string definition = dictionary.Get(key);
            if (definition != null)
            {
                Console.WriteLine($"definition '{key}': {definition}");
            }
            else
            {
                Console.WriteLine($"not found");
            }
        }
    }
}
