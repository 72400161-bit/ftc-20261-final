#nullable disable
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Parte3
{
    public class MaquinaTuring
    {
        public HashSet<string> Q { get; private set; }
        public HashSet<char> Sigma { get; private set; }
        public HashSet<char> Gamma { get; private set; }
        public Dictionary<(string estado, char simbolo), (string novoEstado, char simboloEscrito, char direcao)> Delta { get; private set; }
        public string q0 { get; private set; }
        public char B { get; private set; }
        public HashSet<string> F { get; private set; }

        public MaquinaTuring(HashSet<string> q, HashSet<char> sigma, HashSet<char> gamma,
                             Dictionary<(string, char), (string, char, char)> delta,
                             string q0, char b, HashSet<string> f)
        {
            Q = q;
            Sigma = sigma;
            Gamma = gamma;
            Delta = delta;
            this.q0 = q0;
            B = b;
            F = f;
        }

        public bool Analisar(string palavra)
        {
            var fita = new Dictionary<int, char>();

            for (int i = 0; i < palavra.Length; i++)
            {
                fita[i] = palavra[i];
            }

            if (palavra.Length == 0 || palavra == "E")
            {
                fita[0] = B;
            }

            int cabecote = 0;
            string estadoAtual = q0;

            while (true)
            {
                if (!fita.ContainsKey(cabecote))
                {
                    fita[cabecote] = B;
                }

                char simboloLido = fita[cabecote];

                MostrarFita(fita, cabecote, estadoAtual);

                if (F.Contains(estadoAtual))
                {
                    return true;
                }

                if (Delta.TryGetValue((estadoAtual, simboloLido), out var acao))
                {
                    fita[cabecote] = acao.simboloEscrito;
                    estadoAtual = acao.novoEstado;
                    cabecote += (acao.direcao == 'R') ? 1 : -1;
                }
                else
                {
                    return false;
                }
            }
        }

        private void MostrarFita(Dictionary<int, char> fita, int posicaoAtual, string estado)
        {
            int minId = fita.Keys.Count > 0 ? Math.Min(fita.Keys.Min(), posicaoAtual - 1) : posicaoAtual - 1;
            int maxId = fita.Keys.Count > 0 ? Math.Max(fita.Keys.Max(), posicaoAtual + 1) : posicaoAtual + 1;

            string printFita = "";

            for (int i = minId; i <= maxId; i++)
            {
                char simb = fita.ContainsKey(i) ? fita[i] : B;

                if (i == posicaoAtual)
                {
                    printFita += $" [{estado}>{simb}] ";
                }
                else
                {
                    printFita += $" {simb} ";
                }
            }
            Console.WriteLine($"Fita: ...{printFita}...");
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("MÃ¡quina de Turing (Parte 3)\n");
            CriarTxt();

            TestarL4();
            TestarAdicao();
        }

        static void TestarL4()
        {
            Console.WriteLine("- L4: a^n b^n c^n (n >= 1) -");

            var Q = new HashSet<string> { "q0", "q1", "q2", "q3", "q4", "q_acc" };
            var Sigma = new HashSet<char> { 'a', 'b', 'c' };
            var Gamma = new HashSet<char> { 'a', 'b', 'c', 'X', 'Y', 'Z', '_' };
            char B = '_';

            var Delta = new Dictionary<(string, char), (string, char, char)>
            {
                { ("q0", 'a'), ("q1", 'X', 'R') },
                { ("q1", 'a'), ("q1", 'a', 'R') },
                { ("q1", 'Y'), ("q1", 'Y', 'R') },
                { ("q1", 'b'), ("q2", 'Y', 'R') },
                { ("q2", 'b'), ("q2", 'b', 'R') },
                { ("q2", 'Z'), ("q2", 'Z', 'R') },
                { ("q2", 'c'), ("q3", 'Z', 'L') },
                { ("q3", 'Z'), ("q3", 'Z', 'L') },
                { ("q3", 'b'), ("q3", 'b', 'L') },
                { ("q3", 'Y'), ("q3", 'Y', 'L') },
                { ("q3", 'a'), ("q3", 'a', 'L') },
                { ("q3", 'X'), ("q0", 'X', 'R') },
                { ("q0", 'Y'), ("q4", 'Y', 'R') },
                { ("q4", 'Y'), ("q4", 'Y', 'R') },
                { ("q4", 'Z'), ("q4", 'Z', 'R') },
                { ("q4", '_'), ("q_acc", '_', 'R') }
            };

            var mt = new MaquinaTuring(Q, Sigma, Gamma, Delta, "q0", B, new HashSet<string> { "q_acc" });

            string[] listaTextos = File.ReadAllLines("entradas_mt.txt");

            foreach (var linha in listaTextos)
            {
                string limpa = linha.Trim() == "E" ? "" : linha.Trim();
                Console.WriteLine($"\nProcessando L4: {(limpa == "" ? "vazia" : limpa)}");

                bool deucerto = mt.Analisar(limpa);

                Console.WriteLine($"Resultado: {(deucerto ? "ACEITA" : "REJEITA")}");
                Console.WriteLine(new string('-', 50));
            }
        }

        static void TestarAdicao()
        {
            Console.WriteLine("\nFazendo f(n) = n + 1 adicao unaria");

            var Q = new HashSet<string> { "q0", "q_acc" };
            var Sigma = new HashSet<char> { '1' };
            var Gamma = new HashSet<char> { '1', '_' };
            char B = '_';

            var Delta = new Dictionary<(string, char), (string, char, char)>
            {
                { ("q0", '1'), ("q0", '1', 'R') },
                { ("q0", '_'), ("q_acc", '1', 'R') }
            };

            var mtSoma = new MaquinaTuring(Q, Sigma, Gamma, Delta, "q0", B, new HashSet<string> { "q_acc" });

            string[] listaValores = { "1", "11", "1111" };

            foreach (var v in listaValores)
            {
                Console.WriteLine($"\nprocessando f(n) para n = {v.Length} (cadeia: {v})");
                mtSoma.Analisar(v);
                Console.WriteLine($"resultado funcionou");
                Console.WriteLine(new string('-', 50));
            }
        }

        static void CriarTxt()
        {
            if (!File.Exists("entradas_mt.txt"))
            {
                File.WriteAllLines("entradas_mt.txt", new string[] {
                    "abc", "aabbcc", "aaabbbccc", "abcc", "aabcc", "E", "ac"
                });
            }
        }
    }
}