using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text.Json;

namespace Parte1
{
    public class TransicaoJson
    {
        public string origem { get; set; }
        public string simbolo { get; set; }
        public string destino { get; set; }
    }

    public class AfdJson
    {
        public List<string> estados { get; set; }
        public List<string> alfabeto { get; set; }
        public string estadoInicial { get; set; }
        public List<string> estadosAceitacao { get; set; }
        public List<TransicaoJson> transicoes { get; set; }
    }

    public class AFD
    {
        public HashSet<string> Q { get; private set; }
        public HashSet<char> Sigma { get; private set; }
        public Dictionary<(string estado, char simbolo), string> Delta { get; private set; }
        public string q0 { get; private set; }
        public HashSet<string> F { get; private set; }

        public AFD(HashSet<string> q, HashSet<char> sigma, Dictionary<(string, char), string> delta, string q0, HashSet<string> f)
        {
            Q = q;
            Sigma = sigma;
            Delta = delta;
            this.q0 = q0;
            F = f;
        }

        public bool Analisar(string palavra, out List<string> caminho)
        {
            caminho = new List<string>();
            string estadoAgora = q0;
            caminho.Add(estadoAgora);

            if (palavra == "E" || string.IsNullOrEmpty(palavra))
            {
                return F.Contains(estadoAgora);
            }

            foreach (char letra in palavra)
            {
                if (!Sigma.Contains(letra))
                {
                    return false;
                }

                if (Delta.TryGetValue((estadoAgora, letra), out string proximo))
                {
                    estadoAgora = proximo;
                    caminho.Add(estadoAgora);
                }
                else
                {
                    return false;
                }
            }

            return F.Contains(estadoAgora);
        }

        public void ExibirDiagrama()
        {
            Console.WriteLine("Diagrama de Transições (Tabela)");
            Console.WriteLine($"{"Estado",-10} | " + string.Join(" | ", Sigma.Select(s => s.ToString().PadRight(8))));
            Console.WriteLine(new string('-', 12 + Sigma.Count * 11));

            foreach (var est in Q)
            {
                string linhaTabela = $"{est,-10} | ";
                foreach (var simb in Sigma)
                {
                    if (Delta.TryGetValue((est, simb), out string dest))
                        linhaTabela += $"{dest,-8} | ";
                    else
                        linhaTabela += $"{"-",-8} | ";
                }
                Console.WriteLine(linhaTabela);
            }
            Console.WriteLine("---------------------------------------\n");
        }

        public static AFD CarregarDeJson(string caminho)
        {
            string textoJson = File.ReadAllText(caminho);
            AfdJson dadosJson = JsonSerializer.Deserialize<AfdJson>(textoJson);

            var Q = new HashSet<string>(dadosJson.estados);
            var Sigma = new HashSet<char>(dadosJson.alfabeto.Select(a => a[0]));
            var F = new HashSet<string>(dadosJson.estadosAceitacao);
            var Delta = new Dictionary<(string, char), string>();

            foreach (var transicao in dadosJson.transicoes)
            {
                Delta[(transicao.origem, transicao.simbolo[0])] = transicao.destino;
            }

            return new AFD(Q, Sigma, Delta, dadosJson.estadoInicial, F);
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("AFD (Parte 1)\n");

            var Q = new HashSet<string> { "q0", "q1", "q2" };
            var Sigma = new HashSet<char> { 'a', 'b' };
            string q0 = "q0";
            var F = new HashSet<string> { "q2" };

            var Delta = new Dictionary<(string, char), string>
            {
                { ("q0", 'a'), "q1" }, { ("q0", 'b'), "q0" },
                { ("q1", 'a'), "q1" }, { ("q1", 'b'), "q2" },
                { ("q2", 'a'), "q1" }, { ("q2", 'b'), "q0" }
            };

            AFD maquina1 = new AFD(Q, Sigma, Delta, q0, F);

            maquina1.ExibirDiagrama();

            CriaTxt();

            Console.WriteLine("Processando entradas.txt para L1...\n");
            string[] linhasTxt = File.ReadAllLines("entradas.txt");

            foreach (var linha in linhasTxt)
            {
                if (string.IsNullOrWhiteSpace(linha)) continue;

                string textoLimpo = linha.Trim();
                bool passou = maquina1.Analisar(textoLimpo, out List<string> caminhoFeito);

                string resultado = passou ? "ACEITA" : "REJEITA";
                string caminhoStr = string.Join(" -> ", caminhoFeito);

                Console.WriteLine($"Cadeia: {(textoLimpo == "E" ? "vazia" : textoLimpo)}");
                Console.WriteLine($"Rastro: {caminhoStr}");
                Console.WriteLine($"Resultado: {resultado}");
                Console.WriteLine(new string('-', 30));
            }

            Console.WriteLine("\nCarregando afd.json");
            CriaJson();

            AFD maquinaDinamica = AFD.CarregarDeJson("afd.json");
            Console.WriteLine("Máquina carregada com sucesso do arquivo JSON!");
            maquinaDinamica.ExibirDiagrama();
        }

        static void CriaTxt()
        {
            if (!File.Exists("entradas.txt"))
            {
                File.WriteAllLines("entradas.txt", new string[] {
                    "ab", "aab", "bab", "ababab", "ba", "E", "b"
                });
            }
        }

        static void CriaJson()
        {
            if (!File.Exists("afd.json"))
            {
                string conteudoJson = @"{
                  ""estados"": [""q0"", ""q1""],
                  ""alfabeto"": [""0"", ""1""],
                  ""estadoInicial"": ""q0"",
                  ""estadosAceitacao"": [""q1""],
                  ""transicoes"": [
                    { ""origem"": ""q0"", ""simbolo"": ""1"", ""destino"": ""q1"" },
                    { ""origem"": ""q0"", ""simbolo"": ""0"", ""destino"": ""q0"" },
                    { ""origem"": ""q1"", ""simbolo"": ""1"", ""destino"": ""q0"" },
                    { ""origem"": ""q1"", ""simbolo"": ""0"", ""destino"": ""q1"" }
                  ]
                }";
                File.WriteAllText("afd.json", conteudoJson);
            }
        }
    }
}