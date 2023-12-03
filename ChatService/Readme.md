This is just a prototype to get the technical project working. It can be implemented much better with Kafka streams instead of Queues or Lists.

I dedicated around 6 hours in total for this test. Had I invested more time on it I would have done a much better job. Things that can be improved:

- Better DDD implementation. Some of the logic in this class can be encapsulated in domain classes however due to complexity these were kept here.
- Event driven using Kafka or other event queuing protocol. Each agent could have their own topic which are consumed from a number of chats (max 10).
  This also removes the need for locks which I had to introduce in this app due to multithreading.
- Microservice architecture - As shown in the diagram, the best way for the system to handle chat messages is to have separate chat API and coordinator service.
  This brings seperation of concenrns and separate instance scalability, in case it's needed.
- Instead of chat polling you can use web sockets or SingalR to keep the session alive and have a chat-like experience.

I also commit a small test Powershell file which I used to spawn X number of sessions and keep them alive with continuous polling (every 1 sec): "ChatService.ps1".

Thanks!
