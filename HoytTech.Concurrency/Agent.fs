module HoytTech.Concurrency.Agent

open System
open System.Threading
open System.Threading.Tasks
open HoytTech.Concurrency.Queue
open HoytTech.Core

module AgentFunctor =
    
    type result<'t> =
        | Full
        | Success of 't
    
    type action<'a> = 'a -> 'a
    
    type result<'a, 'r> = 'a -> 'r
    
    type state =
        | Inactive
        | Running
        | Removed
        
    type t<'id, 'a> = {
        id: 'id
        value: 'a ref
        queue: Mpmc.t<'a -> unit>
        state: state ref
        lastRan: int64 ref
    }
    
    let make size id value =
        let lastRan = Clock.timeInFrequency ()
        {
            id = id
            value = ref value
            queue = Mpmc.make size
            state = ref Inactive
            lastRan = ref lastRan
        }
        
    
    let eventLoop t =
        if Interlocked.CompareExchange(t.state, Running, Inactive) = Inactive then
            let rec loop _ =
                match Mpmc.poll t.queue with
                | Some(a) ->
                    a !t.value
                    Volatile.Write(t.lastRan, Clock.timeInFrequency ())
                    loop ()
                | None -> Volatile.Write(t.state, Inactive)
            loop ()
    
    let fmap t action =
        let task = TaskCompletionSource()
        let func a =
            try 
                let r = action a
                t.value := r
                task.SetResult(t, Success(r))
            with
                | :? Exception as ex -> task.SetException ex
         
        if Mpmc.offer t.queue func then
            let run () =
                eventLoop t
            // Queue up the thread
            Task.Run(run) |> ignore
            task.Task
        else
            task.SetResult(t, Full)
            task.Task
    
    let map t action =
        let task = TaskCompletionSource()
        let func a =
            try
                let r = action a
                task.SetResult(Success(r))
            with
                | :? Exception as ex -> task.SetException ex
        if Mpmc.offer t.queue func then
            let run () =
                eventLoop t
            Task.Run(run) |> ignore
            task.Task
        else
            task.SetResult(Full)
            task.Task
                
    let timeSinceRanMs t =
        Clock.millsToFrequency (Clock.timeInFrequency () - Volatile.Read(t.lastRan))
        
        
            
