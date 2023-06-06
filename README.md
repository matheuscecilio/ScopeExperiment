# Requirements:
- Install docker CLI
- Install dotnet ef
- In powershell run docker-compose command: `docker-compose -f ".\docker-compose-infra.yml" up`
- Inside Core project run command: `dotnet ef database update`

Run both Consumer and Producer project. 

In Producer project you can change the time that the messages will be sent to rabbitmq changing `waitTimeInSeconds` value inside `DefaultProducer.cs`

In Consumer project (`Program.cs`) you can comment the consumer that you do not want to use and uncomment what you want.