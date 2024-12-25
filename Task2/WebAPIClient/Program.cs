﻿using System.Net.Http.Json;
using System.Numerics;
using WebAPIModels.Models;

string uri = $"https://localhost:7026/api/Shippers";
HttpClient client = new HttpClient();

var chapter = await client.GetFromJsonAsync<IEnumerable<Shipper>>(uri);
if (chapter is not null)
{
    foreach(var c in chapter)
    {
        Console.WriteLine($"{c.ShipperId}: {c.CompanyName}");
    }
}

Shipper shipper = new Shipper()
{
    ShipperId = 0,
    CompanyName = "BMSTU",
    Phone = "7521789"
   
};
var result = await client.PostAsJsonAsync<Shipper>(uri, shipper);
Console.Write($"{result}");
Console.ReadLine();



