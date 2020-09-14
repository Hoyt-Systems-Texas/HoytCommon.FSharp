module HoytTech.Messaging.Message

open System
open HoytTech.Core

module Request =
    
    [<CLIMutable>]
    type t<'request> = {
        requestId: int64 ref
        requestedBy: Guid
        request: 'request
        userId: option<Guid>   
    }

module Reply =
    
    [<CLIMutable>]
    type t<'reply> = {
        requestId: int64
        responseTo: Guid
        result: ResultMonad.t<'reply>
    }
    
module Event =
    
    [<CLIMutable>]
    type t<'msg> = {
        sendTo: Guid
        msg: 'msg
    }
    
type t<'request, 'reply, 'event> =
    | Request of Request.t<'request>
    | Reply of Reply.t<'reply>
    | Event of Event.t<'event>
    | Ping
    | Pong


