module HoytTech.Messaging.MessageEncoder

open Microsoft.FSharpLu.Json
open Newtonsoft.Json
open Newtonsoft.Json.Serialization

type encoder<'request, 'reply, 'event> = (Message.t<'request, 'reply, 'event> -> string)

let jsonSetting =
    let settings = JsonSerializerSettings(ContractResolver = CamelCasePropertyNamesContractResolver())
    settings.Converters.Add(CompactUnionJsonConverter(true))
    settings
    
let serialize t =
    JsonConvert.SerializeObject(t, jsonSetting)
    
let deserialize body =
    JsonConvert.DeserializeObject<'a>(body)


