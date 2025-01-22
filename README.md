# Fiap.FileCut

[![Bugs](https://sonarcloud.io/api/project_badges/measure?project=Grupo-68-FIAP_Fiap.FileCut&metric=bugs)](https://sonarcloud.io/summary/new_code?id=Grupo-68-FIAP_Fiap.FileCut)
[![Code Smells](https://sonarcloud.io/api/project_badges/measure?project=Grupo-68-FIAP_Fiap.FileCut&metric=code_smells)](https://sonarcloud.io/summary/new_code?id=Grupo-68-FIAP_Fiap.FileCut)
[![Coverage](https://sonarcloud.io/api/project_badges/measure?project=Grupo-68-FIAP_Fiap.FileCut&metric=coverage)](https://sonarcloud.io/summary/new_code?id=Grupo-68-FIAP_Fiap.FileCut)


## Descri��o do Projeto

O **Fiap.FileCut** � um sistema de processamento de v�deos desenvolvido para atender demandas de escalabilidade e efici�ncia. Ele permite que usu�rios enviem v�deos, processem-nos e baixem um arquivo `.zip` contendo as imagens extra�das. Este projeto foi desenvolvido como parte de um desafio de arquitetura de software, implementando boas pr�ticas de microsservi�os, mensageria e qualidade de software.


### Tecnologias Utilizadas
-   **Backend**: .NET 6.0
-   **Mensageria**: RabbitMQ
-   **Armazenamento**: Amazon S3
-   **Autentica��o**: Keycloak para gerenciamento de identidade e acesso.
-   **CI/CD**: GitHub Actions com integra��o ao SonarQube para analise de c�digo
-   **Testes**: xUnit, Moq e SpecFlow para testes funcionais

## Motiva��o

O projeto originou-se de um prot�tipo simples para demonstrar a extra��o de imagens de v�deos. Embora funcional, esse c�digo inicial carecia de boas pr�ticas e uma arquitetura robusta.

Agora, o objetivo � transformar esse prot�tipo em uma solu��o completa e escal�vel, atendendo aos seguintes requisitos:

-   Suporte ao processamento simult�neo de m�ltiplos v�deos
-   Persist�ncia de dados para controle e rastreamento de status
-   Toler�ncia a picos de carga sem perda de requisi��es
-   Sistema protegido por autentica��o (usu�rio e senha)
-   Notifica��es aos usu�rios em caso de erro
-   Arquitetura escal�vel e garantia de qualidade por meio de testes e CI/CD

## Arquitetura

O sistema foi projetado seguindo o conceito de **microsservi�os**. Cada componente tem uma responsabilidade espec�fica:

![Arquitetura do Sistema](./arquitetura-fiap-filecut.jpg)

**Microsservi�os**:

1.  [Fiap.FileCut.Upload](https://github.com/Grupo-68-FIAP/Fiap.FileCut.Upload):
    -   Recebe os v�deos dos usu�rios
    -   Realiza o upload para um bucket de armazenamento, incluindo um arquivo JSON de controle
    -   Publica eventos de estado em filas (upload bem-sucedido ou falhas)
2.  [Fiap.FileCut.Gestao](https://github.com/Grupo-68-FIAP/Fiap.FileCut.Gestao):
    -   Gerencia o estado dos processos e armazena essas informa��es no banco de dados
    -   Notifica os usu�rios via e-mail ou outro meio em caso de falhas
3.  [Fiap.FileCut.Processamento](https://github.com/Grupo-68-FIAP/Fiap.FileCut.Processamento):
    -   Monitora eventos de processamento e inicia a extra��o de imagens dos v�deos
    -   Realiza a zipagem das imagens extra�das
    -   Publica eventos em filas indicando o progresso ou conclus�o do processamento

**Outros componentes:**

1.  **Bucket de Armazenamento**:
    -   Armazena os v�deos enviados e os arquivos de controle
    -   Garante a integridade dos dados durante o fluxo todo
2.  **MQ (Message Queue)**:
    -   Gerencia a comunica��o ass�ncrona entre os microsservi�os
    -   As filas est�o organizadas como:
        -   Fila 1: Controle de estado
        -   Fila 2: Processamento de v�deos
        -   Fila 3: Empacotamento de imagens

## Vis�o Geral do Sistema com DDD

- No Miro: [Link para o Miro](https://miro.com/app/board/uXjVLw0z8YI=/?share_link_id=986601418356)

![DDD do Sistema](./ddd-fiap-filecut.png)

## Como rodar projeto

Em desenvolvimento

## Contribui��o

Sinta-se � vontade para abrir issues e enviar pull requests para melhorias ou corre��es no projeto.

## Licen�a

Este projeto est� licenciado sob a licen�a MIT.