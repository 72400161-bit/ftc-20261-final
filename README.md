# Simulador de Máquinas Abstratas - FTC
#Os commits ficaram com mais de um nome para não confundir ficou 
#Commits do alex as contas usadas foram : 72400161-bit(da faculdade) e a AlexPVCCSilva(Particular), pois n consegui mudar para ficar a mesma da faculdade

#Commits do Bernardo foram utilizando a conta : Bermedmen



## Equipe
* **Alex Paulo Vianna Da Costa Candido e Silva** - Matrícula:72400161
* **Carlos Henrique Queiroz Teixeira Ramos** - Matrícula: 
* **Bernardo Medeiros Mendes** - Matrícula: 

## Descrição do Projeto
Este repositório contém a implementação de três simuladores de máquinas abstratas, desenvolvidos como requisito final para a disciplina de Fundamentos Teóricos da Computação.
* **Parte 1 (Autômato Finito Determinístico - AFD):** Simulador que reconhece a linguagem L1 (palavras terminadas em "ab"). Inclui também um parser capaz de construir a máquina e a tabela de transição dinamicamente a partir de um arquivo `afd.json`.
* **Parte 2 (Autômato de Pilha - AP):** Simulador focado no reconhecimento da linguagem L2 ($a^n b^n$) e no desafio L3 (palíndromos sobre {a,b}), utilizando uma pilha LIFO (Stack) nativa do C# e critério de aceitação restrito à pilha vazia.
* **Parte 3 (Máquina de Turing - MT):** Simulador de uma Máquina de Turing com fita infinita modelada bidirecionalmente via `Dictionary<int, char>`. Reconhece a linguagem L4 ($a^n b^n c^n$) por estratégia de marcação e resolve o problema matemático da Adição Unária f(n) = n+1.

## Instruções de Compilação e Execução
O projeto foi desenvolvido em .NET (C#). Para executar qualquer um dos simuladores, certifique-se de ter o SDK do .NET 6 (ou superior) instalado na máquina.

Navegue até a pasta da máquina desejada via terminal e execute o comando genérico de inicialização:

cd Parte1
dotnet run


#Link do vídeo do Youtube : 