using System;
using System.Collections.Generic;

public class Stack_impl<T>
{
    private LinkedList<T> l;
    private int sz;

    public Stack_impl()
    {
        l = new LinkedList<T>();
        sz = 0;
    }

    public void Push(T it)
    {
        l.AddLast(it);
        sz++;
    }

    public T Pop()
    {
        if (IsEmpty())
        {
            throw new InvalidOperationException("empty");
        }
        
        T it = l.Last.Value;
        l.RemoveLast();
        sz--;
        return it;
    }

    public T Peek()
    {
        if (IsEmpty())
        {
            throw new InvalidOperationException("empty");
        }
        
        return l.Last.Value;
    }

    public bool IsEmpty()
    {
        return sz == 0;
    }
}

class Program
{
    static void Main(string[] args)
    {
        while (true)
        {
            // string inp = "-(2+-3/8^2)"; debug
            Console.Write("> ");
            string inp = Console.ReadLine();
            string cIn = inp.Replace(" ", "");
            string rpn = RPN(cIn);
            // Console.WriteLine(rpn); debug
            
            try
            {
                double res = Res_by_RPN(rpn);
                Console.WriteLine("< " + res);
            }
            catch (Exception e)
            {
                Console.WriteLine("error: " + e.Message);
            }
        }
    }

    static string RPN(string inp)
    {
        Stack_impl<char> oper = new Stack_impl<char>();
        char[] outp = new char[inp.Length];
        int outI = 0;

        Dictionary<char, int> priority = new Dictionary<char, int>
        {
            {'+', 1},
            {'-', 1},
            {'*', 2},
            {'/', 2},
            {'^', 3},
            {'(', 0},
            {')', 0}
        };

        bool lastOper = true;

        foreach (char token in inp)
        {
            if (char.IsDigit(token))
            {
                outp[outI++] = token;
                lastOper = false;
            }
            else if (token == '(')
            {
                oper.Push(token);
                lastOper = true;
            }
            else if (token == ')')
            {
                while (!oper.IsEmpty() && oper.Peek() != '(')
                {
                    outp[outI++] = oper.Pop();
                }
                oper.Pop();
                lastOper = false;
            }
            else
            {
                if (lastOper && (token == '-' || token == '+'))
                {
                    outp[outI++] = '0';
                    oper.Push(token);
                }
                else
                {
                    while (!oper.IsEmpty() && priority[token] <= priority[oper.Peek()])
                    {
                        outp[outI++] = oper.Pop();
                    }
                    oper.Push(token);
                }
                lastOper = true;
            }
        }

        while (!oper.IsEmpty())
        {
            outp[outI++] = oper.Pop();
        }

        return new string(outp, 0, outI);
    }

    static double Res_by_RPN(string rpn)
    {
        Stack_impl<double> ops = new Stack_impl<double>();

        foreach (char token in rpn)
        {
            if (char.IsDigit(token))
            {
                ops.Push(double.Parse(token.ToString()));
            }
            else
            {
                double op2 = ops.Pop();
                double op1 = ops.Pop();
                double res = 0;
                switch (token)
                {
                    case '+':
                        res = op1 + op2;
                        break;
                    case '-':
                        res = op1 - op2;
                        break;
                    case '*':
                        res = op1 * op2;
                        break;
                    case '/':
                        if (op2 == 0)
                        {
                            throw new InvalidOperationException("cant divide by 0");
                        }
                        res = op1 / op2;
                        break;
                    case '^':
                        res = Math.Pow(op1, op2);
                        break;
                }
                ops.Push(res);
            }
        }

        return ops.Pop();
    }
}
