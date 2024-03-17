using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

public class HuffNode
{
    public char Ch { get; set; }
    public int Freq { get; set; }
    public HuffNode Left { get; set; }
    public HuffNode Right { get; set; }
}

public class MinHeap
{
    private List<HuffNode> heap;

    public MinHeap()
    {
        heap = new List<HuffNode>();
    }

    public int sz => heap.Count;

    public void Insert(HuffNode node)
    {
        heap.Add(node);
        int i = sz - 1;
        while (i > 0 && heap[(i - 1) / 2].Freq > heap[i].Freq)
        {
            Swap(i, (i - 1) / 2);
            i = (i - 1) / 2;
        }
    }

    public HuffNode RemMin()
    {
        if (sz == 0) throw new InvalidOperationException("empty");

        HuffNode minNode = heap[0];
        heap[0] = heap[sz - 1];
        heap.RemoveAt(sz - 1);
        MinHeapify(0);
        return minNode;
    }

    private void MinHeapify(int i)
    {
        int left = 2 * i + 1;
        int right = 2 * i + 2;
        int sm = i;

        if (left < sz && heap[left].Freq < heap[sm].Freq)
            sm = left;

        if (right < sz && heap[right].Freq < heap[sm].Freq)
            sm = right;

        if (sm != i)
        {
            Swap(i, sm);
            MinHeapify(sm);
        }
    }

    private void Swap(int i, int j)
    {
        HuffNode tmp = heap[i];
        heap[i] = heap[j];
        heap[j] = tmp;
    }
}

public class HuffEncDecode
{
    private Dictionary<char, int> GetFreq(string str)
    {
        Dictionary<char, int> freqs = new Dictionary<char, int>();
        foreach (char c in str)
        {
            if (freqs.ContainsKey(c))
                freqs[c]++;
            else
                freqs[c] = 1;
        }
        return freqs;
    }

    private HuffNode BuildHuffTree(Dictionary<char, int> freqs)
    {
        MinHeap minHeap = new MinHeap();

        foreach (var i in freqs)
        {
            minHeap.Insert(new HuffNode { Ch = i.Key, Freq = i.Value });
        }

        while (minHeap.sz > 1)
        {
            HuffNode left = minHeap.RemMin();
            HuffNode right = minHeap.RemMin();
            HuffNode par = new HuffNode
            {
                Ch = '\0',
                Freq = left.Freq + right.Freq,
                Left = left,
                Right = right
            };
            minHeap.Insert(par);
        }

        return minHeap.RemMin();
    }

    private Dictionary<char, string> EncTable(HuffNode root)
    {
        Dictionary<char, string> enctable = new Dictionary<char, string>();
        EncTableRec(root, "", enctable);
        return enctable;
    }

    private void EncTableRec(HuffNode node, string code, Dictionary<char, string> enctable)
    {
        if (node.Left == null && node.Right == null)
        {
            enctable[node.Ch] = code;
            return;
        }

        EncTableRec(node.Left, code + "0", enctable);
        EncTableRec(node.Right, code + "1", enctable);
    }

    public void Compress(string inpfile, string outfile)
    {
        string str = File.ReadAllText(inpfile);
        Dictionary<char, int> freqs = GetFreq(str);
        HuffNode root = BuildHuffTree(freqs);
        Dictionary<char, string> enctable = EncTable(root);

        using (var writer = new BinaryWriter(File.Open(outfile, FileMode.Create)))
        {
            writer.Write(enctable.Count);
            foreach (var i in enctable)
            {
                writer.Write(i.Key);
                writer.Write(i.Value);
            }

            string encoded = string.Concat(str.Select(c => enctable[c]));
            int bitpad = 8 - encoded.Length % 8;
            encoded += new string('0', bitpad);
            for (int i = 0; i < encoded.Length; i += 8)
            {
                writer.Write(Convert.ToByte((string)encoded.Substring(i, 8), 2));
            }
        }
    }

    public void Decompress(string inpfile, string outfile)
    {
        using (var reader = new BinaryReader(File.Open(inpfile, FileMode.Open)))
        {
            int batchsz = reader.ReadInt32();
            Dictionary<string, char> dectable = new Dictionary<string, char>();
            for (int i = 0; i < batchsz; i++)
            {
                char ch = reader.ReadChar();
                string strr = reader.ReadString();
                dectable[strr] = ch;
            }

            StringBuilder decstrbuild = new StringBuilder();
            int bufsize = 1024 * 8192;
            byte[] buf = new byte[bufsize];
            int bytesRead;
            string code = "";

            while ((bytesRead = reader.Read(buf, 0, bufsize)) > 0)
            {
                for (int i = 0; i < bytesRead; i++)
                {
                    for (int j = 7; j >= 0; j--)
                    {
                        bool bit = (buf[i] & (1 << j)) != 0;
                        code += bit ? "1" : "0";
                        if (dectable.TryGetValue(code, out char decC))
                        {
                            decstrbuild.Append(decC);
                            code = "";
                        }
                    }
                }
            }

            File.WriteAllText(outfile, decstrbuild.ToString());
        }
    }

}

class Program
{
    static void Main(string[] args)
    {
        HuffEncDecode huffman = new HuffEncDecode();
        string inpfile = "sherlock.txt";
        string compressed = "compressed.bin";
        string decompressed = "decompressed.txt";

        huffman.Compress(inpfile, compressed);
        Console.WriteLine("1");

        huffman.Decompress(compressed, decompressed);
        Console.WriteLine("2");
    }
}
