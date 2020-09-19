module HoytTech.Messaging.MessageEncoder

/// Definition for an encoder.
type encoder<'request, 'reply, 'event> = (Message.t<'request, 'reply, 'event> -> string)

val serialize : 'a -> string

val deserialize : string -> 'a
