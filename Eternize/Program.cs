// See https://aka.ms/new-console-template for more information
using Eternize;

Console.WriteLine("Hello, World!");
IVideoMaker videoMaker = new VideoMaker();

await videoMaker.Inicio();
Console.WriteLine("Fim");
