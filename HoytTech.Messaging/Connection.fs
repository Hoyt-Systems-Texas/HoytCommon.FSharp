module HoytTech.Messaging.Connection

open System
open HoytTech.Concurrency.Queue

module Context =
    
    type t<'m, 'a> = {
        connectionId: Guid
        lastSeen: int64 ref
        metadata: 'm
    }
    
module IncomingConnection =
    
    type state<'a> =
        | Pending
        | Connected
        
    type event<'a> =
        | Ping
        | Pong
    
    type t<'m, 'a> = {
        state: state<'a> ref
        ctx: Context.t<'m, 'a> ref
        missedHeartbeats: int32 ref
        queue: Mpmc.t<event<'a>>
    }
