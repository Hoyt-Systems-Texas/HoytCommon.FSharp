module HoytTech.Concurrency.StateMachine

open System.Threading
open HoytTech.Concurrency.Queue
open System.Threading.Tasks
open HoytTech.Core

module Persisted =
    
    type state =
        | Idle
        | Running
    
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
    
    type t<'s, 'c, 'e> = {
        context: 'c ref
        queue: SkipQueue.t<'e * Option<TaskCompletionSource<result<'c>>>>
        runState: state ref
        currentState: 's
        stateChange: handleStateChange<'s, 'c, 'e>
        whatAction: whatAction<'s, 'c, 'e>
        lastRan: int64 ref
    }
    
    let make ctx queueSize skipSize state handleState action =
        {
            context = ref ctx
            runState = ref Idle
            queue = SkipQueue.make queueSize skipSize
            currentState = state
            stateChange = handleState
            whatAction = action
            lastRan = ref <| Clock.timeInFrequency ()
        }
        
    let eventLoop t =
        if Interlocked.CompareExchange(t.runState, Running, Idle) = Idle then
            let rec loop _ =
                match SkipQueue.poll t.queue with
                | Some(event, task) ->
                async {
                    let setResult r =
                        match task with
                        | Some (task) -> task.SetResult(r)
                        | None -> ()
                    match t.whatAction t.currentState event with
                    | Defer ->
                        let r =
                            if not <| SkipQueue.defer t.queue (event, task) then
                                Full
                            else
                                Deferred
                        setResult(r)
                    | ChangeState(newState) ->
                        let oldState = t.currentState
                        let! c = t.stateChange (Exit oldState) !t.context event
                        t.context := c
                        SkipQueue.reset t.queue
                        let! c = t.stateChange (Entry newState) !t.context event
                        t.context := c
                        setResult(Ran c)
                    | Action act ->
                        let! c = act !t.context event
                        t.context := c
                        setResult(Ran c)
                    | Ignore ->
                        setResult(Ran !t.context)
                } |> Async.Start
                t.lastRan := Clock.timeInFrequency ()
                loop ()
                | None ->
                    Volatile.Write(t.runState, Idle)
            loop ()

    let send t event =
        let task = TaskCompletionSource()
        if SkipQueue.offer t.queue (event, Some(task)) then
            let run () =
                eventLoop t
            Task.Run(run) |> ignore
            task.Task
        else
            task.SetResult(Full)
            task.Task
            
    let getCtx t =
        !t.context
