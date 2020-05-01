using System;
using System.IO;
using PlantUml.Net;

namespace DynamoDbBook.ECommerce.Erd
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

  Customers ||-down-|{ Addresses
  Customers ||-right-|{ Orders
  Orders ||-down-|{ OrderItems", OutputFormat.Png).Result;
            
            File.WriteAllBytes("erd.png", bytes);
        }
    }
}
