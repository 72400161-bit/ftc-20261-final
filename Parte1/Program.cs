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

        public bool Aceitar(string cadeia, out List<string> rastro)
        {
            rastro = new List<string>();
            string estadoAtual = q0;
            rastro.Add(estadoAtual);

            if (cadeia == "E" || string.IsNullOrEmpty(cadeia))
            {
                return F.Contains(estadoAtual);
            }

            foreach (char simbolo in cadeia)
            {
                if (!Sigma.Contains(simbolo))
                {
                    return false;
                }

                if (Delta.TryGetValue((estadoAtual, simbolo), out string proximoEstado))
                {
                    estadoAtual = proximoEstado;
                    rastro.Add(estadoAtual);
                }
                else
                {
                    return false;
                }
            }

            return F.Contains(estadoAtual);
        }

        public void ExibirDiagrama()
        {
            Console.WriteLine("Diagrama de Transições (Tabela)");
            Console.WriteLine($"{"Estado",-10} | " + string.Join(" | ", Sigma.Select(s => s.ToString().PadRight(8))));
            Console.WriteLine(new string('-', 12 + Sigma.Count * 11));

            foreach (var estado in Q)
            {
                string linha = $"{estado,-10} | ";
                foreach (var simbolo in Sigma)
                {
                    if (Delta.TryGetValue((estado, simbolo), out string destino))
                        linha += $"{destino,-8} | ";
                    else
                        linha += $"{"-",-8} | ";
                }
                Console.WriteLine(linha);
            }
            Console.WriteLine("---------------------------------------\n");
        }

        public static AFD CarregarDeJson(string caminhoArquivo)
        {
            string jsonString = File.ReadAllText(caminhoArquivo);
            AfdJson config = JsonSerializer.Deserialize<AfdJson>(jsonString);

            var Q = new HashSet<string>(config.estados);
            var Sigma = new HashSet<char>(config.alfabeto.Select(a => a[0]));
            var F = new HashSet<string>(config.estadosAceitacao);
            var Delta = new Dictionary<(string, char), string>();

            foreach (var t in config.transicoes)
            {
                Delta[(t.origem, t.simbolo[0])] = t.destino;
            }

            return new AFD(Q, Sigma, Delta, config.estadoInicial, F);
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

            AFD afdL1 = new AFD(Q, Sigma, Delta, q0, F);

            afdL1.ExibirDiagrama();

            CriarArquivoDeEntrada(); 

            Console.WriteLine("Processando entradas.txt para L1...\n");
            string[] cadeias = File.ReadAllLines("entradas.txt");

            foreach (var cadeia in cadeias)
            {
                if (string.IsNullOrWhiteSpace(cadeia)) continue;

                string cadeiaFormatada = cadeia.Trim();
                bool aceito = afdL1.Aceitar(cadeiaFormatada, out List<string> rastro);

                string resultado = aceito ? "ACEITA" : "REJEITA";
                string rastroStr = string.Join(" -> ", rastro);

                Console.WriteLine($"Cadeia: {(cadeiaFormatada == "E" ? "vazia" : cadeiaFormatada)}");
                Console.WriteLine($"Rastro: {rastroStr}");
                Console.WriteLine($"Resultado: {resultado}");
                Console.WriteLine(new string('-', 30));
            }

            Console.WriteLine("\nCarregando afd.json");
            CriarArquivoJson();

            AFD afdDinamico = AFD.CarregarDeJson("afd.json");
            Console.WriteLine("Máquina carregada com sucesso do arquivo JSON!");
            afdDinamico.ExibirDiagrama();
        }

        static void CriarArquivoDeEntrada()
        {
            if (!File.Exists("entradas.txt"))
            {
                File.WriteAllLines("entradas.txt", new string[] {
                    "ab", "aab", "bab", "ababab", "ba", "E", "b"
                });
            }
        }

        static void CriarArquivoJson()
        {
            if (!File.Exists("afd.json"))
            {
                string jsonExemplo = @"{
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
                File.WriteAllText("afd.json", jsonExemplo);
            }
        }
    }
}