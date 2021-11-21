# Kaushik SPOT Services

A light weight library to provide Remote Method Invocation (RMI) for application running on .NET Micro Framework.

#### Note: This project was originally published on CodePlex in 2011 and as CodePlex was closed, I have restored it here. There is no active development going on here, now purpose of this repo is to provide reference of how to build a simple RPC/RMI from gorund up!! #### 

This is a digital world where devices collaborate to solve some business problem. As we move ahead these devices are not limited to High end servers; we have PCs, Laptops, Smart Phones and Embedded systems that play a vital role in this collaboration.

This Project provides a Library that can be used on all kinds of .NET Framework i.e. .NET Regular Framework, .NET Micro Framework etc for RPC/RMI. This definitely does not replace .NET Remoting, Web Services and WCF but certainly add one more to the same stack; and it is very simple to be used for very light weight but still mission critical applications.

This framework unites these devices by providing a communication platform based on RPC/RMI and Microsoft’s .NET Framework. You will be able to control and manipulate embedded systems from click of a button on your laptop or a windows phone. Any MS Developer can adopt this RPC/RMI Framework and use it as if (s)he is working with Web Services or WCF. (S)he does not require specialized skills to be able to establish such communication.

Key features of this library are:

* A very lightweight implementation.
* Reusability and Extendibility.
* A new API that enables very easy Service development.
* Serialization and Deserialization of objects which is interoperable in all kinds of .NET Framework.
* Means to provide reliable Network communication.
* Ability to control Electronic Appliances over network using Micro Controller.
* Host and Consume Service between any combination of devices i.e. PC to PC, PC to Micro Controller, Micro Controller to Micro Controller and Micro Controller to PC.
* Built using multi threading to keep operating on stuff while serving Service Calls.
* Provides Code Generator for Proxy Class
* Secure Service calls by using encryption algorithm of your choice.
* Use Windows Phone to consume these services.

Here are the possible setups in which you can use it:

1. Host Secure RPC/RMI enabled Services on Netduino and control it from a PC and/or Windows Phone.
2. Host Secure RPC/RMI enabled Services on PC and control it with a Netduino and/or Windows Phone.

There is no release of this project so far and right now I am not sure if there would be one until Feb 2012, so please download the source code from latest changeset on Source Code Tab. A demo video of the work is avalable on [YouTube](http://www.youtube.com/watch?v=eO0eGp7Bgls)

[![image](https://user-images.githubusercontent.com/34212924/142757824-b2087042-c5d3-4835-9d7d-a285db36f056.png)](http://www.youtube.com/watch?v=eO0eGp7Bgls)

## General Approach ##
* Micro Controller can use GPIO pins to form Transistor based switch to control Appliances.
* Methods can be written to control GPIO output.
* Host the above mentioned control methods as SPOT Services using new Framework developed.
* Instantiate Service Object on client machine using Proxy.
* Invoke methods on the remote object to control Appliances.

## Logical Architecture ##
![Home_LogicalArchitecture](https://user-images.githubusercontent.com/34212924/142756865-53c663e5-2efc-4428-a64d-2b72dfa225c2.jpg)

## Deployment Architecture ##
![Home_DeploymentArchitecture](https://user-images.githubusercontent.com/34212924/142756878-5112db77-20ad-4e45-a91e-4d81d73e26b9.jpg)

## Data Representation ##
![Home_DataRepresentation](https://user-images.githubusercontent.com/34212924/142756892-a523aa29-91c4-4019-93a7-35499d5a35e0.jpg)

## Advantages of SPOT Services Framework ##
* Easy implementation leads to robust, maintainable and flexible applications.
* Distributed system creation is allowed while decoupling the client and server objects simultaneously.
* Provides a rich and extensible framework for Remote Method Invocation seamlessly easy.
* Offers a powerful yet simple programming model and runtime support for making these interactions transparent.
* Provides standard platform for building distributed applications that make use of Micro Controller on Microsoft .NET platform.
* It works between all combinations of devices that support .NET.
* It provides lightweight, reliable and extendible platform that can be reused in any kind of requirement.
* It includes a tool to Auto Generate proxy class for clients.
* It includes all exception details that may have occurred on server.

## Utility of SPOT Services Framework for Developers ##
* Very rich API that makes programming task very easy
  * It involves similar steps as in WCF and Remoting.
  * Enables any Microsoft Developer to easily adopt programming in embedded world.
* Secure
  * API allows developers to add any Encryption algorithm that will secure data over communication Channel.
* Easy to Extend
  * Any new network communication protocol can be added.
  * Alternates to Ethernet like 3G Internet using SIM or Wireless or even Radio Wave can be added.
* Wide Range of Device Support
  * Host and Consume Service between any combination of devices i.e. PC to PC, PC to Micro Controller, Micro Controller to Micro Controller and Micro Controller to PC.
* Easy to Debug
  * Actual debugging on device is supported.
  * Emulator is also another option to Debug.
* Coding Language Options
  * Generate proxy in C# or VB.NET
  * It uses .NET Code DOM for future extensibility 

## Utility of SPOT Services Framework for Business ##

It is possible to use this API to Monitor and Control Micro Controller based equipments from Desktop. These implementation include applications like :

![Home_BusinessUtility](https://user-images.githubusercontent.com/34212924/142756979-89c75be7-9fa4-4549-9685-d652efad98cd.png)

## Why not just use sockets for communication? ##
* Socket programming is tedious.
* Error prone for implementing complex logic.
* Best left to “the experts

## Challenges ##
* There is very limited set of functionality that is available in .NET Micro Framework.
* Resources like memory (28K), space (64K) and CPU time is very premium.
* Netduino can only accept 536 bytes of information at a time on Network port, hence it was required to develop an algorithm that sends data as Chunks over Network and there is a proper handshake between Client & Server.
* Serialization have completely different implementation in .NET Micro Framework and .NET Framework so there are to many compatibility issues in implementing API that supports both platforms simultaneously. Hence completely new algorithm had to be developed for Serialization.

## Limitations ##
* Only fixed number (at present 5) of SPOT services can be hosted on a device that can have any number of SPOT methods.
* SPOT method support .NET Value Types and Strings as input parameters and return type. Array of any type and object is not supported.
* Auto discovery of SPOT services is not supported, one has to have Interface definition or assembly to generate proxy.
* As of now Windows Phone 7 cannot host these services.
* It is .NET only solution.

## Further Improvements ##
* Add more implementations of ISpotSocket interface to support other network protocols.

## Conclusion ##
* There is a very vast scope of possibilities in embedded landscape and this project is a door opener for a few more.
* There is a need to further improve the library but at the same time we will have to consider the resource requirements.
