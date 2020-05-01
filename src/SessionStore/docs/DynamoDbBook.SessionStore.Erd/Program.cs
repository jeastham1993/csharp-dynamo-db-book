using System;
using System.IO;
using PlantUml.Net;

namespace DynamoDbBook.SessionStore.Erd
{
    class Program
    {
        static void Main(string[] args)
        {
            var factory = new RendererFactory();

            var renderer = factory.CreateRenderer(new PlantUmlSettings());

            var bytes = renderer.RenderAsync(@"skinparam linetype ortho
  skinparam packageStyle rectangle
  skinparam shadowing false
  skinparam class {
    BackgroundColor White
    BorderColor Black
    ArrowColor Black
  }
  hide members
  hide circle

  User ||-right-|{ Sessions", OutputFormat.Png).Result;
            
            File.WriteAllBytes("erd.png", bytes);
        }
    }
}
