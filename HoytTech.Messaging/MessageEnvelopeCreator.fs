module HoytTech.Messaging.MessageEnvelopeCreator

open System

type t = {
    message: string
    maxSize: int
    total: int
}

type packet = {
    number: int
    total: int
    body: string
}

let make (body: string) maxSize =
    let total = body.Length / maxSize
    let total =
        if (body.Length % maxSize) > 0 then
            total + 1
        else
            total
    {
        message = body
        maxSize = maxSize
        total = total
    }
    
let processEnvelopes t updateFunc =
    let rec func i =
        let calcStart = i * t.maxSize
        let size = Math.Min(t.maxSize, t.message.Length - calcStart)
        if size <= 0 then
            ()
        else
            let msg = t.message.Substring(calcStart, size)
            updateFunc({
                number = i
                total = t.total
                body = msg
            })
            func (i + 1)
    func 0
  
let getEnvelope t num =
    let calcStart = num * t.maxSize
    let size = Math.Min(t.maxSize, t.message.Length - calcStart)
    if size <= 0 then
        None
    else
        Some({
            number = num
            total = t.total
            body = t.message.Substring(calcStart, size)
        })
        
        