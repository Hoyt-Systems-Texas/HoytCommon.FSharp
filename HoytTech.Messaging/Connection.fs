module HoytTech.Messaging.Connection

open System

module Context =
    
    type state<'a> =
        | Pending
        | NotAuthenticated
        | Authenticated of 'a
        | Timeout
        
    type event<'a> =
        | Ping
        | Pong
        | TimedOut
        | AuthenticationTimeout
        | Authenticated of 'a
    
    type t<'m, 'a> = {
        connectionId: Guid
        lastSeen: int64 ref
        metadata: 'm
        state: state<'a>
    }