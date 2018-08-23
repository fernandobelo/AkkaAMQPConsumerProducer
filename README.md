# AkkaAMQPConsumerProducer

## Descrição 

Aplicação para testes de AMQP e Akka. Ao iniciar, cria uma fila, atrelado ao exchange configurado (fanout) e passa a enviar uma mensagem de *Hello World* a cada 5 segundos.

Mensagens enviadas para o exchange são enviadas para todas as filas registradas, e o applicativo printa na tela quando recebe uma mensagem.

## Dados técnicos básicos

* .NET Core
* Injeção de dependência via Autofac + .NET Core DI
* Actor Model (Akka.NET), com um ator coordenador, um ator para recebimento de mensagens do broker e um ator para envio de mensagens do broker
* Logging via Serilog (log estruturado)
* Preparado para utilizado em container Docker

## Como rodar

* Iniciar broker AMQP. Sugerido utilizado imagem oficial do RabbitMQ

    * docker pull rabbitmq:3-management

    * docker run -it -p 5672:5672  -e RABBITMQ_DEFAULT_USER=user -e RABBITMQ_DEFAULT_PASS=password rabbitmq:3-management

* Configura daods desejados no arquivo **appsettings.json** (atenção para **ip do ethernet adapter do docker**)

* Criar container da aplicação

    * docker build -t akkaamqpconsumerproducer .

* Rodar quantas instâncias da aplicação foram desejadas

    * docker run -it --rm --net=host  akkaamqpconsumerproducer