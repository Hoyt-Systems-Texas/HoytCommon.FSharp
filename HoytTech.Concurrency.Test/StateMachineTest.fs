module HoytTech.Concurrency.Test.StateMachineTest

open NUnit.Framework
open HoytTech.Concurrency.StateMachine

module TestStateMachine =

    type context = {
        entryRan: bool
        exitRan: bool
        defer: bool
    }
    
    type state =
        | State1
        | State2
        | State3
        
    type event =
        | Event1
        | Event2
        
    
    let handleStateChange changeType context event =
        async {
            return match changeType with
                    | Persisted.Entry state ->
                        match state with
                        | State1 -> context
                        | State2 -> {context with entryRan = true}
                        | State3 -> context
                    | Persisted.Exit state ->
                        match state with
                        | State1 -> {context with exitRan = true}
                        | State2 -> context
                        | State3 -> context
        }

    let handleAction state action =
        match (state, action) with
        | (State1, Event1) -> Persisted.ChangeState State2
        | (State2, Event1) -> Persisted.Ignore
        | (State3, Event1) -> Persisted.Ignore

        | (State1, Event2) -> Persisted.Defer
        | (State2, Event2) -> Persisted.Ignore
        | (State3, Event2) -> Persisted.Ignore

[<TestFixture>]
type StateMachineTest () =
    
    
    [<Test>]
    member this.StateMachineTest() =
        let context = {
            TestStateMachine.entryRan = false
            TestStateMachine.exitRan = false
            TestStateMachine.defer = false
        }
        let state = Persisted.make context 32 32 TestStateMachine.State1 TestStateMachine.handleStateChange TestStateMachine.handleAction
        async {
            let! ctx = Persisted.send state TestStateMachine.Event1 |> Async.AwaitTask
            match ctx with
            | Persisted.Ran ctx ->
                Assert.IsTrue(ctx.entryRan)
                Assert.IsTrue(ctx.exitRan)
            | _ ->
                Assert.Fail("Unexpected result.")
        } |> Async.RunSynchronously
        
    [<Test>]
    member this.DeferTest() =
        let context = {
            TestStateMachine.entryRan = false
            TestStateMachine.exitRan = false
            TestStateMachine.defer = false
        }
        let state = Persisted.make context 32 32 TestStateMachine.State1 TestStateMachine.handleStateChange TestStateMachine.handleAction
        async {
            let! result = Persisted.send state TestStateMachine.Event2 |> Async.AwaitTask
            Assert.AreEqual(Persisted.result<TestStateMachine.context>.Deferred, result)
        } |> Async.RunSynchronously