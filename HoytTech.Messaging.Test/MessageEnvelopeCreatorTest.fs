module HoytTech.Messaging.Test

open NUnit.Framework

[<SetUp>]
let Setup () =
    ()

[<Test>]
let EnvelopeTest () =
    let body = "0123456789"
    let envelopes = MessageEnvelopeCreator.make body 4
    let messages = ref []
    MessageEnvelopeCreator.processEnvelopes envelopes (fun m -> messages := m::!messages)
    Assert.AreEqual(3, List.length(!messages))
    
[<Test>]
let EnvelopeEventTest () =
    let body = "0123456789"
    let envelopes = MessageEnvelopeCreator.make body 10
    let messages = ref []
    MessageEnvelopeCreator.processEnvelopes envelopes (fun m -> messages := m::!messages)
    Assert.AreEqual(1, List.length(!messages))
    match (!messages) with
    | [msg] -> Assert.AreEqual(({
        number = 0
        total = 1
        body = body
    }: MessageEnvelopeCreator.packet),  msg)
    | _ -> Assert.Fail("Unexpected value returned.")
    
[<Test>]
let EnvelopeSmallTest () =
    let body = "0123456789"
    let envelopes = MessageEnvelopeCreator.make body 11
    let messages = ref []
    MessageEnvelopeCreator.processEnvelopes envelopes (fun m -> messages := m::!messages)
    Assert.AreEqual(1, List.length(!messages))
    match (!messages) with
    | [msg] -> Assert.AreEqual(({
        number = 0
        total = 1
        body = body
    }: MessageEnvelopeCreator.packet),  msg)
    | _ -> Assert.Fail("Unexpected value returned.");