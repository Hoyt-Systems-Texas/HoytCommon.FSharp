module HoytTech.Concurrency.Test.AgentTest

open HoytTech.Concurrency
open HoytTech.Concurrency.Agent
open HoytTech.Concurrency.Agent.AgentFunctor
open NUnit.Framework

[<TestFixture>]
type AgentTest () =
    
    [<Test>]
    member this.AgentRunTest() =
        let agent = AgentFunctor.make 0x10 1 "hi"
        async {
            let! (agent, result) = AgentFunctor.fmap agent (fun a -> a + " Matt") |> Async.AwaitTask
            Assert.AreEqual(AgentFunctor.Success("hi Matt"), result)
            let! (agent, result) = fmap agent (fun a -> a + " again") |> Async.AwaitTask
            Assert.AreEqual(Success("hi Matt again"), result)
        } |> Async.RunSynchronously
    