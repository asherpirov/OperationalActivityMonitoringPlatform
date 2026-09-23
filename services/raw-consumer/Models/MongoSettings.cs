using System;

namespace RawConsumer.Models;

public class MongoSettings
{
    public string DatabaseName {get;set;} = string.Empty;
    public string CollectionName {get;set;} = string.Empty;
    public string ConnectionString {get; set;} = string.Empty;

}
