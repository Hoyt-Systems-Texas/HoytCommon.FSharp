module HoytTech.Concurrency.StateMachine

open System.Threading.Tasks

module Persisted =
    
    type t<'s, 'c, 'e>
    
    type stateAction<'s, 'c, 'e> =
        | Defer
        | ChangeState of 's
        | Action of ('c -> 'e -> Async<'c>)
        | Ignore
        
    type stateChangeType<'s> =
        | Entry of 's
        | Exit of 's
        
    type result<'c> = 
        | Ran of 'c
        | Full
        | Deferred 
        
    type handleStateChange<'s, 'c, 'e> = (stateChangeType<'s> -> 'c -> 'e -> Async<'c>)
    type whatAction<'s, 'c, 'e> = ('s -> 'e -> stateAction<'s, 'c, 'e>)
    
    val make : 'c -> int32 -> int32 -> 's -> handleStateChange<'s, 'c, 'e> -> whatAction<'s, 'c, 'e> -> t<'s, 'c, 'e>
    
    val send : t<'s, 'c, 'e> -> 'e -> Task<result<'c>>
    
    val getCtx: t<'s, 'c, 'e> -> 'c
    
    val getState: t<'s, 'c, 'e> -> 's
    
    