using System;
using System.Collections.Generic;
using System.IO;

namespace Parte2
{
    public class AutomatoPilha
    {
        public HashSet<string> Q { get; private set; }
        public HashSet<char> Sigma { get; private set; }
        public HashSet<char> Gamma { get; private set; }

        public Dictionary<(string, char, char), List<(string novoEstado, string empilhar)>> Delta { get; private set; }

        public string q0 { get; private set; }
        public char Z0 { get; private set; }
        public HashSet<string> F { get; private set; }

        public AutomatoPilha(HashSet<string> q, HashSet<char> sigma, HashSet<char> gamma,
                             Dictionary<(string, char, char), List<(string, string)>> delta,
                             string q0, char z0)
        {
            Q = q;
            Sigma = sigma;
            Gamma = gamma;
            Delta = delta;
            this.q0 = q0;
            Z0 = z0;
            F = new HashSet<string>();
        }


        public bool Aceitar(string cadeia)
        {
            var pilhaInicial = new Stack<char>();
            pilhaInicial.Push(Z0);

            return Processar(cadeia, q0, pilhaInicial, 0);
        }

        private bool Processar(string cadeia, string estadoAtual, Stack<char> pilhaAtual, int indiceCadeia)
        {
            string cadeiaRestante = indiceCadeia < cadeia.Length ? cadeia.Substring(indiceCadeia) : "E";
            string conteudoPilha = pilhaAtual.Count > 0 ? string.Join("", pilhaAtual) : "[Vazia]";
            Console.WriteLine($"  ├─ Configuração: (q: {estadoAtual}, w: {cadeiaRestante}, Pilha: {conteudoPilha})");

            if (indiceCadeia == cadeia.Length && pilhaAtual.Count == 0)
            {
                return true;
            }

            if (pilhaAtual.Count == 0) return false;

            char topo = pilhaAtual.Peek();
            char simboloLido = indiceCadeia < cadeia.Length ? cadeia[indiceCadeia] : '\0';

            bool aceitou = false;

            if (Delta.TryGetValue((estadoAtual, '\0', topo), out var transicoesEpsilon))
            {
                foreach (var transicao in transicoesEpsilon)
                {
                    var novaPilha = ClonarPilha(pilhaAtual);
                    novaPilha.Pop();

                    if (transicao.empilhar != "")
                    {
                        for (int i = transicao.empilhar.Length - 1; i >= 0; i--)
                            novaPilha.Push(transicao.empilhar[i]);
                    }

                    if (Processar(cadeia, transicao.novoEstado, novaPilha, indiceCadeia))
                        aceitou = true;
                }
            }

            if (!aceitou && simboloLido != '\0')
            {
                if (Delta.TryGetValue((estadoAtual, simboloLido, topo), out var transicoesComSimbolo))
                {
                    foreach (var transicao in transicoesComSimbolo)
                    {
                        var novaPilha = ClonarPilha(pilhaAtual);
                        novaPilha.Pop();

                        if (transicao.empilhar != "")
                        {
                            for (int i = transicao.empilhar.Length - 1; i >= 0; i--)
                                novaPilha.Push(transicao.empilhar[i]);
                        }

                        if (Processar(cadeia, transicao.novoEstado, novaPilha, indiceCadeia + 1))
                            aceitou = true;
                    }
                }
            }

            return aceitou;
        }

        private Stack<char> ClonarPilha(Stack<char> original)
        {
            var array = original.ToArray();
            Array.Reverse(array);
            return new Stack<char>(array);
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== Simulador de Autômato de Pilha (Parte 2) ===\n");
            CriarArquivosDeTeste();

            ExecutarL2();
            ExecutarL3();
        }

        static void ExecutarL2()
        {
            Console.WriteLine("=== L2: a^n b^n (n >= 1) ===");

            var Q = new HashSet<string> { "q0", "q1", "q2" };
            var Sigma = new HashSet<char> { 'a', 'b' };
            var Gamma = new HashSet<char> { 'Z', 'A' };
            char Z0 = 'Z';

            var Delta = new Dictionary<(string, char, char), List<(string, string)>>
            {
                { ("q0", 'a', 'Z'), new List<(string, string)> { ("q0", "AZ") } },
                { ("q0", 'a', 'A'), new List<(string, string)> { ("q0", "AA") } },
                { ("q0", '\0', 'A'), new List<(string, string)> { ("q1", "A") } },
                { ("q1", 'b', 'A'), new List<(string, string)> { ("q1", "") } },
                { ("q1", '\0', 'Z'), new List<(string, string)> { ("q2", "") } }
            };

            var apL2 = new AutomatoPilha(Q, Sigma, Gamma, Delta, "q0", Z0);

            string[] cadeias = File.ReadAllLines("entradas_ap.txt");
            foreach (var cadeia in cadeias)
            {
                if (string.IsNullOrWhiteSpace(cadeia)) continue;
                string c = cadeia.Trim() == "E" ? "" : cadeia.Trim();

                Console.WriteLine($"\nProcessando: {(c == "" ? "vazia" : c)}");
                bool aceita = apL2.Aceitar(c);
                Console.WriteLine($"Resultado: {(aceita ? "ACEITA" : "REJEITA")}");
                Console.WriteLine(new string('-', 30));
            }
        }

        static void ExecutarL3()
        {
            Console.WriteLine("\n=== Desafio L3: Palíndromos sobre {a,b} ===");

            var Q = new HashSet<string> { "q0", "q1", "q2" };
            var Sigma = new HashSet<char> { 'a', 'b' };
            var Gamma = new HashSet<char> { 'Z', 'A', 'B' };
            char Z0 = 'Z';

            var Delta = new Dictionary<(string, char, char), List<(string, string)>>
            {
                { ("q0", 'a', 'Z'), new List<(string, string)> { ("q0", "AZ"), ("q1", "Z") } },
                { ("q0", 'b', 'Z'), new List<(string, string)> { ("q0", "BZ"), ("q1", "Z") } },
                { ("q0", 'a', 'A'), new List<(string, string)> { ("q0", "AA"), ("q1", "A") } },
                { ("q0", 'b', 'A'), new List<(string, string)> { ("q0", "BA"), ("q1", "A") } },
                { ("q0", 'a', 'B'), new List<(string, string)> { ("q0", "AB"), ("q1", "B") } },
                { ("q0", 'b', 'B'), new List<(string, string)> { ("q0", "BB"), ("q1", "B") } },

                { ("q0", '\0', 'Z'), new List<(string, string)> { ("q1", "Z") } },
                { ("q0", '\0', 'A'), new List<(string, string)> { ("q1", "A") } },
                { ("q0", '\0', 'B'), new List<(string, string)> { ("q1", "B") } },

                { ("q1", 'a', 'A'), new List<(string, string)> { ("q1", "") } },
                { ("q1", 'b', 'B'), new List<(string, string)> { ("q1", "") } },

                { ("q1", '\0', 'Z'), new List<(string, string)> { ("q2", "") } }
            };

            var apL3 = new AutomatoPilha(Q, Sigma, Gamma, Delta, "q0", Z0);

            string[] cadeiasL3 = { "a", "aba", "abba", "ab", "aab" };
            foreach (var c in cadeiasL3)
            {
                Console.WriteLine($"\nProcessando: {c}");
                bool aceita = apL3.Aceitar(c);
                Console.WriteLine($"Resultado: {(aceita ? "ACEITA" : "REJEITA")}");
                Console.WriteLine(new string('-', 30));
            }
        }

        static void CriarArquivosDeTeste()
        {
            if (!File.Exists("entradas_ap.txt"))
            {
                File.WriteAllLines("entradas_ap.txt", new string[] {
                    "ab", "aabb", "aaabbb", "aab", "abb", "ba", "E", "abab"
                });
            }
        }
    }
}